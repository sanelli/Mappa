// <copyright file="MappaMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402

using Mappa.Attributes;
using Mappa.Benchmark.Collections.Models;

namespace Mappa.Benchmark.Collections.Mappers;

/// <summary>
/// Mappa mapper for collection benchmarks.
/// </summary>
[Mappa]
internal sealed partial class MappaMapper
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

/// <summary>
/// Mappa mapper with fast-collection settings enabled.
/// </summary>
[Mappa]
[MappaSettings(FastCollections = BooleanSetting.Enable, ContainerCapacityConstructors = BooleanSetting.Enable)]
internal sealed partial class MappaFastMapper
{
    /// <summary>
    /// Maps a list to an array using fast collections.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial CollectionItem[] MapToArray(List<CollectionItemDto> source);
}