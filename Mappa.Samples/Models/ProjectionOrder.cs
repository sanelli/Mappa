// <copyright file="ProjectionOrder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1002, CA2227

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

    /// <summary>
    /// Gets or sets the public identifier as a string (projected to <see cref="Guid"/>).
    /// </summary>
    public string PublicId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a summary tuple (count + label).
    /// </summary>
    public (int Count, string Label) Summary { get; set; }

    /// <summary>
    /// Gets or sets the nested children.
    /// </summary>
    public List<ProjectionChild> Children { get; set; } = [];
}