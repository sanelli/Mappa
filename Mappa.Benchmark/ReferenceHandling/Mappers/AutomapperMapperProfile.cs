// <copyright file="AutomapperMapperProfile.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper;

using Mappa.Benchmark.ReferenceHandling.Models;

namespace Mappa.Benchmark.ReferenceHandling.Mappers;

/// <summary>
/// AutoMapper profile for reference-handling benchmarks (preserve references).
/// </summary>
internal sealed class AutomapperMapperProfile
    : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomapperMapperProfile"/> class.
    /// </summary>
    public AutomapperMapperProfile()
    {
        this.CreateMap<PersonSource, PersonTarget>().PreserveReferences();
        this.CreateMap<AddressSource, AddressTarget>().PreserveReferences();
        this.CreateMap<RootSource, RootTarget>().PreserveReferences();
        this.CreateMap<NodeSource, NodeTarget>().PreserveReferences();
    }
}