// <copyright file="SymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="ISymbol"/>.
/// </summary>
internal static class SymbolExtensions
{
    /// <summary>
    /// Return the string representation of the class modifiers (public/private/..., abstract, sealed, static).
    /// </summary>
    /// <param name="symbol">The type to be investigated.</param>
    /// <returns>The class modifiers.</returns>
    internal static string GetSymbolModifiers(this ISymbol symbol)
    {
        var keywords = new List<string> { symbol.GetSymbolAccessibility() };
        if (symbol.IsAbstract)
        {
            keywords.Add("abstract");
        }

        if (symbol.IsSealed)
        {
            keywords.Add("sealed");
        }

        if (symbol.IsStatic)
        {
            keywords.Add("static");
        }

        if (symbol.IsVirtual)
        {
            keywords.Add("virtual");
        }

        if (symbol.IsOverride)
        {
            keywords.Add("override");
        }

        return string.Join(" ", keywords);
    }

    /// <summary>
    /// Get the accessibility representation of the class.
    /// </summary>
    /// <param name="symbol">The symbol.</param>
    /// <returns>The accessibility representation.</returns>
    private static string GetSymbolAccessibility(this ISymbol symbol)
        => symbol.DeclaredAccessibility.GetAccessibilityAsCode();
}