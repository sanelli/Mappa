// <copyright file="OptionalSourcePropertyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to encapsulate a strategy when the source property
/// is optional (i.e. a property named "Has&lt;SourceProperty&gt;" exists.
/// </summary>
internal sealed class OptionalSourcePropertyMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalSourcePropertyMapStrategy"/> class.
    /// </summary>
    /// <param name="strategy">The strategy to apply.</param>
    public OptionalSourcePropertyMapStrategy(IMapStrategy strategy)
    {
        this.InnerStrategy = strategy;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType => this.InnerStrategy.TargetType;

    /// <inheritdoc/>
    public ITypeSymbol SourceType => this.InnerStrategy.SourceType;

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule => this.InnerStrategy.Rule;

    /// <summary>
    /// Gets the strategy encapsulated.
    /// </summary>
    internal IMapStrategy InnerStrategy { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder()
    {
        throw new NotImplementedException();
    }
}