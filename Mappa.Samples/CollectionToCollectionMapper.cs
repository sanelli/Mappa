// <copyright file="CollectionToCollectionMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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
}