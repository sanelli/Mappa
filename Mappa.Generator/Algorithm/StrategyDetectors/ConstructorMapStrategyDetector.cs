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
internal sealed partial class ConstructorMapStrategyDetector
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
        this.context.ValidateMappaMustMapTargetPropertyAttributes();
        this.context.ValidateMappaAllowInaccessibleMembersAttributes(this.compilation);

        // 00. Object factory registered for TargetType -> InvokeObjectFactoryMapStrategy
        if (this.TryDetectObjectFactory(out var objectFactoryStrategy))
        {
            mapStrategy = objectFactoryStrategy;
        }

        // 01. Constructor TargetType(SourceType input) exists -> InvokeMappingConstructorStrategy ( IMapStrategy(T.InputParameterType, S) )
        else if (this.CanInvokeMappingConstructor(out var invokeConstructor, out var argumentStrategy))
        {
            mapStrategy = new InvokeMappingConstructorMapStrategy(
                this.context.TargetType,
                this.context.SourceType,
                invokeConstructor,
                argumentStrategy,
                this.RequiresUnsafeAccessorForConstructor(invokeConstructor));
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
        else if (mapStrategy is InvokeObjectFactoryMapStrategy invokeObjectFactoryMapStrategy)
        {
            mapStrategy = this.EnrichInvokeObjectFactoryMapStrategyWithAssignToContext(invokeObjectFactoryMapStrategy);
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
        // - Are accessible (or inaccessible constructors are opted in)
        // - Have a mapping for all parameters
        // We sort them in ascending order by number of parameters.
        var constructors = this.GetInvokableConstructors()
            .Where(constructor => constructor.Parameters.Length >= 1)
            .OrderByDescending(constructor => constructor.Parameters.Length)
            .ToArray();

        // If there is at least one constructor.
        if (constructors.Length > 0)
        {
            // Gets the source properties.
            var sourceProperties = this.GetReadableSourceProperties(this.context.SourceType);

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
                                    var requiresUnsafeAccessorOnSource = sourceProperty is not null
                                        && this.TryIsSourcePropertyReadable(sourceProperty, out var sourceRequiresUnsafe)
                                        && sourceRequiresUnsafe;
                                    var strategy = new ParameterMapStrategy(targetParameter, sourceProperty!, propertyStrategyFromAttribute, requiresUnsafeAccessorOnSource);
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
                                    this.TryIsSourcePropertyReadable(sourceProperty, out var requiresUnsafeAccessorOnSource);
                                    var parameterMapStrategy = new ParameterMapStrategy(targetParameter, sourceProperty, propertyStrategy, requiresUnsafeAccessorOnSource);
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
                var selectedConstructor = constructorsWithMappings[0].methodSymbol;
                strategy = new InvokeConstructorMapStrategy(
                    this.context.TargetType,
                    this.context.SourceType,
                    selectedConstructor,
                    constructorsWithMappings[0].strategiesForEachParameter
                        .Select(parameterAndStrategy => (ParameterMapStrategy)parameterAndStrategy.Strategy)
                        .ToArray(),
                    [],
                    [],
                    null,
                    this.RequiresUnsafeAccessorForConstructor(selectedConstructor));
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
        // - Are accessible (or inaccessible constructors are opted in)
        // - Have a mapping from source to the type of the parameter
        var constructors = this.GetInvokableConstructors(1);
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
        // - Are accessible (or inaccessible constructors are opted in)
        var constructors = this.GetInvokableConstructors(0);

        // If there is no constructor with zero parameters cannot apply this strategy.
        if (constructors.Length == 0)
        {
            strategy = noMapStrategy;
            return false;
        }

        if (!this.TryBuildEmptyCtorLikePropertyInitializers(
                requireAtLeastOneMappedProperty: true,
                out var propertiesWithStrategies))
        {
            strategy = noMapStrategy;
            return false;
        }

        var selectedConstructor = constructors[0];
        strategy = new InvokeConstructorMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            selectedConstructor,
            [],
            propertiesWithStrategies,
            [],
            null,
            this.RequiresUnsafeAccessorForConstructor(selectedConstructor));

        return true;
    }

    private InvokeConstructorMapStrategy EnrichInvokeConstructorMapStrategyWithAssignToContext(
        InvokeConstructorMapStrategy strategy)
    {
        var attributesEnabled = !this.context.AlgorithmSettings.UseAttributesForConstructorDetectorSettings
            .Equals(MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Disable);

        if (!this.TryBuildAssignToContextEnrichment(attributesEnabled, out var entries, out var contextParameterName))
        {
            return strategy;
        }

        return new InvokeConstructorMapStrategy(
            strategy.TargetType,
            strategy.SourceType,
            strategy.Constructor,
            strategy.ParametersMapStrategies,
            strategy.InitializerStrategies,
            entries,
            contextParameterName,
            strategy.RequiresUnsafeAccessorOnConstructor);
    }

    private bool TryResolveAssignToContextTargetMember(string memberName)
    {
        var rootMapMethod = this.context.GetRootMapMethod();
        var targetType = this.context.TargetType;
        var parsedPath = PropertyPath.Parse(memberName);

        if (parsedPath.IsNested)
        {
            return PropertyPathSymbolResolver.TryResolveTargetMemberPath(
                targetType,
                parsedPath,
                out _,
                out _);
        }

        var property = targetType
            .GetTypeProperties()
            .FirstOrDefault(candidate => candidate.Name.Equals(memberName, StringComparison.Ordinal));

        if (property is not null)
        {
            return this.TryIsTargetPropertyGetterReadable(property, out _);
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
                    && this.compilation.IsSymbolAccessibleWithin(candidate, rootMapMethod.ContainingType));

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
                .Equals(MappaMapAlgorithmContextSettings.MappaAttributesForConstructorDetectorSettings.Disable))
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }

        if (this.context.MapMethod is null && this.context.PropertyPathContext is null)
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }

        return new HashSet<string>(
            this.GetAttributeMapMethod().GetAttributes<MappaIgnoreTargetPropertyAttribute>()
                .Select(attribute => PropertyPath.Parse(attribute.TargetPropertyName))
                .Where(path => path.Segments.Length == 1)
                .Select(path => path.Segments[0]),
            StringComparer.Ordinal);
    }

    private bool TryGetStrategyBetweenTypes(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool useConstructorMapStrategyDetector,
        out MapStrategy elementStrategy)
        => this.TryGetStrategyBetweenTypes(
            targetType,
            sourceType,
            useConstructorMapStrategyDetector,
            null,
            out elementStrategy);

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

        if (this.context.MapMethod is null && this.context.PropertyPathContext is null)
        {
            return false;
        }

        var matchingAttributes = this.GetAttributeMapMethod()
            .GetAttributes<Attribute>()
            .OfType<IMappaTargetPropertyNameAttribute>()
            .Where(attribute => this.AttributeTargetPathMatches(attribute.TargetPropertyName, targetName, stringComparison))
            .Where(attribute => this.IsMappingAttributeActiveAtCurrentLevel(attribute.TargetPropertyName))
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
                this.GetAttributeMapMethod().MethodDeclarationSyntax ?? throw new MappaGeneratorException("Method declarations syntax has not been defined."),
                targetName));
            return false;
        }

        var attribute = matchingAttributes.Single();
        strategy = this.TryGetStrategyFromSingleTargetPropertyAttribute(
            targetName,
            targetType,
            sourceClassType,
            ref sourceProperty,
            stringComparison,
            isConstructorParameterPath,
            attribute);

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

        var usePropertyAttributes = this.GetAttributeMapMethod()
            .GetAttributes<MappaUsePropertyAttribute>()
            .Where(attribute => this.AttributeTargetPathMatches(attribute.TargetPropertyName, targetName, stringComparison))
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

        var parsedSourcePath = PropertyPath.Parse(sourcePropertyName);
        if (parsedSourcePath.IsNested)
        {
            return;
        }

        var sourceProperties = this.GetReadableSourceProperties(sourceClassType);

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

        var mapMethod = this.GetAttributeMapMethod();
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