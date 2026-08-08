// <copyright file="ReferenceHandlingModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1515, SA1402, SA1649

namespace Mappa.Benchmark.ReferenceHandling.Models;

/// <summary>
/// Source person in a nullable A↔B cycle.
/// </summary>
public sealed class PersonSource
{
    /// <summary>
    /// Gets or sets the person id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the linked address (nullable cycle edge).
    /// </summary>
    public AddressSource? Address { get; set; }
}

/// <summary>
/// Target person in a nullable A↔B cycle.
/// </summary>
public sealed class PersonTarget
{
    /// <summary>
    /// Gets or sets the person id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the linked address (nullable cycle edge).
    /// </summary>
    public AddressTarget? Address { get; set; }
}

/// <summary>
/// Source address in a nullable A↔B cycle.
/// </summary>
public sealed class AddressSource
{
    /// <summary>
    /// Gets or sets the address id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the owning person (nullable cycle edge).
    /// </summary>
    public PersonSource? Owner { get; set; }
}

/// <summary>
/// Target address in a nullable A↔B cycle.
/// </summary>
public sealed class AddressTarget
{
    /// <summary>
    /// Gets or sets the address id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the owning person (nullable cycle edge).
    /// </summary>
    public PersonTarget? Owner { get; set; }
}

/// <summary>
/// Source node shared by multiple parents in a DAG.
/// </summary>
public sealed class NodeSource
{
    /// <summary>
    /// Gets or sets the node id.
    /// </summary>
    public int Id { get; set; }
}

/// <summary>
/// Target node shared by multiple parents in a DAG.
/// </summary>
public sealed class NodeTarget
{
    /// <summary>
    /// Gets or sets the node id.
    /// </summary>
    public int Id { get; set; }
}

/// <summary>
/// Source root with two edges that may point at the same node instance.
/// </summary>
public sealed class RootSource
{
    /// <summary>
    /// Gets or sets the left child.
    /// </summary>
    public NodeSource Left { get; set; } = null!;

    /// <summary>
    /// Gets or sets the right child.
    /// </summary>
    public NodeSource Right { get; set; } = null!;
}

/// <summary>
/// Target root with two edges that may point at the same node instance.
/// </summary>
public sealed class RootTarget
{
    /// <summary>
    /// Gets or sets the left child.
    /// </summary>
    public NodeTarget Left { get; set; } = null!;

    /// <summary>
    /// Gets or sets the right child.
    /// </summary>
    public NodeTarget Right { get; set; } = null!;
}