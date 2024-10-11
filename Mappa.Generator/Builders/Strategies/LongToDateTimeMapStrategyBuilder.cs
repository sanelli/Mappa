// <copyright file="LongToDateTimeMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="LongToDateTimeMapStrategy"/> strategy.
/// </summary>
internal sealed class LongToDateTimeMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly LongToDateTimeMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="LongToDateTimeMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public LongToDateTimeMapStrategyBuilder(LongToDateTimeMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"System.DateTime {temporary} = System.DateTime.UnixEpoch.AddSeconds({source});";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}