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
    /// Projects orders to DTOs.
    /// </summary>
    /// <param name="query">The source queryable.</param>
    /// <returns>The projected queryable.</returns>
    [MappaUseProperty(nameof(ProjectionOrderDto.Title), nameof(ProjectionOrder.Name))]
    public partial IQueryable<ProjectionOrderDto> Project(IQueryable<ProjectionOrder> query);

    /// <summary>
    /// Companion element map for projection.
    /// </summary>
    /// <param name="order">The source order.</param>
    /// <returns>The mapped DTO.</returns>
    [MappaUseProperty(nameof(ProjectionOrderDto.Title), nameof(ProjectionOrder.Name))]
    private partial ProjectionOrderDto MapOrder(ProjectionOrder order);
}