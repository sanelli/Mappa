// <copyright file="MapperlyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.Queryable.Models;

using Riok.Mapperly.Abstractions;

namespace Mappa.Benchmark.Queryable.Mappers;

/// <summary>
/// Mapperly mapper for IQueryable projection benchmarks.
/// </summary>
[Mapper]
internal sealed partial class MapperlyMapper
{
    /// <summary>
    /// Projects orders to DTOs.
    /// </summary>
    /// <param name="query">The source queryable.</param>
    /// <returns>The projected queryable.</returns>
    public partial IQueryable<ProjectionOrderDto> Project(IQueryable<ProjectionOrder> query);

    /// <summary>
    /// Companion element map for projection.
    /// </summary>
    /// <param name="order">The source order.</param>
    /// <returns>The mapped DTO.</returns>
    [MapProperty(nameof(ProjectionOrder.Name), nameof(ProjectionOrderDto.Title))]
    private partial ProjectionOrderDto MapOrder(ProjectionOrder order);
}