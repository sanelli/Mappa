// <copyright file="CompilationExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="Compilation"/>.
/// </summary>
internal static class CompilationExtensions
{
    /// <summary>
    /// Obtain the <see cref="INamedTypeSymbol"/> for type <paramref name="type"/>.
    /// </summary>
    /// <param name="compilation">The current compilation.</param>
    /// <param name="type">The type.</param>
    /// <returns>The <see cref="INamedTypeSymbol"/> for type <paramref name="type"/>.</returns>
    /// <exception cref="MappaGeneratorException">When <paramref name="type"/> cannot be loaded.</exception>
    internal static INamedTypeSymbol GetTypeSymbol(this Compilation compilation, Type type)
    {
        var namedTypeSymbol = compilation.GetTypeByMetadataName(type.FullName);
        return namedTypeSymbol ?? throw new MappaGeneratorException($"Cannot obtain named type symbol for '{type.FullName}'.");
    }

    /// <summary>
    /// Obtain the <see cref="INamedTypeSymbol"/> for type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">The type to be obtained.</typeparam>
    /// <param name="compilation">The current compilation.</param>
    /// <returns>The <see cref="INamedTypeSymbol"/> for type <typeparamref name="TType"/>.</returns>
    /// <exception cref="MappaGeneratorException">When <typeparamref name="TType"/> cannot be loaded.</exception>
    internal static INamedTypeSymbol GetTypeSymbol<TType>(this Compilation compilation)
        => compilation.GetTypeSymbol(typeof(TType));
}