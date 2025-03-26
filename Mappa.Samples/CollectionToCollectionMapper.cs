// <copyright file="CollectionToCollectionMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper showing mapping across different collection types.
/// </summary>
[Mappa]
public sealed partial class CollectionToCollectionMapper
{
    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapIEnumerableToIEnumerable(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map <see cref="CustomCollectionImplementingIEnumerableOfCountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromNonGenericTypeImplementingIEnumerableToIEnumerable(CustomCollectionImplementingIEnumerableOfCountingValues input);

    /// <summary>
    /// Map <see cref="CustomCollectionImplementingIEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromGenericTypeImplementingIEnumerableToIEnumerable(CustomCollectionImplementingIEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromArrayToIEnumerable(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="Span{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromSpanToIEnumerable(Span<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ReadOnlySpan{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromReadOnlySpanToIEnumerable(ReadOnlySpan<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Memory{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromMemoryToIEnumerable(Memory<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ReadOnlyMemory{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromReadOnlyMemoryToIEnumerable(ReadOnlyMemory<CountingValues> input);

    /// <summary>
    /// Map an <see cref="List{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromIListToIEnumerable(IList<CountingValues> input);

    /// <summary>
    /// Map <see cref="CustomCollectionImplementingIListOfCountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromNonGenericTypeImplementingIListToIEnumerable(CustomCollectionImplementingIListOfCountingValues input);

    /// <summary>
    /// Map <see cref="CustomCollectionImplementingIList{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromGenericTypeImplementingIListToIEnumerable(CustomCollectionImplementingIList<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromICollectionToIEnumerable(ICollection<CountingValues> input);

    /// <summary>
    /// Map <see cref="CustomCollectionImplementingICollectionOfCountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromNonGenericTypeImplementingICollectionToIEnumerable(CustomCollectionImplementingICollectionOfCountingValues input);

    /// <summary>
    /// Map <see cref="CustomCollectionImplementingICollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromGenericTypeImplementingICollectionToIEnumerable(CustomCollectionImplementingICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IReadOnlyCollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromIReadOnlyCollectionToIEnumerable(IReadOnlyCollection<CountingValues> input);

    /// <summary>
    /// Map <see cref="CustomCollectionImplementingIReadOnlyCollectionOfCountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromNonGenericTypeImplementingIReadOnlyCollectionToIEnumerable(CustomCollectionImplementingIReadOnlyCollectionOfCountingValues input);

    /// <summary>
    /// Map <see cref="CustomCollectionImplementingIReadOnlyCollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromGenericTypeImplementingIReadOnlyCollectionToIEnumerable(CustomCollectionImplementingIReadOnlyCollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="List{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial List<int> MapFromIEnumerableToList(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="List{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial List<int> MapFromArrayToList(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IList<int> MapFromIEnumerableToIList(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="IList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IList<int> MapFromArrayToIList(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="ICollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ICollection<int> MapFromIEnumerableToICollection(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="ICollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ICollection<int> MapFromArrayToICollection(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IReadOnlyCollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlyCollection<int> MapFromIEnumerableToIReadOnlyCollection(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="IReadOnlyCollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlyCollection<int> MapFromArrayToIReadOnlyCollection(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="Stack{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromStackToIEnumerable(Stack<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Queue{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapFromQueueToIEnumerable(Queue<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromArrayToArray(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromIEnumerableToArray(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="CustomCollectionImplementingIEnumerableOfCountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromNonGenericTypeImplementingIEnumerableToArray(CustomCollectionImplementingIEnumerableOfCountingValues input);

    /// <summary>
    /// Map an <see cref="CustomCollectionImplementingIEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromGenericTypeImplementingIEnumerableToArray(CustomCollectionImplementingIEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromICollectionToArray(ICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="CustomCollectionImplementingICollectionOfCountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromNonGenericTypeImplementingICollectionToArray(CustomCollectionImplementingICollectionOfCountingValues input);

    /// <summary>
    /// Map an <see cref="CustomCollectionImplementingICollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromGenericTypeImplementingICollectionToArray(CustomCollectionImplementingICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IReadOnlyCollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromIReadOnlyCollectionToArray(IReadOnlyCollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="CustomCollectionImplementingIReadOnlyCollectionOfCountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromNonGenericTypeImplementingIReadOnlyCollectionToArray(CustomCollectionImplementingIReadOnlyCollectionOfCountingValues input);

    /// <summary>
    /// Map an <see cref="CustomCollectionImplementingIReadOnlyCollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromGenericTypeImplementingIReadOnlyCollectionToArray(CustomCollectionImplementingIReadOnlyCollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Stack{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromStackToArray(Stack<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Queue{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapFromQueueToArray(Queue<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="Span{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Span<int> MapFromArrayToSpan(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IList{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Span{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Span<int> MapFromIListToSpan(IList<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Span{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Span<int> MapFromICollectionToSpan(ICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Span{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Span<int> MapFromIEnumerableToSpan(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="ReadOnlySpan{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ReadOnlySpan<int> MapFromArrayToReadOnlySpan(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IList{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="ReadOnlySpan{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ReadOnlySpan<int> MapFromIListToReadOnlySpan(IList<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="ReadOnlySpan{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ReadOnlySpan<int> MapFromICollectionToReadOnlySpan(ICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="ReadOnlySpan{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ReadOnlySpan<int> MapFromIEnumerableToReadOnlySpan(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="Memory{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Memory<int> MapFromArrayToMemory(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IList{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Memory{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Memory<int> MapFromIListToMemory(IList<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Memory{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Memory<int> MapFromICollectionToMemory(ICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Memory{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Memory<int> MapFromIEnumerableToMemory(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="ReadOnlyMemory{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ReadOnlyMemory<int> MapFromArrayToReadOnlyMemory(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IList{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="ReadOnlyMemory{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ReadOnlyMemory<int> MapFromIListToReadOnlyMemory(IList<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="ReadOnlyMemory{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ReadOnlyMemory<int> MapFromICollectionToReadOnlyMemory(ICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="ReadOnlyMemory{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ReadOnlyMemory<int> MapFromIEnumerableToReadOnlyMemory(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="int"/>
    /// to an <see cref="CustomCollectionImplementingICollectionOfCountingValues"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomCollectionImplementingICollectionOfIntegers MapFromArrayToNonGenericTypeImplementingICollection(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="CustomCollectionImplementingICollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomCollectionImplementingICollection<int> MapFromArrayToGenericTypeImplementingICollection(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="int"/>
    /// to an <see cref="CustomCollectionImplementingICollectionOfCountingValues"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomCollectionImplementingICollectionOfIntegers MapFromIEnumerableToNonGenericTypeImplementingICollection(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="CustomCollectionImplementingICollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomCollectionImplementingICollection<int> MapFromIEnumerableToGenericTypeImplementingICollection(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="Stack{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Stack<int> MapFromArrayToStack(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="CustomStackOfIntegers"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomStackOfIntegers MapFromArrayToNonGenericTypeDerivedFromStack(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="CustomStack{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomStack<int> MapFromArrayGenericTypeDerivedFromStack(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Stack{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Stack<int> MapFromIEnumerableToStack(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="CustomStackOfIntegers"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomStackOfIntegers MapFromIEnumerableToNonGenericTypeDerivedFromStack(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="CustomStack{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomStack<int> MapFromIEnumerableGenericTypeDerivedFromStack(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="Queue{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Queue<int> MapFromArrayToQueue(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="CustomQueueOfIntegers"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomQueueOfIntegers MapFromArrayToNonGenericTypeDerivedFromQueue(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="CustomQueue{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomQueue<int> MapFromArrayGenericTypeDerivedFromQueue(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Queue{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Queue<int> MapFromIEnumerableToQueue(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="CustomQueueOfIntegers"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomQueueOfIntegers MapFromIEnumerableToNonGenericTypeDerivedFromQueue(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="CustomQueue{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomQueue<int> MapFromIEnumerableGenericTypeDerivedFromQueue(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="ISet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ISet<int> MapFromArrayToISet(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="ISet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ISet<int> MapFromIEnumerableToISet(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="IReadOnlySet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlySet<int> MapFromArrayToIReadOnlySet(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="IReadOnlySet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlySet<int> MapFromIEnumerableToIReadOnlySet(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="HashSet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial HashSet<int> MapFromArrayToHashSet(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="HashSet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial HashSet<int> MapFromIEnumerableToHashSet(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to a <see cref="CustomCollectionImplementingISet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomCollectionImplementingISet<int> MapFromArrayToCustomCollectionImplementingISet(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="CustomCollectionImplementingISet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomCollectionImplementingISet<int> MapFromIEnumerableToCustomCollectionImplementingISet(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to a <see cref="CustomCollectionImplementingISetOfIntegers"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomCollectionImplementingISetOfIntegers MapFromArrayToCustomCollectionImplementingISetOfIntegers(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="CustomCollectionImplementingISetOfIntegers"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial CustomCollectionImplementingISetOfIntegers MapFromIEnumerableToCustomCollectionImplementingISetOfIntegers(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="IReadOnlyList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlyList<int> MapFromIEnumerableToIReadOnlyList(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to a <see cref="IReadOnlyList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlyList<int> MapFromArrayToIReadOnlyList(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="ReadOnlyCollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ReadOnlyCollection<int> MapFromIEnumerableToReadOnlyCollection(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="ReadOnlySet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ReadOnlySet<int> MapFromIEnumerableToReadOnlySet(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="FrozenSet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial FrozenSet<int> MapFromIEnumerableToFrozenSet(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="IImmutableSet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IImmutableSet<int> MapFromIEnumerableToIImmutableSet(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="ImmutableHashSet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ImmutableHashSet<int> MapFromIEnumerableToImmutableHashSet(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="ImmutableSortedSet{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ImmutableSortedSet<int> MapFromIEnumerableToImmutableSortedSet(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="IImmutableList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IImmutableList<int> MapFromIEnumerableToIImmutableList(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="ImmutableArray{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ImmutableArray<int> MapFromIEnumerableToImmutableArray(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="ImmutableList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ImmutableList<int> MapFromIEnumerableToImmutableList(IEnumerable<CountingValues> input);
}