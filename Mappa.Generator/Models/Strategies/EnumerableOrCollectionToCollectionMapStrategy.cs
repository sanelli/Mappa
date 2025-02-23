// <copyright file="EnumerableOrCollectionToCollectionMapStrategy.cs" company="Stefano Anelli">
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
/// <param name="elementStrategy">The strategy that map the array element.</param>
internal sealed class EnumerableOrCollectionToCollectionMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    MapStrategy elementStrategy)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the strategy to map the types encapsulated by the nullable struct.
    /// </summary>
    public MapStrategy ElementStrategy { get; } = elementStrategy;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new EnumerableOrCollectionToCollectionMapStrategyBuilder(this);
}