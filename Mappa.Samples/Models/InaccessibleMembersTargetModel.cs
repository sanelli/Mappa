// <copyright file="InaccessibleMembersTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model demonstrating inaccessible constructors and setters.
/// </summary>
#pragma warning disable S3453 // Private constructor is intentional for the inaccessible-members sample
#pragma warning disable S1144 // Private setters are written via UnsafeAccessor by the mapper
public sealed class InaccessibleMembersTargetModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InaccessibleMembersTargetModel"/> class.
    /// </summary>
    private InaccessibleMembersTargetModel()
    {
    }

    /// <summary>
    /// Gets the name (private setter; written via UnsafeAccessor when opted in).
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the age (private setter; written via UnsafeAccessor when opted in).
    /// </summary>
    public int Age { get; private set; }
}
#pragma warning restore S1144
#pragma warning restore S3453