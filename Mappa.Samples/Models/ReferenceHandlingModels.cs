// <copyright file="ReferenceHandlingModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

namespace Mappa.Samples.Models;

/// <summary>
/// Source node A in a nullable A↔B cycle used by reference-reusing samples.
/// </summary>
public sealed class ReferenceHandlingPersonSource
{
    /// <summary>
    /// Gets or sets the person id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the linked address (nullable cycle edge).
    /// </summary>
    public ReferenceHandlingAddressSource? Address { get; set; }
}

/// <summary>
/// Target node A in a nullable A↔B cycle used by reference-reusing samples.
/// </summary>
public sealed class ReferenceHandlingPersonTarget
{
    /// <summary>
    /// Gets or sets the person id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the linked address (nullable cycle edge).
    /// </summary>
    public ReferenceHandlingAddressTarget? Address { get; set; }
}

/// <summary>
/// Source node B in a nullable A↔B cycle used by reference-reusing samples.
/// </summary>
public sealed class ReferenceHandlingAddressSource
{
    /// <summary>
    /// Gets or sets the address id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the owning person (nullable cycle edge).
    /// </summary>
    public ReferenceHandlingPersonSource? Owner { get; set; }
}

/// <summary>
/// Target node B in a nullable A↔B cycle used by reference-reusing samples.
/// </summary>
public sealed class ReferenceHandlingAddressTarget
{
    /// <summary>
    /// Gets or sets the address id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the owning person (nullable cycle edge).
    /// </summary>
    public ReferenceHandlingPersonTarget? Owner { get; set; }
}

/// <summary>
/// Leaf source for the <c>MaxRuntimeDepth</c> sample.
/// </summary>
public sealed class ReferenceHandlingLevel2Source
{
    /// <summary>
    /// Gets or sets the leaf value.
    /// </summary>
    public int Value { get; set; }
}

/// <summary>
/// Leaf target for the <c>MaxRuntimeDepth</c> sample.
/// </summary>
public sealed class ReferenceHandlingLevel2Target
{
    /// <summary>
    /// Gets or sets the leaf value.
    /// </summary>
    public int Value { get; set; }
}

/// <summary>
/// Mid-level source for the <c>MaxRuntimeDepth</c> sample.
/// </summary>
public sealed class ReferenceHandlingLevel1Source
{
    /// <summary>
    /// Gets or sets the nested child.
    /// </summary>
    public ReferenceHandlingLevel2Source Child { get; set; } = null!;
}

/// <summary>
/// Mid-level target for the <c>MaxRuntimeDepth</c> sample.
/// </summary>
public sealed class ReferenceHandlingLevel1Target
{
    /// <summary>
    /// Gets or sets the nested child.
    /// </summary>
    public ReferenceHandlingLevel2Target Child { get; set; } = null!;
}

/// <summary>
/// Root source for the <c>MaxRuntimeDepth</c> sample.
/// </summary>
public sealed class ReferenceHandlingLevel0Source
{
    /// <summary>
    /// Gets or sets the nested child.
    /// </summary>
    public ReferenceHandlingLevel1Source Child { get; set; } = null!;
}

/// <summary>
/// Root target for the <c>MaxRuntimeDepth</c> sample.
/// </summary>
public sealed class ReferenceHandlingLevel0Target
{
    /// <summary>
    /// Gets or sets the nested child.
    /// </summary>
    public ReferenceHandlingLevel1Target Child { get; set; } = null!;
}