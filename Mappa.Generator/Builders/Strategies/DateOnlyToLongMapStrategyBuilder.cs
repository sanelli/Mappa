// <copyright file="DateOnlyToLongMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToDateOnlyMapStrategy"/> strategy.
/// </summary>
internal sealed class DateOnlyToLongMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly DateOnlyToLongMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateOnlyToLongMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public DateOnlyToLongMapStrategyBuilder(DateOnlyToLongMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"long {temporary} = (long)new System.DateTime({source}, System.TimeOnly.MinValue).ToUniversalTime().Subtract(System.DateOnly.UnixEpoch).TotalSeconds;";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}