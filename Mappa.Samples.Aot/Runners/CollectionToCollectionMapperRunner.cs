// <copyright file="CollectionToCollectionMapperRunner.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples;
using Mappa.Samples.Models;

namespace Mappa.Samples.Aot.Runners;

/// <summary>
/// AOT runner for <see cref="CollectionToCollectionMapper"/>.
/// </summary>
internal static class CollectionToCollectionMapperRunner
{
    /// <summary>
    /// Runs all map methods on <see cref="CollectionToCollectionMapper"/>.
    /// </summary>
    /// <param name="report">The AOT report.</param>
    public static void Run(AotReport report)
    {
        report.BeginMapper(nameof(CollectionToCollectionMapper));
        var mapper = new CollectionToCollectionMapper();
        var array = AotSampleData.CountingValuesOneThreeArray;
        var blockingCollection = AotSampleData.CountingValuesOneThreeBlockingCollection;
        var concurrentBag = AotSampleData.CountingValuesOneThreeConcurrentBag;
        var concurrentQueue = AotSampleData.CountingValuesOneThreeConcurrentQueue;
        var concurrentStack = AotSampleData.CountingValuesOneThreeConcurrentStack;
        var customICollection = AotSampleData.CustomICollectionOneThree;
        var customICollectionOf = AotSampleData.CustomICollectionOfCountingValuesOneThree;
        var customIEnumerable = AotSampleData.CustomIEnumerableOneThree;
        var customIEnumerableOf = AotSampleData.CustomIEnumerableOfCountingValuesOneThree;
        var customIList = AotSampleData.CustomIListOneThree;
        var customIListOf = AotSampleData.CustomIListOfCountingValuesOneThree;
        var customIReadOnlyCollection = AotSampleData.CustomIReadOnlyCollectionOneThree;
        var customIReadOnlyCollectionOf = AotSampleData.CustomIReadOnlyCollectionOfCountingValuesOneThree;
        var enumerable = AotSampleData.CountingValuesOneThreeEnumerable;
        var iCollection = AotSampleData.CountingValuesOneThreeICollection;
        var iList = AotSampleData.CountingValuesOneThreeIList;
        var iReadOnlyCollection = AotSampleData.CountingValuesOneThreeIReadOnlyCollection;
        var list = AotSampleData.CountingValuesOneThreeList;
        var memory = AotSampleData.CountingValuesOneThreeMemory;
        var producerConsumer = AotSampleData.CountingValuesOneThreeIProducerConsumerCollection;
        var queue = AotSampleData.CountingValuesOneThreeQueue;
        var readOnlyMemory = AotSampleData.CountingValuesOneThreeReadOnlyMemory;
        var stack = AotSampleData.CountingValuesOneThreeStack;

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapIEnumerableToIEnumerable),
            "IEnumerable<CountingValues>",
            "IEnumerable<int>",
            enumerable,
            mapper.MapIEnumerableToIEnumerable(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromNonGenericTypeImplementingIEnumerableToIEnumerable),
            "CustomCollectionImplementingIEnumerableOfCountingValues",
            "IEnumerable<int>",
            customIEnumerableOf,
            mapper.MapFromNonGenericTypeImplementingIEnumerableToIEnumerable(customIEnumerableOf));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromGenericTypeImplementingIEnumerableToIEnumerable),
            "CustomCollectionImplementingIEnumerable<CountingValues>",
            "IEnumerable<int>",
            customIEnumerable,
            mapper.MapFromGenericTypeImplementingIEnumerableToIEnumerable(customIEnumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToIEnumerable),
            "CountingValues[]",
            "IEnumerable<int>",
            array,
            mapper.MapFromArrayToIEnumerable(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromSpanToIEnumerable),
            "Span<CountingValues>",
            "IEnumerable<int>",
            array,
            mapper.MapFromSpanToIEnumerable(array.AsSpan()));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromReadOnlySpanToIEnumerable),
            "ReadOnlySpan<CountingValues>",
            "IEnumerable<int>",
            array,
            mapper.MapFromReadOnlySpanToIEnumerable(array.AsSpan()));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromMemoryToIEnumerable),
            "Memory<CountingValues>",
            "IEnumerable<int>",
            memory,
            mapper.MapFromMemoryToIEnumerable(memory));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromReadOnlyMemoryToIEnumerable),
            "ReadOnlyMemory<CountingValues>",
            "IEnumerable<int>",
            readOnlyMemory,
            mapper.MapFromReadOnlyMemoryToIEnumerable(readOnlyMemory));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIListToIEnumerable),
            "IList<CountingValues>",
            "IEnumerable<int>",
            iList,
            mapper.MapFromIListToIEnumerable(iList));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromNonGenericTypeImplementingIListToIEnumerable),
            "CustomCollectionImplementingIListOfCountingValues",
            "IEnumerable<int>",
            customIListOf,
            mapper.MapFromNonGenericTypeImplementingIListToIEnumerable(customIListOf));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromGenericTypeImplementingIListToIEnumerable),
            "CustomCollectionImplementingIList<CountingValues>",
            "IEnumerable<int>",
            customIList,
            mapper.MapFromGenericTypeImplementingIListToIEnumerable(customIList));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromICollectionToIEnumerable),
            "ICollection<CountingValues>",
            "IEnumerable<int>",
            iCollection,
            mapper.MapFromICollectionToIEnumerable(iCollection));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromNonGenericTypeImplementingICollectionToIEnumerable),
            "CustomCollectionImplementingICollectionOfCountingValues",
            "IEnumerable<int>",
            customICollectionOf,
            mapper.MapFromNonGenericTypeImplementingICollectionToIEnumerable(customICollectionOf));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromGenericTypeImplementingICollectionToIEnumerable),
            "CustomCollectionImplementingICollection<CountingValues>",
            "IEnumerable<int>",
            customICollection,
            mapper.MapFromGenericTypeImplementingICollectionToIEnumerable(customICollection));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIReadOnlyCollectionToIEnumerable),
            "IReadOnlyCollection<CountingValues>",
            "IEnumerable<int>",
            iReadOnlyCollection,
            mapper.MapFromIReadOnlyCollectionToIEnumerable(iReadOnlyCollection));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromNonGenericTypeImplementingIReadOnlyCollectionToIEnumerable),
            "CustomCollectionImplementingIReadOnlyCollectionOfCountingValues",
            "IEnumerable<int>",
            customIReadOnlyCollectionOf,
            mapper.MapFromNonGenericTypeImplementingIReadOnlyCollectionToIEnumerable(customIReadOnlyCollectionOf));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromGenericTypeImplementingIReadOnlyCollectionToIEnumerable),
            "CustomCollectionImplementingIReadOnlyCollection<CountingValues>",
            "IEnumerable<int>",
            customIReadOnlyCollection,
            mapper.MapFromGenericTypeImplementingIReadOnlyCollectionToIEnumerable(customIReadOnlyCollection));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToList),
            "IEnumerable<CountingValues>",
            "List<int>",
            enumerable,
            mapper.MapFromIEnumerableToList(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToList),
            "CountingValues[]",
            "List<int>",
            array,
            mapper.MapFromArrayToList(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToIList),
            "IEnumerable<CountingValues>",
            "IList<int>",
            enumerable,
            mapper.MapFromIEnumerableToIList(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToIList),
            "CountingValues[]",
            "IList<int>",
            array,
            mapper.MapFromArrayToIList(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToICollection),
            "IEnumerable<CountingValues>",
            "ICollection<int>",
            enumerable,
            mapper.MapFromIEnumerableToICollection(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToICollection),
            "CountingValues[]",
            "ICollection<int>",
            array,
            mapper.MapFromArrayToICollection(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToIReadOnlyCollection),
            "IEnumerable<CountingValues>",
            "IReadOnlyCollection<int>",
            enumerable,
            mapper.MapFromIEnumerableToIReadOnlyCollection(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToIReadOnlyCollection),
            "CountingValues[]",
            "IReadOnlyCollection<int>",
            array,
            mapper.MapFromArrayToIReadOnlyCollection(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromStackToIEnumerable),
            "Stack<CountingValues>",
            "IEnumerable<int>",
            stack,
            mapper.MapFromStackToIEnumerable(stack));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromQueueToIEnumerable),
            "Queue<CountingValues>",
            "IEnumerable<int>",
            queue,
            mapper.MapFromQueueToIEnumerable(queue));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToArray),
            "CountingValues[]",
            "int[]",
            array,
            mapper.MapFromArrayToArray(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToArray),
            "IEnumerable<CountingValues>",
            "int[]",
            enumerable,
            mapper.MapFromIEnumerableToArray(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromNonGenericTypeImplementingIEnumerableToArray),
            "CustomCollectionImplementingIEnumerableOfCountingValues",
            "int[]",
            customIEnumerableOf,
            mapper.MapFromNonGenericTypeImplementingIEnumerableToArray(customIEnumerableOf));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromGenericTypeImplementingIEnumerableToArray),
            "CustomCollectionImplementingIEnumerable<CountingValues>",
            "int[]",
            customIEnumerable,
            mapper.MapFromGenericTypeImplementingIEnumerableToArray(customIEnumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromICollectionToArray),
            "ICollection<CountingValues>",
            "int[]",
            iCollection,
            mapper.MapFromICollectionToArray(iCollection));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromNonGenericTypeImplementingICollectionToArray),
            "CustomCollectionImplementingICollectionOfCountingValues",
            "int[]",
            customICollectionOf,
            mapper.MapFromNonGenericTypeImplementingICollectionToArray(customICollectionOf));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromGenericTypeImplementingICollectionToArray),
            "CustomCollectionImplementingICollection<CountingValues>",
            "int[]",
            customICollection,
            mapper.MapFromGenericTypeImplementingICollectionToArray(customICollection));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIReadOnlyCollectionToArray),
            "IReadOnlyCollection<CountingValues>",
            "int[]",
            iReadOnlyCollection,
            mapper.MapFromIReadOnlyCollectionToArray(iReadOnlyCollection));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromNonGenericTypeImplementingIReadOnlyCollectionToArray),
            "CustomCollectionImplementingIReadOnlyCollectionOfCountingValues",
            "int[]",
            customIReadOnlyCollectionOf,
            mapper.MapFromNonGenericTypeImplementingIReadOnlyCollectionToArray(customIReadOnlyCollectionOf));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromGenericTypeImplementingIReadOnlyCollectionToArray),
            "CustomCollectionImplementingIReadOnlyCollection<CountingValues>",
            "int[]",
            customIReadOnlyCollection,
            mapper.MapFromGenericTypeImplementingIReadOnlyCollectionToArray(customIReadOnlyCollection));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromStackToArray),
            "Stack<CountingValues>",
            "int[]",
            stack,
            mapper.MapFromStackToArray(stack));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromQueueToArray),
            "Queue<CountingValues>",
            "int[]",
            queue,
            mapper.MapFromQueueToArray(queue));

        var mapFromArrayToSpanOutput = mapper.MapFromArrayToSpan(array);
        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToSpan),
            "CountingValues[]",
            "Span<int>",
            array,
            mapFromArrayToSpanOutput.ToArray());

        var mapFromIListToSpanOutput = mapper.MapFromIListToSpan(iList);
        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIListToSpan),
            "IList<CountingValues>",
            "Span<int>",
            iList,
            mapFromIListToSpanOutput.ToArray());

        var mapFromICollectionToSpanOutput = mapper.MapFromICollectionToSpan(iCollection);
        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromICollectionToSpan),
            "ICollection<CountingValues>",
            "Span<int>",
            iCollection,
            mapFromICollectionToSpanOutput.ToArray());

        var mapFromIEnumerableToSpanOutput = mapper.MapFromIEnumerableToSpan(enumerable);
        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToSpan),
            "IEnumerable<CountingValues>",
            "Span<int>",
            enumerable,
            mapFromIEnumerableToSpanOutput.ToArray());

        var mapFromArrayToReadOnlySpanOutput = mapper.MapFromArrayToReadOnlySpan(array);
        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToReadOnlySpan),
            "CountingValues[]",
            "ReadOnlySpan<int>",
            array,
            mapFromArrayToReadOnlySpanOutput.ToArray());

        var mapFromIListToReadOnlySpanOutput = mapper.MapFromIListToReadOnlySpan(iList);
        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIListToReadOnlySpan),
            "IList<CountingValues>",
            "ReadOnlySpan<int>",
            iList,
            mapFromIListToReadOnlySpanOutput.ToArray());

        var mapFromICollectionToReadOnlySpanOutput = mapper.MapFromICollectionToReadOnlySpan(iCollection);
        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromICollectionToReadOnlySpan),
            "ICollection<CountingValues>",
            "ReadOnlySpan<int>",
            iCollection,
            mapFromICollectionToReadOnlySpanOutput.ToArray());

        var mapFromIEnumerableToReadOnlySpanOutput = mapper.MapFromIEnumerableToReadOnlySpan(enumerable);
        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToReadOnlySpan),
            "IEnumerable<CountingValues>",
            "ReadOnlySpan<int>",
            enumerable,
            mapFromIEnumerableToReadOnlySpanOutput.ToArray());

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToMemory),
            "CountingValues[]",
            "Memory<int>",
            array,
            mapper.MapFromArrayToMemory(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIListToMemory),
            "IList<CountingValues>",
            "Memory<int>",
            iList,
            mapper.MapFromIListToMemory(iList));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromICollectionToMemory),
            "ICollection<CountingValues>",
            "Memory<int>",
            iCollection,
            mapper.MapFromICollectionToMemory(iCollection));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToMemory),
            "IEnumerable<CountingValues>",
            "Memory<int>",
            enumerable,
            mapper.MapFromIEnumerableToMemory(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToReadOnlyMemory),
            "CountingValues[]",
            "ReadOnlyMemory<int>",
            array,
            mapper.MapFromArrayToReadOnlyMemory(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIListToReadOnlyMemory),
            "IList<CountingValues>",
            "ReadOnlyMemory<int>",
            iList,
            mapper.MapFromIListToReadOnlyMemory(iList));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromICollectionToReadOnlyMemory),
            "ICollection<CountingValues>",
            "ReadOnlyMemory<int>",
            iCollection,
            mapper.MapFromICollectionToReadOnlyMemory(iCollection));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToReadOnlyMemory),
            "IEnumerable<CountingValues>",
            "ReadOnlyMemory<int>",
            enumerable,
            mapper.MapFromIEnumerableToReadOnlyMemory(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToNonGenericTypeImplementingICollection),
            "CountingValues[]",
            "CustomCollectionImplementingICollectionOfIntegers",
            array,
            mapper.MapFromArrayToNonGenericTypeImplementingICollection(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToGenericTypeImplementingICollection),
            "CountingValues[]",
            "CustomCollectionImplementingICollection<int>",
            array,
            mapper.MapFromArrayToGenericTypeImplementingICollection(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToNonGenericTypeImplementingICollection),
            "IEnumerable<CountingValues>",
            "CustomCollectionImplementingICollectionOfIntegers",
            enumerable,
            mapper.MapFromIEnumerableToNonGenericTypeImplementingICollection(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToGenericTypeImplementingICollection),
            "IEnumerable<CountingValues>",
            "CustomCollectionImplementingICollection<int>",
            enumerable,
            mapper.MapFromIEnumerableToGenericTypeImplementingICollection(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToStack),
            "CountingValues[]",
            "Stack<int>",
            array,
            mapper.MapFromArrayToStack(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToNonGenericTypeDerivedFromStack),
            "CountingValues[]",
            "CustomStackOfIntegers",
            array,
            mapper.MapFromArrayToNonGenericTypeDerivedFromStack(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayGenericTypeDerivedFromStack),
            "CountingValues[]",
            "CustomStack<int>",
            array,
            mapper.MapFromArrayGenericTypeDerivedFromStack(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToStack),
            "IEnumerable<CountingValues>",
            "Stack<int>",
            enumerable,
            mapper.MapFromIEnumerableToStack(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToNonGenericTypeDerivedFromStack),
            "IEnumerable<CountingValues>",
            "CustomStackOfIntegers",
            enumerable,
            mapper.MapFromIEnumerableToNonGenericTypeDerivedFromStack(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableGenericTypeDerivedFromStack),
            "IEnumerable<CountingValues>",
            "CustomStack<int>",
            enumerable,
            mapper.MapFromIEnumerableGenericTypeDerivedFromStack(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToQueue),
            "CountingValues[]",
            "Queue<int>",
            array,
            mapper.MapFromArrayToQueue(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToNonGenericTypeDerivedFromQueue),
            "CountingValues[]",
            "CustomQueueOfIntegers",
            array,
            mapper.MapFromArrayToNonGenericTypeDerivedFromQueue(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayGenericTypeDerivedFromQueue),
            "CountingValues[]",
            "CustomQueue<int>",
            array,
            mapper.MapFromArrayGenericTypeDerivedFromQueue(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToQueue),
            "IEnumerable<CountingValues>",
            "Queue<int>",
            enumerable,
            mapper.MapFromIEnumerableToQueue(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToNonGenericTypeDerivedFromQueue),
            "IEnumerable<CountingValues>",
            "CustomQueueOfIntegers",
            enumerable,
            mapper.MapFromIEnumerableToNonGenericTypeDerivedFromQueue(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableGenericTypeDerivedFromQueue),
            "IEnumerable<CountingValues>",
            "CustomQueue<int>",
            enumerable,
            mapper.MapFromIEnumerableGenericTypeDerivedFromQueue(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToISet),
            "CountingValues[]",
            "ISet<int>",
            array,
            mapper.MapFromArrayToISet(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToISet),
            "IEnumerable<CountingValues>",
            "ISet<int>",
            enumerable,
            mapper.MapFromIEnumerableToISet(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToIReadOnlySet),
            "CountingValues[]",
            "IReadOnlySet<int>",
            array,
            mapper.MapFromArrayToIReadOnlySet(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToIReadOnlySet),
            "IEnumerable<CountingValues>",
            "IReadOnlySet<int>",
            enumerable,
            mapper.MapFromIEnumerableToIReadOnlySet(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToHashSet),
            "CountingValues[]",
            "HashSet<int>",
            array,
            mapper.MapFromArrayToHashSet(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToHashSet),
            "IEnumerable<CountingValues>",
            "HashSet<int>",
            enumerable,
            mapper.MapFromIEnumerableToHashSet(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToCustomCollectionImplementingISet),
            "CountingValues[]",
            "CustomCollectionImplementingISet<int>",
            array,
            mapper.MapFromArrayToCustomCollectionImplementingISet(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToCustomCollectionImplementingISet),
            "IEnumerable<CountingValues>",
            "CustomCollectionImplementingISet<int>",
            enumerable,
            mapper.MapFromIEnumerableToCustomCollectionImplementingISet(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToCustomCollectionImplementingISetOfIntegers),
            "CountingValues[]",
            "CustomCollectionImplementingISetOfIntegers",
            array,
            mapper.MapFromArrayToCustomCollectionImplementingISetOfIntegers(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToCustomCollectionImplementingISetOfIntegers),
            "IEnumerable<CountingValues>",
            "CustomCollectionImplementingISetOfIntegers",
            enumerable,
            mapper.MapFromIEnumerableToCustomCollectionImplementingISetOfIntegers(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToIReadOnlyList),
            "IEnumerable<CountingValues>",
            "IReadOnlyList<int>",
            enumerable,
            mapper.MapFromIEnumerableToIReadOnlyList(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToIReadOnlyList),
            "CountingValues[]",
            "IReadOnlyList<int>",
            array,
            mapper.MapFromArrayToIReadOnlyList(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToReadOnlyCollection),
            "IEnumerable<CountingValues>",
            "ReadOnlyCollection<int>",
            enumerable,
            mapper.MapFromIEnumerableToReadOnlyCollection(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToReadOnlySet),
            "IEnumerable<CountingValues>",
            "ReadOnlySet<int>",
            enumerable,
            mapper.MapFromIEnumerableToReadOnlySet(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToFrozenSet),
            "IEnumerable<CountingValues>",
            "FrozenSet<int>",
            enumerable,
            mapper.MapFromIEnumerableToFrozenSet(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToIImmutableSet),
            "IEnumerable<CountingValues>",
            "IImmutableSet<int>",
            enumerable,
            mapper.MapFromIEnumerableToIImmutableSet(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToImmutableHashSet),
            "IEnumerable<CountingValues>",
            "ImmutableHashSet<int>",
            enumerable,
            mapper.MapFromIEnumerableToImmutableHashSet(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToImmutableSortedSet),
            "IEnumerable<CountingValues>",
            "ImmutableSortedSet<int>",
            enumerable,
            mapper.MapFromIEnumerableToImmutableSortedSet(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToIImmutableList),
            "IEnumerable<CountingValues>",
            "IImmutableList<int>",
            enumerable,
            mapper.MapFromIEnumerableToIImmutableList(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToImmutableArray),
            "IEnumerable<CountingValues>",
            "ImmutableArray<int>",
            enumerable,
            mapper.MapFromIEnumerableToImmutableArray(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToImmutableList),
            "IEnumerable<CountingValues>",
            "ImmutableList<int>",
            enumerable,
            mapper.MapFromIEnumerableToImmutableList(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToIImmutableQueue),
            "CountingValues[]",
            "IImmutableQueue<int>",
            array,
            mapper.MapFromArrayToIImmutableQueue(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToImmutableQueue),
            "CountingValues[]",
            "ImmutableQueue<int>",
            array,
            mapper.MapFromArrayToImmutableQueue(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToIImmutableStack),
            "CountingValues[]",
            "IImmutableStack<int>",
            array,
            mapper.MapFromArrayToIImmutableStack(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromArrayToImmutableStack),
            "CountingValues[]",
            "ImmutableStack<int>",
            array,
            mapper.MapFromArrayToImmutableStack(array));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToCustomCollectionImplementingExplicitlyICollection),
            "IEnumerable<CountingValues>",
            "CustomCollectionImplementingExplicitlyICollection<int>",
            enumerable,
            mapper.MapFromIEnumerableToCustomCollectionImplementingExplicitlyICollection(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToCustomCollectionImplementingExplicitlyICollectionOfIntegers),
            "IEnumerable<CountingValues>",
            "CustomCollectionImplementingExplicitlyICollectionOfIntegers",
            enumerable,
            mapper.MapFromIEnumerableToCustomCollectionImplementingExplicitlyICollectionOfIntegers(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToCustomCollectionImplementingExplicitlyISet),
            "IEnumerable<CountingValues>",
            "CustomCollectionImplementingExplicitlyISet<int>",
            enumerable,
            mapper.MapFromIEnumerableToCustomCollectionImplementingExplicitlyISet(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToCustomCollectionImplementingExplicitlyISetOfIntegers),
            "IEnumerable<CountingValues>",
            "CustomCollectionImplementingExplicitlyISetOfIntegers",
            enumerable,
            mapper.MapFromIEnumerableToCustomCollectionImplementingExplicitlyISetOfIntegers(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromBlockingCollectionToList),
            "BlockingCollection<CountingValues>",
            "List<int>",
            blockingCollection,
            mapper.MapFromBlockingCollectionToList(blockingCollection));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToBlockingCollection),
            "IEnumerable<CountingValues>",
            "BlockingCollection<int>",
            enumerable,
            mapper.MapFromIEnumerableToBlockingCollection(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToGenericCustomBlockingCollection),
            "IEnumerable<CountingValues>",
            "CustomBlockingCollection<int>",
            enumerable,
            mapper.MapFromIEnumerableToGenericCustomBlockingCollection(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToNonGenericCustomBlockingCollection),
            "IEnumerable<CountingValues>",
            "CustomBlockingCollection",
            enumerable,
            mapper.MapFromIEnumerableToNonGenericCustomBlockingCollection(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToBlockingCollection),
            "List<CountingValues>",
            "BlockingCollection<int>",
            list,
            mapper.MapFromListToBlockingCollection(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToGenericCustomBlockingCollection),
            "List<CountingValues>",
            "CustomBlockingCollection<int>",
            list,
            mapper.MapFromListToGenericCustomBlockingCollection(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToNonGenericCustomBlockingCollection),
            "List<CountingValues>",
            "CustomBlockingCollection",
            list,
            mapper.MapFromListToNonGenericCustomBlockingCollection(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromConcurrentBagToList),
            "ConcurrentBag<CountingValues>",
            "List<int>",
            concurrentBag,
            mapper.MapFromConcurrentBagToList(concurrentBag));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToConcurrentBag),
            "IEnumerable<CountingValues>",
            "ConcurrentBag<int>",
            enumerable,
            mapper.MapFromIEnumerableToConcurrentBag(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToGenericCustomConcurrentBag),
            "IEnumerable<CountingValues>",
            "CustomConcurrentBag<int>",
            enumerable,
            mapper.MapFromIEnumerableToGenericCustomConcurrentBag(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToNonGenericCustomConcurrentBag),
            "IEnumerable<CountingValues>",
            "CustomConcurrentBag",
            enumerable,
            mapper.MapFromIEnumerableToNonGenericCustomConcurrentBag(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToConcurrentBag),
            "List<CountingValues>",
            "ConcurrentBag<int>",
            list,
            mapper.MapFromListToConcurrentBag(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToGenericCustomConcurrentBag),
            "List<CountingValues>",
            "CustomConcurrentBag<int>",
            list,
            mapper.MapFromListToGenericCustomConcurrentBag(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToNonGenericCustomConcurrentBag),
            "List<CountingValues>",
            "CustomConcurrentBag",
            list,
            mapper.MapFromListToNonGenericCustomConcurrentBag(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromConcurrentQueueToList),
            "ConcurrentQueue<CountingValues>",
            "List<int>",
            concurrentQueue,
            mapper.MapFromConcurrentQueueToList(concurrentQueue));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToConcurrentQueue),
            "IEnumerable<CountingValues>",
            "ConcurrentQueue<int>",
            enumerable,
            mapper.MapFromIEnumerableToConcurrentQueue(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToGenericCustomConcurrentQueue),
            "IEnumerable<CountingValues>",
            "CustomConcurrentQueue<int>",
            enumerable,
            mapper.MapFromIEnumerableToGenericCustomConcurrentQueue(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToNonGenericCustomConcurrentQueue),
            "IEnumerable<CountingValues>",
            "CustomConcurrentQueue",
            enumerable,
            mapper.MapFromIEnumerableToNonGenericCustomConcurrentQueue(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToConcurrentQueue),
            "List<CountingValues>",
            "ConcurrentQueue<int>",
            list,
            mapper.MapFromListToConcurrentQueue(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToGenericCustomConcurrentQueue),
            "List<CountingValues>",
            "CustomConcurrentQueue<int>",
            list,
            mapper.MapFromListToGenericCustomConcurrentQueue(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToNonGenericCustomConcurrentQueue),
            "List<CountingValues>",
            "CustomConcurrentQueue",
            list,
            mapper.MapFromListToNonGenericCustomConcurrentQueue(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromConcurrentStackToList),
            "ConcurrentStack<CountingValues>",
            "List<int>",
            concurrentStack,
            mapper.MapFromConcurrentStackToList(concurrentStack));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToConcurrentStack),
            "IEnumerable<CountingValues>",
            "ConcurrentStack<int>",
            enumerable,
            mapper.MapFromIEnumerableToConcurrentStack(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToGenericCustomConcurrentStack),
            "IEnumerable<CountingValues>",
            "CustomConcurrentStack<int>",
            enumerable,
            mapper.MapFromIEnumerableToGenericCustomConcurrentStack(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToNonGenericCustomConcurrentStack),
            "IEnumerable<CountingValues>",
            "CustomConcurrentStack",
            enumerable,
            mapper.MapFromIEnumerableToNonGenericCustomConcurrentStack(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToConcurrentStack),
            "List<CountingValues>",
            "ConcurrentStack<int>",
            list,
            mapper.MapFromListToConcurrentStack(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToGenericCustomConcurrentStack),
            "List<CountingValues>",
            "CustomConcurrentStack<int>",
            list,
            mapper.MapFromListToGenericCustomConcurrentStack(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToNonGenericCustomConcurrentStack),
            "List<CountingValues>",
            "CustomConcurrentStack",
            list,
            mapper.MapFromListToNonGenericCustomConcurrentStack(list));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIProducerConsumerCollectionToList),
            "IProducerConsumerCollection<CountingValues>",
            "List<int>",
            producerConsumer,
            mapper.MapFromIProducerConsumerCollectionToList(producerConsumer));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromIEnumerableToIProducerConsumerCollection),
            "IEnumerable<CountingValues>",
            "IProducerConsumerCollection<int>",
            enumerable,
            mapper.MapFromIEnumerableToIProducerConsumerCollection(enumerable));

        report.RecordInvocation(
            nameof(CollectionToCollectionMapper.MapFromListToIProducerConsumerCollection),
            "List<CountingValues>",
            "IProducerConsumerCollection<int>",
            list,
            mapper.MapFromListToIProducerConsumerCollection(list));
    }
}