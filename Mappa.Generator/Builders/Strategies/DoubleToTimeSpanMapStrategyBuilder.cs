// <copyright file="DoubleToTimeSpanMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToTimeSpanMapStrategy"/> strategy.
/// </summary>
internal sealed class DoubleToTimeSpanMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly DoubleToTimeSpanMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleToTimeSpanMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public DoubleToTimeSpanMapStrategyBuilder(DoubleToTimeSpanMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"System.TimeSpan {temporary} = System.TimeSpan.FromSeconds({source});";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}