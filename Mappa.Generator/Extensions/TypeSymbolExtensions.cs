// <copyright file="TypeSymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="ITypeSymbol"/>.
/// </summary>
internal static class TypeSymbolExtensions
{
    /// <summary>
    /// Check if the type is <see cref="Void"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <c>void</c>.</returns>
    internal static bool IsVoid(this ITypeSymbol typeSymbol)
         => typeSymbol.SpecialType == SpecialType.System_Void;

    /// <summary>
    /// Check if the type is <see cref="object"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <c>void</c>.</returns>
    internal static bool IsObject(this ITypeSymbol typeSymbol)
         => typeSymbol.SpecialType == SpecialType.System_Object;

    /// <summary>
    /// Check if the type is <c>void</c>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation used to obtain the required types.</param>
    /// <returns><c>true</c> if the type symbol is <c>void</c>.</returns>
    internal static bool IsAnyTaskType(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var task = compilation.GetTypeSymbol<Task>();
        var taskGeneric = compilation.GetTypeSymbol(typeof(Task<>));
        var valueTask = compilation.GetTypeSymbol<ValueTask>();
        var valueTaskGeneric = compilation.GetTypeSymbol(typeof(ValueTask<>));

        if (!typeSymbol.IsDefinition)
        {
            typeSymbol = typeSymbol.OriginalDefinition;
        }

        return SymbolEqualityComparer.Default.Equals(typeSymbol, task)
            || SymbolEqualityComparer.Default.Equals(typeSymbol, taskGeneric)
            || SymbolEqualityComparer.Default.Equals(typeSymbol, valueTask)
            || SymbolEqualityComparer.Default.Equals(typeSymbol, valueTaskGeneric);
    }

    /// <summary>
    /// Get the accessibility representation of the class.
    /// </summary>
    /// <param name="namedTypeSymbol">The name types symbol.</param>
    /// <returns>The accessibility representation.</returns>
    internal static string GetClassAccessibility(this ITypeSymbol namedTypeSymbol)
        => namedTypeSymbol.DeclaredAccessibility.GetAccessibilityAsCode();

    /// <summary>
    /// Return the string representation of the class modifiers (public/private/..., abstract, sealed, static).
    /// </summary>
    /// <param name="namedTypeSymbol">The type to be investigated.</param>
    /// <returns>The class modifiers.</returns>
    internal static string GetClassModifiers(this ITypeSymbol namedTypeSymbol)
    {
        var keywords = new List<string> { namedTypeSymbol.GetClassAccessibility() };
        if (namedTypeSymbol.IsAbstract)
        {
            keywords.Add("abstract");
        }

        if (namedTypeSymbol.IsSealed)
        {
            keywords.Add("sealed");
        }

        if (namedTypeSymbol.IsStatic)
        {
            keywords.Add("static");
        }

        return string.Join(" ", keywords);
    }
}