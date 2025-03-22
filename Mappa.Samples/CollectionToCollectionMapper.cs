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
}