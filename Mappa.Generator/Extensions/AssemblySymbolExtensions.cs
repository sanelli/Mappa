// <copyright file="AssemblySymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="IAssemblySymbol"/>.
/// </summary>
internal static class AssemblySymbolExtensions
{
    /// <summary>
    /// Recursively collects all named types declared in <paramref name="assembly"/>.
    /// </summary>
    /// <param name="assembly">The assembly to walk.</param>
    /// <returns>All named types in declaration order within each namespace, then sorted by display string.</returns>
    internal static ImmutableArray<INamedTypeSymbol> GetAllNamedTypes(this IAssemblySymbol assembly)
    {
        var types = new List<INamedTypeSymbol>();
        CollectNamedTypes(assembly.GlobalNamespace, types);
        return [.. types.OrderBy(type => type.ToDisplayString(), StringComparer.Ordinal)];
    }

    private static void CollectNamedTypes(INamespaceSymbol namespaceSymbol, List<INamedTypeSymbol> types)
    {
        foreach (var type in namespaceSymbol.GetTypeMembers())
        {
            CollectNamedTypes(type, types);
        }

        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            CollectNamedTypes(childNamespace, types);
        }
    }

    private static void CollectNamedTypes(INamedTypeSymbol typeSymbol, List<INamedTypeSymbol> types)
    {
        types.Add(typeSymbol);
        foreach (var nestedType in typeSymbol.GetTypeMembers())
        {
            CollectNamedTypes(nestedType, types);
        }
    }
}