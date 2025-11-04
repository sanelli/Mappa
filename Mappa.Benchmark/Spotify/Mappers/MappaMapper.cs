// <copyright file="MappaMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Benchmark.Spotify.Models;

namespace Mappa.Benchmark.Spotify.Mappers;

/// <summary>
/// Mapper using <see cref="Mappa"/>.
/// </summary>
[Mappa]
[MappaSettings(ContainerCapacityConstructors = BooleanSetting.Enable, FastCollections = BooleanSetting.Enable)]
internal sealed partial class MappaMapper
{
    /// <summary>
    /// Map from <see cref="SpotifyAlbumDto"/>
    /// to <see cref="SpotifyAlbum"/>.
    /// </summary>
    /// <param name="input">The input dto.</param>
    /// <returns>The output model.</returns>
    public partial SpotifyAlbum Map(SpotifyAlbumDto input);
}