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

    private enum InsertionMethod
    {
        Indexer,
        Add,
        Push,
        Enqueue,
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var stringBuilder = new PrettyCode.StringBuilder();

        AppendTargetVariable(stringBuilder, source, context, this.strategy.TargetType, this.strategy.SourceType, out var targetVariableName, out var addMethod, out var targetCounterTemporary);
        using (AppendLoopBlock(
                   stringBuilder,
                   source,
                   context,
                   this.strategy.SourceType,
                   out var loopVariableName,
                   out var loopCounterTemporary))
        {
            var elementStrategyBuilder = this.strategy.ElementStrategy.GetBuilder();
            var (targetElementVariable, targetElementCode) = elementStrategyBuilder.BuildSource(loopVariableName, context, mappaGlobalOptions);
            if (!string.IsNullOrWhiteSpace(targetElementCode))
            {
                stringBuilder.AppendLine(targetElementCode);
            }

            switch (addMethod)
            {
                case InsertionMethod.Indexer:
                    var index = loopCounterTemporary ?? targetCounterTemporary ?? throw new MappaGeneratorException("Cannot identify a suitable index");
                    stringBuilder.AppendLine($"{targetVariableName}[{index}] = {targetElementVariable};");

                    // If there is no counting variable from the loop the target counter must be increased.
                    if (string.IsNullOrWhiteSpace(loopCounterTemporary))
                    {
                        stringBuilder.AppendLine($"{targetCounterTemporary} = {targetCounterTemporary} + 1;");
                    }

                    break;
                case InsertionMethod.Add:
                    stringBuilder.AppendLine($"{targetVariableName}.Add({targetElementVariable});");
                    break;
                case InsertionMethod.Push:
                    stringBuilder.AppendLine($"{targetVariableName}.Push({targetElementVariable});");
                    break;
                case InsertionMethod.Enqueue:
                    stringBuilder.AppendLine($"{targetVariableName}.Enqueue({targetElementVariable});");
                    break;
                default:
                    throw new MappaGeneratorException("Unexpected add method.");
            }
        }

        // For some types we need to do a bit of post-processing to make sure we always return the correct type
        // (e.g. if we convert a T[] into a Span<T>, even if not needed it clarifies the code).
        if (this.strategy.TargetType.IsSpan(context.Compilation)
            || this.strategy.TargetType.IsReadOnlySpan(context.Compilation)
            || this.strategy.TargetType.IsMemory(context.Compilation)
            || this.strategy.TargetType.IsReadOnlyMemory(context.Compilation))
        {
            var targetTypeDisplayString = this.strategy.TargetType.ToDisplayString();
            var postTargetVariableName = context.NextTemporary();
            stringBuilder.AppendEmptyLine();
            stringBuilder.AppendLine($"global::{targetTypeDisplayString} {postTargetVariableName} = new global::{targetTypeDisplayString}({targetVariableName});");
            targetVariableName = postTargetVariableName;
        }

        return (targetVariableName, stringBuilder.ToString());
    }

    private static void AppendTargetVariable(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        out string targetVariableName,
        out InsertionMethod insertionMethod,
        out string? counterVariableName)
    {
        targetVariableName = context.NextTemporary();
        counterVariableName = null;

        // TODO [#105] Handle HashSet separately in order to make sure we could use capacity if available.
        if (targetTypeSymbol.IsArray()
            || targetTypeSymbol.IsSpan(context.Compilation)
            || targetTypeSymbol.IsReadOnlySpan(context.Compilation)
            || targetTypeSymbol.IsMemory(context.Compilation)
            || targetTypeSymbol.IsReadOnlyMemory(context.Compilation))
        {
            // Array need indexers.
            insertionMethod = InsertionMethod.Indexer;

            // Capacity is always mandatory for arrays.
            // In some scenarios it might mean we invoke the Enumerable.Count() extension method which
            // might results in enumerations being executed twice.
            var capacity = GetLengthExpression(source, sourceTypeSymbol, context.Compilation);
            stringBuilder.AppendLine($"{targetTypeSymbol.GetElementType().ToDisplayString()}[] {targetVariableName} = new {targetTypeSymbol.GetElementType().ToDisplayString()}[{capacity}];");

            // If source does not have an indexer we need to create a new counter variable
            // this for instance is used when mapping generic IEnumerable<TSource> to TTarget[].
            if (!HasIndexer(context, sourceTypeSymbol))
            {
                counterVariableName = context.NextTemporary();
                stringBuilder.AppendLine($"int {counterVariableName} = 0;");
            }
        }
        else if (targetTypeSymbol.IsISet(context.Compilation)
                 || targetTypeSymbol.IsIReadOnlySet(context.Compilation)
                 || targetTypeSymbol.IsHashSet(context.Compilation))
        {
            // We are going to always use an HashSet so Add method is best here.
            insertionMethod = InsertionMethod.Add;
            TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, context.Compilation, out var capacity);
            stringBuilder.AppendLine($"global::System.Collections.Generic.HashSet<{targetTypeSymbol.GetElementType().ToDisplayString()}> {targetVariableName} = new global::System.Collections.Generic.HashSet<{targetTypeSymbol.GetElementType().ToDisplayString()}>({capacity});");
        }
        else if (targetTypeSymbol.IsOrImplementStack(context.Compilation))
        {
            insertionMethod = InsertionMethod.Push;
            var capacity = string.Empty;
            if (targetTypeSymbol.IsStack(context.Compilation)
                && TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, context.Compilation, out var detectedCapacity))
            {
                capacity = detectedCapacity;
            }

            stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {targetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}({capacity});");
        }
        else if (targetTypeSymbol.IsOrImplementQueue(context.Compilation))
        {
            insertionMethod = InsertionMethod.Enqueue;
            var capacity = string.Empty;
            if (targetTypeSymbol.IsQueue(context.Compilation)
                && TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, context.Compilation, out var detectedCapacity))
            {
                capacity = detectedCapacity;
            }

            stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {targetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}({capacity});");
        }
        else if (targetTypeSymbol.IsIEnumerable()
            || targetTypeSymbol.IsList(context.Compilation)
            || targetTypeSymbol.IsIList()
            || targetTypeSymbol.IsICollection()
            || targetTypeSymbol.IsIReadOnlyCollection())
        {
            // We are going to always use a list so Add method is best here.
            insertionMethod = InsertionMethod.Add;

            // Note: even if we set capacity the list would be empty so we cannot invoke an indexer, but only Add.
            // (having an initial capacity is anyway an improvement on the performances).
            TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, context.Compilation, out var capacity);
            stringBuilder.AppendLine($"global::System.Collections.Generic.List<{targetTypeSymbol.GetElementType().ToDisplayString()}> {targetVariableName} = new global::System.Collections.Generic.List<{targetTypeSymbol.GetElementType().ToDisplayString()}>({capacity});");
        }
        else if (targetTypeSymbol.ImplementICollection())
        {
            // TODO [#109] Support constructor with 1 integer parameter (capacity) via mappaSettings.
            // here we handle the scenario of the a concrete type implementing ICollection<T>.
            // We are sure that is concrete because ICollection<T> is implemented in a different branch
            // and we re also sure it has a constructor with 0 arguments that can be used.
            insertionMethod = InsertionMethod.Add;
            stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {targetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}();");
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
        string? spanTemporary = null;
        if (HasIndexer(context, sourceTypeSymbol))
        {
            // For Memory<T> or ReadOnlyMemory<T> we need to access the Span<T>/ReadOnlySpan<T> instance via the Span property.
            if (sourceTypeSymbol.IsMemory(context.Compilation))
            {
                spanTemporary = context.NextTemporary();
                stringBuilder.AppendLine($"global::System.Span<{sourceTypeSymbol.GetElementType().ToDisplayString()}> {spanTemporary} = {source}.Span;");
            }
            else if (sourceTypeSymbol.IsReadOnlyMemory(context.Compilation))
            {
                spanTemporary = context.NextTemporary();
                stringBuilder.AppendLine($"global::System.ReadOnlySpan<{sourceTypeSymbol.GetElementType().ToDisplayString()}> {spanTemporary} = {source}.Span;");
            }

            countingVariableName = context.NextTemporary();
            loopVariableName = context.NextTemporary();

            stringBuilder.AppendLine($"for (int {countingVariableName} = 0; {countingVariableName} < {GetLengthExpression(spanTemporary ?? source, sourceTypeSymbol, context.Compilation)}; ++{countingVariableName})");
            var block = stringBuilder.CurlyBracesBlock();
            stringBuilder.AppendLine($"{sourceTypeSymbol.GetElementType().ToDisplayString()} {loopVariableName} = {spanTemporary ?? source}[{countingVariableName}];");
            return block;
        }

        // Let's use a generic foreach loop (therefore without a counter)!
        countingVariableName = null;
        loopVariableName = context.NextTemporary();
        stringBuilder.AppendLine($"foreach ({sourceTypeSymbol.GetElementType().ToDisplayString()} {loopVariableName} in {source})");
        return stringBuilder.CurlyBracesBlock();
    }

    private static bool HasIndexer(MappaBuilderContext context, ITypeSymbol sourceTypeSymbol)
    {
        return sourceTypeSymbol.IsArray()
               || sourceTypeSymbol.IsSpan(context.Compilation)
               || sourceTypeSymbol.IsReadOnlySpan(context.Compilation)
               || sourceTypeSymbol.IsMemory(context.Compilation) // Indexer by accessing the Span property
               || sourceTypeSymbol.IsReadOnlyMemory(context.Compilation) // Indexer by accessing the Span property
               || sourceTypeSymbol.IsOrImplementIList();
    }

    private static bool TryGetLengthExpressionFromProperty(
        string source,
        ITypeSymbol sourceTypeSymbol,
        Compilation compilation,
        out string lengthExpression)
    {
        if (sourceTypeSymbol.IsArray()
            || sourceTypeSymbol.IsSpan(compilation)
            || sourceTypeSymbol.IsReadOnlySpan(compilation)
            || sourceTypeSymbol.IsMemory(compilation)
            || sourceTypeSymbol.IsReadOnlyMemory(compilation))
        {
            lengthExpression = $"{source}.Length";
        }
        else if (sourceTypeSymbol.IsOrImplementICollection()
                 || sourceTypeSymbol.IsOrImplementIReadOnlyCollection())
        {
            lengthExpression = $"{source}.Count";
        }
        else
        {
            lengthExpression = string.Empty;
        }

        return lengthExpression.Length > 0;
    }

    private static string GetLengthExpression(string source, ITypeSymbol sourceTypeSymbol, Compilation compilation)
    {
        if (TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, compilation, out var lengthExpression))
        {
            return lengthExpression;
        }

        return $"global::System.Linq.Enumerable.Count<{sourceTypeSymbol.GetElementType().ToDisplayString()}>({source})";
    }
}