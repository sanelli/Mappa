// <copyright file="MapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describe a strategy to map from a type to another.
/// </summary>
internal abstract class MapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type of the mapping.</param>
    /// <param name="sourceType">The source type of the mapping.</param>
    protected MapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
    }

    /// <summary>
    /// Gets the target type.
    /// </summary>
    internal ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the source type.
    /// </summary>
    internal ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the strategy builder for this strategy.
    /// </summary>
    /// <returns>The strategy builder.</returns>
    internal abstract IMappaStrategyBuilder GetBuilder();
}