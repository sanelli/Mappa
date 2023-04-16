// <copyright file="IdentityMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="strategy"/> strategy.
/// </summary>
internal sealed class IdentityMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly IdentityMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public IdentityMapStrategyBuilder(IdentityMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string StrategySource, string Header) BuildSource(MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;
        return ($"{ruleComment}{this.strategy.Source}", string.Empty);
    }
}