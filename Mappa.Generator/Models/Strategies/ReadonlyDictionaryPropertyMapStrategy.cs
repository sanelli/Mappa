// <copyright file="ReadonlyDictionaryPropertyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy used when mapping to a dictionary property that
/// either does not have a setter or the setter is not accessible.
/// </summary>
/// <param name="targetProperty">The target property (a dictionary type).</param>
/// <param name="sourceProperty">The source property (a dictionary type).</param>
/// <param name="keyStrategy">The strategy for the key mapping.</param>
/// <param name="valueStrategy">The strategy for the value mappings.</param>
internal sealed class ReadonlyDictionaryPropertyMapStrategy(
    IPropertySymbol targetProperty,
    IPropertySymbol sourceProperty,
    MapStrategy keyStrategy,
    MapStrategy valueStrategy)
    : MapStrategy(targetProperty.Type, sourceProperty.Type)
{
    /// <summary>
    /// Gets the strategy for the keys.
    /// </summary>
    public MapStrategy KeyStrategy { get; } = keyStrategy;

    /// <summary>
    /// Gets the strategy for the values.
    /// </summary>
    public MapStrategy ValueStrategy { get; } = valueStrategy;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder()
    {
        // TODO [#87] Implement me.
        throw new NotImplementedException("#87");
    }
}