// <copyright file="IQueryableProjectionMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <c>IQueryable&lt;TSource&gt;</c> → <c>IQueryable&lt;TTarget&gt;</c> projection.
/// </summary>
/// <remarks>
/// Projection methods emit <c>Select</c> expression trees suitable for ORM providers such as EF Core.
/// Limitations for projection methods:
/// <list type="bullet">
/// <item><description>No <see cref="MappaBeforeMapAttribute"/> / <see cref="MappaAfterMapAttribute"/> hooks.</description></item>
/// <item><description>No <c>MappaContext</c> parameter.</description></item>
/// <item><description>No nested <c>IQueryable</c> properties inside the element map (nested <c>List&lt;T&gt;</c> / arrays are supported).</description></item>
/// <item><description>No polymorphic root element mapping.</description></item>
/// <item><description>Prefer numeric or description enum mappings over case-insensitive member-name matching.</description></item>
/// </list>
/// Mapping attributes such as <see cref="MappaUsePropertyAttribute"/> can be declared on the projection method
/// and on a private companion element map (Mapperly-style). Companion maps are inlined structurally into the
/// projection expression and may report warning <c>MP00086</c>.
/// </remarks>
[Mappa]
[MappaProjection]
public static partial class IQueryableProjectionMapper
{
    /// <summary>
    /// Projects <see cref="ProjectionOrder"/> queryables to <see cref="ProjectionOrderDto"/>.
    /// </summary>
    /// <param name="query">The source queryable.</param>
    /// <returns>The projected queryable.</returns>
#pragma warning disable MP00086 // Companion MapOrder is inlined into the Select expression by design.
    [MappaUseProperty(nameof(ProjectionOrderDto.Title), nameof(ProjectionOrder.Name))]
    public static partial IQueryable<ProjectionOrderDto> ProjectToDto(this IQueryable<ProjectionOrder> query);
#pragma warning restore MP00086

    /// <summary>
    /// Maps a single <see cref="ProjectionOrder"/> to <see cref="ProjectionOrderDto"/> (Mapperly-style companion element map).
    /// </summary>
    /// <param name="order">The source order.</param>
    /// <returns>The mapped DTO.</returns>
    [MappaUseProperty(nameof(ProjectionOrderDto.Title), nameof(ProjectionOrder.Name))]
    private static partial ProjectionOrderDto MapOrder(ProjectionOrder order);
}