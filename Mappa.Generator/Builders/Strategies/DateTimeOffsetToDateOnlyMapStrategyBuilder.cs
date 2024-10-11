// <copyright file="DateTimeOffsetToDateOnlyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="DateTimeOffsetToDateOnlyMapStrategy"/> strategy.
/// </summary>
internal sealed class DateTimeOffsetToDateOnlyMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly DateTimeOffsetToDateOnlyMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeOffsetToDateOnlyMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public DateTimeOffsetToDateOnlyMapStrategyBuilder(DateTimeOffsetToDateOnlyMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"System.DateOnly {temporary} = System.DateOnly.FromDateTime({source}.DateTime);";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}