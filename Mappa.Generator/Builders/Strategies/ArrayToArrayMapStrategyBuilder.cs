// <copyright file="ArrayToArrayMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ArrayToArrayMapStrategy"/> strategy.
/// </summary>
internal sealed class ArrayToArrayMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly ArrayToArrayMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayToArrayMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public ArrayToArrayMapStrategyBuilder(ArrayToArrayMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (inner strategy: {this.strategy.ElementStrategy.Rule}) */ "
            : string.Empty;

        var targetUnderlyingType = this.strategy.TargetType.GetArrayElementType();
        var sourceUnderlyingType = this.strategy.SourceType.GetArrayElementType();

        var returnVariable = context.NextTemporary();
        var counterTemporary = context.NextTemporary();

        var builder = new IndentStringBuilder();
        builder.AppendLine($"{targetUnderlyingType.ToDisplayString()}[] {returnVariable} = new {targetUnderlyingType.ToDisplayString()}[{source}.Length];");
        builder.AppendLine($"for (int {counterTemporary} = 0 ; {counterTemporary} < {source}.Length ; ++{counterTemporary})");
        using (builder.CodeBlock())
        using (builder.Indent())
        {
            var itemTemporary = context.NextTemporary();
            builder.AppendLine($"{sourceUnderlyingType} {itemTemporary} = {source}[{counterTemporary}];");
            var (innerVariable, innerStrategyCode) = this.strategy.ElementStrategy.GetBuilder().BuildSource(itemTemporary, context, mappaGlobalOptions);
            if (!string.IsNullOrEmpty(innerStrategyCode))
            {
                builder.AppendLine(innerStrategyCode);
                builder.AppendEmptyLine();
            }

            builder.AppendLine($"{returnVariable}[{counterTemporary}] = {innerVariable};");
        }

        return ($"{ruleComment}{returnVariable}", builder.ToString());
    }
}