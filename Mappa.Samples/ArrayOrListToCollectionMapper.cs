// <copyright file="ArrayOrListToCollectionMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using array-or-list-to-collection strategy.
/// </summary>
[Mappa]
public sealed partial class ArrayOrListToCollectionMapper
{
    /// <summary>
    /// Map an array of <see cref="CountingValues"/> to an <see cref="IList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IList<int> MapArrayToIList(CountingValues[] input);

    /// <summary>
    /// Map an array of <see cref="CountingValues"/> to an <see cref="List{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial List<int> MapArrayToList(CountingValues[] input);

    /// <summary>
    /// Map an array of <see cref="CountingValues"/> to an <see cref="ICollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ICollection<int> MapArrayToICollection(CountingValues[] input);

    /// <summary>
    /// Map an array of <see cref="CountingValues"/> to an <see cref="IReadOnlyCollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlyCollection<int> MapArrayToIReadOnlyCollection(CountingValues[] input);

    /// <summary>
    /// Map an array of <see cref="CountingValues"/> to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapArrayToIEnumerable(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="IList{T}"/> of <see cref="CountingValues"/> to an <see cref="IList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IList<int> MapIListToIList(IList<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IList{T}"/> of <see cref="CountingValues"/> to an <see cref="List{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial List<int> MapIListToList(IList<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IList{T}"/> of <see cref="CountingValues"/> to an <see cref="ICollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ICollection<int> MapIListToICollection(IList<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IList{T}"/> of <see cref="CountingValues"/> to an <see cref="IReadOnlyCollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlyCollection<int> MapIListToIReadOnlyCollection(IList<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IList{T}"/> of <see cref="CountingValues"/> to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapIListToIEnumerable(IList<CountingValues> input);

    /// <summary>
    /// Map an <see cref="List{T}"/> of <see cref="CountingValues"/> to an <see cref="IList{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IList<int> MapListToIList(List<CountingValues> input);

    /// <summary>
    /// Map an <see cref="List{T}"/> of <see cref="CountingValues"/> to an <see cref="List{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial List<int> MapListToList(List<CountingValues> input);

    /// <summary>
    /// Map an <see cref="List{T}"/> of <see cref="CountingValues"/> to an <see cref="ICollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial ICollection<int> MapListToICollection(List<CountingValues> input);

    /// <summary>
    /// Map an <see cref="List{T}"/> of <see cref="CountingValues"/> to an <see cref="IReadOnlyCollection{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IReadOnlyCollection<int> MapListToIReadOnlyCollection(List<CountingValues> input);

    /// <summary>
    /// Map an <see cref="List{T}"/> of <see cref="CountingValues"/> to an <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial IEnumerable<int> MapListToIEnumerable(List<CountingValues> input);
}