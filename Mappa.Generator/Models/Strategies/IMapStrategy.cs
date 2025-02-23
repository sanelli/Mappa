// <copyright file="IMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describe a strategy to map from a type to another.
/// </summary>
internal interface IMapStrategy
{
    /// <summary>
    /// Gets the target type.
    /// </summary>
    ITypeSymbol TargetType { get; }

    /// <summary>
    /// Gets the source type.
    /// </summary>
    ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the strategy builder for this strategy.
    /// </summary>
    /// <returns>The strategy builder.</returns>
    IMappaStrategyBuilder GetBuilder();
}