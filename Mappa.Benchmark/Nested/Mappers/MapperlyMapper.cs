// <copyright file="MapperlyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.Nested.Models;

using Riok.Mapperly.Abstractions;

namespace Mappa.Benchmark.Nested.Mappers;

/// <summary>
/// Mapperly mapper for nested DTO benchmarks.
/// </summary>
[Mapper]
internal sealed partial class MapperlyMapper
{
    /// <summary>
    /// Maps an order DTO.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial NestedOrder Map(NestedOrderDto source);

    /// <summary>
    /// Maps a polymorphic party DTO.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    [MapDerivedType(typeof(PersonPartyDto), typeof(PersonParty))]
    [MapDerivedType(typeof(OrganizationPartyDto), typeof(OrganizationParty))]
    private partial Party MapParty(PartyDto source);

    /// <summary>
    /// Maps a polymorphic line-item DTO.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    [MapDerivedType(typeof(PhysicalLineItemDto), typeof(PhysicalLineItem))]
    [MapDerivedType(typeof(DigitalLineItemDto), typeof(DigitalLineItem))]
    private partial LineItemBase MapLineItem(LineItemBaseDto source);
}