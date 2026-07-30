// <copyright file="AutomapperMapperProfile.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper;

using Mappa.Benchmark.Polymorphism.Models;

namespace Mappa.Benchmark.Polymorphism.Mappers;

/// <summary>
/// AutoMapper profile for polymorphic benchmarks.
/// </summary>
internal sealed class AutomapperMapperProfile
    : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomapperMapperProfile"/> class.
    /// </summary>
    public AutomapperMapperProfile()
    {
        this.CreateMap<AnimalDto, Animal>()
            .Include<DogDto, Dog>()
            .Include<CatDto, Cat>()
            .Include<BirdDto, Bird>();
        this.CreateMap<DogDto, Dog>();
        this.CreateMap<CatDto, Cat>();
        this.CreateMap<BirdDto, Bird>();
    }
}