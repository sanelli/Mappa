// <copyright file="ConstructorMapStrategyDetector.ObjectFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Object factory detection for <see cref="ConstructorMapStrategyDetector"/>.
/// </summary>
internal sealed partial class ConstructorMapStrategyDetector
{
    private bool TryDetectObjectFactory(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        var rootMapMethod = this.context.GetRootMapMethod();
        var classAttributes = rootMapMethod.MethodSymbol.ContainingType
            .GetAttributes()
            .GetMappaObjectFactoryAttributes(this.compilation);
        var methodAttributes = rootMapMethod.MethodSymbol
            .GetAttributes()
            .GetMappaObjectFactoryAttributes(this.compilation);

        var hasMatchingFactory = classAttributes
            .Concat(methodAttributes)
            .Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.TargetType, this.context.TargetType));
        if (!hasMatchingFactory)
        {
            return false;
        }

        var resolver = new ObjectFactoryResolver(this.compilation, this.context, rootMapMethod);
        if (!resolver.TryResolveForTargetType(classAttributes, methodAttributes, out var objectFactory) ||
            objectFactory is null)
        {
            return false;
        }

        ParameterMapStrategy[] parametersMapStrategies = [];
        PropertyMapStrategy[] initializerStrategies = [];

        switch (objectFactory.InvocationKind)
        {
            case ObjectFactoryInvocationKind.FullyProduced:
                break;

            case ObjectFactoryInvocationKind.EmptyCtorLike:
                if (!this.TryBuildEmptyCtorLikePropertyInitializers(
                        requireAtLeastOneMappedProperty: false,
                        out initializerStrategies))
                {
                    return false;
                }

                break;

            case ObjectFactoryInvocationKind.ParameterizedLike:
                if (!this.TryMapFactoryParameters(objectFactory.Method, out parametersMapStrategies))
                {
                    return false;
                }

                break;

            default:
                throw new MappaGeneratorException($"Unexpected object factory invocation kind '{objectFactory.InvocationKind}'.");
        }

        mapStrategy = new InvokeObjectFactoryMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            objectFactory,
            parametersMapStrategies,
            initializerStrategies,
            [],
            null);
        return true;
    }

    private bool TryMapFactoryParameters(
        IMethodSymbol factoryMethod,
        out ParameterMapStrategy[] parametersMapStrategies)
    {
        parametersMapStrategies = [];
        var noMapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        var sourceProperties = this.context.SourceType.GetTypeProperties()
            .Where(property => !property.IsIndexer && property.IsGetterAccessible(this.compilation, this.context.GetRootMapMethod()))
            .ToArray();

        var strategiesForEachParameter = factoryMethod.Parameters
            .Select(targetParameter =>
            {
                var usePropertyAttributes = this.context.MapMethod is not null
                    ? this.context.MapMethod.GetAttributes<MappaUsePropertyAttribute>()
                        .Where(attribute => attribute.TargetPropertyName.Equals(targetParameter.Name, StringComparison.OrdinalIgnoreCase))
                        .ToArray()
                    : [];

                string expectedSourcePropertyName;
                var useExactNameFromAttribute = false;
                switch (usePropertyAttributes.Length)
                {
                    case 0:
                        expectedSourcePropertyName = targetParameter.Name;
                        break;
                    case 1:
                        expectedSourcePropertyName = usePropertyAttributes[0].SourcePropertyName;
                        useExactNameFromAttribute = true;
                        break;
                    default:
                        this.context.ReportDiagnostic(MappaDiagnostics.TooManyUsePropertyAttributesForTheSameTargetProperty(
                            this.context.GetRootMapMethod().MethodDeclarationSyntax,
                            this.context.GetRootMapMethod().MethodName,
                            targetParameter.Name));
                        return (Parameter: targetParameter, Strategy: (MapStrategy)noMapStrategy);
                }

                PropertyMapNameMatcher.TryFindSourceProperty(
                    sourceProperties,
                    expectedSourcePropertyName,
                    this.context.MappaUserSettings.CaseInsensitivePropertyMap,
                    this.context.MappaUserSettings.IgnoreUnderscoreForPropertyMap,
                    isConstructorParameterPath: true,
                    useExactNameFromAttribute,
                    out IPropertySymbol? sourceProperty);

                if (this.context.MapMethod is not null &&
                    this.TryGetStrategyForPropertyOrArgumentUsingAttributesOnMethod(
                        targetParameter.Name,
                        targetParameter.Type,
                        this.context.SourceType,
                        ref sourceProperty,
                        StringComparison.OrdinalIgnoreCase,
                        isConstructorParameterPath: true,
                        out var propertyStrategyFromAttribute))
                {
                    propertyStrategyFromAttribute = this.EncapsulateMapStrategyForSourceOptional(sourceProperty, sourceProperties, propertyStrategyFromAttribute);
                    if (sourceProperty is null)
                    {
                        return (Parameter: targetParameter, Strategy: (MapStrategy)noMapStrategy);
                    }

                    var strategy = new ParameterMapStrategy(targetParameter, sourceProperty, propertyStrategyFromAttribute);
                    return (Parameter: targetParameter, Strategy: (MapStrategy)strategy);
                }

                if (sourceProperty is null)
                {
                    return (Parameter: targetParameter, Strategy: (MapStrategy)noMapStrategy);
                }

                var targetParameterType = targetParameter.Type;
                var sourcePropertyType = sourceProperty.Type;

                if (SymbolEqualityComparer.Default.Equals(targetParameterType, this.context.TargetType))
                {
                    return (Parameter: targetParameter, Strategy: (MapStrategy)noMapStrategy);
                }

                if (this.TryGetStrategyBetweenTypes(targetParameterType, sourcePropertyType, true, out var propertyStrategy))
                {
                    propertyStrategy = this.EncapsulateMapStrategyForSourceOptional(sourceProperty, sourceProperties, propertyStrategy);
                    var parameterMapStrategy = new ParameterMapStrategy(targetParameter, sourceProperty, propertyStrategy);
                    return (Parameter: targetParameter, Strategy: (MapStrategy)parameterMapStrategy);
                }

                return (Parameter: targetParameter, Strategy: (MapStrategy)noMapStrategy);
            })
            .ToArray();

        if (!Array.TrueForAll(strategiesForEachParameter, parameterAndStrategy => parameterAndStrategy.Strategy is not NoMapStrategy))
        {
            return false;
        }

        parametersMapStrategies = strategiesForEachParameter
            .Select(parameterAndStrategy => (ParameterMapStrategy)parameterAndStrategy.Strategy)
            .ToArray();
        return true;
    }

    private InvokeObjectFactoryMapStrategy EnrichInvokeObjectFactoryMapStrategyWithAssignToContext(
        InvokeObjectFactoryMapStrategy strategy)
    {
        // Nested (derived) contexts do not expose a MapMethod; attribute enrichment is root-only.
        if (this.context.MapMethod is null)
        {
            return strategy;
        }

        var attributes = this.context.MapMethod.GetAttributes<MappaAssignToContextAttribute>();
        if (attributes.Length == 0)
        {
            return strategy;
        }

        var methodDeclarationSyntax = this.context.MapMethod.MethodDeclarationSyntax
            ?? throw new MappaGeneratorException("Method declaration syntax has not been defined.");
        var rootMapMethod = this.context.GetRootMapMethod();
        var methodName = rootMapMethod.MethodName;
        var targetTypeName = this.context.TargetType.ToDisplayString();
        var providesContext = rootMapMethod.ProvideMappaContextWhenInvoked();

        var duplicateContextKeys = new HashSet<string>(
            attributes
                .GroupBy(attribute => attribute.ContextKey, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key),
            StringComparer.Ordinal);

        foreach (var duplicateContextKey in duplicateContextKeys)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.MultipleMappaAssignToContextAttributesUseTheSameContextKey(
                methodDeclarationSyntax,
                methodName,
                duplicateContextKey));
        }

        List<MappaAssignToContextEntry> assignToContextEntries = new();
        string? contextParameterName = null;

        foreach (var attribute in attributes)
        {
            if (duplicateContextKeys.Contains(attribute.ContextKey))
            {
                continue;
            }

            if (!providesContext)
            {
                this.context.ReportDiagnostic(MappaDiagnostics.CannotUseMappaAssignToContextAttributeWithoutContextParameter(
                    methodDeclarationSyntax,
                    methodName,
                    attribute.ContextKey));
                continue;
            }

            if (!this.TryResolveAssignToContextTargetMember(attribute.TargetPropertyName))
            {
                this.context.ReportDiagnostic(MappaDiagnostics.MappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible(
                    methodDeclarationSyntax,
                    methodName,
                    attribute.ContextKey,
                    attribute.TargetPropertyName,
                    targetTypeName));
                continue;
            }

            assignToContextEntries.Add(new MappaAssignToContextEntry(attribute.ContextKey, attribute.TargetPropertyName));
            contextParameterName ??= rootMapMethod.GetMappaContextParameterName();
        }

        if (assignToContextEntries.Count == 0)
        {
            return strategy;
        }

        return new InvokeObjectFactoryMapStrategy(
            strategy.TargetType,
            strategy.SourceType,
            strategy.ObjectFactory,
            strategy.ParametersMapStrategies,
            strategy.InitializerStrategies,
            [.. assignToContextEntries],
            contextParameterName);
    }

    private bool TryBuildEmptyCtorLikePropertyInitializers(
        bool requireAtLeastOneMappedProperty,
        out PropertyMapStrategy[] initializerStrategies)
    {
        initializerStrategies = [];
        var noMapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        var allTargetProperties = this.context.TargetType.GetTypeProperties().ToArray();
        var ignoredTargetPropertyNames = this.GetIgnoredTargetPropertyNames();

        var targetProperties = allTargetProperties
            .Where(property => !property.IsIndexer)
            .Where(property => !ignoredTargetPropertyNames.Contains(property.Name))
            .Where(property => !this.ShouldIgnoreTargetPropertyAtCurrentLevel(property.Name))
            .ToArray();

        if (targetProperties.Length == 0)
        {
            return !requireAtLeastOneMappedProperty;
        }

        var sourceProperties = this.context.SourceType.GetTypeProperties()
            .Where(property => !property.IsIndexer && property.IsGetterAccessible(this.compilation, this.context.GetRootMapMethod()))
            .ToArray();

        var mappedInitializerStrategies = targetProperties
            .Where(targetProperty => this.context.MappaUserSettings.ProtobufOptional is not BooleanSetting.Enable
                                     || allTargetProperties.All(otherProperty => !targetProperty.Name.Equals($"Has{otherProperty.Name}", StringComparison.Ordinal)))
            .Select(
                targetProperty =>
                {
                    var usePropertyAttributes = this.GetMatchingUsePropertyAttributes(
                        targetProperty.Name,
                        StringComparison.Ordinal);

                    string expectedSourcePropertyName;
                    var useExactNameFromAttribute = false;
                    PropertyPathContext? nestedPropertyPathContext = null;
                    ChainedSourcePropertyPathInfo? chainedSourcePropertyPath = null;
                    switch (usePropertyAttributes.Length)
                    {
                        case 0:
                            expectedSourcePropertyName = targetProperty.Name;
                            break;
                        case 1:
                        {
                            var usePropertyAttribute = usePropertyAttributes[0];
                            var isLeafTargetMapping = this.IsLeafTargetMappingForAttribute(usePropertyAttribute.TargetPropertyName);
                            if (this.TryResolveExpectedSourcePropertyName(
                                    usePropertyAttribute,
                                    isLeafTargetMapping,
                                    out expectedSourcePropertyName,
                                    out useExactNameFromAttribute,
                                    out nestedPropertyPathContext,
                                    out chainedSourcePropertyPath))
                            {
                                break;
                            }

                            useExactNameFromAttribute = true;
                            expectedSourcePropertyName = targetProperty.Name;
                            break;
                        }

                        default:
                        {
                            var distinctTargetPaths = usePropertyAttributes
                                .Select(attribute => attribute.TargetPropertyName)
                                .Distinct(StringComparer.Ordinal)
                                .ToArray();
                            if (distinctTargetPaths.Length == 1)
                            {
                                this.context.ReportDiagnostic(MappaDiagnostics.TooManyUsePropertyAttributesForTheSameTargetProperty(this.context.GetRootMapMethod().MethodDeclarationSyntax, this.context.GetRootMapMethod().MethodName, targetProperty.Name));
                                return new PropertyMapStrategy(targetProperty, null, noMapStrategy, false);
                            }

                            var distinctSourceRoots = usePropertyAttributes
                                .Select(attribute => PropertyPath.Parse(attribute.SourcePropertyName).GetFirstSegment())
                                .Where(segment => segment is not null)
                                .Distinct(StringComparer.Ordinal)
                                .ToArray();
                            if (distinctSourceRoots.Length == 1)
                            {
                                var sourceRoot = distinctSourceRoots[0];
                                if (sourceRoot is not null)
                                {
                                    expectedSourcePropertyName = sourceRoot;
                                    useExactNameFromAttribute = true;
                                }
                                else
                                {
                                    expectedSourcePropertyName = targetProperty.Name;
                                }
                            }
                            else
                            {
                                expectedSourcePropertyName = targetProperty.Name;
                            }

                            nestedPropertyPathContext = PropertyPathContext.CreateNestedAttributeScope(targetProperty.Name);
                            break;
                        }
                    }

                    if (this.context.PropertyPathContext is null
                        && this.HasNestedPathAttributesForTargetMember(targetProperty.Name, StringComparison.Ordinal)
                        && (nestedPropertyPathContext is null
                            || this.CountNestedPathAttributesForTargetMember(targetProperty.Name, StringComparison.Ordinal) > 1))
                    {
                        nestedPropertyPathContext = PropertyPathContext.CreateNestedAttributeScope(targetProperty.Name);
                    }

                    IPropertySymbol? sourceProperty = null;
                    if (chainedSourcePropertyPath is null)
                    {
                        PropertyMapNameMatcher.TryFindSourceProperty(
                            sourceProperties,
                            expectedSourcePropertyName,
                            this.context.MappaUserSettings.CaseInsensitivePropertyMap,
                            this.context.MappaUserSettings.IgnoreUnderscoreForPropertyMap,
                            isConstructorParameterPath: false,
                            useExactNameFromAttribute,
                            out sourceProperty);
                    }
                    else if (this.TryResolveChainedSourceProperty(
                                 chainedSourcePropertyPath,
                                 out var resolvedSourceProperties,
                                 out _))
                    {
                        sourceProperty = resolvedSourceProperties[0];
                    }

                    if ((this.context.MapMethod is not null || this.context.PropertyPathContext is not null)
                        && this.TryGetStrategyForPropertyOrArgumentUsingAttributesOnMethod(
                            targetProperty.Name,
                            targetProperty.Type,
                            this.context.SourceType,
                            ref sourceProperty,
                            StringComparison.Ordinal,
                            isConstructorParameterPath: false,
                            out var propertyStrategyFromAttribute))
                    {
                        if (!targetProperty.IsSetterAccessible(this.compilation, this.context.GetRootMapMethod()))
                        {
                            this.context.ReportDiagnostic(MappaDiagnostics.PropertySetterIsNotAccessible(this.context.GetRootMapMethod().MethodDeclarationSyntax, this.context.TargetType, targetProperty));
                            return new PropertyMapStrategy(targetProperty, null, noMapStrategy, false);
                        }

                        propertyStrategyFromAttribute = this.EncapsulateMapStrategyForSourceOptional(sourceProperty, sourceProperties, propertyStrategyFromAttribute);
                        propertyStrategyFromAttribute = this.EncapsulateMapStrategyForTargetOptional(targetProperty, allTargetProperties, propertyStrategyFromAttribute, out var postConstructorInitializer);
                        return new PropertyMapStrategy(targetProperty, sourceProperty, propertyStrategyFromAttribute, postConstructorInitializer, chainedSourcePropertyPath);
                    }

                    if (chainedSourcePropertyPath is not null
                        && this.TryResolveChainedSourceProperty(
                            chainedSourcePropertyPath,
                            out var chainedSourceProperties,
                            out _))
                    {
                        var innerSourceType = chainedSourceProperties[chainedSourceProperties.Length - 1].Type;
                        MapStrategy chainedPropertyStrategy = new IdentityMapStrategy(targetProperty.Type, innerSourceType);
                        chainedPropertyStrategy = this.EncapsulateMapStrategyForTargetOptional(targetProperty, allTargetProperties, chainedPropertyStrategy, out var chainedPostConstructorInitializer);
                        return new PropertyMapStrategy(
                            targetProperty,
                            null,
                            chainedPropertyStrategy,
                            chainedPostConstructorInitializer,
                            chainedSourcePropertyPath);
                    }

                    if (sourceProperty is not null &&
                        (targetProperty.SetMethod is null || (this.context.MapMethod is not null && targetProperty.SetMethod is not null && !targetProperty.IsSetterAccessible(this.compilation, this.context.MapMethod))) &&
                        targetProperty.GetMethod is not null)
                    {
                        if (targetProperty.Type.IsOrImplementIDictionary(this.compilation)
                            && sourceProperty.Type.IsOrImplementIDictionary(this.compilation)
                            && this.context.TryGetKeyAndValueStrategy(
                                targetProperty.Type,
                                sourceProperty.Type,
                                this.compilation,
                                out var keyStrategy,
                                out var valueStrategy,
                                this.cancellationToken))
                        {
                            var dictionaryPropertyStrategy = new ReadonlyDictionaryPropertyMapStrategy(
                                targetProperty,
                                sourceProperty,
                                keyStrategy,
                                valueStrategy,
                                DictionaryAssignmentSettingHelper.GetEffective(this.context.MappaUserSettings.DictionaryAssignment));
                            return new PropertyMapStrategy(targetProperty, sourceProperty, dictionaryPropertyStrategy, true);
                        }
                        else if ((targetProperty.Type.IsOrDerivedFromStack(this.compilation)
                                  || targetProperty.Type.IsOrDerivedFromConcurrentStack(this.compilation))
                                 && (sourceProperty.Type.IsArray() || sourceProperty.Type.IsOrImplementIEnumerable())
                                 && this.context.TryGetElementStrategy(
                                     targetProperty.Type,
                                     sourceProperty.Type,
                                     this.compilation,
                                     out var stackElementStrategy,
                                     this.cancellationToken))
                        {
                            var stackPropertyStrategy = new ReadonlyStackPropertyMapStrategy(targetProperty, sourceProperty, stackElementStrategy);
                            return new PropertyMapStrategy(targetProperty, sourceProperty, stackPropertyStrategy, true);
                        }
                        else if ((targetProperty.Type.IsOrDerivedFromQueue(this.compilation)
                                  || targetProperty.Type.IsOrImplementConcurrentQueue(this.compilation))
                                 && (sourceProperty.Type.IsArray() || sourceProperty.Type.IsOrImplementIEnumerable())
                                 && this.context.TryGetElementStrategy(
                                     targetProperty.Type,
                                     sourceProperty.Type,
                                     this.compilation,
                                     out var queueElementStrategy,
                                     this.cancellationToken))
                        {
                            var queuePropertyStrategy = new ReadonlyQueuePropertyMapStrategy(targetProperty, sourceProperty, queueElementStrategy);
                            return new PropertyMapStrategy(targetProperty, sourceProperty, queuePropertyStrategy, true);
                        }
                        else if ((targetProperty.Type.IsOrDerivedFromConcurrentBag(this.compilation)
                                  || targetProperty.Type.IsOrDerivedFromBlockingCollection(this.compilation))
                                 && (sourceProperty.Type.IsArray() || sourceProperty.Type.IsOrImplementIEnumerable())
                                 && this.context.TryGetElementStrategy(
                                     targetProperty.Type,
                                     sourceProperty.Type,
                                     this.compilation,
                                     out var addCollectionElementStrategy,
                                     this.cancellationToken))
                        {
                            var addCollectionPropertyStrategy = new ReadonlyAddCollectionPropertyMapStrategy(targetProperty, sourceProperty, addCollectionElementStrategy);
                            return new PropertyMapStrategy(targetProperty, sourceProperty, addCollectionPropertyStrategy, true);
                        }
                        else if (targetProperty.Type.IsOrImplementICollection()
                                 && (sourceProperty.Type.IsArray() || sourceProperty.Type.IsOrImplementIEnumerable())
                                 && this.context.TryGetElementStrategy(
                                     targetProperty.Type,
                                     sourceProperty.Type,
                                     this.compilation,
                                     out var elementStrategy,
                                     this.cancellationToken))
                        {
                            var collectionPropertyStrategy = new ReadonlyCollectionPropertyMapStrategy(targetProperty, sourceProperty, elementStrategy);
                            return new PropertyMapStrategy(targetProperty, sourceProperty, collectionPropertyStrategy, true);
                        }

                        return new PropertyMapStrategy(targetProperty, null, noMapStrategy, false);
                    }

                    if (sourceProperty is null)
                    {
                        return new PropertyMapStrategy(targetProperty, sourceProperty, noMapStrategy, false);
                    }

                    if (targetProperty.SetMethod is null || !targetProperty.IsSetterAccessible(this.compilation, this.context.GetRootMapMethod()))
                    {
                        return new PropertyMapStrategy(targetProperty, sourceProperty, noMapStrategy, false);
                    }

                    var targetPropertyType = targetProperty.Type;
                    var sourcePropertyType = sourceProperty.Type;

                    if (this.TryGetStrategyBetweenTypes(
                            targetPropertyType,
                            sourcePropertyType,
                            true,
                            ConstructorMapStrategyDetector.GetNestedTypeMappingPropertyPathContext(targetProperty.Name, nestedPropertyPathContext),
                            out var propertyStrategy))
                    {
                        propertyStrategy = this.EncapsulateMapStrategyForSourceOptional(sourceProperty, sourceProperties, propertyStrategy);
                        propertyStrategy = this.EncapsulateMapStrategyForTargetOptional(targetProperty, allTargetProperties, propertyStrategy, out var postConstructorInitializer);
                        return new PropertyMapStrategy(targetProperty, sourceProperty, propertyStrategy, postConstructorInitializer);
                    }

                    return new PropertyMapStrategy(targetProperty, null, noMapStrategy, false);
                })
            .ToArray();

        if (Array.Exists(mappedInitializerStrategies, propertyStrategy => propertyStrategy.TargetProperty.IsRequired && propertyStrategy.PropertyStrategy is NoMapStrategy))
        {
            return false;
        }

        var propertiesWithStrategies = mappedInitializerStrategies
            .Where(propertyStrategy => propertyStrategy.PropertyStrategy is not NoMapStrategy)
            .ToArray();

        var propertiesWithoutStrategy = mappedInitializerStrategies
            .Where(propertyStrategy => propertyStrategy.PropertyStrategy is NoMapStrategy)
            .ToArray();

        var mustMapAttribute = this.GetMustMapTargetPropertyAttribute();
        var mustMapFailed = false;

        foreach (var propertyWithoutStrategy in propertiesWithoutStrategy.Select(propertyStrategy => propertyStrategy.TargetProperty))
        {
            var targetCollections = propertyWithoutStrategy.Type.IsPostInitializationCollectionType(this.compilation);
            var hasSetter = propertyWithoutStrategy.SetMethod is not null && propertyWithoutStrategy.IsSetterAccessible(this.compilation, this.context.GetRootMapMethod());

            if (!hasSetter && !targetCollections)
            {
                continue;
            }

            var isMustMapCandidate = mustMapAttribute is not null
                                     && !propertyWithoutStrategy.IsRequired
                                     && (mustMapAttribute.TargetPropertyNames.Length == 0
                                         || mustMapAttribute.TargetPropertyNames.Contains(propertyWithoutStrategy.Name, StringComparer.Ordinal));

            if (isMustMapCandidate)
            {
                this.context.ReportDiagnostic(MappaDiagnostics.MustMapTargetPropertyWasNotMapped(
                    this.context.GetRootMapMethod().MethodDeclarationSyntax,
                    this.context.TargetType,
                    propertyWithoutStrategy));
                mustMapFailed = true;
            }
            else if (propertiesWithStrategies.Length > 0)
            {
                this.context.ReportDiagnostic(MappaDiagnostics.CannotMapNonRequiredProperty(
                    this.context.GetRootMapMethod().MethodDeclarationSyntax,
                    this.context.TargetType,
                    propertyWithoutStrategy));
            }
        }

        if (mustMapFailed)
        {
            return false;
        }

        if (requireAtLeastOneMappedProperty && propertiesWithStrategies.Length == 0)
        {
            return false;
        }

        initializerStrategies = propertiesWithStrategies;
        return true;
    }

    private MappaMustMapTargetPropertyAttribute? GetMustMapTargetPropertyAttribute()
    {
        if (this.context.AlgorithmSettings.UseAttributesForConstructorDetectorSettings
                .Equals(MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Disable))
        {
            return null;
        }

        if (this.context.MapMethod is null && this.context.PropertyPathContext is null)
        {
            return null;
        }

        return this.GetAttributeMapMethod().GetAttribute<MappaMustMapTargetPropertyAttribute>();
    }
}