// <copyright file="TypeMappingStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Strategy detector to support polymorphism.
/// </summary>
internal sealed class TypeMappingStrategyDetector(MappaMapAlgorithmContext context, Compilation compilation, CancellationToken cancellationToken)
    : IMapStrategyDetector
{
    private readonly MappaMapAlgorithmContext context = context;
    private readonly Compilation compilation = compilation;
    private readonly CancellationToken cancellationToken = cancellationToken;

    /// <inheritdoc/>
    public bool TryDetect(out MapStrategy mapStrategy)
    {
        mapStrategy = new NoMapStrategy(this.context.TargetType, this.context.SourceType);

        // Check the method is being mapped.
        if (this.context.MapMethod is null)
        {
            return false;
        }

        // Check the method has the type mapping attributes.
        var mapMethod = this.context.GetMapMethod();
        var typeMappingAttributes = mapMethod.GetAttributes<MappaTypeMappingAttribute>();
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

            // Generate source type and target type by adding the same annotations of the map methods for consistency.
            var sourceType = attributeSourceType.WithNullableAnnotation(this.context.MapMethod.SourceType.NullableAnnotation);
            var targetType = attributeSourceType.WithNullableAnnotation(this.context.MapMethod.TargetType.NullableAnnotation);

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

        var mappaTypeMappingDefaultAttribute = mapMethod.GetAttribute<MappaTypeMappingDefaultAttribute>()
                                               ?? new MappaTypeMappingDefaultAttribute(MappaTypeMappingDefaultBehavior.Throw);

        var validationSuccessful = mappaTypeMappingDefaultAttribute.IsValid(mapMethod, this.compilation, out var validationDiagnosis);
        if (validationDiagnosis is not null)
        {
            this.context.ReportDiagnostic(validationDiagnosis);
        }

        if (!validationSuccessful)
        {
            return false;
        }

        mapStrategy = new TypeMappingStrategy(
            this.context.TargetType,
            this.context.SourceType,
            [.. subtypesMappingsStrategies],
            mappaTypeMappingDefaultAttribute);
        return true;
    }
}