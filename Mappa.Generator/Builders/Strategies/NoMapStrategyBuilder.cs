// <copyright file="NoMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder fo strategy <see cref="NoMapStrategy"/>.
/// </summary>
internal sealed class NoMapStrategyBuilder
    : IMappaStrategyBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="noMapStrategy">The strategy.</param>
    public NoMapStrategyBuilder(NoMapStrategy noMapStrategy)
    {
        this.NoMapStrategy = noMapStrategy;
    }

    /// <summary>
    /// Gets the No Map Strategy.
    /// </summary>
    private NoMapStrategy NoMapStrategy { get; }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        return ($"/* ?? {source} ?? */", string.Empty);
    }
}