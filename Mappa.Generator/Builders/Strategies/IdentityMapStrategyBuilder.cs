// <copyright file="IdentityMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="Strategy"/> strategy.
/// </summary>
internal sealed class IdentityMapStrategyBuilder
    : IMappaStrategyBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public IdentityMapStrategyBuilder(IdentityMapStrategy strategy)
    {
        this.Strategy = strategy;
    }

    /// <summary>
    /// Gets the strategy.
    /// </summary>
    private IdentityMapStrategy Strategy { get; }

    /// <inheritdoc/>
    public (string StrategySource, string Header) BuildSource(MappaGlobalOptions mappaGlobalOptions)
    {
        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.Strategy.Rule} */ "
            : string.Empty;
        return ($"{ruleComment}{this.Strategy.Source}", string.Empty);
    }
}