// <copyright file="DictionaryToDictionaryMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a dictionary to a dictionary.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="keyStrategy">The strategy for the keys.</param>
/// <param name="valueStrategy">The strategy for the values.</param>
internal sealed class DictionaryToDictionaryMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    MapStrategy keyStrategy,
    MapStrategy valueStrategy)
        : MapStrategy(targetType, sourceType)
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
    internal override IMappaStrategyBuilder GetBuilder() => new DictionaryToDictionaryMapStrategyBuilder(this);
}