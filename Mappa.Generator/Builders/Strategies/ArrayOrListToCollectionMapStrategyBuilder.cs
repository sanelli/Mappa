// <copyright file="ArrayOrListToCollectionMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ArrayOrListToCollectionMapStrategy"/> strategy.
/// </summary>
internal sealed class ArrayOrListToCollectionMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly ArrayOrListToCollectionMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayOrListToCollectionMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public ArrayOrListToCollectionMapStrategyBuilder(ArrayOrListToCollectionMapStrategy strategy)
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
        var countProperty = this.strategy.SourceType.GetCountProperty();

        var lengthTemporary = context.NextTemporary();
        var returnVariable = context.NextTemporary();
        var indexTemporary = context.NextTemporary();

        var builder = new PrettyCode.StringBuilder();
        builder.AppendLine($"int {lengthTemporary} = {source}.{countProperty};");
        builder.AppendLine($"System.Collections.Generic.List<{targetElementType.ToDisplayString()}> {returnVariable} = new System.Collections.Generic.List<{targetElementType.ToDisplayString()}>({lengthTemporary});");
        builder.AppendLine($"for (int {indexTemporary} = 0 ; {indexTemporary} < {lengthTemporary} ; ++{indexTemporary})");
        using (builder.CurlyBracesBlock())
        {
            var itemTemporary = context.NextTemporary();
            builder.AppendLine($"{sourceElementType} {itemTemporary} = {source}[{indexTemporary}];");
            var (innerVariable, innerStrategyCode) = this.strategy.ElementStrategy.GetBuilder().BuildSource(itemTemporary, context, mappaGlobalOptions);
            if (!string.IsNullOrEmpty(innerStrategyCode))
            {
                builder.AppendLine(innerStrategyCode);
                builder.AppendEmptyLine();
            }

            builder.AppendLine($"{returnVariable}.Add({innerVariable});");
        }

        return ($"{ruleComment}{returnVariable}", builder.ToString());
    }
}