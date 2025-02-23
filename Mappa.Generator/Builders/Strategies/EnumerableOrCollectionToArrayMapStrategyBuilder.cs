// <copyright file="EnumerableOrCollectionToArrayMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="EnumerableOrCollectionToArrayMapStrategy"/> strategy.
/// </summary>
internal sealed class EnumerableOrCollectionToArrayMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly EnumerableOrCollectionToArrayMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerableOrCollectionToArrayMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public EnumerableOrCollectionToArrayMapStrategyBuilder(EnumerableOrCollectionToArrayMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var targetElementType = this.strategy.TargetType.GetElementType();
        var sourceElementType = this.strategy.SourceType.GetElementType();

        var listTemporary = context.NextTemporary();
        var loopTemporary = context.NextTemporary();

        var builder = new PrettyCode.StringBuilder();
        builder.AppendLine($"System.Collections.Generic.List<{targetElementType.ToDisplayString()}> {listTemporary} = new System.Collections.Generic.List<{targetElementType.ToDisplayString()}>();");
        builder.AppendLine($"foreach ({sourceElementType.ToDisplayString()} {loopTemporary} in {source})");
        using (builder.CurlyBracesBlock())
        {
            var (innerVariable, innerStrategyCode) = this.strategy.ElementStrategy.GetBuilder().BuildSource(loopTemporary, context, mappaGlobalOptions);
            builder.AppendLine(innerStrategyCode);
            builder.AppendEmptyLine();

            builder.AppendLine($"{listTemporary}.Add({innerVariable});");
        }

        var arrayTemporary = context.NextTemporary();
        builder.AppendLine($"{targetElementType.ToDisplayString()}[] {arrayTemporary} = {listTemporary}.ToArray();");

        return (arrayTemporary, builder.ToString());
    }
}