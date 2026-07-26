// <copyright file="ProjectionOrder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source entity used by <see cref="IQueryableProjectionMapper"/>.
/// </summary>
public sealed class ProjectionOrder
{
    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the order name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;
}