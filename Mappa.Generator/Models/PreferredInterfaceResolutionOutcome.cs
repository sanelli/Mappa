// <copyright file="PreferredInterfaceResolutionOutcome.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Result of a preferred-interface resolution attempt.
/// </summary>
internal enum PreferredInterfaceResolutionOutcome
{
    /// <summary>
    /// A preferred closed interface was selected.
    /// </summary>
    Resolved,

    /// <summary>
    /// A matching attribute exists but the preferred interface cannot be used.
    /// </summary>
    AttributeCannotBeUsed,

    /// <summary>
    /// No preferred interface could be selected (silent fall-through).
    /// </summary>
    NotResolved,

    /// <summary>
    /// Resolution was skipped due to a recursion guard.
    /// </summary>
    RecursionGuard,
}