// <copyright file="CollectionModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1002, CA1515, CA1724, CA1815, CA1819, CA2227, SA1201, SA1402, SA1649

namespace Mappa.Benchmark.Collections.Models;

/// <summary>
/// Collection element DTO that includes a nested dictionary.
/// </summary>
public sealed class CollectionItemDto
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
    /// Gets or sets per-item attributes.
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = new();
}

/// <summary>
/// Collection element target that includes a nested dictionary.
/// </summary>
public sealed class CollectionItem
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
    /// Gets or sets per-item attributes.
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = new();
}