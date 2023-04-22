// <copyright file="IMappaStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Describe a strategy builder.
/// </summary>
internal interface IMappaStrategyBuilder
{
    /// <summary>
    /// Build the source code for the strategy.
    /// </summary>
    /// <param name="source">The input variable source of the mapping.</param>
    /// <param name="context">The building context.</param>
    /// <param name="mappaGlobalOptions">The global options.</param>
    /// <returns>The variable name containing the mapping and the code needed to populate the variable.</returns>
    (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions);
}