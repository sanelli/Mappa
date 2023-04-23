// <copyright file="ArrayOrListToArrayMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ArrayOrListToArrayMapStrategy"/> strategy.
/// </summary>
internal sealed class ArrayOrListToArrayMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly ArrayOrListToArrayMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayOrListToArrayMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public ArrayOrListToArrayMapStrategyBuilder(ArrayOrListToArrayMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var ruleComment = mappaGlobalOptions.MappaDebugComments
            ? $"/* Mappa Rule: {this.strategy.Rule} (inner strategy: {this.strategy.ElementStrategy.Rule}) */ "
            : string.Empty;

        var targetElementType = this.strategy.TargetType.GetElementType();
        var sourceElementType = this.strategy.SourceType.GetElementType();

        var lengthTemporary = context.NextTemporary();
        var returnVariable = context.NextTemporary();
        var indexTemporary = context.NextTemporary();

        var builder = new IndentStringBuilder();
        builder.AppendLine($"int {lengthTemporary} = {source}.{this.strategy.SourceType.GetCountProperty()};");
        builder.AppendLine($"{targetElementType.ToDisplayString()}[] {returnVariable} = new {targetElementType.ToDisplayString()}[{lengthTemporary}];");
        builder.AppendLine($"for (int {indexTemporary} = 0 ; {indexTemporary} < {lengthTemporary} ; ++{indexTemporary})");
        using (builder.CodeBlock())
        using (builder.Indent())
        {
            var itemTemporary = context.NextTemporary();
            builder.AppendLine($"{sourceElementType} {itemTemporary} = {source}[{indexTemporary}];");
            var (innerVariable, innerStrategyCode) = this.strategy.ElementStrategy.GetBuilder().BuildSource(itemTemporary, context, mappaGlobalOptions);
            if (!string.IsNullOrEmpty(innerStrategyCode))
            {
                builder.AppendLine(innerStrategyCode);
                builder.AppendEmptyLine();
            }

            builder.AppendLine($"{returnVariable}[{indexTemporary}] = {innerVariable};");
        }

        return ($"{ruleComment}{returnVariable}", builder.ToString());
    }
}