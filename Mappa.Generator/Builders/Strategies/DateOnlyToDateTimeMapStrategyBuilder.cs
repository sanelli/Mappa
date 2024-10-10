// <copyright file="DateOnlyToDateTimeMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToDateTimeMapStrategy"/> strategy.
/// </summary>
internal sealed class DateOnlyToDateTimeMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly DateOnlyToDateTimeMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateOnlyToDateTimeMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public DateOnlyToDateTimeMapStrategyBuilder(DateOnlyToDateTimeMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"System.DateTime {temporary} = new System.DateTime({source}, System.TimeOnly.MinValue, System.DateTimeKind.Utc);";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}