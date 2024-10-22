// <copyright file="ConstructorMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Detector for the constructor strategies.
/// </summary>
// TODO [#22] Add support for polymorphism.
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

    private enum MethodDetectorMethodStaticRequirement
    {
        StaticOrNotStatic,
        Static,
        NotStatic,
    }

    /// <inheritdoc/>
    // TODO [#1] if nullable is not enabled we might want to throw if input is null.
    public bool TryDetect(out IMapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

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

        // 03. If there is no empty constructor try identifying the best one -> InvokeConstructorMapStrategy( IMapStrategy[] parameters, IMapStrategy[] initProperties )
        else if (this.CanInvokeConstructorWithParameters(out var nonEmptyConstructorStrategy))
        {
            mapStrategy = nonEmptyConstructorStrategy;
        }

        return mapStrategy is not NoMapStrategy;
    }

    private bool CanInvokeConstructorWithParameters(out IMapStrategy strategy)
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
                // TODO [#7] Ensure property getter method is accessible.
                .Where(property => !property.IsIndexer && property.GetMethod is not null)
                .ToArray();

            // For each constructor identifier we get all the arguments,
            // and we try to match with a property of the source.
            var constructorsWithMappings = constructors.Select(methodSymbol =>
                {
                    // For each argument of the constructor
                    (IParameterSymbol Parameter, IPropertySymbol Property, IMapStrategy Strategy)[] strategiesForEachParameter = methodSymbol.Parameters
                        .Select<IParameterSymbol, (IParameterSymbol Parameter, IPropertySymbol Property, IMapStrategy Strategy)>(
                            targetParameter =>
                            {
                                // TODO [#8] Allow property mapping where source property name differ from target parameter name using an attribute.
                                IPropertySymbol? sourceProperty = Array.Find(sourceProperties, property => property.Name.Equals(targetParameter.Name, StringComparison.OrdinalIgnoreCase));

                                // Look for any attribute action that can be applied
                                if (this.context.MapMethod is not null &&
                                    this.TryGetStrategyForPropertyOrArgumentUsingAttributesOnMethod(
                                        targetParameter.Name,
                                        targetParameter.Type,
                                        this.context.SourceType,
                                        sourceProperty,
                                        StringComparison.OrdinalIgnoreCase,
                                        out var propertyStrategyFromAttribute))
                                {
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
                    MappaAlgorithmRule.InvokeConstructor,
                    this.context.TargetType,
                    this.context.SourceType,
                    constructorsWithMappings[0].methodSymbol,
                    constructorsWithMappings[0].strategiesForEachParameter
                        .Select(parameterAndStrategy => (ParameterMapStrategy)parameterAndStrategy.Strategy)
                        .ToArray(),
                    []);
            }
        }

        return strategy is not NoMapStrategy;
    }

    private bool CanInvokeMappingConstructor(out IMethodSymbol constructor, out IMapStrategy strategy)
    {
        constructor = null!;
        var noMapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // Detect all constructors that:
        // - Have 1 argument
        // - Are accessible
        // - Have a mapping from source to the type of the parameter
        var constructors = this.context.TargetType.GetAccessibleConstructors(this.compilation, this.context.ParentSymbol, 1);
        var constructorsWithStrategy = constructors
            .Select<IMethodSymbol, (IMethodSymbol Constructor, IMapStrategy Strategy)>(constructor =>
            {
                var constructorParameterType = constructor.Parameters.Single().Type;

                if (this.TryGetStrategyBetweenTypes(constructorParameterType, this.context.SourceType, false, out var constructorParameterStrategy))
                {
                    return (constructor, constructorParameterStrategy);
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

    private bool CanInvokeEmptyConstructor(out IMapStrategy strategy)
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
            // Gets the target properties
            // TODO [#3] Allow to ignore some target properties when looking for for one empty constructor mapping.
            // TODO [#4] Ensure property setter is accessible.
            var targetProperties = this.context.TargetType.GetTypeProperties()

                // Ignore indexer properties.
                // Ignore properties without a setter.
                // TODO [#5] Accept target properties implementing IList<T>.
                // TODO [#6] Accept target properties implementing IDictionary<K, V>.
                .Where(property => !property.IsIndexer && property.SetMethod is not null)
                .ToArray();

            // If no target properties exist there is no point in applying this strategy
            // this way we can avoid to attempt using this strategy for basic types
            // like string, int, etc...
            if (targetProperties.Length > 0)
            {
                // Gets the source properties.
                var sourceProperties = this.context.SourceType.GetTypeProperties()

                    // Ignore indexer properties.
                    // Ignore properties without a setter.
                    // TODO [#7] Ensure property getter method is accessible.
                    .Where(property => !property.IsIndexer && property.GetMethod is not null)

                    // Map them to a dictionary
                    .ToDictionary(property => property.Name);

                // Match target property with a source property.
                var initializerStrategies = targetProperties
                    .Select(
                        targetProperty =>
                        {
                            // TODO [#8] Allow property mapping where source property name differ from target property name using an attribute.
                            // TODO [#9] Allow property mapping regardless of casing using an attribute.
                            // Try to get a matching property
                            var hasSourceProperty = sourceProperties.TryGetValue(targetProperty.Name, out var sourceProperty);

                            // Look for any attribute action that can be applied
                            if (this.context.MapMethod is not null &&
                                this.TryGetStrategyForPropertyOrArgumentUsingAttributesOnMethod(
                                    targetProperty.Name,
                                    targetProperty.Type,
                                    this.context.SourceType,
                                    hasSourceProperty ? sourceProperty : null,
                                    StringComparison.Ordinal,
                                    out var propertyStrategyFromAttribute))
                            {
                                return new PropertyMapStrategy(targetProperty, sourceProperty!, propertyStrategyFromAttribute);
                            }

                            // Look for a matching source property
                            if (!hasSourceProperty || sourceProperty is null)
                            {
                                return new PropertyMapStrategy(targetProperty, null!, noMapStrategy);
                            }

                            var targetPropertyType = targetProperty.Type;
                            var sourcePropertyType = sourceProperty.Type;

                            if (this.TryGetStrategyBetweenTypes(targetPropertyType, sourcePropertyType, true, out var propertyStrategy))
                            {
                                return new PropertyMapStrategy(targetProperty, sourceProperty, propertyStrategy);
                            }

                            return new PropertyMapStrategy(targetProperty, null!, noMapStrategy);
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
                    initializerStrategies = initializerStrategies
                        .Where(propertyStrategy => propertyStrategy.PropertyStrategy is not NoMapStrategy)
                        .ToArray();

                    // TODO [#21] Allow to return an error if some source properties are not mapped.
                    strategy = new InvokeConstructorMapStrategy(
                        MappaAlgorithmRule.InvokeEmptyConstructor,
                        this.context.TargetType,
                        this.context.SourceType,
                        constructors.Single(),
                        [],
                        initializerStrategies);
                }
            }
            else
            {
                strategy = noMapStrategy;
            }
        }

        return strategy is not NoMapStrategy;
    }

    private bool TryGetStrategyBetweenTypes(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        bool useConstructorMapStrategyDetector,
        out IMapStrategy elementStrategy)
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
        IPropertySymbol? sourceProperty,
        StringComparison stringComparison,
        out IMapStrategy strategy)
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
                this.TryGetStrategyUsingMappaInvokeMethodAttribute(
                    targetName,
                    targetType,
                    sourceClassType,
                    sourceProperty,
                    mappaInvokeMethodAttribute,
                    out strategy);
                break;
        }

        return strategy is not NoMapStrategy;
    }

    private void TryGetStrategyUsingMappaInvokeMethodAttribute(
        string targetName,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        IPropertySymbol? sourceProperty,
        MappaInvokeMethodAttribute mappaInvokeMethodAttribute,
        out IMapStrategy strategy)
    {
        strategy = new NoMapStrategy(targetType, null!);

        IMethodSymbol? method;
        var mapMethod = this.context.MapMethod ?? throw new MappaGeneratorException("Map method needs to be defined.");
        var mapMethodMethodDeclarationSyntax = mapMethod.MethodDeclarationSyntax ?? throw new MappaGeneratorException("Method declaration syntax has not been defined.");
        var mapMethodClass = (INamedTypeSymbol)mapMethod.MethodSymbol.ContainingSymbol;

        if (mappaInvokeMethodAttribute.FieldName is not null)
        {
            var classMembers = mapMethodClass.GetMembers();
            var targets = classMembers
                .OfType<IPropertySymbol>()
                .Where(property => property.Name.Equals(mappaInvokeMethodAttribute.FieldName, StringComparison.Ordinal))
                .Select(property => property.Type)
                .Concat(classMembers
                    .OfType<IFieldSymbol>()
                    .Where(field => field.Name.Equals(mappaInvokeMethodAttribute.FieldName, StringComparison.Ordinal))
                    .Select(field => field.Type))
                .OfType<INamedTypeSymbol>()
                .ToArray();

            if (targets.Length != 1)
            {
                this.context.ReportDiagnostic(MappaDiagnostics.CannotFindFieldOrProperty(
                    mapMethodMethodDeclarationSyntax,
                    mappaInvokeMethodAttribute.FieldName));
                return;
            }

            method = GetBestMethodSymbol(
                this.compilation,
                mapMethodClass,
                targets.Single().GetMembers().OfType<IMethodSymbol>().ToArray(),
                mappaInvokeMethodAttribute.MethodName,
                targetType,
                sourceClassType,
                sourceProperty,
                this.context.IsNullableEnabled(),
                MethodDetectorMethodStaticRequirement.NotStatic);
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

            method = GetBestMethodSymbol(
                this.compilation,
                mapMethodClass,
                className.GetMembers().OfType<IMethodSymbol>().ToArray(),
                mappaInvokeMethodAttribute.MethodName,
                targetType,
                sourceClassType,
                sourceProperty,
                this.context.IsNullableEnabled(),
                MethodDetectorMethodStaticRequirement.StaticOrNotStatic);
        }
        else
        {
            // At this point we look for a method (static or not static in the same class the method is defined)
            method = GetBestMethodSymbol(
                this.compilation,
                mapMethodClass,
                mapMethodClass.GetMembers().OfType<IMethodSymbol>().ToArray(),
                mappaInvokeMethodAttribute.MethodName,
                targetType,
                sourceClassType,
                sourceProperty,
                this.context.IsNullableEnabled(),
                MethodDetectorMethodStaticRequirement.StaticOrNotStatic);
        }

        if (method is null)
        {
            this.context.ReportDiagnostic(MappaDiagnostics.CannotDetectSuitableMethodToInvoke(
                mapMethodMethodDeclarationSyntax,
                targetName,
                mappaInvokeMethodAttribute.MethodName,
                mapMethodClass.ToDisplayString()));
            return;
        }

        strategy = new MappaInvokeMethodAttributeStrategy(
            targetType,
            sourceClassType,
            mappaInvokeMethodAttribute,
            method,
            sourceProperty,
            this.context.MapMethod.NullableEnabled);

        static IMethodSymbol? GetBestMethodSymbol(
            Compilation compilation,
            ITypeSymbol mapClass,
            IMethodSymbol[] methods,
            string methodName,
            ITypeSymbol targetType,
            ITypeSymbol sourceClassType,
            IPropertySymbol? sourceProperty,
            bool nullableEnabled,
            MethodDetectorMethodStaticRequirement isStatic)
        {
            var methodsWithTheRightNameAndReturnType = methods
                .Where(method =>
                    method.Name.Equals(methodName, StringComparison.Ordinal) &&
                    compilation.IsSymbolAccessibleWithin(method, mapClass) &&
                    (method.ReturnType.IsEqualTo(targetType, nullableEnabled) || compilation.HasImplicitConversion(method.ReturnType, targetType)) &&
                    isStatic switch
                    {
                        MethodDetectorMethodStaticRequirement.StaticOrNotStatic => true,
                        MethodDetectorMethodStaticRequirement.Static => method.IsStatic,
                        MethodDetectorMethodStaticRequirement.NotStatic => !method.IsStatic,
                        _ => throw new MappaGeneratorException($"'isStatic' attribute is not valid (value: {isStatic})"),
                    })
                .ToArray();

            // No method found :( .
            if (methodsWithTheRightNameAndReturnType.Length == 0)
            {
                return null;
            }

            // If multiple methods are available first look for one having
            // two parameters, first one being type source class
            // and the second being the source property
            if (sourceProperty is not null)
            {
                var methodWithTwoParameters = Array.Find(
                    methodsWithTheRightNameAndReturnType,
                    method => method.Parameters.Length == 2 &&
                                method.Parameters[0].Type.IsEqualTo(sourceClassType, nullableEnabled) &&
                                method.Parameters[1].Type.IsEqualTo(sourceProperty.Type, nullableEnabled));
                if (methodWithTwoParameters is not null)
                {
                    return methodWithTwoParameters;
                }
            }

            // Then look for one having
            // two parameters, first one being implicitly convertible from source class
            // and the second being implicitly convertible from source property
            if (sourceProperty is not null)
            {
                var methodWithTwoParameters = Array.Find(
                    methodsWithTheRightNameAndReturnType,
                    method => method.Parameters.Length == 2 &&
                              (method.Parameters[0].Type.IsEqualTo(sourceClassType, nullableEnabled) || compilation.HasImplicitConversion(sourceClassType, method.Parameters[0].Type)) &&
                              (method.Parameters[1].Type.IsEqualTo(sourceProperty.Type, nullableEnabled) || compilation.HasImplicitConversion(sourceProperty.Type, method.Parameters[1].Type)));
                if (methodWithTwoParameters is not null)
                {
                    return methodWithTwoParameters;
                }
            }

            // Then look for one having
            // one parameter being equal to the type of source class.
            var methodWithOneParamOfTypeClassType = Array.Find(
                    methodsWithTheRightNameAndReturnType,
                    method => method.Parameters.Length == 1 && method.Parameters[0].Type.IsEqualTo(sourceClassType, nullableEnabled));
            if (methodWithOneParamOfTypeClassType is not null)
            {
                return methodWithOneParamOfTypeClassType;
            }

            // Then look for one having
            // one parameter being implicitly convertible to the type of source class.
            var methodWithOneParamOfTypeImplicitConvertibleClassType = Array.Find(
                methodsWithTheRightNameAndReturnType,
                method => method.Parameters.Length == 1 &&
                          compilation.HasImplicitConversion(sourceClassType, method.Parameters[0].Type));
            if (methodWithOneParamOfTypeImplicitConvertibleClassType is not null)
            {
                return methodWithOneParamOfTypeImplicitConvertibleClassType;
            }

            // Then look for one having
            // one parameter being equal to the type of the source property.
            if (sourceProperty is not null)
            {
                var methodWithOneParamOfTypeSourceType = Array.Find(
                    methodsWithTheRightNameAndReturnType,
                    method => method.Parameters.Length == 1 &&
                              method.Parameters[0].Type.IsEqualTo(sourceProperty.Type, nullableEnabled));
                if (methodWithOneParamOfTypeSourceType is not null)
                {
                    return methodWithOneParamOfTypeSourceType;
                }
            }

            // Then look for one having
            // one parameter being implicitly convertible from the type of the source property.
            if (sourceProperty is not null)
            {
                var methodWithOneParamOfTypeConvertibleFromSourceType = Array.Find(
                    methodsWithTheRightNameAndReturnType,
                    method => method.Parameters.Length == 1 &&
                              compilation.HasImplicitConversion(sourceProperty.Type, method.Parameters[0].Type));
                if (methodWithOneParamOfTypeConvertibleFromSourceType is not null)
                {
                    return methodWithOneParamOfTypeConvertibleFromSourceType;
                }
            }

            // Then look for one having
            // no parameters.
            var methodWithNoParameters = Array.Find(
                 methodsWithTheRightNameAndReturnType,
                 method => method.Parameters.Length == 0);
            if (methodWithNoParameters is not null)
            {
                return methodWithNoParameters;
            }

            // No method has been identified.
            return null;
        }
    }
}