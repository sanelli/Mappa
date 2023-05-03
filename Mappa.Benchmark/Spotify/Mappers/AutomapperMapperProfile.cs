// <copyright file="AutomapperMapperProfile.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper;

using Mappa.Benchmark.Spotify.Models;

namespace Mappa.Benchmark.Spotify.Mappers;

/// <summary>
/// The mapper for <see cref="AutomapperMapperProfile"/>.
/// </summary>
public sealed class AutomapperMapperProfile
    : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomapperMapperProfile"/> class.
    /// </summary>
    public AutomapperMapperProfile()
    {
        this.CreateMap<SpotifyAlbumDto, SpotifyAlbum>();
        this.CreateMap<CopyrightDto, Copyright>();
        this.CreateMap<ArtistDto, Artist>();
        this.CreateMap<ExternalIdsDto, ExternalIds>();
        this.CreateMap<ExternalUrlsDto, ExternalUrls>();
        this.CreateMap<TracksDto, Tracks>();
        this.CreateMap<ImageDto, Image>();
        this.CreateMap<ItemDto, Item>();
        this.CreateMap<SpotifyAlbum, SpotifyAlbumDto>();
        this.CreateMap<Copyright, CopyrightDto>();
        this.CreateMap<Artist, ArtistDto>();
        this.CreateMap<ExternalIds, ExternalIdsDto>();
        this.CreateMap<ExternalUrls, ExternalUrlsDto>();
        this.CreateMap<Tracks, TracksDto>();
        this.CreateMap<Image, ImageDto>();
        this.CreateMap<Item, ItemDto>();
    }
}