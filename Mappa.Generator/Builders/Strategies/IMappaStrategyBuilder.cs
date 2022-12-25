// <copyright file="IMappaStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Describe a strategy builder.
/// </summary>
internal interface IMappaStrategyBuilder
{
    /// <summary>
    /// Build the source code for the strategy.
    /// </summary>
    /// <returns>The strategy source code and the potential header code.</returns>
    (string StrategySource, string Header) BuildSource();
}