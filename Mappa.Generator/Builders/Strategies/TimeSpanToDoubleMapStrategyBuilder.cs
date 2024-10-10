// <copyright file="TimeSpanToDoubleMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToTimeSpanMapStrategy"/> strategy.
/// </summary>
internal sealed class TimeSpanToDoubleMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly TimeSpanToDoubleMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSpanToDoubleMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public TimeSpanToDoubleMapStrategyBuilder(TimeSpanToDoubleMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"double {temporary} = {source}.TotalSeconds;";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}