// <copyright file="InvokeParseMethodMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for strategy <see cref="InvokeParseMethodMapStrategy"/>.
/// </summary>
/// <param name="strategy">The strategy to build.</param>
internal sealed class InvokeParseMethodMapStrategyBuilder(InvokeParseMethodMapStrategy strategy)
    : IMappaStrategyBuilder
{
    private readonly InvokeParseMethodMapStrategy strategy = strategy;

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var variableName = context.NextTemporary();
        var code = $"{this.strategy.TargetType.ToDisplayString()} {variableName} = {this.strategy.TargetType.ToDisplayString()}.Parse({source});";
        return (variableName, code);
    }
}