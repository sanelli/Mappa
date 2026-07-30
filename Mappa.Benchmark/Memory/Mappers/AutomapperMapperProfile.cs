// <copyright file="AutomapperMapperProfile.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper;

namespace Mappa.Benchmark.Memory.Mappers;

/// <summary>
/// AutoMapper profile for memory/array benchmarks.
/// </summary>
internal sealed class AutomapperMapperProfile
    : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomapperMapperProfile"/> class.
    /// </summary>
    public AutomapperMapperProfile()
    {
        this.CreateMap<Memory<int>, int[]>().ConvertUsing(memory => memory.ToArray());
        this.CreateMap<int[], Memory<int>>().ConvertUsing(array => new Memory<int>(array));
    }
}