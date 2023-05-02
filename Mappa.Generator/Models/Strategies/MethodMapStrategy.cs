// <copyright file="MethodMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describe a strategy for mapping between two types that uses a map method.
/// </summary>
internal sealed class MethodMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodMapStrategy"/> class.
    /// </summary>
    /// <param name="rule">The rule used to generate this strategy.</param>
    /// <param name="mapMethod">The method to be used for the mapping.</param>
    public MethodMapStrategy(MappaAlgorithmRule rule, MapMethod mapMethod)
    {
        this.MapMethod = mapMethod;
        this.Rule = rule;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType => this.MapMethod.TargetType;

    /// <inheritdoc/>
    public ITypeSymbol SourceType => this.MapMethod.SourceType;

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule { get; }

    /// <summary>
    /// Gets the method used for the mapping.
    /// </summary>
    public MapMethod MapMethod { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new MethodMapStrategyBuilder(this);
}