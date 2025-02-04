// <copyright file="SpotifyBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Spotify.Mappers;
using Mappa.Benchmark.Spotify.Models;

using Mapster;

namespace Mappa.Benchmark.Spotify;

/// <summary>
/// Spotify benchmark.
/// </summary>
[MemoryDiagnoser]
internal sealed class SpotifyBenchmark
{
    private readonly SpotifyAlbumDto spotifyAlbumDto;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpotifyBenchmark"/> class.
    /// </summary>
    public SpotifyBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(AutomapperMapperProfile));
        }).CreateMapper();

        this.mapperlyMapper = new();

        this.mappaMapper = new();

        this.spotifyAlbumDto = new AutoBogus.AutoFaker<SpotifyAlbumDto>().Generate();
    }

    /// <summary>
    /// Map using <see cref="AutoMapper"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public SpotifyAlbum Automapper()
        => this.automapperMapper.Map<SpotifyAlbum>(this.spotifyAlbumDto);

    /// <summary>
    /// Map using <see cref="Riok.Mapperly"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public SpotifyAlbum Mapperly()
        => this.mapperlyMapper.Map(this.spotifyAlbumDto);

    /// <summary>
    /// Map using <see cref="Mapster"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public SpotifyAlbum Mapster()
        => this.spotifyAlbumDto.Adapt<SpotifyAlbum>();

    /// <summary>
    /// Map using <see cref="Mappa.Attributes"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark(Baseline = true)]
    public SpotifyAlbum Mappa()
        => this.mappaMapper.Map(this.spotifyAlbumDto);
}