// <copyright file="ConstructorMapStrategyDetector.cs" company="Stefano Anelli">
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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for the constructor strategies.
/// </summary>
internal sealed class ConstructorMapStrategyDetector
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context;
    private readonly Compilation compilation;
    private readonly CancellationToken cancellationToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructorMapStrategyDetector"/> class.
    /// </summary>
    /// <param name="context">The mappa class generator context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public ConstructorMapStrategyDetector(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        this.context = context;
        this.cancellationToken = cancellationToken;
        this.compilation = compilation;
    }

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        this.context.ValidateTargetNamesExist(this.compilation);
        this.context.ValidateMappaIgnoreTargetPropertyAttributes();

        // 01. Constructor TargetType(SourceType input) exists -> InvokeMappingConstructorStrategy ( IMapStrategy(T.InputParameterType, S) )
        if (this.CanInvokeMappingConstructor(out var invokeConstructor, out var argumentStrategy))
        {
            mapStrategy = new InvokeMappingConstructorMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                invokeConstructor,
                argumentStrategy);
        }

        // 02. Can map individual properties using an empty parameter constructor. -> InvokeConstructorMapStrategy( IMapStrategy[] parameters, IMapStrategy[] initProperties )
        else if (this.CanInvokeEmptyConstructor(out var emptyConstructorStrategy))
        {
            mapStrategy = emptyConstructorStrategy;
        }

        // 03. If there is no empty constructor then try identifying the best one -> InvokeConstructorMapStrategy( IMapStrategy[] parameters, IMapStrategy[] initProperties )
        else if (this.CanInvokeConstructorWithParameters(out var nonEmptyConstructorStrategy))
        {
            mapStrategy = nonEmptyConstructorStrategy;
        }

        if (mapStrategy is InvokeConstructorMapStrategy invokeConstructorMapStrategy)
        {
            mapStrategy = this.EnrichInvokeConstructorMapStrategyWithAssignToContext(invokeConstructorMapStrategy);
        }

        return mapStrategy is not NoMapStrategy;
    }

    private static void TryGetStrategyUsingMappaAssignFromConstantAttribute(
        ITypeSymbol targetType,
        MappaAssignFromConstantAttribute attribute,
        out MapStrategy strategy)
    {
        strategy = new MappaAssignFromConstantAttributeStrategy(targetType, attribute);
    }

    private bool CanInvokeConstructorWithParameters(out MapStrategy strategy)
    {
        var noMapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        strategy = noMapStrategy;

        // Detect all constructors that:
        // - Have at least one argument
        // - Are accessible
        // - Have a mapping for all parameters
        // We sort them in ascending order by number of parameters.
        var constructors = this.context.TargetType.GetAccessibleConstructors(this.compilation, this.context.ParentSymbol)
            .Where(constructor => constructor.Parameters.Length >= 1)
            .OrderByDescending(constructor => constructor.Parameters.Length)
            .ToArray();

        // If there is at least one constructor.
        if (constructors.Length > 0)
        {
            // Gets the source properties.
            var sourceProperties = this.context.SourceType.GetTypeProperties()

                // Ignore indexer properties.
                // Ignore properties without a setter.
                .Where(property => !property.IsIndexer && property.IsGetterAccessible(this.compilation, this.context.GetRootMapMethod()))
                .ToArray();

            // For each constructor identifier we get all the arguments,
            // and we try to match with a property of the source.
            var constructorsWithMappings = constructors.Select(methodSymbol =>
                {
                    // For each argument of the constructor
                    (IParameterSymbol Parameter, IPropertySymbol Property, MapStrategy Strategy)[] strategiesForEachParameter = methodSymbol.Parameters
                        .Select<IParameterSymbol, (IParameterSymbol Parameter, IPropertySymbol Property, MapStrategy Strategy)>(
                            targetParameter =>
                            {
                                var usePropertyAttributes = this.context.MapMethod is not null
                                    ? this.context.MapMethod.GetAttributes<MappaUsePropertyAttribute>().Where(attribute => attribute.TargetPropertyName.Equals(targetParameter.Name, StringComparison.OrdinalIgnoreCase)).ToArray()
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
                                        this.context.ReportDiagnostic(MappaDiagnostics.TooManyUsePropertyAttributesForTheSameTargetProperty(this.context.GetRootMapMethod().MethodDeclarationSyntax, this.context.GetRootMapMethod().MethodName, targetParameter.Name));
                                        return (targetParameter, null!, noMapStrategy);
                                }

                                PropertyMapNameMatcher.TryFindSourceProperty(
                                    sourceProperties,
                                    expectedSourcePropertyName,
                                    this.context.MappaUserSettings.CaseInsensitivePropertyMap,
                                    this.context.MappaUserSettings.IgnoreUnderscoreForPropertyMap,
                                    isConstructorParameterPath: true,
                                    useExactNameFromAttribute,
                                    out IPropertySymbol? sourceProperty);

                                // Look for any attribute action that can be applied
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
                                    var strategy = new ParameterMapStrategy(targetParameter, sourceProperty!, propertyStrategyFromAttribute);
                                    return (targetParameter, sourceProperty!, strategy);
                                }

                                if (sourceProperty is null)
                                {
                                    return (targetParameter, null!, noMapStrategy);
                                }

                                var targetParameterType = targetParameter.Type;
                                var sourcePropertyType = sourceProperty.Type;

                                // Prevent circular mapping if the target type of the parameter
                                // is the same type of the current type being mapped.
                                if (SymbolEqualityComparer.Default.Equals(targetParameterType, this.context.TargetType))
                                {
                                    return (targetParameter, null!, noMapStrategy);
                                }

                                // Get a strategy from source to target
                                if (this.TryGetStrategyBetweenTypes(targetParameterType, sourcePropertyType, true, out var propertyStrategy))
                                {
                                    propertyStrategy = this.EncapsulateMapStrategyForSourceOptional(sourceProperty, sourceProperties, propertyStrategy);
                                    var parameterMapStrategy = new ParameterMapStrategy(targetParameter, sourceProperty, propertyStrategy);
                                    return (targetParameter, sourceProperty, parameterMapStrategy);
                                }

                                // There is no mapping from source property to target parameter.
                                return (targetParameter, null!, noMapStrategy);
                            })
                        .ToArray();

                    return (methodSymbol, strategiesForEachParameter);
                })

                // Only select constructor for which all parameters are mapped
                .Where(constructorsAndMappings => Array.TrueForAll(constructorsAndMappings.strategiesForEachParameter, parameterAndStrategy => parameterAndStrategy.Strategy is not NoMapStrategy))
                .ToArray();

            // If there is more than one constructor we pick up the first one
            // because we sorted the constructors by number of parameters
            // so we can pick up the one with the highest number of parameters.
            if (constructorsWithMappings.Length > 0)
            {
                strategy = new InvokeConstructorMapStrategy(
                    this.context.TargetType,
                    this.context.SourceType,
                    constructorsWithMappings[0].methodSymbol,
                    constructorsWithMappings[0].strategiesForEachParameter
                        .Select(parameterAndStrategy => (ParameterMapStrategy)parameterAndStrategy.Strategy)
                        .ToArray(),
                    [],
                    [],
                    null);
            }
        }

        return strategy is not NoMapStrategy;
    }

    private MapStrategy EncapsulateMapStrategyForSourceOptional(
        IPropertySymbol? sourceProperty,
        IPropertySymbol[] sourceProperties,
        MapStrategy inputStrategy)
    {
        if (sourceProperty is null)
        {
            return inputStrategy;
        }

        if (this.context.MappaUserSettings.ProtobufOptional is not BooleanSetting.Enable)
        {
            return inputStrategy;
        }

        IPropertySymbol? hasProperty = Array.Find(sourceProperties, property => property.Name.Equals($"Has{sourceProperty.Name}", StringComparison.Ordinal));
        if (hasProperty is null)
        {
            return inputStrategy;
        }

        if (!hasProperty.Type.IsBoolean())
        {
            return inputStrategy;
        }

        return new OptionalSourcePropertyMapStrategy(inputStrategy, sourceProperty);
    }

    private MapStrategy EncapsulateMapStrategyForTargetOptional(
        IPropertySymbol targetProperty,
        IPropertySymbol[] allTargetProperties,
        MapStrategy inputStrategy,
        out bool requirePostConstructorInitialization)
    {
        requirePostConstructorInitialization = false;
        if (this.context.MappaUserSettings.ProtobufOptional is not BooleanSetting.Enable)
        {
            return inputStrategy;
        }

        if (targetProperty.IsIndexer)
        {
            return inputStrategy;
        }

        if (targetProperty.IsRequired)
        {
            return inputStrategy;
        }

        IPropertySymbol? hasProperty = Array.Find(allTargetProperties, property => property.Name.Equals($"Has{targetProperty.Name}", StringComparison.Ordinal));
        if (hasProperty is null)
        {
            return inputStrategy;
        }

        if (!hasProperty.Type.IsBoolean())
        {
            return inputStrategy;
        }

        requirePostConstructorInitialization = true;
        return new OptionalTargetPropertyMapStrategy(inputStrategy, targetProperty);
    }

    private bool CanInvokeMappingConstructor(out IMethodSymbol constructor, out MapStrategy strategy)
    {
        constructor = null!;
        var noMapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // Detect all constructors that:
        // - Have 1 argument
        // - Are accessible
        // - Have a mapping from source to the type of the parameter
        var constructors = this.context.TargetType.GetAccessibleConstructors(this.compilation, this.context.ParentSymbol, 1);
        var constructorsWithStrategy = constructors
            .Select<IMethodSymbol, (IMethodSymbol Constructor, MapStrategy Strategy)>(constructor =>
            {
                var constructorParameterType = constructor.Parameters.Single().Type;

                // Only use this strategy when they are the same type
                if (constructorParameterType.IsEqualTo(this.context.SourceType, this.context.GetRootMapMethod().NullableEnabled))
                {
                    return (constructor, new IdentityMapStrategy(constructorParameterType, this.context.SourceType));
                }

                return (constructor, noMapStrategy);
            })
            .Where(constructorAndStrategy => constructorAndStrategy.Strategy is not NoMapStrategy)
            .ToArray();

        // Either user the only one that has been found.
        if (constructorsWithStrategy.Length == 1)
        {
            constructor = constructorsWithStrategy.Single().Constructor;
            strategy = constructorsWithStrategy.Single().Strategy;
        }

        // If more than one has been found check if any of these
        // that has the very same input type and ise that for the mapping.
        else
        {
            var constructorWithSameInputTypeAsSource = constructorsWithStrategy
                .Where(constructorWithStrategy =>
                    constructorWithStrategy.Constructor.Parameters.Single().Type.IsEqualTo(this.context.SourceType, this.context.IsNullableEnabled()))
                .ToArray();

            if (constructorWithSameInputTypeAsSource.Length > 0)
            {
                constructor = constructorWithSameInputTypeAsSource.Single().Constructor;
                strategy = constructorWithSameInputTypeAsSource.Single().Strategy;
            }

            // No matching constructor has been found
            else
            {
                strategy = noMapStrategy;
            }
        }

        return strategy is not NoMapStrategy;
    }

    private bool CanInvokeEmptyConstructor(out MapStrategy strategy)
    {
        var noMapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // Detect all constructors that:
        // - Have 0 parameter
        // - Is accessible
        var constructors = this.context.TargetType.GetAccessibleConstructors(this.compilation, this.context.ParentSymbol, 0);

        // If there is no constructor with zero parameters cannot apply this strategy.
        if (constructors.Length == 0)
        {
            strategy = noMapStrategy;
        }
        else
        {
            // Gets all the properties
            var allTargetProperties = this.context.TargetType.GetTypeProperties().ToArray();

            // Gets the target properties
            var ignoredTargetPropertyNames = this.GetIgnoredTargetPropertyNames();

            var targetProperties = allTargetProperties

                // Ignore indexer properties.
                .Where(property => !property.IsIndexer)

                // Ignore properties marked via MappaIgnoreTargetPropertyAttribute.
                .Where(property => !ignoredTargetPropertyNames.Contains(property.Name))
                .ToArray();

            // If no target properties exist, then there is no point in applying this strategy
            // this way we can avoid using this strategy for basic types
            // like string, int, etc...
            if (targetProperties.Length > 0)
            {
                // Gets the source properties.
                var sourceProperties = this.context.SourceType.GetTypeProperties()

                    // Ignore indexer properties.
                    // Ignore properties without a setter.
                    .Where(property => !property.IsIndexer && property.IsGetterAccessible(this.compilation, this.context.GetRootMapMethod()))
                    .ToArray();

                // Match target property with a source property.
                var initializerStrategies = targetProperties

                    // Remove all the optional identifier properties from the list
                    // when the optional setting is enabled.
                    .Where(targetProperty => this.context.MappaUserSettings.ProtobufOptional is not BooleanSetting.Enable
                                             || allTargetProperties.All(otherProperty => !targetProperty.Name.Equals($"Has{otherProperty.Name}", StringComparison.Ordinal)))

                    // Look up for mapping
                    .Select(
                        targetProperty =>
                        {
                            // Try to get a matching property
                            var usePropertyAttributes = this.context.MapMethod is not null
                                ? this.context.MapMethod.GetAttributes<MappaUsePropertyAttribute>().Where(attribute => attribute.TargetPropertyName.Equals(targetProperty.Name, StringComparison.Ordinal)).ToArray()
                                : [];

                            string expectedSourcePropertyName;
                            var useExactNameFromAttribute = false;
                            switch (usePropertyAttributes.Length)
                            {
                                case 0:
                                    expectedSourcePropertyName = targetProperty.Name;
                                    break;
                                case 1:
                                    expectedSourcePropertyName = usePropertyAttributes[0].SourcePropertyName;
                                    useExactNameFromAttribute = true;
                                    break;
                                default:
                                    this.context.ReportDiagnostic(MappaDiagnostics.TooManyUsePropertyAttributesForTheSameTargetProperty(this.context.GetRootMapMethod().MethodDeclarationSyntax, this.context.GetRootMapMethod().MethodName, targetProperty.Name));
                                    return new PropertyMapStrategy(targetProperty, null, noMapStrategy, false);
                            }

                            PropertyMapNameMatcher.TryFindSourceProperty(
                                sourceProperties,
                                expectedSourcePropertyName,
                                this.context.MappaUserSettings.CaseInsensitivePropertyMap,
                                this.context.MappaUserSettings.IgnoreUnderscoreForPropertyMap,
                                isConstructorParameterPath: false,
                                useExactNameFromAttribute,
                                out IPropertySymbol? sourceProperty);

                            // Look for any attribute action that can be applied
                            if (this.context.MapMethod is not null
                                && this.TryGetStrategyForPropertyOrArgumentUsingAttributesOnMethod(
                                    targetProperty.Name,
                                    targetProperty.Type,
                                    this.context.SourceType,
                                    ref sourceProperty,
                                    StringComparison.Ordinal,
                                    isConstructorParameterPath: false,
                                    out var propertyStrategyFromAttribute))
                            {
                                if (!targetProperty.IsSetterAccessible(this.compilation, this.context.MapMethod))
                                {
                                    this.context.ReportDiagnostic(MappaDiagnostics.PropertySetterIsNotAccessible(this.context.GetRootMapMethod().MethodDeclarationSyntax, this.context.TargetType, targetProperty));
                                    return new PropertyMapStrategy(targetProperty, null, noMapStrategy, false);
                                }

                                propertyStrategyFromAttribute = this.EncapsulateMapStrategyForSourceOptional(sourceProperty, sourceProperties, propertyStrategyFromAttribute);
                                propertyStrategyFromAttribute = this.EncapsulateMapStrategyForTargetOptional(targetProperty, allTargetProperties, propertyStrategyFromAttribute, out var postConstructorInitializer);
                                return new PropertyMapStrategy(targetProperty, sourceProperty, propertyStrategyFromAttribute, postConstructorInitializer);
                            }

                            // Look up for post initialization collection properties
                            if (sourceProperty is not null &&
                                (targetProperty.SetMethod is null || (this.context.MapMethod is not null && targetProperty.SetMethod is not null && !targetProperty.IsSetterAccessible(this.compilation, this.context.MapMethod))) &&
                                targetProperty.GetMethod is not null)
                            {
                                // Check if it implements IDictionary<K, V>
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

                                // Check if it is or derives from Stack<T> or ConcurrentStack<T>
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

                                // Check if it is or derives from Queue<T> or ConcurrentQueue<T>
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

                                // Check if it is or derives from ConcurrentBag<T> or BlockingCollection<T>
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

                                // Check if it implements ICollection<T>
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

                            // Look for a matching source property
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

                            if (this.TryGetStrategyBetweenTypes(targetPropertyType, sourcePropertyType, true, out var propertyStrategy))
                            {
                                propertyStrategy = this.EncapsulateMapStrategyForSourceOptional(sourceProperty, sourceProperties, propertyStrategy);
                                propertyStrategy = this.EncapsulateMapStrategyForTargetOptional(targetProperty, allTargetProperties, propertyStrategy, out var postConstructorInitializer);
                                return new PropertyMapStrategy(targetProperty, sourceProperty, propertyStrategy, postConstructorInitializer);
                            }

                            return new PropertyMapStrategy(targetProperty, null, noMapStrategy, false);
                        })
                    .ToArray();

                // Check if any property strategy is required but no strategy has been found
                if (Array.Exists(initializerStrategies, propertyStrategy => propertyStrategy.TargetProperty.IsRequired && propertyStrategy.PropertyStrategy is NoMapStrategy))
                {
                    strategy = noMapStrategy;
                }
                else
                {
                    // Filter out properties that have no mapping.
                    // TODO [#20] Allow to prevent skipping some non required parameters.
                    var propertiesWithStrategies = initializerStrategies
                        .Where(propertyStrategy => propertyStrategy.PropertyStrategy is not NoMapStrategy)
                        .ToArray();

                    var propertiesWithoutStrategy = initializerStrategies
                        .Where(propertyStrategy => propertyStrategy.PropertyStrategy is NoMapStrategy)
                        .ToArray();

                    // If no property can be mapped, then we should not be applying this.
                    if (propertiesWithStrategies.Length > 0)
                    {
                        // Report a warning for every property that cannot be mapped.
                        foreach (var propertyWithoutStrategy in propertiesWithoutStrategy.Select(propertyStrategy => propertyStrategy.TargetProperty))
                        {
                            // Check if targets a collections that could be filled even without a getter
                            var targetCollections = propertyWithoutStrategy.Type.IsPostInitializationCollectionType(this.compilation);
                            var hasSetter = propertyWithoutStrategy.SetMethod is not null && propertyWithoutStrategy.IsSetterAccessible(this.compilation, this.context.GetRootMapMethod());

                            if (hasSetter || targetCollections)
                            {
                                // Report diagnostics
                                this.context.ReportDiagnostic(MappaDiagnostics.CannotMapNonRequiredProperty(
                                    this.context.GetRootMapMethod().MethodDeclarationSyntax,
                                    this.context.TargetType,
                                    propertyWithoutStrategy));
                            }
                        }

                        strategy = new InvokeConstructorMapStrategy(
                            this.context.TargetType,
                            this.context.SourceType,
                            constructors.Single(),
                            [],
                            propertiesWithStrategies,
                            [],
                            null);
                    }
                    else
                    {
                        strategy = noMapStrategy;
                    }
                }
            }
            else
            {
                strategy = noMapStrategy;
            }
        }

        return strategy is not NoMapStrategy;
    }

    private InvokeConstructorMapStrategy EnrichInvokeConstructorMapStrategyWithAssignToContext(
        InvokeConstructorMapStrategy strategy)
    {
        if (this.context.AlgorithmSettings.UseAttributesForConstructorDetectorSettings
                .Equals(MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Disable)
            || this.context.MapMethod is null)
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

        return new InvokeConstructorMapStrategy(
            strategy.TargetType,
            strategy.SourceType,
            strategy.Constructor,
            strategy.ParametersMapStrategies,
            strategy.InitializerStrategies,
            [.. assignToContextEntries],
            contextParameterName);
    }

    private bool TryResolveAssignToContextTargetMember(string memberName)
    {
        var rootMapMethod = this.context.GetRootMapMethod();
        var targetType = this.context.TargetType;

        var property = targetType
            .GetTypeProperties()
            .FirstOrDefault(candidate => candidate.Name.Equals(memberName, StringComparison.Ordinal));

        if (property is not null)
        {
            return property.IsGetterAccessible(this.compilation, rootMapMethod);
        }

        return this.TryFindAccessibleTargetField(memberName, rootMapMethod) is not null;
    }

    private IFieldSymbol? TryFindAccessibleTargetField(string fieldName, MapMethod rootMapMethod)
    {
        ITypeSymbol? currentType = this.context.TargetType;
        while (currentType is not null)
        {
            var field = currentType.GetMembers()
                .OfType<IFieldSymbol>()
                .FirstOrDefault(candidate =>
                    candidate.Name.Equals(fieldName, StringComparison.Ordinal)
                    && this.compilation.IsSymbolAccessibleWithin(candidate, rootMapMethod.MethodSymbol.ContainingSymbol));

            if (field is not null)
            {
                return field;
            }

            currentType = currentType.BaseType;
        }

        return null;
    }

    private HashSet<string> GetIgnoredTargetPropertyNames()
    {
        if (this.context.AlgorithmSettings.UseAttributesForConstructorDetectorSettings
                .Equals(MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Disable)
            || this.context.MapMethod is null)
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }

        return new HashSet<string>(
            this.context.MapMethod.GetAttributes<MappaIgnoreTargetPropertyAttribute>()
                .Select(attribute => attribute.TargetPropertyName),
            StringComparer.Ordinal);
    }

    private bool TryGetStrategyBetweenTypes(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool useConstructorMapStrategyDetector,
        out MapStrategy elementStrategy)
    {
        using (this.context.AlgorithmSettings.UseConstructorMapStrategyDetector.Apply(useConstructorMapStrategyDetector))
        {
            using (this.context.AlgorithmSettings.UseAttributesForConstructorDetectorSettings.Apply(MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Disable))
            {
                var derivedContext = new DerivedMappaMapAlgorithmContext(
                    this.context,
                    targetType,
                    sourceType);
                var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(derivedContext, this.compilation, this.cancellationToken);
                elementStrategy = algorithm.GetStrategy();
                return elementStrategy is not NoMapStrategy;
            }
        }
    }

    private bool TryGetStrategyForPropertyOrArgumentUsingAttributesOnMethod(
        string targetName,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        ref IPropertySymbol? sourceProperty,
        StringComparison stringComparison,
        bool isConstructorParameterPath,
        out MapStrategy strategy)
    {
        strategy = new NoMapStrategy(targetType, null!);

        if (this.context.AlgorithmSettings.UseAttributesForConstructorDetectorSettings
            .Equals(MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Disable))
        {
            return false;
        }

        if (this.context.MapMethod is null)
        {
            throw new MappaGeneratorException("Map method needs to be defined.");
        }

        var matchingAttributes = this.context.MapMethod
            .GetAttributes<Attribute>()
            .OfType<IMappaTargetPropertyNameAttribute>()
            .Where(attribute => attribute.TargetPropertyName.Equals(targetName, stringComparison))
            .ToArray();

        // No such attribute.
        if (matchingAttributes.Length <= 0)
        {
            return false;
        }

        // Too many attributes!
        if (matchingAttributes.Length > 1)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.MultipleAttributesTargetTheSamePropertyOrParameter(
                this.context.MapMethod.MethodDeclarationSyntax ?? throw new MappaGeneratorException("Method declarations syntax has not been defined."),
                targetName));
            return false;
        }

        // Apply the unique attribute that has been discovered.
        var attribute = matchingAttributes.Single();
        switch (attribute)
        {
            case MappaInvokeMethodAttribute mappaInvokeMethodAttribute:
                if (!string.IsNullOrWhiteSpace(mappaInvokeMethodAttribute.SourcePropertyName))
                {
                    this.ReportMappaUsePropertySourcePropertyWillNotBeUsedIfPresent(
                        targetName,
                        stringComparison,
                        nameof(MappaInvokeMethodAttribute));
                    this.TryResolveSourcePropertyForMappaInvokeMethodAttribute(
                        mappaInvokeMethodAttribute,
                        sourceClassType,
                        isConstructorParameterPath,
                        ref sourceProperty);
                }

                this.TryGetStrategyUsingMappaInvokeMethodAttribute(
                    targetName,
                    targetType,
                    sourceClassType,
                    sourceProperty,
                    mappaInvokeMethodAttribute,
                    stringComparison,
                    out strategy);
                break;
            case MappaAssignFromContextAttribute mappaAssignFromContextAttribute:
                this.TryGetStrategyUsingMappaAssignFromContextAttribute(
                    targetName,
                    targetType,
                    mappaAssignFromContextAttribute,
                    ref sourceProperty,
                    out strategy);
                if (strategy is not NoMapStrategy)
                {
                    this.ReportMappaUsePropertySourcePropertyWillNotBeUsedIfPresent(
                        targetName,
                        stringComparison,
                        nameof(MappaAssignFromContextAttribute));
                }

                break;
            case MappaAssignFromConstantAttribute mappaAssignFromConstantAttribute:
                TryGetStrategyUsingMappaAssignFromConstantAttribute(
                    targetType,
                    mappaAssignFromConstantAttribute,
                    out strategy);
                if (strategy is not NoMapStrategy)
                {
                    this.ReportMappaUsePropertySourcePropertyWillNotBeUsedIfPresent(
                        targetName,
                        stringComparison,
                        nameof(MappaAssignFromConstantAttribute));
                }

                break;
        }

        return strategy is not NoMapStrategy;
    }

    private void ReportMappaUsePropertySourcePropertyWillNotBeUsedIfPresent(
        string targetName,
        StringComparison stringComparison,
        string conflictingAttributeName)
    {
        if (this.context.MapMethod is null)
        {
            return;
        }

        var methodDeclarationSyntax = this.context.MapMethod.MethodDeclarationSyntax;
        if (methodDeclarationSyntax is null)
        {
            return;
        }

        var usePropertyAttributes = this.context.MapMethod
            .GetAttributes<MappaUsePropertyAttribute>()
            .Where(attribute => attribute.TargetPropertyName.Equals(targetName, stringComparison))
            .ToArray();

        if (usePropertyAttributes.Length != 1)
        {
            return;
        }

        this.context.ReportDiagnostic(MappaDiagnostics.MappaUsePropertySourcePropertyWillNotBeUsed(
            methodDeclarationSyntax,
            this.context.GetRootMapMethod().MethodName,
            targetName,
            usePropertyAttributes[0].SourcePropertyName,
            conflictingAttributeName));
    }

    private void TryResolveSourcePropertyForMappaInvokeMethodAttribute(
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        ITypeSymbol sourceClassType,
        bool isConstructorParameterPath,
        ref IPropertySymbol? sourceProperty)
    {
        if (mappaInvokeMethodAttribute.SourcePropertyName is not string sourcePropertyName ||
            string.IsNullOrWhiteSpace(sourcePropertyName))
        {
            return;
        }

        var sourceProperties = sourceClassType.GetTypeProperties()
            .Where(property => !property.IsIndexer && property.IsGetterAccessible(this.compilation, this.context.GetRootMapMethod()))
            .ToArray();

        PropertyMapNameMatcher.TryFindSourceProperty(
            sourceProperties,
            sourcePropertyName,
            this.context.MappaUserSettings.CaseInsensitivePropertyMap,
            this.context.MappaUserSettings.IgnoreUnderscoreForPropertyMap,
            isConstructorParameterPath,
            useExactNameFromAttribute: true,
            out sourceProperty);
    }

    private void TryGetStrategyUsingMappaAssignFromContextAttribute(
        string targetName,
        ITypeSymbol targetType,
        MappaAssignFromContextAttribute attribute,
        ref IPropertySymbol? sourceProperty,
        out MapStrategy strategy)
    {
        strategy = new NoMapStrategy(targetType, null!);

        var mapMethod = this.context.MapMethod ?? throw new MappaGeneratorException("Map method needs to be defined.");
        var mapMethodMethodDeclarationSyntax = mapMethod.MethodDeclarationSyntax ?? throw new MappaGeneratorException("Method declaration syntax has not been defined.");

        var rootMapMethod = this.context.GetRootMapMethod();
        if (rootMapMethod.ProvideMappaContextWhenInvoked())
        {
            sourceProperty = null; // Ignore any input property.
            strategy = new MappaAssignFromContextAttributeStrategy(targetType, attribute, rootMapMethod.GetMappaContextParameterName());
        }
        else
        {
            this.context.ReportDiagnostic(MappaDiagnostics.CannotUseMappaAssignFromContextAttributeWithoutContextParameter(
                mapMethodMethodDeclarationSyntax,
                targetName));
        }
    }

    private void TryGetStrategyUsingMappaInvokeMethodAttribute(
        string targetName,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        IPropertySymbol? sourceProperty,
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        StringComparison stringComparison,
        out MapStrategy strategy)
    {
        strategy = new NoMapStrategy(targetType, null!);

        var mapMethod = this.context.MapMethod ?? throw new MappaGeneratorException("Map method needs to be defined.");
        var mapMethodMethodDeclarationSyntax = mapMethod.MethodDeclarationSyntax ?? throw new MappaGeneratorException("Method declaration syntax has not been defined.");
        var mapMethodClass = (INamedTypeSymbol)mapMethod.MethodSymbol.ContainingSymbol;

        var rootMethod = this.context.GetRootMapMethod();
        ISymbol? fieldOrProperty = null;
        InvokeMethodResolutionResult resolutionResult;
        IMethodSymbol? method;
        if (mappaInvokeMethodAttribute.FieldName is not null)
        {
            fieldOrProperty = this.compilation.LocateAccessibleFieldOrPropertyInTypeHierarchy(
                mapMethodClass,
                mappaInvokeMethodAttribute.FieldName,
                mapMethodClass);

            if (fieldOrProperty is null)
            {
                this.context.ReportDiagnostic(MappaDiagnostics.CannotFindFieldOrProperty(
                    mapMethodMethodDeclarationSyntax,
                    mappaInvokeMethodAttribute.FieldName));
                return;
            }

            if (rootMethod.MethodSymbol.IsStatic && !fieldOrProperty.IsStatic)
            {
                this.context.ReportDiagnostic(MappaDiagnostics.FieldOrPropertyMustBeStatic(
                      fieldOrProperty.Name,
                      rootMethod.Location));
                return;
            }

            var fieldOrPropertyType = fieldOrProperty switch
            {
                IFieldSymbol field => field.Type,
                IPropertySymbol property => property.Type,
                _ => throw new MappaGeneratorException($"Unexpected symbol kind '{fieldOrProperty.Kind}' for field or property '{fieldOrProperty.Name}'."),
            };

            resolutionResult = this.TryResolveInvokeMethodForAttribute(
                mapMethodClass,
                fieldOrPropertyType.LocateMethods(mappaInvokeMethodAttribute.MethodName),
                mappaInvokeMethodAttribute.MethodName,
                targetType,
                sourceClassType,
                sourceProperty,
                InvokeMethodStaticRequirement.NotStatic,
                rootMethod,
                mapMethodMethodDeclarationSyntax,
                out method);
        }
        else if (mappaInvokeMethodAttribute.ClassType is not null)
        {
            var className = this.compilation.GetTypeByMetadataName(
                mappaInvokeMethodAttribute.ClassType.FullName ?? throw new MappaGeneratorException("Cannot detect type full name"));
            if (className is null)
            {
                this.context.ReportDiagnostic(MappaDiagnostics.CannotDetectType(
                    mapMethodMethodDeclarationSyntax,
                    mappaInvokeMethodAttribute.ClassType.FullName!));
                return;
            }

            resolutionResult = this.TryResolveInvokeMethodForAttribute(
                mapMethodClass,
                className.LocateMethods(mappaInvokeMethodAttribute.MethodName),
                mappaInvokeMethodAttribute.MethodName,
                targetType,
                sourceClassType,
                sourceProperty,
                InvokeMethodStaticRequirement.Static,
                rootMethod,
                mapMethodMethodDeclarationSyntax,
                out method);
        }
        else
        {
            var rootMapMethod = rootMethod;
            var staticRequirement = rootMapMethod.MethodSymbol.IsStatic
                ? InvokeMethodStaticRequirement.Static
                : InvokeMethodStaticRequirement.StaticOrNotStatic;

            resolutionResult = this.TryResolveInvokeMethodForAttribute(
                mapMethodClass,
                mapMethodClass.LocateMethods(mappaInvokeMethodAttribute.MethodName),
                mappaInvokeMethodAttribute.MethodName,
                targetType,
                sourceClassType,
                sourceProperty,
                staticRequirement,
                rootMethod,
                mapMethodMethodDeclarationSyntax,
                out method);
        }

        if (resolutionResult is InvokeMethodResolutionResult.Ambiguous)
        {
            return;
        }

        if (resolutionResult is not InvokeMethodResolutionResult.Success || method is null)
        {
            var displayClassName = mappaInvokeMethodAttribute.ClassType is not null
                ? mappaInvokeMethodAttribute.ClassType.FullName ?? "unknown"
                : mapMethodClass.ToDisplayString();
            this.context.ReportDiagnostic(MappaDiagnostics.CannotDetectSuitableMethodToInvokeForParameter(
                mapMethodMethodDeclarationSyntax,
                targetName,
                mappaInvokeMethodAttribute.MethodName,
                displayClassName));
            return;
        }

        var contextParameterName = method.MethodHasMappaContextParameter(this.compilation)
            ? rootMethod.MaybeGetMappaContextParameterName()
            : null;

        strategy = new MappaInvokeMethodAttributeStrategy(
            targetType,
            sourceClassType,
            mappaInvokeMethodAttribute,
            fieldOrProperty,
            method,
            sourceProperty,
            this.context.MapMethod.NullableEnabled,
            contextParameterName);

        var usePropertyAttributes = mapMethod
            .GetAttributes<MappaUsePropertyAttribute>()
            .Where(attribute => attribute.TargetPropertyName.Equals(targetName, stringComparison))
            .ToArray();

        string? explicitSourcePropertyName = null;
        if (!string.IsNullOrWhiteSpace(mappaInvokeMethodAttribute.SourcePropertyName))
        {
            explicitSourcePropertyName = mappaInvokeMethodAttribute.SourcePropertyName;
        }
        else if (usePropertyAttributes.Length == 1)
        {
            explicitSourcePropertyName = usePropertyAttributes[0].SourcePropertyName;
        }

        if (explicitSourcePropertyName is not null &&
            !method.UsesSourceProperty(
                this.compilation,
                sourceProperty,
                sourceClassType,
                this.context.IsNullableEnabled()))
        {
            this.context.ReportDiagnostic(MappaDiagnostics.MappaUsePropertyNotUsedByInvokeMethod(
                mapMethodMethodDeclarationSyntax,
                this.context.GetRootMapMethod().MethodName,
                targetName,
                explicitSourcePropertyName,
                mappaInvokeMethodAttribute.MethodName));
        }
    }

    private InvokeMethodResolutionResult TryResolveInvokeMethodForAttribute(
        ITypeSymbol mapClass,
        IMethodSymbol[] methods,
        string methodName,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        IPropertySymbol? sourceProperty,
        InvokeMethodStaticRequirement staticRequirement,
        MapMethod rootMapMethod,
        MethodDeclarationSyntax mapMethodMethodDeclarationSyntax,
        out IMethodSymbol? method)
    {
        var resolutionResult = InvokeMethodResolution.TryResolveMappaInvokeMethod(
            this.compilation,
            mapClass,
            methods,
            methodName,
            targetType,
            sourceClassType,
            sourceProperty,
            this.context.IsNullableEnabled(),
            staticRequirement,
            rootMapMethod,
            out method,
            out var ambiguityDetails);

        if (resolutionResult is InvokeMethodResolutionResult.Ambiguous)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.AmbiguousInvokeMethodResolution(
                mapMethodMethodDeclarationSyntax.GetLocation(),
                ambiguityDetails));
        }

        return resolutionResult;
    }
}