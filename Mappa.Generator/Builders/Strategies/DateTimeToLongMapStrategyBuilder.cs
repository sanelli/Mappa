// <copyright file="DateTimeToLongMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="DateTimeToLongMapStrategy"/> strategy.
/// </summary>
internal sealed class DateTimeToLongMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly DateTimeToLongMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeToLongMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public DateTimeToLongMapStrategyBuilder(DateTimeToLongMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"long {temporary} = (long){source}.ToUniversalTime().Subtract(System.DateTime.UnixEpoch).TotalSeconds;";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}