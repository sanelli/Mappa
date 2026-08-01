// <copyright file="MappaMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Benchmark.Nested.Models;

namespace Mappa.Benchmark.Nested.Mappers;

/// <summary>
/// Mappa mapper for nested DTO benchmarks.
/// </summary>
[Mappa]
internal sealed partial class MappaMapper
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
    [MappaTypeMapping(typeof(PersonParty), typeof(PersonPartyDto))]
    [MappaTypeMapping(typeof(OrganizationParty), typeof(OrganizationPartyDto))]
    public partial Party MapParty(PartyDto source);

    /// <summary>
    /// Maps a polymorphic line-item DTO.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    [MappaTypeMapping(typeof(PhysicalLineItem), typeof(PhysicalLineItemDto))]
    [MappaTypeMapping(typeof(DigitalLineItem), typeof(DigitalLineItemDto))]
    public partial LineItemBase MapLineItem(LineItemBaseDto source);
}