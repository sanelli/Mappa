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
/// <param name="methodSymbol">The method this strategy is used for.</param>
/// <param name="fastCollections">Enable or disable the fast collection iterations.</param>
internal sealed class CollectionToCollectionMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    MapStrategy elementStrategy,
    IMethodSymbol? methodSymbol,
    BooleanSetting fastCollections)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the strategy for the keys.
    /// </summary>
    internal MapStrategy ElementStrategy { get; } = elementStrategy;

    /// <summary>
    /// Gets the method symbol.
    /// </summary>
    internal IMethodSymbol? MethodSymbol { get; } = methodSymbol;

    /// <summary>
    /// Gets a value indicating whether to enable the fast collection iterations.
    /// </summary>
    internal BooleanSetting FastCollections { get; } = fastCollections;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new CollectionToCollectionMapStrategyBuilder(this);
}