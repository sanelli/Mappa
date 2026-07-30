// <copyright file="AutomapperMapperProfile.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper;

using Mappa.Benchmark.Objects.Models;

namespace Mappa.Benchmark.Objects.Mappers;

/// <summary>
/// AutoMapper profile for class/record/struct person models.
/// </summary>
internal sealed class AutomapperMapperProfile
    : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomapperMapperProfile"/> class.
    /// </summary>
    public AutomapperMapperProfile()
    {
        this.CreateMap<AddressDto, Address>();
        this.CreateMap<PersonClassDto, PersonClass>();
        this.CreateMap<PersonRecordDto, PersonRecord>();
        this.CreateMap<PersonStructDto, PersonStruct>();
    }
}