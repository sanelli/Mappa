// <copyright file="MappaMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Benchmark.Queryable.Models;

namespace Mappa.Benchmark.Queryable.Mappers;

/// <summary>
/// Mappa mapper for IQueryable projection benchmarks.
/// </summary>
[Mappa]
internal sealed partial class MappaMapper
{
    /// <summary>
    /// Projects orders to DTOs (including nested children).
    /// </summary>
    /// <param name="query">The source queryable.</param>
    /// <returns>The projected queryable.</returns>
#pragma warning disable MP00086 // Companion MapOrder is inlined into the Select expression by design.
    [MappaUseProperty(nameof(ProjectionOrderDto.Title), nameof(ProjectionOrder.Name))]
    public partial IQueryable<ProjectionOrderDto> Project(IQueryable<ProjectionOrder> query);
#pragma warning restore MP00086

    /// <summary>
    /// Companion element map for projection.
    /// </summary>
    /// <param name="order">The source order.</param>
    /// <returns>The mapped DTO.</returns>
    [MappaUseProperty(nameof(ProjectionOrderDto.Title), nameof(ProjectionOrder.Name))]
    private partial ProjectionOrderDto MapOrder(ProjectionOrder order);
}