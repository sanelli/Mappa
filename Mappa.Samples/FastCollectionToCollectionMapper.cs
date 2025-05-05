// <copyright file="FastCollectionToCollectionMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper showing mapping across different collection types.
/// </summary>
[Mappa]
[MappaSettings(FastCollections = BooleanSetting.Enable)]
public sealed partial class FastCollectionToCollectionMapper
{
    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapArrayToArray(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="Array"/> of <see cref="CountingValues"/>
    /// to an <see cref="List{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial List<int> MapArrayToList(CountingValues[] input);

    /// <summary>
    /// Map an <see cref="List{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapListToArray(List<CountingValues> input);

    /// <summary>
    /// Map an <see cref="List{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="List{T}"/> of <see cref="int"/>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial List<int> MapListToList(List<CountingValues> input);
}