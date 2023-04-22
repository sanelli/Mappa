// <copyright file="AutoMapperProfile.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper;

namespace Mappa.Benchmark.Mappers;

/// <summary>
/// Automapper mapper profile.
/// </summary>
public sealed class AutoMapperProfile
    : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
    /// </summary>
    public AutoMapperProfile()
    {
        this.CreateMap<string, object>();
        this.CreateMap<string, string>();
    }
}