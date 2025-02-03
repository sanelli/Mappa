// <copyright file="MapperlyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.Spotify.Models;

namespace Mappa.Benchmark.Spotify.Mappers;

/// <summary>
/// Mapper using <see cref="Riok.Mapperly"/>.
/// </summary>
[Riok.Mapperly.Abstractions.Mapper]
internal sealed partial class MapperlyMapper
{
    /// <summary>
    /// Map from <see cref="SpotifyAlbumDto"/>
    /// to <see cref="SpotifyAlbum"/>.
    /// </summary>
    /// <param name="input">The input dto.</param>
    /// <returns>The output model.</returns>
    public partial SpotifyAlbum Map(SpotifyAlbumDto input);
}