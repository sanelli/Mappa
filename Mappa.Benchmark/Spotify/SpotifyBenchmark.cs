// <copyright file="SpotifyBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoBogus;

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Common;
using Mappa.Benchmark.Spotify.Mappers;
using Mappa.Benchmark.Spotify.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Spotify;

/// <summary>
/// Spotify benchmark.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class SpotifyBenchmark
#pragma warning restore CA1515
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
        BenchmarkSeed.Apply();

        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg =>
                {
                    cfg.AddProfile(new AutomapperMapperProfile());
                },
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000

        this.mapperlyMapper = new();

        this.mappaMapper = new();

        this.spotifyAlbumDto = new AutoFaker<SpotifyAlbumDto>()
            .Configure(builder => builder.WithRepeatCount(BenchmarkConstants.CollectionSize))
            .Generate();
    }

    /// <summary>
    /// Map using <see cref="AutoMapper"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark(Baseline = true)]
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
    [Benchmark]
    public SpotifyAlbum Mappa()
        => this.mappaMapper.Map(this.spotifyAlbumDto);
}