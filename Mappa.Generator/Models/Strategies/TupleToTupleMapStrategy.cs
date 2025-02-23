// <copyright file="TupleToTupleMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to mup a tuple to another tuple.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source ty[e.</param>
/// <param name="elementStrategies">The strategies for each element of the tuple.</param>
internal sealed class TupleToTupleMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    MapStrategy[] elementStrategies)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the strategies for each element of the tuple.
    /// </summary>
    public MapStrategy[] ElementStrategies { get; } = elementStrategies;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new TupleToTupleMapStrategyBuilder(this);
}