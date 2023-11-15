// <copyright file="AssertionsHelpers.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Tests.Assertions;

/// <summary>
/// Assertions helpers.
/// </summary>
internal static class AssertionsHelpers
{
    /// <summary>
    /// Extract a symbol from the type name.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="type">The name of the type.</param>
    /// <returns>The symbol.</returns>
    internal static ITypeSymbol GetTypeSymbol(Compilation compilation, string type)
    {
        ArgumentNullException.ThrowIfNull(compilation);
        ArgumentException.ThrowIfNullOrWhiteSpace(type);

        var typeParts = type.Split("[");
        var namedTypeSymbol = compilation.GetTypeByMetadataName(typeParts[0])!;
        if (typeParts.Length > 1)
        {
            var typeArguments = typeParts[^1]
                .Replace("]", string.Empty, StringComparison.Ordinal)
                .Split(",")
                .Select(compilation.GetTypeByMetadataName)
                .Where(t => t is not null)
                .OfType<ITypeSymbol>()
                .ToArray();
            var constructedGenericType = namedTypeSymbol.Construct(typeArguments);
            return constructedGenericType;
        }

        return namedTypeSymbol;
    }
}