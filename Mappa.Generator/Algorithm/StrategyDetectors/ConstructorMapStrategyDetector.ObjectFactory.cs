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
        var classAttributes = rootMapMethod.ContainingType
            .GetAttributes()
            .GetMappaObjectFactoryAttributes(this.compilation);
        var methodAttributes = rootMapMethod.MethodSymbol is null
            ? []
            : rootMapMethod.MethodSymbol.GetAttributes().GetMappaObjectFactoryAttributes(this.compilation);

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

        if (!this.TryBuildObjectFactoryParameterAndInitializerStrategies(
                objectFactory,
                out var parametersMapStrategies,
                out var initializerStrategies))
        {
            return false;
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

        var sourceProperties = this.GetReadableSourceProperties(this.context.SourceType);

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

                    this.TryIsSourcePropertyReadable(sourceProperty, out var requiresUnsafeAccessorOnSourceFromAttribute);
                    var strategy = new ParameterMapStrategy(targetParameter, sourceProperty, propertyStrategyFromAttribute, requiresUnsafeAccessorOnSourceFromAttribute);
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
                    this.TryIsSourcePropertyReadable(sourceProperty, out var requiresUnsafeAccessorOnSource);
                    var parameterMapStrategy = new ParameterMapStrategy(targetParameter, sourceProperty, propertyStrategy, requiresUnsafeAccessorOnSource);
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
        if (!this.TryBuildAssignToContextEnrichment(attributesEnabled: true, out var entries, out var contextParameterName))
        {
            return strategy;
        }

        return new InvokeObjectFactoryMapStrategy(
            strategy.TargetType,
            strategy.SourceType,
            strategy.ObjectFactory,
            strategy.ParametersMapStrategies,
            strategy.InitializerStrategies,
            entries,
            contextParameterName);
    }

    private bool TryBuildObjectFactoryParameterAndInitializerStrategies(
        ObjectFactory objectFactory,
        out ParameterMapStrategy[] parametersMapStrategies,
        out PropertyMapStrategy[] initializerStrategies)
    {
        parametersMapStrategies = [];
        initializerStrategies = [];

        switch (objectFactory.InvocationKind)
        {
            case ObjectFactoryInvocationKind.FullyProduced:
                return true;

            case ObjectFactoryInvocationKind.EmptyCtorLike:
                return this.TryBuildEmptyCtorLikePropertyInitializers(
                    requireAtLeastOneMappedProperty: false,
                    out initializerStrategies);

            case ObjectFactoryInvocationKind.ParameterizedLike:
                return this.TryMapFactoryParameters(objectFactory.Method, out parametersMapStrategies);

            default:
                throw new MappaGeneratorException($"Unexpected object factory invocation kind '{objectFactory.InvocationKind}'.");
        }
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

        var sourceProperties = this.GetReadableSourceProperties(this.context.SourceType);

        var mappedInitializerStrategies = targetProperties
            .Where(targetProperty => this.context.MappaUserSettings.ProtobufOptional is not BooleanSetting.Enable
                                     || allTargetProperties.All(otherProperty => !targetProperty.Name.Equals($"Has{otherProperty.Name}", StringComparison.Ordinal)))
            .Select(targetProperty => this.TryCreateEmptyCtorPropertyMapStrategy(
                targetProperty,
                allTargetProperties,
                sourceProperties,
                noMapStrategy))
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

        if (this.ReportEmptyCtorDiagnosticsForUnmappedProperties(propertiesWithStrategies, propertiesWithoutStrategy))
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