// <copyright file="ProjectionChildDto.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Nested child DTO used by <see cref="IQueryableProjectionMapper"/>.
/// </summary>
public sealed class ProjectionChildDto
{
    /// <summary>
    /// Gets or sets the child identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the child label.
    /// </summary>
    public string Label { get; set; } = string.Empty;
}