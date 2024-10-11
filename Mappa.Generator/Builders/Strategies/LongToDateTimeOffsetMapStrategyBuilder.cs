// <copyright file="LongToDateTimeOffsetMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="LongToDateTimeOffsetMapStrategy"/> strategy.
/// </summary>
internal sealed class LongToDateTimeOffsetMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly LongToDateTimeOffsetMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="LongToDateTimeOffsetMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public LongToDateTimeOffsetMapStrategyBuilder(LongToDateTimeOffsetMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"System.DateTimeOffset {temporary} = System.DateTimeOffset.FromUnixTimeSeconds({source});";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}