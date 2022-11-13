// <copyright file="MethodSymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="IMethodSymbol"/>.
/// </summary>
internal static class MethodSymbolExtensions
{
    /// <summary>
    /// Returns <c>true</c> if the method returns <c>void</c>.
    /// </summary>
    /// <param name="methodSymbol">The method to validate.</param>
    /// <returns><c>true</c> if the method returns <c>void</c>.</returns>
    internal static bool IsVoid(this IMethodSymbol methodSymbol)
        => methodSymbol.ReturnType.IsVoid();
}