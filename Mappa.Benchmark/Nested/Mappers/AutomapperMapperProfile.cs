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
        this.CreateMap<CoordinateDto, Coordinate>();
        this.CreateMap<GeoRegionDto, GeoRegion>();
        this.CreateMap<AddressDto, Address>();

        this.CreateMap<PartyDto, Party>()
            .Include<PersonPartyDto, PersonParty>()
            .Include<OrganizationPartyDto, OrganizationParty>();
        this.CreateMap<PersonPartyDto, PersonParty>();
        this.CreateMap<OrganizationPartyDto, OrganizationParty>();

        this.CreateMap<CustomerDto, Customer>();

        this.CreateMap<LineItemBaseDto, LineItemBase>()
            .Include<PhysicalLineItemDto, PhysicalLineItem>()
            .Include<DigitalLineItemDto, DigitalLineItem>();
        this.CreateMap<PhysicalLineItemDto, PhysicalLineItem>();
        this.CreateMap<DigitalLineItemDto, DigitalLineItem>();

        this.CreateMap<Memory<int>, int[]>().ConvertUsing(memory => memory.ToArray());
        this.CreateMap<ReadOnlyMemory<int>, ReadOnlyMemory<int>>().ConvertUsing(memory => memory);

        this.CreateMap<NestedOrderDto, NestedOrder>()
            .ForMember(destination => destination.Notes, options => options.Ignore())
            .AfterMap((source, destination) =>
            {
                destination.Notes.Clear();
                foreach (var note in source.Notes)
                {
                    destination.Notes.Add(note);
                }
            });
    }
}