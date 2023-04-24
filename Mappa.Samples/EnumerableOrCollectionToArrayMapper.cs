// <copyright file="EnumerableOrCollectionToArrayMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using enumerable-or-collection-to-array strategy.
/// </summary>
[Mappa]
public sealed partial class EnumerableOrCollectionToArrayMapper
{
    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/> to an array of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] Map(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="ICollection{T}"/> of <see cref="CountingValues"/> to an array of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] Map(ICollection<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IReadOnlyCollection{T}"/> of <see cref="CountingValues"/> to an array of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] Map(IReadOnlyCollection<CountingValues> input);
}