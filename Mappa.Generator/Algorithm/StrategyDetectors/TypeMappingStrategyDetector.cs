// <copyright file="TypeMappingStrategyDetector.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
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
        mapStrategy = new NoMapStrategy(null!, null!);

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
        var attributeStrategies = new List<(MappaTypeMappingAttribute Attribute, MapStrategy Strategy)>();
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

            // For map type source type interfaces: check attribute source type implement map method source type.
            // TODO [#49] Implement me.

            // For map type source type NOT interfaces: check attribute source type derived from map method source type.
            // TODO [#49] Implement me.

            // For map type target type interfaces: check attribute target type implement map method source type.
            // TODO [#49] Implement me.

            // For map type target type NOT interfaces: check attribute target type derived from map method source type.
            // TODO [#49] Implement me.

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

            attributeStrategies.Add((attribute, attributeStrategy));
        }

        /* TODO [#49] Identify (if possible) strategy from mapMethod source -> mapMethod target.
         This is to take into account the default; if this is not possible then
         the default should just be implements as throwing an ArgumentException. */

        // TODO [#49] Create the strategy by collating attributeStrategis into a new ad-host strategy.
        return true;
    }
}