// <copyright file="StringToNumberMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="StringToNumberMapStrategy"/> strategy.
/// </summary>
internal sealed class StringToNumberMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly StringToNumberMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringToNumberMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public StringToNumberMapStrategyBuilder(StringToNumberMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var targetType = this.strategy.TargetType.ToDisplayString();

        var temporary = context.NextTemporary();
        var code = $"{targetType} {temporary} = {targetType}.Parse({source});";

        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (target-type is \"{targetType}\") */ "
            : string.Empty;

        return ($"{ruleComment}{temporary}", code);
    }
}