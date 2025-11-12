// <copyright file="TypeMappingStrategy.cs" company="Stefano Anelli">
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
/// <param name="defaultBehaviorStrategy">Strategy to map <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/> behavior.</param>
/// <param name="nullableEnabled"><c>true</c> if nullable is enabled, <c>false</c> otherwise.</param>
/// <param name="mapMethodContextParameterName">The name of the map method context parameter (if present).</param>
internal sealed class TypeMappingStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    MapStrategy[] subtypesMappingsStrategies,
    MappaTypeMappingDefaultAttribute defaultBehavior,
    MapStrategy defaultBehaviorStrategy,
    bool nullableEnabled,
    string? mapMethodContextParameterName)
    : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the default behavior when no other mapping succeeds.
    /// </summary>
    internal MappaTypeMappingDefaultAttribute DefaultBehavior { get; } = defaultBehavior;

    /// <summary>
    /// Gets the strategy used to map the <see cref="MappaTypeMappingDefaultBehavior.MapSourceType"/> behavior.
    /// </summary>
    internal MapStrategy DefaultBehaviorStrategy { get; } = defaultBehaviorStrategy;

    /// <summary>
    /// Gets a value indicating whether nullable is enabled or not.
    /// </summary>
    internal bool NullableEnabled { get; } = nullableEnabled;

    /// <summary>
    /// Gets a value context parameter name of the map method, or <c>null</c> if not present.
    /// </summary>
    internal string? MapMethodContextParameterName { get; } = mapMethodContextParameterName;

    /// <summary>
    /// Gets the mappings for all the subtypes.
    /// </summary>
    internal MapStrategy[] SubtypesMappingsStrategies => subtypesMappingsStrategies;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new TypeMappingStrategyBuilder(this);
}