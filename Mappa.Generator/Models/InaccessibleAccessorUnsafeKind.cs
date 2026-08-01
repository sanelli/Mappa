// <copyright file="InaccessibleAccessorUnsafeKind.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// The <c>UnsafeAccessorKind</c> value to emit for an inaccessible accessor.
/// </summary>
internal enum InaccessibleAccessorUnsafeKind
{
    /// <summary>
    /// <c>UnsafeAccessorKind.Method</c> (property getters and setters).
    /// </summary>
    Method,

    /// <summary>
    /// <c>UnsafeAccessorKind.Constructor</c>.
    /// </summary>
    Constructor,
}