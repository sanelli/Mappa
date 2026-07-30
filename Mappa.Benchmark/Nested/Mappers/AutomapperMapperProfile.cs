// <copyright file="AutomapperMapperProfile.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper;

using Mappa.Benchmark.Nested.Models;

namespace Mappa.Benchmark.Nested.Mappers;

/// <summary>
/// AutoMapper profile for nested DTO benchmarks.
/// </summary>
internal sealed class AutomapperMapperProfile
    : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomapperMapperProfile"/> class.
    /// </summary>
    public AutomapperMapperProfile()
    {
        this.CreateMap<LineItemDto, LineItem>();
        this.CreateMap<CustomerDto, Customer>();
        this.CreateMap<NestedOrderDto, NestedOrder>();
    }
}