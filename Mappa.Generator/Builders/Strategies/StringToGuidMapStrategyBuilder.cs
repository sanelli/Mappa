// <copyright file="StringToGuidMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToGuidMapStrategy"/> strategy.
/// </summary>
internal sealed class StringToGuidMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly StringToGuidMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringToGuidMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public StringToGuidMapStrategyBuilder(StringToGuidMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var parameters = source;
        var parseMethod = nameof(Guid.Parse);
        if (!string.IsNullOrWhiteSpace(this.strategy.UserSettings.GuidFormat))
        {
            parseMethod = nameof(Guid.ParseExact);
            parameters = $"{parameters}, \"{this.strategy.UserSettings.GuidFormat}\"";
        }

        var temporary = context.NextTemporary();
        var code = $"System.Guid {temporary} = System.Guid.{parseMethod}({parameters});";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}