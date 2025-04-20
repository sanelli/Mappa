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

        AppendTargetVariable(
            stringBuilder,
            source,
            context,
            this.strategy.MethodSymbol,
            this.strategy.TargetType,
            this.strategy.SourceType,
            out var targetVariableName,
            out var addMethod,
            out var targetCounterTemporary,
            out var interfaceMethodAccessMode,
            out var interfaceToAccessFrom);
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
                    if (interfaceMethodAccessMode == InterfaceMethodAccessMode.InterfaceExplicit)
                    {
                        var interfaceTemporary = context.NextTemporary();
                        stringBuilder.AppendLine($"{interfaceToAccessFrom} {interfaceTemporary} = {targetVariableName};");
                        stringBuilder.AppendLine($"{interfaceTemporary}.Add({targetElementVariable});");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"{targetVariableName}.Add({targetElementVariable});");
                    }

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
        AppendPostLoopCode(stringBuilder, context, this.strategy.TargetType, ref targetVariableName);

        return (targetVariableName, stringBuilder.ToString());
    }

    private static void AppendPostLoopCode(PrettyCode.StringBuilder stringBuilder, MappaBuilderContext context, ITypeSymbol targetTypeSymbol, ref string targetVariableName)
    {
        if (targetTypeSymbol.IsSpan(context.Compilation)
            || targetTypeSymbol.IsReadOnlySpan(context.Compilation)
            || targetTypeSymbol.IsMemory(context.Compilation)
            || targetTypeSymbol.IsReadOnlyMemory(context.Compilation)
            || targetTypeSymbol.IsReadOnlyCollection(context.Compilation)
            || targetTypeSymbol.IsReadOnlySet(context.Compilation))
        {
            var targetTypeDisplayString = targetTypeSymbol.ToDisplayString();
            var postLoopVariableName = context.NextTemporary();
            stringBuilder.AppendEmptyLine();
            stringBuilder.AppendLine($"global::{targetTypeDisplayString} {postLoopVariableName} = new global::{targetTypeDisplayString}({targetVariableName});");
            targetVariableName = postLoopVariableName;
        }
        else if (targetTypeSymbol.IsFrozenSet(context.Compilation))
        {
            var postLoopVariableName = context.NextTemporary();
            stringBuilder.AppendEmptyLine();
            string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
            stringBuilder.AppendLine($"global::System.Collections.Frozen.FrozenSet<{elementTypeDisplayString}> {postLoopVariableName} = System.Collections.Frozen.FrozenSet.ToFrozenSet<{elementTypeDisplayString}>({targetVariableName});");
            targetVariableName = postLoopVariableName;
        }
        else if (targetTypeSymbol.IsIImmutableSet(context.Compilation)
                 || targetTypeSymbol.IsImmutableHashSet(context.Compilation))
        {
            var postLoopVariableName = context.NextTemporary();
            stringBuilder.AppendEmptyLine();
            string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
            stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {postLoopVariableName} = System.Collections.Immutable.ImmutableHashSet.ToImmutableHashSet<{elementTypeDisplayString}>({targetVariableName});");
            targetVariableName = postLoopVariableName;
        }
        else if (targetTypeSymbol.IsImmutableSortedSet(context.Compilation))
        {
            var postLoopVariableName = context.NextTemporary();
            stringBuilder.AppendEmptyLine();
            string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
            stringBuilder.AppendLine($"global::System.Collections.Immutable.ImmutableSortedSet<{elementTypeDisplayString}> {postLoopVariableName} = System.Collections.Immutable.ImmutableSortedSet.ToImmutableSortedSet<{elementTypeDisplayString}>({targetVariableName});");
            targetVariableName = postLoopVariableName;
        }
        else if (targetTypeSymbol.IsIImmutableList(context.Compilation)
                 || targetTypeSymbol.IsImmutableArray(context.Compilation))
        {
            var postLoopVariableName = context.NextTemporary();
            stringBuilder.AppendEmptyLine();
            string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
            stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {postLoopVariableName} = System.Collections.Immutable.ImmutableArray.ToImmutableArray<{elementTypeDisplayString}>({targetVariableName});");
            targetVariableName = postLoopVariableName;
        }
        else if (targetTypeSymbol.IsImmutableList(context.Compilation))
        {
            var postLoopVariableName = context.NextTemporary();
            stringBuilder.AppendEmptyLine();
            string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
            stringBuilder.AppendLine($"global::System.Collections.Immutable.ImmutableList<{elementTypeDisplayString}> {postLoopVariableName} = System.Collections.Immutable.ImmutableList.ToImmutableList<{elementTypeDisplayString}>({targetVariableName});");
            targetVariableName = postLoopVariableName;
        }
        else if (targetTypeSymbol.IsIImmutableQueue(context.Compilation)
                 || targetTypeSymbol.IsImmutableQueue(context.Compilation))
        {
            var postLoopVariableName = context.NextTemporary();
            stringBuilder.AppendEmptyLine();
            string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
            stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {postLoopVariableName} = System.Collections.Immutable.ImmutableQueue.Create<{elementTypeDisplayString}>({targetVariableName});");
            targetVariableName = postLoopVariableName;
        }
        else if (targetTypeSymbol.IsIImmutableStack(context.Compilation)
                 || targetTypeSymbol.IsImmutableStack(context.Compilation))
        {
            var postLoopVariableName = context.NextTemporary();
            stringBuilder.AppendEmptyLine();
            string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
            stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {postLoopVariableName} = System.Collections.Immutable.ImmutableStack.Create<{elementTypeDisplayString}>({targetVariableName});");
            targetVariableName = postLoopVariableName;
        }
    }

    private static void AppendTargetVariable(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        IMethodSymbol? methodSymbol,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        out string targetVariableName,
        out InsertionMethod insertionMethod,
        out string? counterVariableName,
        out InterfaceMethodAccessMode interfaceMethodAccessMode,
        out string interfaceToAccessFrom)
    {
        targetVariableName = context.NextTemporary();
        counterVariableName = null;
        interfaceMethodAccessMode = InterfaceMethodAccessMode.None;
        interfaceToAccessFrom = string.Empty;

        if (targetTypeSymbol.IsArray()
            || targetTypeSymbol.IsSpan(context.Compilation)
            || targetTypeSymbol.IsReadOnlySpan(context.Compilation)
            || targetTypeSymbol.IsMemory(context.Compilation)
            || targetTypeSymbol.IsReadOnlyMemory(context.Compilation)
            || targetTypeSymbol.IsIImmutableQueue(context.Compilation)
            || targetTypeSymbol.IsImmutableQueue(context.Compilation)
            || targetTypeSymbol.IsIImmutableStack(context.Compilation)
            || targetTypeSymbol.IsImmutableStack(context.Compilation))
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
                 || targetTypeSymbol.IsHashSet(context.Compilation)
                 || targetTypeSymbol.IsReadOnlySet(context.Compilation))
        {
            // We are going to always use an HashSet so Add method is best here.
            insertionMethod = InsertionMethod.Add;
            TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, context.Compilation, out var capacity);
            stringBuilder.AppendLine($"global::System.Collections.Generic.HashSet<{targetTypeSymbol.GetElementType().ToDisplayString()}> {targetVariableName} = new global::System.Collections.Generic.HashSet<{targetTypeSymbol.GetElementType().ToDisplayString()}>({capacity});");
        }
        else if (targetTypeSymbol.ImplementISet(context.Compilation)
                 && targetTypeSymbol.HasSymbolAccessibleZeroParametersConstructor(context.Compilation, methodSymbol))
        {
            insertionMethod = InsertionMethod.Add;
            var elementType = targetTypeSymbol.GetElementType();

            // Use ICollection because ISet derive the Add from ICollection
            interfaceToAccessFrom = $"System.Collections.Generic.ICollection<{TypeSymbolExtensions.NormalizeType(elementType.ToDisplayString())}>";
            interfaceMethodAccessMode = targetTypeSymbol.GetInterfaceMethodAccessMode(
                "Add",
                "System.Collections.Generic.ICollection",
                TypeSymbolExtensions.NormalizeType(elementType.ToDisplayString()),
                returnType => returnType.IsVoid(),
                [elementType]);

            stringBuilder.AppendLine($"global::{targetTypeSymbol} {targetVariableName} = new global::{targetTypeSymbol}();");
        }
        else if (targetTypeSymbol.IsOrImplementStack(context.Compilation)
                 || targetTypeSymbol.IsOrImplementConcurrentStack(context.Compilation))
        {
            insertionMethod = InsertionMethod.Push;
            var capacity = string.Empty;

            // NOTE: ConcurrentStack does not have a constructor accepting a capacity.
            if (targetTypeSymbol.IsStack(context.Compilation)
                && TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, context.Compilation, out var detectedCapacity))
            {
                capacity = detectedCapacity;
            }

            stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {targetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}({capacity});");
        }
        else if (targetTypeSymbol.IsOrImplementQueue(context.Compilation)
                 || targetTypeSymbol.IsOrImplementConcurrentQueue(context.Compilation))
        {
            insertionMethod = InsertionMethod.Enqueue;
            var capacity = string.Empty;

            // NOTE: ConcurrentQueue does not have a constructor accepting a capacity.
            if (targetTypeSymbol.IsQueue(context.Compilation)
                && TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, context.Compilation, out var detectedCapacity))
            {
                capacity = detectedCapacity;
            }

            stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {targetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}({capacity});");
        }
        else if (targetTypeSymbol.IsOrImplementBlockingCollection(context.Compilation)
                 || targetTypeSymbol.IsOrImplementConcurrentBag(context.Compilation))
        {
            insertionMethod = InsertionMethod.Add;
            var capacity = string.Empty;

            // NOTE: ConcurrentBag does not have a constructor accepting a capacity.
            if (targetTypeSymbol.IsBlockingCollection(context.Compilation)
                && TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, context.Compilation, out var detectedCapacity))
            {
                capacity = detectedCapacity;
            }

            stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {targetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}({capacity});");
        }
        else if (targetTypeSymbol.IsIEnumerable()
            || targetTypeSymbol.IsList(context.Compilation)
            || targetTypeSymbol.IsIList()
            || targetTypeSymbol.IsIReadOnlyList()
            || targetTypeSymbol.IsICollection()
            || targetTypeSymbol.IsIReadOnlyCollection()
            || targetTypeSymbol.IsReadOnlyCollection(context.Compilation)
            || targetTypeSymbol.IsFrozenSet(context.Compilation)
            || targetTypeSymbol.IsIImmutableSet(context.Compilation)
            || targetTypeSymbol.IsImmutableHashSet(context.Compilation)
            || targetTypeSymbol.IsImmutableSortedSet(context.Compilation)
            || targetTypeSymbol.IsIImmutableList(context.Compilation)
            || targetTypeSymbol.IsImmutableArray(context.Compilation)
            || targetTypeSymbol.IsImmutableList(context.Compilation))
        {
            // We are going to always use a list, so Add method is best here.
            insertionMethod = InsertionMethod.Add;

            // Note: even if we set capacity, the list would be empty so we cannot invoke an indexer, but only Add.
            // (having an initial capacity is anyway an improvement on the performances).
            TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, context.Compilation, out var capacity);
            stringBuilder.AppendLine($"global::System.Collections.Generic.List<{targetTypeSymbol.GetElementType().ToDisplayString()}> {targetVariableName} = new global::System.Collections.Generic.List<{targetTypeSymbol.GetElementType().ToDisplayString()}>({capacity});");
        }
        else if (targetTypeSymbol.ImplementICollection()
                 && targetTypeSymbol.HasSymbolAccessibleZeroParametersConstructor(context.Compilation, methodSymbol))
        {
            // TODO [#109] Support constructor with 1 integer parameter (capacity) via mappaSettings.
            // here we handle the scenario of the a concrete type implementing ICollection<T>.
            // We are sure that is concrete because ICollection<T> is implemented in a different branch
            // and we re also sure it has a constructor with 0 arguments that can be used.
            insertionMethod = InsertionMethod.Add;

            var elementType = targetTypeSymbol.GetElementType();
            interfaceToAccessFrom = $"System.Collections.Generic.ICollection<{TypeSymbolExtensions.NormalizeType(elementType.ToDisplayString())}>";
            interfaceMethodAccessMode = targetTypeSymbol.GetInterfaceMethodAccessMode(
                "Add",
                "System.Collections.Generic.ICollection",
                TypeSymbolExtensions.NormalizeType(elementType.ToDisplayString()),
                returnType => returnType.IsVoid(),
                [elementType]);

            stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {targetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}();");
        }
        else if (targetTypeSymbol.IsIProducerConsumerCollection(context.Compilation))
        {
            // We are going to always use a concurrent bag, so Add method is best here.
            insertionMethod = InsertionMethod.Add;
            stringBuilder.AppendLine($"global::System.Collections.Concurrent.ConcurrentBag<{targetTypeSymbol.GetElementType().ToDisplayString()}> {targetVariableName} = new global::System.Collections.Concurrent.ConcurrentBag<{targetTypeSymbol.GetElementType().ToDisplayString()}>();");
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