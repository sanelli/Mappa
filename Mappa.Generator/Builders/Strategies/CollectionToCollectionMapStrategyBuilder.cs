// <copyright file="CollectionToCollectionMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="CollectionToCollectionMapStrategy"/>.
/// </summary>
internal sealed class CollectionToCollectionMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly CollectionToCollectionMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionToCollectionMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public CollectionToCollectionMapStrategyBuilder(CollectionToCollectionMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    private enum AddMethod
    {
        UseIndexer,
        UseAdd,
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var stringBuilder = new PrettyCode.StringBuilder();

        BuildTargetVariable(stringBuilder, source, context, this.strategy.TargetType, this.strategy.SourceType, out var targetVariableName, out var addMethod, out var targerCounterVariableName);
        using (AppendLoopBlock(
                   stringBuilder,
                   source,
                   context,
                   this.strategy.SourceType,
                   out var loopVariableName,
                   out var countingVariableName))
        {
            var elementStrategyBuilder = this.strategy.ElementStrategy.GetBuilder();
            var (targetElementVariable, targetElementCode) = elementStrategyBuilder.BuildSource(loopVariableName, context, mappaGlobalOptions);
            if (!string.IsNullOrWhiteSpace(targetElementCode))
            {
                stringBuilder.AppendLine(targetElementCode);
            }

            switch (addMethod)
            {
                case AddMethod.UseIndexer:
                    var index = countingVariableName ?? targerCounterVariableName ?? throw new MappaGeneratorException("Cannot identify a suitable index");
                    stringBuilder.AppendLine($"{targetVariableName}[{index}] = {targetElementVariable};");
                    if (string.IsNullOrWhiteSpace(countingVariableName))
                    {
                        // If there is no counting variable from the loop the target counter must be increased.
                        stringBuilder.AppendLine($"{targerCounterVariableName} += 1;");
                    }

                    break;
                case AddMethod.UseAdd:
                    stringBuilder.AppendLine($"{targetVariableName}.Add({targetElementVariable});");
                    break;
                default:
                    throw new MappaGeneratorException("Unexpected add method.");
            }
        }

        return (targetVariableName, stringBuilder.ToString());
    }

    private static void BuildTargetVariable(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        out string targetVariableName,
        out AddMethod addMethod,
        out string? counterVariableName)
    {
        targetVariableName = context.NextTemporary();
        var sourceHasIndexer = HasIndexer(context, sourceTypeSymbol);
        var targetHasIndexer = HasIndexer(context, targetTypeSymbol);
        string? capacity = null;
        addMethod = AddMethod.UseAdd;
        counterVariableName = null;

        if (targetTypeSymbol.IsIEnumerable())
        {
            if (sourceHasIndexer || sourceTypeSymbol.IsOrImplementICollection())
            {
                capacity = GetLengthExpression(source, sourceTypeSymbol, context.Compilation);
            }

            if (targetHasIndexer)
            {
                addMethod = AddMethod.UseIndexer;
            }

            stringBuilder.AppendLine($"System.Collections.Generic.List<{targetTypeSymbol.GetElementType().ToDisplayString()}> {targetVariableName} = new System.Collections.Generic.List<{targetTypeSymbol.GetElementType().ToDisplayString()}>({capacity ?? string.Empty});");
            if (targetHasIndexer && !sourceHasIndexer)
            {
                counterVariableName = context.NextTemporary();
                stringBuilder.AppendLine($"int {counterVariableName} = 0;");
            }
        }
        else
        {
            throw new MappaGeneratorException($"Unsupported target type {targetTypeSymbol.ToDisplayString()} during generation of collection to collection mapping.");
        }
    }

    private static IDisposable AppendLoopBlock(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        ITypeSymbol sourceTypeSymbol,
        out string loopVariableName,
        out string? countingVariableName)
    {
        // For array, Span<T> or anything implementing IList we can use a for loop
        // this way we can also use Span<> for ever better performances.
        if (HasIndexer(context, sourceTypeSymbol))
        {
            countingVariableName = context.NextTemporary();
            loopVariableName = context.NextTemporary();

            stringBuilder.AppendLine($"for (int {countingVariableName} = 0; {countingVariableName} < {GetLengthExpression(source, sourceTypeSymbol, context.Compilation)}; ++{countingVariableName})");
            var block = stringBuilder.CurlyBracesBlock();
            stringBuilder.AppendLine($"{sourceTypeSymbol.GetElementType().ToDisplayString()} {loopVariableName} = {source}[{countingVariableName}];");
            return block;
        }

        // Let's use a generic foreach loop!
        countingVariableName = string.Empty;
        loopVariableName = context.NextTemporary();
        stringBuilder.AppendLine($"foreach ({sourceTypeSymbol.GetElementType().ToDisplayString()} {loopVariableName} in {source})");
        return stringBuilder.CurlyBracesBlock();
    }

    private static bool HasIndexer(MappaBuilderContext context, ITypeSymbol sourceTypeSymbol)
    {
        return sourceTypeSymbol.IsArray()
               || sourceTypeSymbol.IsSpan(context.Compilation)
               || sourceTypeSymbol.IsReadOnlySpan(context.Compilation)
               || sourceTypeSymbol.IsMemory(context.Compilation)
               || sourceTypeSymbol.IsReadOnlyMemory(context.Compilation)
               || sourceTypeSymbol.IsOrImplementIList();
    }

    private static string GetLengthExpression(string source, ITypeSymbol sourceTypeSymbol, Compilation compilation)
    {
        if (sourceTypeSymbol.IsArray()
            || sourceTypeSymbol.IsSpan(compilation)
            || sourceTypeSymbol.IsReadOnlySpan(compilation)
            || sourceTypeSymbol.IsMemory(compilation)
            || sourceTypeSymbol.IsReadOnlyMemory(compilation))
        {
            return $"{source}.Length";
        }

        if (sourceTypeSymbol.IsOrImplementICollection())
        {
            return $"{source}.Count";
        }

        return $"global::System.Linq.Enumerable.Count<{sourceTypeSymbol.GetElementType().ToDisplayString()}>({source})";
    }
}