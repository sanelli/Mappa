// <copyright file="CollectionToCollectionMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper showing mapping across different collection types.
/// </summary>
// TODO [#105] impl non-generic IEnumerable<CountingValues> -> IEnumerable<int>.
// TODO [#105] impl generic IEnumerable<CountingValues> -> IEnumerable<int>.
// TODO [#105] CountingValues[] -> IEnumerable<int>.
// TODO [#105] Span<CountingValues> -> IEnumerable<int>.
// TODO [#105] ReadOnlySpan<CountingValues> -> IEnumerable<int>.
// TODO [#105] Memory<CountingValues> -> IEnumerable<string>.
// TODO [#105] ReadOnlyMemory<CountingValues> -> IEnumerable<string>.
// TODO [#105] is IList<int> -> IEnumerable<string>.
// TODO [#105] non-generic impl IList<int> -> IEnumerable<string>.
// TODO [#105] generic impl IList<int> -> IEnumerable<string>.
// TODO [#105] is ICollection<int> -> IEnumerable<string>.
// TODO [#105] non-generic impl ICollection<int> -> IEnumerable<string>.
// TODO [#105] generic impl ICollection<int> -> IEnumerable<string>.
// TODO [#105] IEnumerable<int> -> List<string>.
// TODO [#105] int[] -> List<string>.
// TODO [#105] IEnumerable<int> -> IList<string>.
// TODO [#105] int[] -> IList<string>.
// TODO [#105] IEnumerable<int> -> ICollection<string>.
// TODO [#105] int[] -> ICollection<string>.
// TODO [#105] IEnumerable<int> -> IReadOnlyCollection<string>.
// TODO [#105] int[] -> IReadOnlyCollection<string>.
// TODO [#105] int[] -> string[].
// TODO [#105] int[] -> Span<long>.
// TODO [#105] int[] -> ReadOnlySpan<long>.
// TODO [#105] int[] -> Memory<long>.
// TODO [#105] int[] -> ReadOnlyMemory<long>.
// TODO [#105] ICollection<int> -> string[].
// TODO [#105] generic impl ICollection<int> -> string[].
// TODO [#105] non-generic impl ICollection<int> -> string[].
// TODO [#105] ICollection<int> -> Span<long>.
// TODO [#105] ICollection<int> -> ReadOnlySpan<long>.
// TODO [#105] ICollection<int> -> Memory<long>.
// TODO [#105] ICollection<int> -> ReadOnlyMemory<long>.
// TODO [#105] IEnumerable<int> -> string[].
// TODO [#105] non-generic impl IEnumerable<int> -> string[].
// TODO [#105] generic impl IEnumerable<int> -> string[].
// TODO [#105] IList<int> -> string[].
// TODO [#105] non-generic impl IList<int> -> string[].
// TODO [#105] generic impl IList<int> -> string[].
// TODO [#105] IEnumerable<int> -> Span<long>.
// TODO [#105] IEnumerable<int> -> ReadOnlySpan<long>.
// TODO [#105] IEnumerable<int> -> Memory<long>.
// TODO [#105] IEnumerable<int> -> ReadOnlyMemory<long>.
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
}