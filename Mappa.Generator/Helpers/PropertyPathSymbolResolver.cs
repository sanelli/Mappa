// <copyright file="PropertyPathSymbolResolver.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Resolves dot-separated property paths against type symbols.
/// </summary>
internal static class PropertyPathSymbolResolver
{
    /// <summary>
    /// Tries to resolve a property path on a type.
    /// </summary>
    /// <param name="typeSymbol">The starting type.</param>
    /// <param name="path">The property path.</param>
    /// <param name="resolvedProperties">The resolved properties in path order.</param>
    /// <param name="missingSegment">The first segment that could not be resolved, if any.</param>
    /// <returns><c>true</c> when the full path resolves; otherwise, <c>false</c>.</returns>
    internal static bool TryResolvePropertyPath(
        ITypeSymbol typeSymbol,
        PropertyPath path,
        out IPropertySymbol[] resolvedProperties,
        out string? missingSegment)
    {
        List<IPropertySymbol> properties = new();
        var currentType = typeSymbol;
        foreach (var segment in path.Segments)
        {
            var property = currentType
                .GetTypeProperties()
                .FirstOrDefault(candidate => candidate.Name.Equals(segment, StringComparison.Ordinal));

            if (property is null)
            {
                resolvedProperties = [];
                missingSegment = segment;
                return false;
            }

            properties.Add(property);
            currentType = property.Type;
        }

        resolvedProperties = [.. properties];
        missingSegment = null;
        return true;
    }

    /// <summary>
    /// Tries to resolve a property path on a type, including fields for assign-to-context targets.
    /// </summary>
    /// <param name="typeSymbol">The starting type.</param>
    /// <param name="path">The property path.</param>
    /// <param name="memberAccessExpression">The member access expression for the resolved path.</param>
    /// <param name="missingSegment">The first segment that could not be resolved, if any.</param>
    /// <returns><c>true</c> when the full path resolves; otherwise, <c>false</c>.</returns>
    internal static bool TryResolveTargetMemberPath(
        ITypeSymbol typeSymbol,
        PropertyPath path,
        out string memberAccessExpression,
        out string? missingSegment)
    {
        if (path.Segments.Length == 0)
        {
            memberAccessExpression = string.Empty;
            missingSegment = string.Empty;
            return false;
        }

        if (path.Segments.Length == 1)
        {
            memberAccessExpression = path.Segments[0];
            missingSegment = null;
            return true;
        }

        var currentType = typeSymbol;
        List<string> segments = new();
        foreach (var segment in path.Segments)
        {
            var property = currentType
                .GetTypeProperties()
                .FirstOrDefault(candidate => candidate.Name.Equals(segment, StringComparison.Ordinal));

            if (property is null)
            {
                memberAccessExpression = string.Empty;
                missingSegment = segment;
                return false;
            }

            segments.Add(segment);
            currentType = property.Type;
        }

        memberAccessExpression = string.Join(".", segments);
        missingSegment = null;
        return true;
    }
}