// <copyright file="CollectionToCollectionMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="CollectionToCollectionMapStrategy"/>.
/// </summary>
internal sealed class CollectionToCollectionMapStrategyBuilder(CollectionToCollectionMapStrategy strategy)
    : IMappaStrategyBuilder
{
    private static readonly Func<ITypeSymbol, Compilation, bool>[] ArraySpanMemoryOrImmutableQueueStackPredicates =
    [
        static (type, _) => type.IsArray(),
        static (type, compilation) => type.IsSpan(compilation),
        static (type, compilation) => type.IsReadOnlySpan(compilation),
        static (type, compilation) => type.IsMemory(compilation),
        static (type, compilation) => type.IsReadOnlyMemory(compilation),
        static (type, compilation) => type.IsIImmutableQueue(compilation),
        static (type, compilation) => type.IsImmutableQueue(compilation),
        static (type, compilation) => type.IsIImmutableStack(compilation),
        static (type, compilation) => type.IsImmutableStack(compilation),
    ];

    private static readonly Func<ITypeSymbol, Compilation, bool>[] ListLikeEnumerableTargetPredicates =
    [
        static (type, _) => type.IsIEnumerable(),
        static (type, compilation) => type.IsList(compilation),
        static (type, _) => type.IsIList(),
        static (type, _) => type.IsIReadOnlyList(),
        static (type, _) => type.IsICollection(),
        static (type, _) => type.IsIReadOnlyCollection(),
        static (type, compilation) => type.IsReadOnlyCollection(compilation),
        static (type, compilation) => type.IsFrozenSet(compilation),
        static (type, compilation) => type.IsIImmutableSet(compilation),
        static (type, compilation) => type.IsImmutableHashSet(compilation),
        static (type, compilation) => type.IsImmutableSortedSet(compilation),
        static (type, compilation) => type.IsIImmutableList(compilation),
        static (type, compilation) => type.IsImmutableArray(compilation),
        static (type, compilation) => type.IsImmutableList(compilation),
    ];

    private static readonly PostLoopDispatchEntry[] PostLoopDispatchEntries =
    [
        new(
            IsSpanMemoryOrReadOnlyWrapperTarget,
            static (stringBuilder, context, targetTypeSymbol, ref targetVariableName) =>
                AppendConstructFromBufferPostLoop(stringBuilder, context, targetTypeSymbol, ref targetVariableName),
            stopAfterMatch: true),
        new(
            static (type, compilation) => type.IsFrozenSet(compilation),
            static (stringBuilder, context, targetTypeSymbol, ref targetVariableName) =>
                AppendFrozenSetPostLoop(stringBuilder, context, targetTypeSymbol, ref targetVariableName),
            stopAfterMatch: true),
        new(
            static (type, compilation) =>
                type.IsIImmutableSet(compilation) || type.IsImmutableHashSet(compilation),
            static (stringBuilder, context, targetTypeSymbol, ref targetVariableName) =>
                AppendImmutableHashSetPostLoop(stringBuilder, context, targetTypeSymbol, ref targetVariableName),
            stopAfterMatch: true),
        new(
            static (type, compilation) => type.IsImmutableSortedSet(compilation),
            static (stringBuilder, context, targetTypeSymbol, ref targetVariableName) =>
                AppendImmutableSortedSetPostLoop(stringBuilder, context, targetTypeSymbol, ref targetVariableName),
            stopAfterMatch: true),
        new(
            static (type, compilation) =>
                type.IsIImmutableList(compilation) || type.IsImmutableArray(compilation),
            static (stringBuilder, context, targetTypeSymbol, ref targetVariableName) =>
                AppendImmutableArrayPostLoop(stringBuilder, context, targetTypeSymbol, ref targetVariableName),
            stopAfterMatch: true),
        new(
            static (type, compilation) => type.IsImmutableList(compilation),
            static (stringBuilder, context, targetTypeSymbol, ref targetVariableName) =>
                AppendImmutableListPostLoop(stringBuilder, context, targetTypeSymbol, ref targetVariableName),
            stopAfterMatch: true),
        new(
            static (type, compilation) =>
                type.IsIImmutableQueue(compilation) || type.IsImmutableQueue(compilation),
            static (stringBuilder, context, targetTypeSymbol, ref targetVariableName) =>
                AppendImmutableQueuePostLoop(stringBuilder, context, targetTypeSymbol, ref targetVariableName),
            stopAfterMatch: true),
        new(
            static (type, compilation) =>
                type.IsIImmutableStack(compilation) || type.IsImmutableStack(compilation),
            static (stringBuilder, context, targetTypeSymbol, ref targetVariableName) =>
                AppendImmutableStackPostLoop(stringBuilder, context, targetTypeSymbol, ref targetVariableName),
            stopAfterMatch: false),
    ];

    private static readonly TargetVariableDispatchEntry[] TargetVariableDispatchEntries =
    [
        new(
            static ctx => IsFastCollectionArrayTarget(
                ctx.FastCollections,
                ctx.SourceTypeSymbol,
                ctx.TargetTypeSymbol,
                ctx.BuilderContext.Compilation),
            static ctx => AppendFastCollectionArrayTarget(
                ctx.StringBuilder,
                ctx.Source,
                ctx.BuilderContext,
                ctx.TargetTypeSymbol,
                ctx.SourceTypeSymbol,
                ctx.State)),
        new(
            static ctx => IsArraySpanMemoryOrImmutableQueueStackTarget(ctx.TargetTypeSymbol, ctx.BuilderContext.Compilation),
            static ctx => AppendArraySpanMemoryOrImmutableQueueStackTarget(
                ctx.StringBuilder,
                ctx.Source,
                ctx.BuilderContext,
                ctx.TargetTypeSymbol,
                ctx.SourceTypeSymbol,
                ctx.PreventEnumerableCount,
                ctx.EnumerableConcreteType,
                ctx.State)),
        new(
            static ctx => IsHashSetLikeTarget(ctx.TargetTypeSymbol, ctx.BuilderContext.Compilation),
            static ctx => AppendHashSetLikeTarget(
                ctx.StringBuilder,
                ctx.Source,
                ctx.BuilderContext,
                ctx.TargetTypeSymbol,
                ctx.SourceTypeSymbol,
                ctx.State)),
        new(
            static ctx => ctx.TargetTypeSymbol.IsOrDerivedFromStack(ctx.BuilderContext.Compilation),
            static ctx => AppendStackTarget(
                ctx.StringBuilder,
                ctx.Source,
                ctx.BuilderContext,
                ctx.MethodSymbol,
                ctx.TargetTypeSymbol,
                ctx.SourceTypeSymbol,
                ctx.ContainerCapacityConstructors,
                ctx.State)),
        new(
            static ctx => ctx.TargetTypeSymbol.IsOrDerivedFromConcurrentStack(ctx.BuilderContext.Compilation),
            static ctx => AppendConcurrentStackTarget(ctx.StringBuilder, ctx.TargetTypeSymbol, ctx.State)),
        new(
            static ctx => ctx.TargetTypeSymbol.IsOrDerivedFromQueue(ctx.BuilderContext.Compilation),
            static ctx => AppendQueueTarget(
                ctx.StringBuilder,
                ctx.Source,
                ctx.BuilderContext,
                ctx.MethodSymbol,
                ctx.TargetTypeSymbol,
                ctx.SourceTypeSymbol,
                ctx.ContainerCapacityConstructors,
                ctx.State)),
        new(
            static ctx => ctx.TargetTypeSymbol.IsOrImplementConcurrentQueue(ctx.BuilderContext.Compilation),
            static ctx => AppendConcurrentQueueTarget(ctx.StringBuilder, ctx.TargetTypeSymbol, ctx.State)),
        new(
            static ctx => IsBlockingCollectionOrConcurrentBagTarget(ctx.TargetTypeSymbol, ctx.BuilderContext.Compilation),
            static ctx => AppendBlockingCollectionOrConcurrentBagTarget(
                ctx.StringBuilder,
                ctx.Source,
                ctx.BuilderContext,
                ctx.MethodSymbol,
                ctx.TargetTypeSymbol,
                ctx.SourceTypeSymbol,
                ctx.ContainerCapacityConstructors,
                ctx.State)),
        new(
            static ctx => ShouldUseArrayForEnumerableInterfaceTarget(ctx.TargetTypeSymbol, ctx.EnumerableConcreteType),
            AppendArrayEnumerableInterfaceTarget),
        new(
            static ctx => IsListLikeEnumerableTarget(ctx.TargetTypeSymbol, ctx.BuilderContext.Compilation),
            static ctx => AppendListLikeEnumerableTarget(
                ctx.StringBuilder,
                ctx.Source,
                ctx.BuilderContext,
                ctx.TargetTypeSymbol,
                ctx.SourceTypeSymbol,
                ctx.State)),
        new(
            static ctx => ctx.TargetTypeSymbol.IsIProducerConsumerCollection(ctx.BuilderContext.Compilation),
            static ctx => AppendProducerConsumerCollectionTarget(ctx.StringBuilder, ctx.TargetTypeSymbol, ctx.State)),
        new(
            static ctx => ctx.TargetTypeSymbol.ImplementISet(ctx.BuilderContext.Compilation),
            static ctx => AppendImplementISetTarget(
                ctx.StringBuilder,
                ctx.Source,
                ctx.BuilderContext,
                ctx.MethodSymbol,
                ctx.TargetTypeSymbol,
                ctx.SourceTypeSymbol,
                ctx.ContainerCapacityConstructors,
                ctx.State)),
        new(
            static ctx => ctx.TargetTypeSymbol.ImplementICollection(),
            static ctx => AppendImplementICollectionTarget(
                ctx.StringBuilder,
                ctx.Source,
                ctx.BuilderContext,
                ctx.MethodSymbol,
                ctx.TargetTypeSymbol,
                ctx.SourceTypeSymbol,
                ctx.ContainerCapacityConstructors,
                ctx.State)),
    ];

    private readonly CollectionToCollectionMapStrategy strategy = strategy;

    private delegate void PostLoopAppendAction(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName);

    private delegate void TargetVariableBranchAppender(AppendTargetVariableContext context);

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
            this.strategy.FastCollections,
            this.strategy.ContainerCapacityConstructors,
            this.strategy.PreventEnumerableCount,
            this.strategy.EnumerableConcreteType,
            out var targetVariableName,
            out var addMethod,
            out var targetCounterTemporary,
            out var interfaceMethodAccessMode,
            out var interfaceToAccessFrom,
            out var variableToAccessFrom,
            out var usedGrowableBuffer);
        using (AppendLoopBlock(
                   stringBuilder,
                   source,
                   context,
                   this.strategy.SourceType,
                   this.strategy.FastCollections,
                   out var loopVariableName,
                   out var loopCounterTemporary))
        {
            var (targetElementVariable, targetElementCode) = ReferenceHandlingCodeGenerator.BuildNestedSource(
                this.strategy.ElementStrategy,
                loopVariableName,
                context,
                mappaGlobalOptions);
            if (!string.IsNullOrWhiteSpace(targetElementCode))
            {
                stringBuilder.AppendLine(targetElementCode);
            }

            AppendMappedElementToTarget(
                stringBuilder,
                context,
                addMethod,
                targetVariableName,
                variableToAccessFrom,
                interfaceMethodAccessMode,
                interfaceToAccessFrom,
                targetElementVariable,
                loopCounterTemporary,
                targetCounterTemporary);
        }

        // For some types we need to do a bit of post-processing to make sure we always return the correct type
        // (e.g. if we convert a T[] into a Span<T>, even if not needed it clarifies the code).
        AppendPostLoopCode(stringBuilder, context, this.strategy.TargetType, ref targetVariableName, usedGrowableBuffer);

        return (targetVariableName, stringBuilder.ToString());
    }

    private static void AppendMappedElementToTarget(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        InsertionMethod addMethod,
        string targetVariableName,
        string? variableToAccessFrom,
        InterfaceMethodAccessMode interfaceMethodAccessMode,
        string interfaceToAccessFrom,
        string targetElementVariable,
        string? loopCounterTemporary,
        string? targetCounterTemporary)
    {
        switch (addMethod)
        {
            case InsertionMethod.Indexer:
                AppendMappedElementWithIndexer(
                    stringBuilder,
                    targetVariableName,
                    variableToAccessFrom,
                    targetElementVariable,
                    loopCounterTemporary,
                    targetCounterTemporary);
                break;
            case InsertionMethod.Add:
                AppendMappedElementWithAdd(
                    stringBuilder,
                    context,
                    targetVariableName,
                    interfaceMethodAccessMode,
                    interfaceToAccessFrom,
                    targetElementVariable);
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

    private static void AppendMappedElementWithIndexer(
        PrettyCode.StringBuilder stringBuilder,
        string targetVariableName,
        string? variableToAccessFrom,
        string targetElementVariable,
        string? loopCounterTemporary,
        string? targetCounterTemporary)
    {
        var index = loopCounterTemporary ?? targetCounterTemporary ?? throw new MappaGeneratorException("Cannot identify a suitable index");
        stringBuilder.AppendLine($"{variableToAccessFrom ?? targetVariableName}[{index}] = {targetElementVariable};");

        if (string.IsNullOrWhiteSpace(loopCounterTemporary))
        {
            stringBuilder.AppendLine($"{targetCounterTemporary} = {targetCounterTemporary} + 1;");
        }
    }

    private static void AppendMappedElementWithAdd(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        string targetVariableName,
        InterfaceMethodAccessMode interfaceMethodAccessMode,
        string interfaceToAccessFrom,
        string targetElementVariable)
    {
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
    }

    private static void AppendPostLoopCode(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName,
        bool usedGrowableBuffer)
    {
        if (usedGrowableBuffer)
        {
            AppendGrowableBufferToArrayPostLoop(stringBuilder, context, targetTypeSymbol, ref targetVariableName);
        }

        var compilation = context.Compilation;
        foreach (var entry in PostLoopDispatchEntries)
        {
            if (!entry.Matches(targetTypeSymbol, compilation))
            {
                continue;
            }

            entry.Append(stringBuilder, context, targetTypeSymbol, ref targetVariableName);
            if (entry.StopAfterMatch)
            {
                return;
            }
        }
    }

    private static bool IsSpanMemoryOrReadOnlyWrapperTarget(ITypeSymbol targetTypeSymbol, Compilation compilation)
        => targetTypeSymbol.IsSpan(compilation)
           || targetTypeSymbol.IsReadOnlySpan(compilation)
           || targetTypeSymbol.IsMemory(compilation)
           || targetTypeSymbol.IsReadOnlyMemory(compilation)
           || targetTypeSymbol.IsReadOnlyCollection(compilation)
           || targetTypeSymbol.IsReadOnlySet(compilation);

    private static void AppendGrowableBufferToArrayPostLoop(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName)
    {
        var elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
        var arrayVariableName = context.NextTemporary();
        stringBuilder.AppendEmptyLine();
        stringBuilder.AppendLine($"{elementTypeDisplayString}[] {arrayVariableName} = {targetVariableName}.ToArray();");
        targetVariableName = arrayVariableName;
    }

    private static void AppendConstructFromBufferPostLoop(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName)
    {
        var targetTypeDisplayString = targetTypeSymbol.ToDisplayString();
        var postLoopVariableName = context.NextTemporary();
        stringBuilder.AppendEmptyLine();
        stringBuilder.AppendLine($"global::{targetTypeDisplayString} {postLoopVariableName} = new global::{targetTypeDisplayString}({targetVariableName});");
        targetVariableName = postLoopVariableName;
    }

    private static void AppendFrozenSetPostLoop(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName)
    {
        var postLoopVariableName = context.NextTemporary();
        stringBuilder.AppendEmptyLine();
        string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
        stringBuilder.AppendLine($"global::System.Collections.Frozen.FrozenSet<{elementTypeDisplayString}> {postLoopVariableName} = System.Collections.Frozen.FrozenSet.ToFrozenSet<{elementTypeDisplayString}>({targetVariableName});");
        targetVariableName = postLoopVariableName;
    }

    private static void AppendImmutableHashSetPostLoop(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName)
    {
        var postLoopVariableName = context.NextTemporary();
        stringBuilder.AppendEmptyLine();
        string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
        stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {postLoopVariableName} = System.Collections.Immutable.ImmutableHashSet.ToImmutableHashSet<{elementTypeDisplayString}>({targetVariableName});");
        targetVariableName = postLoopVariableName;
    }

    private static void AppendImmutableSortedSetPostLoop(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName)
    {
        var postLoopVariableName = context.NextTemporary();
        stringBuilder.AppendEmptyLine();
        string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
        stringBuilder.AppendLine($"global::System.Collections.Immutable.ImmutableSortedSet<{elementTypeDisplayString}> {postLoopVariableName} = System.Collections.Immutable.ImmutableSortedSet.ToImmutableSortedSet<{elementTypeDisplayString}>({targetVariableName});");
        targetVariableName = postLoopVariableName;
    }

    private static void AppendImmutableArrayPostLoop(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName)
    {
        var postLoopVariableName = context.NextTemporary();
        stringBuilder.AppendEmptyLine();
        string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
        stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {postLoopVariableName} = System.Collections.Immutable.ImmutableArray.ToImmutableArray<{elementTypeDisplayString}>({targetVariableName});");
        targetVariableName = postLoopVariableName;
    }

    private static void AppendImmutableListPostLoop(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName)
    {
        var postLoopVariableName = context.NextTemporary();
        stringBuilder.AppendEmptyLine();
        string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
        stringBuilder.AppendLine($"global::System.Collections.Immutable.ImmutableList<{elementTypeDisplayString}> {postLoopVariableName} = System.Collections.Immutable.ImmutableList.ToImmutableList<{elementTypeDisplayString}>({targetVariableName});");
        targetVariableName = postLoopVariableName;
    }

    private static void AppendImmutableQueuePostLoop(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName)
    {
        var postLoopVariableName = context.NextTemporary();
        stringBuilder.AppendEmptyLine();
        string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
        stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {postLoopVariableName} = System.Collections.Immutable.ImmutableQueue.Create<{elementTypeDisplayString}>({targetVariableName});");
        targetVariableName = postLoopVariableName;
    }

    private static void AppendImmutableStackPostLoop(
        PrettyCode.StringBuilder stringBuilder,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName)
    {
        var postLoopVariableName = context.NextTemporary();
        stringBuilder.AppendEmptyLine();
        string elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
        stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {postLoopVariableName} = System.Collections.Immutable.ImmutableStack.Create<{elementTypeDisplayString}>({targetVariableName});");
        targetVariableName = postLoopVariableName;
    }

    private static void AppendTargetVariable(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        IMethodSymbol? methodSymbol,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        BooleanSetting fastCollections,
        BooleanSetting containerCapacityConstructors,
        BooleanSetting preventEnumerableCount,
        EnumerableConcreteTypeSetting enumerableConcreteType,
        out string targetVariableName,
        out InsertionMethod insertionMethod,
        out string? counterVariableName,
        out InterfaceMethodAccessMode interfaceMethodAccessMode,
        out string interfaceToAccessFrom,
        out string? variableToAccessFrom,
        out bool usedGrowableBuffer)
    {
        var state = new TargetVariableAppendState(context.NextTemporary());
        var dispatchContext = new AppendTargetVariableContext(
            stringBuilder,
            source,
            context,
            methodSymbol,
            targetTypeSymbol,
            sourceTypeSymbol,
            fastCollections,
            containerCapacityConstructors,
            preventEnumerableCount,
            enumerableConcreteType,
            state);

        foreach (var entry in TargetVariableDispatchEntries)
        {
            if (!entry.Matches(dispatchContext))
            {
                continue;
            }

            entry.Append(dispatchContext);
            targetVariableName = state.TargetVariableName;
            insertionMethod = state.InsertionMethod;
            counterVariableName = state.CounterVariableName;
            interfaceMethodAccessMode = state.InterfaceMethodAccessMode;
            interfaceToAccessFrom = state.InterfaceToAccessFrom;
            variableToAccessFrom = state.VariableToAccessFrom;
            usedGrowableBuffer = state.UsedGrowableBuffer;
            return;
        }

        throw new MappaGeneratorException($"Unsupported target type {targetTypeSymbol.ToDisplayString()} during generation of collection to collection mapping.");
    }

    private static void AppendArrayEnumerableInterfaceTarget(AppendTargetVariableContext context)
    {
        var targetVariableNameForArray = context.State.TargetVariableName;
        AppendArrayTargetVariable(
            context.StringBuilder,
            context.Source,
            context.BuilderContext,
            context.TargetTypeSymbol,
            context.SourceTypeSymbol,
            context.FastCollections,
            context.PreventEnumerableCount,
            context.EnumerableConcreteType,
            ref targetVariableNameForArray,
            out var arrayInsertionMethod,
            out var arrayCounterVariableName,
            out var arrayVariableToAccessFrom,
            out var arrayUsedGrowableBuffer);
        context.State.TargetVariableName = targetVariableNameForArray;
        context.State.InsertionMethod = arrayInsertionMethod;
        context.State.CounterVariableName = arrayCounterVariableName;
        context.State.VariableToAccessFrom = arrayVariableToAccessFrom;
        context.State.UsedGrowableBuffer = arrayUsedGrowableBuffer;
    }

    private static bool IsFastCollectionArrayTarget(
        BooleanSetting fastCollections,
        ITypeSymbol sourceTypeSymbol,
        ITypeSymbol targetTypeSymbol,
        Compilation compilation)
    {
        var isFastCollectionOnSource = fastCollections is BooleanSetting.Enable
            && (sourceTypeSymbol.IsList(compilation) || sourceTypeSymbol.IsArray());
        return isFastCollectionOnSource && targetTypeSymbol.IsArray();
    }

    private static bool MatchesAnyPredicate(ITypeSymbol typeSymbol, Compilation compilation, Func<ITypeSymbol, Compilation, bool>[] predicates)
        => predicates.Any(predicate => predicate(typeSymbol, compilation));

    private static bool IsArraySpanMemoryOrImmutableQueueStackTarget(ITypeSymbol targetTypeSymbol, Compilation compilation)
        => MatchesAnyPredicate(targetTypeSymbol, compilation, ArraySpanMemoryOrImmutableQueueStackPredicates);

    private static bool IsHashSetLikeTarget(ITypeSymbol targetTypeSymbol, Compilation compilation)
        => targetTypeSymbol.IsISet(compilation)
           || targetTypeSymbol.IsIReadOnlySet(compilation)
           || targetTypeSymbol.IsHashSet(compilation)
           || targetTypeSymbol.IsReadOnlySet(compilation);

    private static bool IsBlockingCollectionOrConcurrentBagTarget(ITypeSymbol targetTypeSymbol, Compilation compilation)
        => targetTypeSymbol.IsOrDerivedFromBlockingCollection(compilation)
           || targetTypeSymbol.IsOrDerivedFromConcurrentBag(compilation);

    private static bool IsListLikeEnumerableTarget(ITypeSymbol targetTypeSymbol, Compilation compilation)
        => MatchesAnyPredicate(targetTypeSymbol, compilation, ListLikeEnumerableTargetPredicates);

    private static void AppendFastCollectionArrayTarget(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        TargetVariableAppendState state)
    {
        state.InsertionMethod = InsertionMethod.Indexer;
        var capacity = GetLengthExpression(source, sourceTypeSymbol, context.Compilation);
        state.VariableToAccessFrom = context.NextTemporary();
        stringBuilder.AppendLine($"{targetTypeSymbol.GetElementType().ToDisplayString()}[] {state.TargetVariableName} = new {targetTypeSymbol.GetElementType().ToDisplayString()}[{capacity}];");
        stringBuilder.AppendLine($"global::System.Span<{targetTypeSymbol.GetElementType().ToDisplayString()}> {state.VariableToAccessFrom} = {state.TargetVariableName}.AsSpan();");
    }

    private static void AppendArraySpanMemoryOrImmutableQueueStackTarget(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        BooleanSetting preventEnumerableCount,
        EnumerableConcreteTypeSetting enumerableConcreteType,
        TargetVariableAppendState state)
    {
        if (ShouldUseGrowableBuffer(
                preventEnumerableCount,
                source,
                sourceTypeSymbol,
                targetTypeSymbol,
                enumerableConcreteType,
                context.Compilation))
        {
            var targetVariableNameForGrowable = state.TargetVariableName;
            AppendGrowableListTargetVariable(stringBuilder, targetTypeSymbol, ref targetVariableNameForGrowable, out var growableInsertionMethod);
            state.TargetVariableName = targetVariableNameForGrowable;
            state.InsertionMethod = growableInsertionMethod;
            state.UsedGrowableBuffer = true;
            return;
        }

        // Array need indexers.
        state.InsertionMethod = InsertionMethod.Indexer;

        // Capacity is always mandatory for arrays.
        // In some scenarios it might mean we invoke the Enumerable.Count() extension method which
        // might results in enumerations being executed twice.
        var capacity = GetLengthExpression(source, sourceTypeSymbol, context.Compilation);
        stringBuilder.AppendLine($"{targetTypeSymbol.GetElementType().ToDisplayString()}[] {state.TargetVariableName} = new {targetTypeSymbol.GetElementType().ToDisplayString()}[{capacity}];");

        // If source does not have an indexer we need to create a new counter variable
        // this for instance is used when mapping generic IEnumerable<TSource> to TTarget[].
        if (!HasIndexer(context, sourceTypeSymbol))
        {
            state.CounterVariableName = context.NextTemporary();
            stringBuilder.AppendLine($"int {state.CounterVariableName} = 0;");
        }
    }

    private static void AppendHashSetLikeTarget(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        TargetVariableAppendState state)
    {
        // We are going to always use an HashSet so Add method is best here.
        state.InsertionMethod = InsertionMethod.Add;
        TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, context.Compilation, out var capacity);
        stringBuilder.AppendLine($"global::System.Collections.Generic.HashSet<{targetTypeSymbol.GetElementType().ToDisplayString()}> {state.TargetVariableName} = new global::System.Collections.Generic.HashSet<{targetTypeSymbol.GetElementType().ToDisplayString()}>({capacity});");
    }

    private static void AppendStackTarget(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        IMethodSymbol? methodSymbol,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        BooleanSetting containerCapacityConstructors,
        TargetVariableAppendState state)
    {
        state.InsertionMethod = InsertionMethod.Push;
        var capacity = ResolveContainerCapacity(
            source,
            sourceTypeSymbol,
            targetTypeSymbol,
            context.Compilation,
            methodSymbol,
            containerCapacityConstructors,
            usePropertyLengthOnly: targetTypeSymbol.IsStack(context.Compilation),
            allowOptionalIntegerConstructor: true);
        stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {state.TargetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}({capacity});");
    }

    private static void AppendConcurrentStackTarget(
        PrettyCode.StringBuilder stringBuilder,
        ITypeSymbol targetTypeSymbol,
        TargetVariableAppendState state)
    {
        // NOTE: ConcurrentStack{T} does not have a constructor accepting a capacity.
        state.InsertionMethod = InsertionMethod.Push;
        stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {state.TargetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}();");
    }

    private static void AppendQueueTarget(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        IMethodSymbol? methodSymbol,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        BooleanSetting containerCapacityConstructors,
        TargetVariableAppendState state)
    {
        state.InsertionMethod = InsertionMethod.Enqueue;
        var capacity = ResolveContainerCapacity(
            source,
            sourceTypeSymbol,
            targetTypeSymbol,
            context.Compilation,
            methodSymbol,
            containerCapacityConstructors,
            usePropertyLengthOnly: targetTypeSymbol.IsQueue(context.Compilation),
            allowOptionalIntegerConstructor: true);
        stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {state.TargetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}({capacity});");
    }

    private static void AppendConcurrentQueueTarget(
        PrettyCode.StringBuilder stringBuilder,
        ITypeSymbol targetTypeSymbol,
        TargetVariableAppendState state)
    {
        // NOTE: ConcurrentQueue{T} does not have a constructor accepting a capacity.
        state.InsertionMethod = InsertionMethod.Enqueue;
        stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {state.TargetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}();");
    }

    private static void AppendBlockingCollectionOrConcurrentBagTarget(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        IMethodSymbol? methodSymbol,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        BooleanSetting containerCapacityConstructors,
        TargetVariableAppendState state)
    {
        state.InsertionMethod = InsertionMethod.Add;

        // NOTE: ConcurrentBag does not have a constructor accepting a capacity.
        var capacity = ResolveContainerCapacity(
            source,
            sourceTypeSymbol,
            targetTypeSymbol,
            context.Compilation,
            methodSymbol,
            containerCapacityConstructors,
            usePropertyLengthOnly: targetTypeSymbol.IsBlockingCollection(context.Compilation),
            allowOptionalIntegerConstructor: targetTypeSymbol.IsDerivedFromBlockingCollection(context.Compilation));
        stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {state.TargetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}({capacity});");
    }

    private static void AppendListLikeEnumerableTarget(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        TargetVariableAppendState state)
    {
        // We are going to always use a list, so Add method is best here.
        state.InsertionMethod = InsertionMethod.Add;

        // Note: even if we set capacity, the list would be empty so we cannot invoke an indexer, but only Add.
        // (having an initial capacity is anyway an improvement on the performances).
        TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, context.Compilation, out var capacity);
        stringBuilder.AppendLine($"global::System.Collections.Generic.List<{targetTypeSymbol.GetElementType().ToDisplayString()}> {state.TargetVariableName} = new global::System.Collections.Generic.List<{targetTypeSymbol.GetElementType().ToDisplayString()}>({capacity});");
    }

    private static void AppendProducerConsumerCollectionTarget(
        PrettyCode.StringBuilder stringBuilder,
        ITypeSymbol targetTypeSymbol,
        TargetVariableAppendState state)
    {
        // We are going to always use a concurrent bag, so Add method is best here.
        state.InsertionMethod = InsertionMethod.Add;
        stringBuilder.AppendLine($"global::System.Collections.Concurrent.ConcurrentBag<{targetTypeSymbol.GetElementType().ToDisplayString()}> {state.TargetVariableName} = new global::System.Collections.Concurrent.ConcurrentBag<{targetTypeSymbol.GetElementType().ToDisplayString()}>();");
    }

    private static void AppendImplementISetTarget(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        IMethodSymbol? methodSymbol,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        BooleanSetting containerCapacityConstructors,
        TargetVariableAppendState state)
    {
        state.InsertionMethod = InsertionMethod.Add;
        var elementType = targetTypeSymbol.GetElementType();

        // Use ICollection because ISet derive the Add from ICollection
        state.InterfaceToAccessFrom = $"System.Collections.Generic.ICollection<{TypeSymbolExtensions.NormalizeType(elementType.ToDisplayString())}>";
        state.InterfaceMethodAccessMode = targetTypeSymbol.GetInterfaceMethodAccessMode(
            "Add",
            "System.Collections.Generic.ICollection",
            TypeSymbolExtensions.NormalizeType(elementType.ToDisplayString()),
            returnType => returnType.IsVoid(),
            [elementType]);

        var capacity = ResolveContainerCapacity(
            source,
            sourceTypeSymbol,
            targetTypeSymbol,
            context.Compilation,
            methodSymbol,
            containerCapacityConstructors,
            usePropertyLengthOnly: false,
            allowOptionalIntegerConstructor: targetTypeSymbol.TypeKind != TypeKind.Interface);
        stringBuilder.AppendLine($"global::{targetTypeSymbol} {state.TargetVariableName} = new global::{targetTypeSymbol}({capacity});");
    }

    private static void AppendImplementICollectionTarget(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        IMethodSymbol? methodSymbol,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        BooleanSetting containerCapacityConstructors,
        TargetVariableAppendState state)
    {
        // Here we handle the scenario of a concrete type implementing ICollection<T>.
        // We are sure that is concrete because ICollection<T> is addressed in a different branch
        // and we re also sure it has a constructor with 0 or 1 arguments that can be used.
        // And if it is one argument is must be an integer (and the ContainerCapacityConstructors
        // must be enabled too).
        state.InsertionMethod = InsertionMethod.Add;

        var elementType = targetTypeSymbol.GetElementType();
        state.InterfaceToAccessFrom = $"System.Collections.Generic.ICollection<{TypeSymbolExtensions.NormalizeType(elementType.ToDisplayString())}>";
        state.InterfaceMethodAccessMode = targetTypeSymbol.GetInterfaceMethodAccessMode(
            "Add",
            "System.Collections.Generic.ICollection",
            TypeSymbolExtensions.NormalizeType(elementType.ToDisplayString()),
            returnType => returnType.IsVoid(),
            [elementType]);

        var capacity = ResolveContainerCapacity(
            source,
            sourceTypeSymbol,
            targetTypeSymbol,
            context.Compilation,
            methodSymbol,
            containerCapacityConstructors,
            usePropertyLengthOnly: false,
            allowOptionalIntegerConstructor: targetTypeSymbol.TypeKind != TypeKind.Interface);
        stringBuilder.AppendLine($"global::{targetTypeSymbol.ToDisplayString()} {state.TargetVariableName} = new global::{targetTypeSymbol.ToDisplayString()}({capacity});");
    }

    /// <summary>
    /// Resolves capacity for collection constructors that optionally accept an integer capacity.
    /// </summary>
    /// <param name="source">The source expression.</param>
    /// <param name="sourceTypeSymbol">The source type.</param>
    /// <param name="targetTypeSymbol">The target type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="methodSymbol">The method symbol used for accessibility checks.</param>
    /// <param name="containerCapacityConstructors">Whether capacity constructors are enabled.</param>
    /// <param name="usePropertyLengthOnly">When <see langword="true"/>, only a property-based length is used.</param>
    /// <param name="allowOptionalIntegerConstructor">When <see langword="true"/>, optional integer constructors may be used.</param>
    /// <returns>The capacity expression, or an empty string when capacity is omitted.</returns>
    private static string ResolveContainerCapacity(
        string source,
        ITypeSymbol sourceTypeSymbol,
        ITypeSymbol targetTypeSymbol,
        Compilation compilation,
        IMethodSymbol? methodSymbol,
        BooleanSetting containerCapacityConstructors,
        bool usePropertyLengthOnly,
        bool allowOptionalIntegerConstructor)
    {
        if (usePropertyLengthOnly)
        {
            TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, compilation, out var capacity);
            return capacity;
        }

        if (!allowOptionalIntegerConstructor
            || containerCapacityConstructors is not BooleanSetting.Enable
            || !targetTypeSymbol.HasSymbolAccessibleSingleIntegerParametersConstructor(compilation, methodSymbol))
        {
            return string.Empty;
        }

        if (!targetTypeSymbol.HasSymbolAccessibleZeroParametersConstructor(compilation, methodSymbol))
        {
            // Since only the constructor with one integer parameter exists the capacity MUST be used.
            return GetLengthExpression(source, sourceTypeSymbol, compilation);
        }

        TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, compilation, out var capacityFromProperty);
        return capacityFromProperty;
    }

    private static bool ShouldUseArrayForEnumerableInterfaceTarget(
        ITypeSymbol targetTypeSymbol,
        EnumerableConcreteTypeSetting enumerableConcreteType)
    {
        if (enumerableConcreteType is not EnumerableConcreteTypeSetting.Array)
        {
            return false;
        }

        if (targetTypeSymbol.TypeKind is not TypeKind.Interface)
        {
            return false;
        }

        return targetTypeSymbol.IsIEnumerable()
               || targetTypeSymbol.IsIList()
               || targetTypeSymbol.IsIReadOnlyList()
               || targetTypeSymbol.IsICollection()
               || targetTypeSymbol.IsIReadOnlyCollection();
    }

    private static void AppendArrayTargetVariable(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        ITypeSymbol targetTypeSymbol,
        ITypeSymbol sourceTypeSymbol,
        BooleanSetting fastCollections,
        BooleanSetting preventEnumerableCount,
        EnumerableConcreteTypeSetting enumerableConcreteType,
        ref string targetVariableName,
        out InsertionMethod insertionMethod,
        out string? counterVariableName,
        out string? variableToAccessFrom,
        out bool usedGrowableBuffer)
    {
        counterVariableName = null;
        variableToAccessFrom = null;
        usedGrowableBuffer = false;

        if (ShouldUseGrowableBuffer(
                preventEnumerableCount,
                source,
                sourceTypeSymbol,
                targetTypeSymbol,
                enumerableConcreteType,
                context.Compilation))
        {
            AppendGrowableListTargetVariable(stringBuilder, targetTypeSymbol, ref targetVariableName, out insertionMethod);
            usedGrowableBuffer = true;
            return;
        }

        insertionMethod = InsertionMethod.Indexer;

        var elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
        var isFastCollectionOnSource = fastCollections is BooleanSetting.Enable
            && (sourceTypeSymbol.IsList(context.Compilation) || sourceTypeSymbol.IsArray());

        if (isFastCollectionOnSource)
        {
            var capacity = GetLengthExpression(source, sourceTypeSymbol, context.Compilation);
            variableToAccessFrom = context.NextTemporary();
            stringBuilder.AppendLine($"{elementTypeDisplayString}[] {targetVariableName} = new {elementTypeDisplayString}[{capacity}];");
            stringBuilder.AppendLine($"global::System.Span<{elementTypeDisplayString}> {variableToAccessFrom} = {targetVariableName}.AsSpan();");
        }
        else
        {
            var capacity = GetLengthExpression(source, sourceTypeSymbol, context.Compilation);
            stringBuilder.AppendLine($"{elementTypeDisplayString}[] {targetVariableName} = new {elementTypeDisplayString}[{capacity}];");

            if (!HasIndexer(context, sourceTypeSymbol))
            {
                counterVariableName = context.NextTemporary();
                stringBuilder.AppendLine($"int {counterVariableName} = 0;");
            }
        }
    }

    private static void AppendGrowableListTargetVariable(
        PrettyCode.StringBuilder stringBuilder,
        ITypeSymbol targetTypeSymbol,
        ref string targetVariableName,
        out InsertionMethod insertionMethod)
    {
        insertionMethod = InsertionMethod.Add;
        var elementTypeDisplayString = targetTypeSymbol.GetElementType().ToDisplayString();
        stringBuilder.AppendLine($"global::System.Collections.Generic.List<{elementTypeDisplayString}> {targetVariableName} = new global::System.Collections.Generic.List<{elementTypeDisplayString}>();");
    }

    private static bool ShouldUseGrowableBuffer(
        BooleanSetting preventEnumerableCount,
        string source,
        ITypeSymbol sourceTypeSymbol,
        ITypeSymbol targetTypeSymbol,
        EnumerableConcreteTypeSetting enumerableConcreteType,
        Compilation compilation)
    {
        if (preventEnumerableCount is not BooleanSetting.Enable)
        {
            return false;
        }

        if (TryGetLengthExpressionFromProperty(source, sourceTypeSymbol, compilation, out _))
        {
            return false;
        }

        if (TargetRequiresFixedSizeBuffer(targetTypeSymbol, compilation))
        {
            return true;
        }

        return ShouldUseArrayForEnumerableInterfaceTarget(targetTypeSymbol, enumerableConcreteType);
    }

    private static bool TargetRequiresFixedSizeBuffer(ITypeSymbol targetTypeSymbol, Compilation compilation)
        => targetTypeSymbol.IsArray()
           || targetTypeSymbol.IsSpan(compilation)
           || targetTypeSymbol.IsReadOnlySpan(compilation)
           || targetTypeSymbol.IsMemory(compilation)
           || targetTypeSymbol.IsReadOnlyMemory(compilation);

    private static bool HasIndexer(MappaBuilderContext context, ITypeSymbol sourceTypeSymbol)
    {
        return sourceTypeSymbol.IsArray()
               || sourceTypeSymbol.IsSpan(context.Compilation)
               || sourceTypeSymbol.IsReadOnlySpan(context.Compilation)
               || sourceTypeSymbol.IsMemory(context.Compilation) // Indexed by accessing the Span property
               || sourceTypeSymbol.IsReadOnlyMemory(context.Compilation) // Indexed by accessing the Span property
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

    private static IDisposable AppendLoopBlock(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        ITypeSymbol sourceTypeSymbol,
        BooleanSetting fastCollections,
        out string loopVariableName,
        out string? countingVariableName)
    {
        if (!HasIndexer(context, sourceTypeSymbol))
        {
            return AppendForeachLoopBlock(stringBuilder, source, context, sourceTypeSymbol, out loopVariableName, out countingVariableName);
        }

        var spanTemporary = TryAppendSpanTemporaryForIndexerLoop(
            stringBuilder,
            source,
            context,
            sourceTypeSymbol,
            fastCollections,
            out var lengthExpression);

        countingVariableName = context.NextTemporary();
        loopVariableName = context.NextTemporary();

        stringBuilder.AppendLine($"for (int {countingVariableName} = 0; {countingVariableName} < {lengthExpression ?? GetLengthExpression(spanTemporary ?? source, sourceTypeSymbol, context.Compilation)}; ++{countingVariableName})");
        var block = stringBuilder.CurlyBracesBlock();
        stringBuilder.AppendLine($"{sourceTypeSymbol.GetElementType().ToDisplayString()} {loopVariableName} = {spanTemporary ?? source}[{countingVariableName}];");
        return block;
    }

    private static IDisposable AppendForeachLoopBlock(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        ITypeSymbol sourceTypeSymbol,
        out string loopVariableName,
        out string? countingVariableName)
    {
        countingVariableName = null;
        loopVariableName = context.NextTemporary();
        stringBuilder.AppendLine($"foreach ({sourceTypeSymbol.GetElementType().ToDisplayString()} {loopVariableName} in {source})");
        return stringBuilder.CurlyBracesBlock();
    }

    private static string? TryAppendSpanTemporaryForIndexerLoop(
        PrettyCode.StringBuilder stringBuilder,
        string source,
        MappaBuilderContext context,
        ITypeSymbol sourceTypeSymbol,
        BooleanSetting fastCollections,
        out string? lengthExpression)
    {
        lengthExpression = null;
        var compilation = context.Compilation;
        var elementTypeDisplay = sourceTypeSymbol.GetElementType().ToDisplayString();

        if (sourceTypeSymbol.IsMemory(compilation))
        {
            var spanTemporary = context.NextTemporary();
            stringBuilder.AppendLine($"global::System.Span<{elementTypeDisplay}> {spanTemporary} = {source}.Span;");
            return spanTemporary;
        }

        if (sourceTypeSymbol.IsReadOnlyMemory(compilation))
        {
            var spanTemporary = context.NextTemporary();
            stringBuilder.AppendLine($"global::System.ReadOnlySpan<{elementTypeDisplay}> {spanTemporary} = {source}.Span;");
            return spanTemporary;
        }

        if (fastCollections is BooleanSetting.Enable && sourceTypeSymbol.IsArray())
        {
            var spanTemporary = context.NextTemporary();
            stringBuilder.AppendLine($"global::System.Span<{elementTypeDisplay}> {spanTemporary} = {source}.AsSpan();");
            return spanTemporary;
        }

        if (fastCollections is BooleanSetting.Enable && sourceTypeSymbol.IsList(compilation))
        {
            var spanTemporary = context.NextTemporary();
            lengthExpression = $"{spanTemporary}.Length";
            stringBuilder.AppendLine($"global::System.Span<{elementTypeDisplay}> {spanTemporary} = global::System.Runtime.InteropServices.CollectionsMarshal.AsSpan<{elementTypeDisplay}>({source});");
            return spanTemporary;
        }

        return null;
    }

    /// <summary>
    /// Inputs shared while dispatching target variable append branches.
    /// </summary>
    private sealed class AppendTargetVariableContext
    {
        internal AppendTargetVariableContext(
            PrettyCode.StringBuilder stringBuilder,
            string source,
            MappaBuilderContext builderContext,
            IMethodSymbol? methodSymbol,
            ITypeSymbol targetTypeSymbol,
            ITypeSymbol sourceTypeSymbol,
            BooleanSetting fastCollections,
            BooleanSetting containerCapacityConstructors,
            BooleanSetting preventEnumerableCount,
            EnumerableConcreteTypeSetting enumerableConcreteType,
            TargetVariableAppendState state)
        {
            this.StringBuilder = stringBuilder;
            this.Source = source;
            this.BuilderContext = builderContext;
            this.MethodSymbol = methodSymbol;
            this.TargetTypeSymbol = targetTypeSymbol;
            this.SourceTypeSymbol = sourceTypeSymbol;
            this.FastCollections = fastCollections;
            this.ContainerCapacityConstructors = containerCapacityConstructors;
            this.PreventEnumerableCount = preventEnumerableCount;
            this.EnumerableConcreteType = enumerableConcreteType;
            this.State = state;
        }

        internal PrettyCode.StringBuilder StringBuilder { get; }

        internal string Source { get; }

        internal MappaBuilderContext BuilderContext { get; }

        internal IMethodSymbol? MethodSymbol { get; }

        internal ITypeSymbol TargetTypeSymbol { get; }

        internal ITypeSymbol SourceTypeSymbol { get; }

        internal BooleanSetting FastCollections { get; }

        internal BooleanSetting ContainerCapacityConstructors { get; }

        internal BooleanSetting PreventEnumerableCount { get; }

        internal EnumerableConcreteTypeSetting EnumerableConcreteType { get; }

        internal TargetVariableAppendState State { get; }
    }

    /// <summary>
    /// Mutable state collected while appending the target collection variable.
    /// </summary>
    private sealed class TargetVariableAppendState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TargetVariableAppendState"/> class.
        /// </summary>
        /// <param name="targetVariableName">The target variable name.</param>
        public TargetVariableAppendState(string targetVariableName)
        {
            this.TargetVariableName = targetVariableName;
            this.InterfaceToAccessFrom = string.Empty;
        }

        /// <summary>
        /// Gets or sets the target variable name.
        /// </summary>
        public string TargetVariableName { get; set; }

        /// <summary>
        /// Gets or sets the insertion method.
        /// </summary>
        public InsertionMethod InsertionMethod { get; set; }

        /// <summary>
        /// Gets or sets the counter variable name.
        /// </summary>
        public string? CounterVariableName { get; set; }

        /// <summary>
        /// Gets or sets the interface method access mode.
        /// </summary>
        public InterfaceMethodAccessMode InterfaceMethodAccessMode { get; set; }

        /// <summary>
        /// Gets or sets the interface used for explicit method access.
        /// </summary>
        public string InterfaceToAccessFrom { get; set; }

        /// <summary>
        /// Gets or sets the variable used for indexer access.
        /// </summary>
        public string? VariableToAccessFrom { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a growable buffer was used.
        /// </summary>
        public bool UsedGrowableBuffer { get; set; }
    }

    private sealed class PostLoopDispatchEntry
    {
        internal PostLoopDispatchEntry(
            Func<ITypeSymbol, Compilation, bool> matches,
            PostLoopAppendAction append,
            bool stopAfterMatch)
        {
            this.Matches = matches;
            this.Append = append;
            this.StopAfterMatch = stopAfterMatch;
        }

        internal Func<ITypeSymbol, Compilation, bool> Matches { get; }

        internal PostLoopAppendAction Append { get; }

        internal bool StopAfterMatch { get; }
    }

    private sealed class TargetVariableDispatchEntry
    {
        internal TargetVariableDispatchEntry(
            Func<AppendTargetVariableContext, bool> matches,
            TargetVariableBranchAppender append)
        {
            this.Matches = matches;
            this.Append = append;
        }

        internal Func<AppendTargetVariableContext, bool> Matches { get; }

        internal TargetVariableBranchAppender Append { get; }
    }
}