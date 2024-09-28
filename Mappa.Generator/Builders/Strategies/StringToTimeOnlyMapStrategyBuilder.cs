// <copyright file="StringToTimeOnlyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToTimeOnlyMapStrategy"/> strategy.
/// </summary>
internal sealed class StringToTimeOnlyMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly StringToTimeOnlyMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringToTimeOnlyMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public StringToTimeOnlyMapStrategyBuilder(StringToTimeOnlyMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"System.TimeOnly {temporary} = System.TimeOnly.Parse({source});";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}