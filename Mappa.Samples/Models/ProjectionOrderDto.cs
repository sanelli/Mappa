// <copyright file="ProjectionOrderDto.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1002, CA2227

namespace Mappa.Samples.Models;

/// <summary>
/// Projection DTO used by <see cref="IQueryableProjectionMapper"/>.
/// </summary>
public sealed class ProjectionOrderDto
{
    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display title (mapped from <see cref="ProjectionOrder.Name"/>).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the public identifier.
    /// </summary>
    public Guid PublicId { get; set; }

    /// <summary>
    /// Gets or sets the summary tuple with a string count.
    /// </summary>
    public (string Count, string Label) Summary { get; set; }

    /// <summary>
    /// Gets or sets the nested children.
    /// </summary>
    public List<ProjectionChildDto> Children { get; set; } = [];
}