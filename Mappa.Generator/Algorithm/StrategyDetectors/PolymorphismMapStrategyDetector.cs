// <copyright file="PolymorphismMapStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Strategy detector to support polymorphism.
/// </summary>
internal sealed class PolymorphismMapStrategyDetector(MappaMapAlgorithmContext context, Compilation compilation, CancellationToken cancellationToken)
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context = context;
    private readonly Compilation compilation = compilation;
    private readonly CancellationToken cancellationToken = cancellationToken;

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // Check the method has the type mapping attributes.
        var typeMappingAttributes = this.GetTypeMappingAttributes();
        if (typeMappingAttributes.Length == 0)
        {
            return false;
        }

        // Check the attributes.
        var sourceTypeFullNames = new HashSet<string>();
        var subtypesMappingsStrategies = new List<MapStrategy>();
        foreach (var attribute in typeMappingAttributes)
        {
            // Check attribute source type name is valid.
            if (string.IsNullOrWhiteSpace(attribute.SourceType.FullName))
            {
                // TODO [#49] Add diagnostic that the type name cannot be loaded.
                return false;
            }

            // Check attribute source type is not duplicated.
            if (!sourceTypeFullNames.Add(attribute.SourceType.FullName))
            {
                // TODO [#49] Add diagnostic: duplicated attribute with the same source type.
                return false;
            }

            // Check attribute target type name is valid.
            if (string.IsNullOrWhiteSpace(attribute.TargetType.FullName))
            {
                // TODO [#49] Add diagnostic that the type name cannot be loaded.
                return false;
            }

            // Check attribute source type can be loaded.
            var attributeSourceType = this.compilation.GetTypeByMetadataName(attribute.SourceType.FullName);
            if (attributeSourceType is null)
            {
                // TODO [#49] Add diagnostic that the type cannot be loaded.
                return false;
            }

            // Check attribute target type can be loaded.
            var attributeTargetType = this.compilation.GetTypeByMetadataName(attribute.TargetType.FullName);
            if (attributeTargetType is null)
            {
                // TODO [#49] Add diagnostic that the type cannot be loaded.
                return false;
            }

            // Check attribute source type and map method source type are different types.
            if (SymbolEqualityComparer.Default.Equals(attributeSourceType, this.context.SourceType))
            {
                // TODO [#49] Add diagnostic that attribute source type should be different than map method source type and to use MappaTypeMappingDefault default attribute.
                return false;
            }

            // Check attribute source type is derived form the source type in the map method somehow.
            if (!attributeSourceType.IsImplementingOrIsDerivedFromClass(this.context.SourceType))
            {
                // TODO [#49] Add diagnostic that source type is not derived from map source type.
                return false;
            }

            // Check attribute source type is derived form the source type in the map method somehow.
            if (!attributeSourceType.IsImplementingOrIsDerivedFromClass(this.context.SourceType))
            {
                // TODO [#49] Add diagnostic that target type is not derived from map target type.
                return false;
            }

            // At this stage any nullability concern is already handled by the nullability detector
            // and if we do not force it to be NotAnnotated it will be None which is handled like
            // annotated to support the non-nullable context.
            var sourceType = attributeSourceType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
            var targetType = attributeTargetType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

            // Identify mapping from attribute source type to attribute target type.
            var attributeContext = new DerivedMappaMapAlgorithmContext(this.context, targetType, sourceType);
            var attributeAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(attributeContext, this.compilation, this.cancellationToken);
            var attributeStrategy = attributeAlgorithm.GetStrategy();
            if (attributeStrategy is NoMapStrategy)
            {
                // TODO [#49] Add diagnostic mapping cannot be found for attribute.
                return false;
            }

            subtypesMappingsStrategies.Add(attributeStrategy);
        }

        var mappaTypeMappingDefaultAttribute = this.GetTypeMappingDefaultAttribute()
                                               ?? new MappaTypeMappingDefaultAttribute(MappaTypeMappingDefaultBehavior.Throw);

        var rootMapMethod = this.context.GetRootMapMethod();
        var methodSymbolContainingSymbol = rootMapMethod.MethodSymbol.ContainingSymbol as ITypeSymbol ?? throw new MappaGeneratorException("Method parent is not a type symbol");
        var mapMethodHasTwoParameters = rootMapMethod.MethodSymbol.Parameters.Length == 2;
        var validationSuccessful = mappaTypeMappingDefaultAttribute.IsValid(
            this.context.TargetType,
            this.context.SourceType,
            methodSymbolContainingSymbol,
            rootMapMethod.NullableEnabled,
            mapMethodHasTwoParameters,
            this.compilation,
            out var validationDiagnosis);
        foreach (var diagnostic in validationDiagnosis)
        {
            this.context.ReportDiagnostic(diagnostic);
        }

        if (!validationSuccessful)
        {
            // No need to report any extra diagnostic.
            return false;
        }

        // Identify a mapping if required.
        MapStrategy defaultMappingStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);
        if (mappaTypeMappingDefaultAttribute.Behavior is MappaTypeMappingDefaultBehavior.MapSourceType)
        {
            var targetTypeFullNameFromAttribute = mappaTypeMappingDefaultAttribute.Type?.FullName;

            var targetSymbol = (!string.IsNullOrWhiteSpace(targetTypeFullNameFromAttribute) ? this.compilation.GetTypeByMetadataName(targetTypeFullNameFromAttribute!) : null)
                               ?? this.context.TargetType;

            if (targetSymbol.TypeKind == TypeKind.Interface)
            {
                // TODO [#49] Report diagnostic that default mapping is trying to map to an interface.
                return false;
            }

            if (targetSymbol.IsAbstract)
            {
                // TODO [#49] Report diagnostic that default mapping is trying to map to an abstract type.
                return false;
            }

            // TODO [#49] Apply a flag to prevent this strategy to run twice.
            var derivedContext = new DerivedMappaMapAlgorithmContext(this.context, targetSymbol, this.context.SourceType);
            var algorithm = new TypeMapIdentifierAlgorithm(derivedContext, this.compilation, this.cancellationToken);
            defaultMappingStrategy = algorithm.GetStrategy();

            if (defaultMappingStrategy is NoMapStrategy)
            {
                // TODO [#49] Report diagnostic that default mapping cannot be identified.
                return false;
            }
        }

        mapStrategy = new PolymorphismMapStrategy(
            this.context.TargetType,
            this.context.SourceType,
            [.. subtypesMappingsStrategies],
            mappaTypeMappingDefaultAttribute,
            defaultMappingStrategy,
            rootMapMethod.NullableEnabled,
            rootMapMethod.MaybeGetMappaContextParameterName());
        return true;
    }

    private MappaTypeMappingAttribute[] GetTypeMappingAttributes()
        => this.context.GetRootMapMethod().GetAttributes<MappaTypeMappingAttribute>();

    private MappaTypeMappingDefaultAttribute? GetTypeMappingDefaultAttribute()
        => this.context.GetRootMapMethod().GetAttribute<MappaTypeMappingDefaultAttribute>();
}