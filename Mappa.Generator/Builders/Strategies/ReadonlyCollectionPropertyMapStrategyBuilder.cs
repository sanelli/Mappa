// <copyright file="ReadonlyCollectionPropertyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="ReadonlyCollectionPropertyMapStrategy"/>.
/// </summary>
internal sealed class ReadonlyCollectionPropertyMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly ReadonlyCollectionPropertyMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadonlyCollectionPropertyMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public ReadonlyCollectionPropertyMapStrategyBuilder(ReadonlyCollectionPropertyMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var stringBuilder = new PrettyCode.StringBuilder();
        var counterTemporary = context.NextTemporary();

        // For array or lists use a for loop
        if (this.strategy.SourceType.IsArray() || this.strategy.SourceType.IsOrImplementIList())
        {
            stringBuilder.AppendLine($"for (int {counterTemporary} = 0; {counterTemporary} < {source}.{GetLengthPropertyName(this.strategy.SourceType)}; ++{counterTemporary})");
            using (stringBuilder.CurlyBracesBlock())
            {
                var elementTemporary = context.NextTemporary();
                stringBuilder.AppendLine($"{this.strategy.SourceType.GetElementType().ToDisplayString()} {elementTemporary} = {source}[{counterTemporary}];");
                var (targetElementTemporary, targetElementCode) = this.strategy.ElementStrategy.GetBuilder().BuildSource(elementTemporary, context, mappaGlobalOptions);
                if (!string.IsNullOrWhiteSpace(targetElementCode))
                {
                    stringBuilder.AppendLine(targetElementCode);
                }

                stringBuilder.AppendLine($"{context.GetCompositeTypeTargetName()}.{this.strategy.TargetProperty.Name}.Add({targetElementTemporary});");
            }
        }

        // For generic IEnumerable use foreach
        else
        {
            stringBuilder.AppendLine($"foreach ({this.strategy.SourceType.GetElementType().ToDisplayString()} {counterTemporary} in {source})");
            using (stringBuilder.CurlyBracesBlock())
            {
                var (targetElementTemporary, targetElementCode) = this.strategy.ElementStrategy.GetBuilder().BuildSource(counterTemporary, context, mappaGlobalOptions);
                if (!string.IsNullOrWhiteSpace(targetElementCode))
                {
                    stringBuilder.AppendLine(targetElementCode);
                }

                stringBuilder.AppendLine($"{context.GetCompositeTypeTargetName()}.{this.strategy.TargetProperty.Name}.Add({targetElementTemporary});");
            }
        }

        return (string.Empty, stringBuilder.ToString());
    }

    private static string GetLengthPropertyName(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsArray())
        {
            return nameof(Array.Length);
        }

        if (typeSymbol.IsOrImplementICollection())
        {
            return nameof(ICollection<int>.Count);
        }

        throw new MappaGeneratorException($"Unable to get length property name for {typeSymbol}");
    }
}