// <copyright file="IQueryableProjectionSqliteMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating nested-collection <c>IQueryable</c> projection against EF Core Sqlite.
/// </summary>
/// <remarks>
/// Use with a relational provider (see sample tests). Generated projection methods are annotated
/// with <c>[RequiresDynamicCode]</c> and are not exercised under Native AOT.
/// </remarks>
[Mappa]
[MappaProjection]
public static partial class IQueryableProjectionSqliteMapper
{
    /// <summary>
    /// Projects <see cref="SqliteProjectionOrder"/> queryables to <see cref="SqliteProjectionOrderDto"/>.
    /// </summary>
    /// <param name="query">The source queryable.</param>
    /// <returns>The projected queryable.</returns>
    [MappaUseProperty(nameof(SqliteProjectionOrderDto.Title), nameof(SqliteProjectionOrder.Name))]
    public static partial IQueryable<SqliteProjectionOrderDto> ProjectToDto(this IQueryable<SqliteProjectionOrder> query);
}