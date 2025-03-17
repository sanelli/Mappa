// <copyright file="CollectionToCollectionMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a collection to a collection.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="elementStrategy">The strategy for the element.</param>
internal sealed class CollectionToCollectionMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    MapStrategy elementStrategy)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the strategy for the keys.
    /// </summary>
    public MapStrategy ElementStrategy { get; } = elementStrategy;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new CollectionToCollectionMapStrategyBuilder(this);
}