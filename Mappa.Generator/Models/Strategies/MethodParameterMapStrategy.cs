// <copyright file="MethodParameterMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map the source parameter of a method using a specific strategy.
/// </summary>
/// <param name="strategy">The strategy to be used for mapping the method parameter.</param>
internal sealed class MethodParameterMapStrategy(MapStrategy strategy)
        : MapStrategy(strategy.TargetType, strategy.SourceType)
{
    /// <summary>
    /// Gets the strategy to be used to map the method.
    /// </summary>
    internal MapStrategy Strategy { get; } = strategy;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new MethodParameterMapStrategyBuilder(this);
}