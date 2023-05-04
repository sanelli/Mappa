// <copyright file="AutomapperMapperProfile.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper;

namespace Mappa.Benchmark.Enums.Mappers;

/// <summary>
/// The mapper profile for <see cref="AutoMapper"/>.
/// </summary>
public sealed class AutomapperMapperProfile
    : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomapperMapperProfile"/> class.
    /// </summary>
    public AutomapperMapperProfile()
    {
        this.CreateMap<StringComparison, string>();
        this.CreateMap<string, StringComparison>();

        this.CreateMap<StringComparison, int>();
        this.CreateMap<int, StringComparison>();
    }
}