// <copyright file="AutomapperMapperProfile.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper;

using Mappa.Benchmark.Collections.Models;

namespace Mappa.Benchmark.Collections.Mappers;

/// <summary>
/// AutoMapper profile for collection benchmarks.
/// </summary>
internal sealed class AutomapperMapperProfile
    : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomapperMapperProfile"/> class.
    /// </summary>
    public AutomapperMapperProfile()
    {
        this.CreateMap<CollectionItemDto, CollectionItem>();
        this.CreateMap<CollectionItemDto[], List<CollectionItem>>();
        this.CreateMap<List<CollectionItemDto>, CollectionItem[]>();
        this.CreateMap<List<int>, HashSet<int>>();
        this.CreateMap<Dictionary<string, CollectionItemDto>, Dictionary<string, CollectionItem>>();
    }
}