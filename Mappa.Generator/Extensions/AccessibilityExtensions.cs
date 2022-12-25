// <copyright file="AccessibilityExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="Accessibility"/>.
/// </summary>
internal static class AccessibilityExtensions
{
    /// <summary>
    /// Get the accessibility cose representation.
    /// </summary>
    /// <param name="accessibility">The accessibility flag.</param>
    /// <returns>The accessibility representation.</returns>
    internal static string GetAccessibilityAsCode(this Accessibility accessibility)
        => accessibility switch
        {
            Accessibility.Internal => "internal",
            Accessibility.Private => "private",
            Accessibility.Protected => "protected",
            Accessibility.ProtectedAndInternal => "protected internal",
            Accessibility.Public => "public",
            _ => string.Empty,
        };
}