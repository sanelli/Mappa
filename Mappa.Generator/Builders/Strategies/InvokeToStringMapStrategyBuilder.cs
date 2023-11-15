// <copyright file="InvokeToStringMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="InvokeToStringMapStrategy"/> strategy.
/// </summary>
internal sealed class InvokeToStringMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly InvokeToStringMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeToStringMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public InvokeToStringMapStrategyBuilder(InvokeToStringMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var temporary = context.NextTemporary();
        var code = $"string {temporary} = {source}.ToString();";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}