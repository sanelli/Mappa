// <copyright file="TupleToTupleMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to mup a tuple to another tuple.
/// </summary>
internal sealed class TupleToTupleMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TupleToTupleMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source ty[e.</param>
    /// <param name="elementStrategies">The strategies for each element of the tuple.</param>
    public TupleToTupleMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        IMapStrategy[] elementStrategies)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.ElementStrategies = elementStrategies;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the strategies for each element of the tuple.
    /// </summary>
    public IMapStrategy[] ElementStrategies { get; }

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.TupleToTuple;

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new TupleToTupleMapStrategyBuilder(this);
}