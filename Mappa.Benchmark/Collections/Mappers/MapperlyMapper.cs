// <copyright file="MapperlyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.Collections.Models;

using Riok.Mapperly.Abstractions;

namespace Mappa.Benchmark.Collections.Mappers;

/// <summary>
/// Mapperly mapper for collection benchmarks.
/// </summary>
[Mapper]
internal sealed partial class MapperlyMapper
{
    /// <summary>
    /// Maps an array to a list.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial List<CollectionItem> MapToList(CollectionItemDto[] source);

    /// <summary>
    /// Maps a list to an array.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial CollectionItem[] MapToArray(List<CollectionItemDto> source);

    /// <summary>
    /// Maps a list to a hash set of identifiers.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial HashSet<int> MapToHashSet(List<int> source);

    /// <summary>
    /// Maps a dictionary of items.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial Dictionary<string, CollectionItem> Map(Dictionary<string, CollectionItemDto> source);
}