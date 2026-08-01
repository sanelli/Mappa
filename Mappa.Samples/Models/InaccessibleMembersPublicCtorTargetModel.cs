// <copyright file="InaccessibleMembersPublicCtorTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model with a public constructor and private property setters.
/// </summary>
#pragma warning disable S1144 // Private setters are written via UnsafeAccessor by the mapper
public sealed class InaccessibleMembersPublicCtorTargetModel
{
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