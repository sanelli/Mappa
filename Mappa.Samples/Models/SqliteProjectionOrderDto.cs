// <copyright file="SqliteProjectionOrderDto.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1002, CA2227

namespace Mappa.Samples.Models;

/// <summary>
/// Projection DTO for the EF Core Sqlite projection sample.
/// </summary>
public sealed class SqliteProjectionOrderDto
{
    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display title (mapped from <see cref="SqliteProjectionOrder.Name"/>).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the nested children.
    /// </summary>
    public List<SqliteProjectionChildDto> Children { get; set; } = [];
}