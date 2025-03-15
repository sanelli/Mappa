// <copyright file="PropertySymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods over <see cref="IPropertySymbol"/>.
/// </summary>
internal static class PropertySymbolExtensions
{
    /// <summary>
    /// Check if a property setter is accessible from inside a method.
    /// </summary>
    /// <param name="property">The property which setter is to investigate.</param>
    /// <param name="method">The method that will access the property.</param>
    /// <returns><c>true</c> if the setter method of property <paramref name="property"/> exists and is accessible from <paramref name="method"/>, <c>false</c> otherwise.</returns>
    internal static bool IsSetterAccessible(
        this IPropertySymbol property,
        IMethodSymbol method)
    {
        var setMethod = property.SetMethod;
        if (setMethod is null)
        {
            return false;
        }

        return setMethod.DeclaredAccessibility switch
        {
            Accessibility.Public => true,
            Accessibility.Internal => setMethod.ContainingAssembly.Equals(method.ContainingAssembly, SymbolEqualityComparer.Default),
            _ => false,
        };
    }

    /// <summary>
    /// Check if a property setter is accessible from inside a method.
    /// </summary>
    /// <param name="property">The property which setter is to investigate.</param>
    /// <param name="method">The method that will access the property.</param>
    /// <returns><c>true</c> if the setter method of property <paramref name="property"/> exists and is accessible from <paramref name="method"/>, <c>false</c> otherwise.</returns>
    internal static bool IsSetterAccessible(
        this IPropertySymbol property,
        MapMethod method)
         => property.IsSetterAccessible(method.MethodSymbol);

    /// <summary>
    /// Check if a property getter is accessible from inside a method.
    /// </summary>
    /// <param name="property">The property which getter is to investigate.</param>
    /// <param name="method">The method that will access the property.</param>
    /// <returns><c>true</c> if the getter method of property <paramref name="property"/> exists and is accessible from <paramref name="method"/>, <c>false</c> otherwise.</returns>
    internal static bool IsGetterAccessible(
        this IPropertySymbol property,
        IMethodSymbol method)
    {
        var getMethod = property.GetMethod;
        if (getMethod is null)
        {
            return false;
        }

        return getMethod.DeclaredAccessibility switch
        {
            Accessibility.Public => true,
            Accessibility.Internal => getMethod.ContainingAssembly.Equals(method.ContainingAssembly, SymbolEqualityComparer.Default),
            _ => false,
        };
    }

    /// <summary>
    /// Check if a property getter is accessible from inside a method.
    /// </summary>
    /// <param name="property">The property which getter is to investigate.</param>
    /// <param name="method">The method that will access the property.</param>
    /// <returns><c>true</c> if the getter method of property <paramref name="property"/> exists and is accessible from <paramref name="method"/>, <c>false</c> otherwise.</returns>
    internal static bool IsGetterAccessible(
        this IPropertySymbol property,
        MapMethod method)
        => property.IsGetterAccessible(method.MethodSymbol);
}