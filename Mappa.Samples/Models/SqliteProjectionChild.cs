// <copyright file="SqliteProjectionChild.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Nested child entity for the EF Core Sqlite projection sample.
/// </summary>
public sealed class SqliteProjectionChild
{
    /// <summary>
    /// Gets or sets the child identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the parent order identifier.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the child label.
    /// </summary>
    public string Label { get; set; } = string.Empty;
}