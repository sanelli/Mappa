// <copyright file="TypeMappingStrategyDetectorStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to implement polymorphism.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="defaultBehavior">The attribute defining the default behavior when subtype mapping can be applied.</param>
/// <param name="subtypesMappingsStrategies">The list of strategies for the source subtypes.</param>
internal sealed class TypeMappingStrategyDetectorStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    MapStrategy[] subtypesMappingsStrategies,
    MappaTypeMappingDefaultAttribute defaultBehavior)
    : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the default behavior when no other mapping succeeds.
    /// </summary>
    internal MappaTypeMappingDefaultAttribute DefaultBehavior { get; } = defaultBehavior;

    /// <summary>
    /// Gets the mappings for all the subtypes.
    /// </summary>
    internal MapStrategy[] SubtypesMappingsStrategies => subtypesMappingsStrategies;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder()
    {
        throw new NotImplementedException();
    }
}