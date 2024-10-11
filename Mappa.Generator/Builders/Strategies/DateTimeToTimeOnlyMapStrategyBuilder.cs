// <copyright file="DateTimeToTimeOnlyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="DateTimeToTimeOnlyMapStrategy"/> strategy.
/// </summary>
internal sealed class DateTimeToTimeOnlyMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly DateTimeToTimeOnlyMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeToTimeOnlyMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public DateTimeToTimeOnlyMapStrategyBuilder(DateTimeToTimeOnlyMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"System.TimeOnly {temporary} = System.TimeOnly.FromDateTime({source});";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}