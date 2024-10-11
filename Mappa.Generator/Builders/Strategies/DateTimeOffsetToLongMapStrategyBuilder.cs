// <copyright file="DateTimeOffsetToLongMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="DateTimeOffsetToLongMapStrategy"/> strategy.
/// </summary>
internal sealed class DateTimeOffsetToLongMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly DateTimeOffsetToLongMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeOffsetToLongMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public DateTimeOffsetToLongMapStrategyBuilder(DateTimeOffsetToLongMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"long {temporary} = {source}.ToUnixTimeSeconds();";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}