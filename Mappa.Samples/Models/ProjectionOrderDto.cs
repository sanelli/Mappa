// <copyright file="ProjectionOrderDto.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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
}