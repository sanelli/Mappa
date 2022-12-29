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
    /// Gets the full name of the property or fields that is the source
    /// of the mapping. This can be the name of the input parameter
    /// or the full field path to reach a deeply nested property.
    /// Multiple properties are split by <c>.</c>.
    /// </summary>
    string Source { get; }

    /// <summary>
    /// Gets the rule applied.
    /// </summary>
    MappaAlgorithmRule Rule { get; }

    /// <summary>
    /// Gets the strategy builder for this strategy.
    /// </summary>
    /// <returns>The strategy builder.</returns>
    IMappaStrategyBuilder GetBuilder();
}