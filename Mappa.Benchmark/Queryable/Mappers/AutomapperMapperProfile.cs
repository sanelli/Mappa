// <copyright file="AutomapperMapperProfile.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper;

using Mappa.Benchmark.Queryable.Models;

namespace Mappa.Benchmark.Queryable.Mappers;

/// <summary>
/// AutoMapper profile for IQueryable projection benchmarks.
/// </summary>
internal sealed class AutomapperMapperProfile
    : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomapperMapperProfile"/> class.
    /// </summary>
    public AutomapperMapperProfile()
    {
        this.CreateMap<ProjectionOrder, ProjectionOrderDto>()
            .ForMember(destination => destination.Title, options => options.MapFrom(source => source.Name));
    }
}