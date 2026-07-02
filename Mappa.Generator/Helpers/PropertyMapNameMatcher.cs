// <copyright file="PropertyMapNameMatcher.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Matches source property names to target property or constructor parameter names.
/// </summary>
internal static class PropertyMapNameMatcher
{
    /// <summary>
    /// Tries to find a single source property matching the expected name.
    /// </summary>
    /// <param name="sourceProperties">The source properties.</param>
    /// <param name="expectedName">The expected source property name.</param>
    /// <param name="caseInsensitivePropertyMap">The case-insensitive matching setting.</param>
    /// <param name="ignoreUnderscoreForPropertyMap">The ignore-underscore matching setting.</param>
    /// <param name="isConstructorParameterPath">Whether the match is for a constructor parameter.</param>
    /// <param name="useExactNameFromAttribute">Whether an explicit source property name was provided.</param>
    /// <param name="match">The matched source property, if any.</param>
    /// <returns><c>true</c> when exactly one source property matches; otherwise <c>false</c>.</returns>
    internal static bool TryFindSourceProperty(
        IReadOnlyList<IPropertySymbol> sourceProperties,
        string expectedName,
        BooleanSetting caseInsensitivePropertyMap,
        BooleanSetting ignoreUnderscoreForPropertyMap,
        bool isConstructorParameterPath,
        bool useExactNameFromAttribute,
        out IPropertySymbol? match)
    {
        match = null;
        IPropertySymbol? candidate = null;
        var matchCount = 0;

        foreach (var property in sourceProperties)
        {
            if (!NamesMatch(
                    expectedName,
                    property.Name,
                    caseInsensitivePropertyMap,
                    ignoreUnderscoreForPropertyMap,
                    isConstructorParameterPath,
                    useExactNameFromAttribute))
            {
                continue;
            }

            candidate = property;
            matchCount++;

            if (matchCount > 1)
            {
                match = null;
                return false;
            }
        }

        match = candidate;
        return matchCount == 1;
    }

    private static bool NamesMatch(
        string expectedName,
        string actualName,
        BooleanSetting caseInsensitivePropertyMap,
        BooleanSetting ignoreUnderscoreForPropertyMap,
        bool isConstructorParameterPath,
        bool useExactNameFromAttribute)
    {
        if (useExactNameFromAttribute)
        {
            return expectedName.Equals(actualName, StringComparison.Ordinal);
        }

        var normalizedExpected = Normalize(expectedName, ignoreUnderscoreForPropertyMap);
        var normalizedActual = Normalize(actualName, ignoreUnderscoreForPropertyMap);
        var comparison = GetComparison(caseInsensitivePropertyMap, isConstructorParameterPath);
        return normalizedExpected.Equals(normalizedActual, comparison);
    }

    private static string Normalize(string name, BooleanSetting ignoreUnderscoreForPropertyMap)
        => ignoreUnderscoreForPropertyMap is BooleanSetting.Enable
            ? name.Replace("_", string.Empty)
            : name;

    private static StringComparison GetComparison(BooleanSetting caseInsensitivePropertyMap, bool isConstructorParameterPath)
        => caseInsensitivePropertyMap is BooleanSetting.Enable || isConstructorParameterPath
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
}