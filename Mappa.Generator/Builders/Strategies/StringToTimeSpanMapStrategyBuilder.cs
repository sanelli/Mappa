// <copyright file="StringToTimeSpanMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToTimeSpanMapStrategy"/> strategy.
/// </summary>
internal sealed class StringToTimeSpanMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly StringToTimeSpanMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringToTimeSpanMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public StringToTimeSpanMapStrategyBuilder(StringToTimeSpanMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"System.TimeSpan {temporary} = System.TimeSpan.Parse({source});";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}