// <copyright file="EnumerableOrCollectionToCollectionMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using enumerable-or-collection-to-collection strategy.
/// </summary>
[Mappa]
public sealed partial class EnumerableOrCollectionToCollectionMapper
{
    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/> to an <see cref="IList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IList<int> MapIEnumerableToIList(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/> to an <see cref="List{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial List<int> MapIEnumerableToList(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/> to an <see cref="ICollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ICollection<int> MapIEnumerableToICollection(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/> to an <see cref="IReadOnlyCollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlyCollection<int> MapIEnumerableToIReadOnlyCollection(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/> to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapIEnumerableToIEnumerable(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/> to an <see cref="IList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IList<int> MapICollectionToIList(ICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/> to an <see cref="List{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial List<int> MapICollectionToList(ICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/> to an <see cref="ICollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ICollection<int> MapICollectionToICollection(ICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/> to an <see cref="IReadOnlyCollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlyCollection<int> MapICollectionToIReadOnlyCollection(ICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/> to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapICollectionToIEnumerable(ICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IReadOnlyCollection{T}"/> of <see cref="CountingValues"/> to an <see cref="IList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IList<int> MapIReadOnlyCollectionToIList(IReadOnlyCollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IReadOnlyCollection{T}"/> of <see cref="CountingValues"/> to an <see cref="List{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial List<int> MapIReadOnlyCollectionToList(IReadOnlyCollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IReadOnlyCollection{T}"/> of <see cref="CountingValues"/> to an <see cref="ICollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ICollection<int> MapIReadOnlyCollectionToICollection(IReadOnlyCollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IReadOnlyCollection{T}"/> of <see cref="CountingValues"/> to an <see cref="IReadOnlyCollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlyCollection<int> MapIReadOnlyCollectionToIReadOnlyCollection(IReadOnlyCollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IReadOnlyCollection{T}"/> of <see cref="CountingValues"/> to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapIReadOnlyCollectionToIEnumerable(IReadOnlyCollection<CountingValues> input);
}