// <copyright file="ProjectionModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1002, CA1515, CA1724, CA1815, CA1819, CA2227, SA1201, SA1402, SA1649

namespace Mappa.Benchmark.Queryable.Models;

/// <summary>
/// Source entity for IQueryable projection benchmarks.
/// </summary>
public sealed class ProjectionOrder
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;
}

/// <summary>
/// Target DTO for IQueryable projection benchmarks.
/// </summary>
public sealed class ProjectionOrderDto
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;
}