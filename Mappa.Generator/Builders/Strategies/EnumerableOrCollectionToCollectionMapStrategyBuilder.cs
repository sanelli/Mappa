// <copyright file="EnumerableOrCollectionToCollectionMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="EnumerableOrCollectionToCollectionMapStrategy"/> strategy.
/// </summary>
internal sealed class EnumerableOrCollectionToCollectionMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly EnumerableOrCollectionToCollectionMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerableOrCollectionToCollectionMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public EnumerableOrCollectionToCollectionMapStrategyBuilder(EnumerableOrCollectionToCollectionMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var targetElementType = this.strategy.TargetType.GetElementType();
        var sourceElementType = this.strategy.SourceType.GetElementType();

        var returnVariable = context.NextTemporary();
        var loopTemporary = context.NextTemporary();

        // TODO [#12] If the input is IList<T>, List<T> or T[] we might be able to optimise the code.
        var builder = new PrettyCode.StringBuilder();
        builder.AppendLine($"System.Collections.Generic.List<{targetElementType.ToDisplayString()}> {returnVariable} = new System.Collections.Generic.List<{targetElementType.ToDisplayString()}>();");
        builder.AppendLine($"foreach ({sourceElementType.ToDisplayString()} {loopTemporary} in {source})");
        using (builder.CurlyBracesBlock())
        {
            var (innerVariable, innerStrategyCode) = this.strategy.ElementStrategy.GetBuilder().BuildSource(loopTemporary, context, mappaGlobalOptions);
            if (!string.IsNullOrEmpty(innerStrategyCode))
            {
                builder.AppendLine(innerStrategyCode);
                builder.AppendEmptyLine();
            }

            builder.AppendLine($"{returnVariable}.Add({innerVariable});");
        }

        return (returnVariable, builder.ToString());
    }
}