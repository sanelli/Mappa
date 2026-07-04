// <copyright file="IdentityMapDeepCopyChild.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Nested reference type used by the identity map deep copy sample mappers.
/// </summary>
public sealed class IdentityMapDeepCopyChild
{
#pragma warning disable CA1051 // Do not declare visible instance fields
#pragma warning disable SA1401 // Field should be private
#pragma warning disable S1104 // Fields should not be public
    /// <summary>
    /// The child name.
    /// </summary>
    public string Name = string.Empty;
#pragma warning restore S1104
#pragma warning restore SA1401
#pragma warning restore CA1051

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityMapDeepCopyChild"/> class.
    /// </summary>
    public IdentityMapDeepCopyChild()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityMapDeepCopyChild"/> class
    /// as a copy of another instance.
    /// </summary>
    /// <param name="other">The instance to copy.</param>
    public IdentityMapDeepCopyChild(IdentityMapDeepCopyChild other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this.Name = other.Name;
    }
}