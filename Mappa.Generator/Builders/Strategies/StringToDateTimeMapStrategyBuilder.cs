// <copyright file="StringToDateTimeMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToDateTimeMapStrategy"/> strategy.
/// </summary>
internal sealed class StringToDateTimeMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly StringToDateTimeMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringToDateTimeMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public StringToDateTimeMapStrategyBuilder(StringToDateTimeMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string StrategySource, string Header) BuildSource(MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new IndentStringBuilder();

        var code = $"System.DateTime.Parse({this.strategy.Source})";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{code}", builder.ToString());
    }
}