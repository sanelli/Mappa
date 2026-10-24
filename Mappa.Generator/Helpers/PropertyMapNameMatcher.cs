// <copyright file="PropertyMapNameMatcher.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Matches source property and field names to target property, field, or constructor parameter names.
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
        if (!TryFindSourceMember(
                sourceProperties,
                sourceFields: [],
                expectedName,
                caseInsensitivePropertyMap,
                ignoreUnderscoreForPropertyMap,
                isConstructorParameterPath,
                useExactNameFromAttribute,
                out var member)
            || member?.Property is not { } property)
        {
            return false;
        }

        match = property;
        return true;
    }

    /// <summary>
    /// Tries to find a single source member matching the expected name.
    /// When both a property and a field match, the property is preferred.
    /// </summary>
    /// <param name="sourceProperties">The source properties.</param>
    /// <param name="sourceFields">The source fields.</param>
    /// <param name="expectedName">The expected source member name.</param>
    /// <param name="caseInsensitivePropertyMap">The case-insensitive matching setting.</param>
    /// <param name="ignoreUnderscoreForPropertyMap">The ignore-underscore matching setting.</param>
    /// <param name="isConstructorParameterPath">Whether the match is for a constructor parameter.</param>
    /// <param name="useExactNameFromAttribute">Whether an explicit source member name was provided.</param>
    /// <param name="match">The matched source member, if any.</param>
    /// <returns><c>true</c> when exactly one preferred source member matches; otherwise <c>false</c>.</returns>
    internal static bool TryFindSourceMember(
        IReadOnlyList<IPropertySymbol> sourceProperties,
        IReadOnlyList<IFieldSymbol> sourceFields,
        string expectedName,
        BooleanSetting caseInsensitivePropertyMap,
        BooleanSetting ignoreUnderscoreForPropertyMap,
        bool isConstructorParameterPath,
        bool useExactNameFromAttribute,
        out MappableMember? match)
    {
        match = null;

        if (TryFindSingleMatchingProperty(
                sourceProperties,
                expectedName,
                caseInsensitivePropertyMap,
                ignoreUnderscoreForPropertyMap,
                isConstructorParameterPath,
                useExactNameFromAttribute,
                out var propertyMatch))
        {
            match = MappableMember.FromProperty(propertyMatch, propertyMatch.GetMappablePropertyName());
            return true;
        }

        if (TryFindSingleMatchingField(
                sourceFields,
                expectedName,
                caseInsensitivePropertyMap,
                ignoreUnderscoreForPropertyMap,
                isConstructorParameterPath,
                useExactNameFromAttribute,
                out var fieldMatch))
        {
            match = MappableMember.FromField(fieldMatch);
            return true;
        }

        return false;
    }

    private static bool TryFindSingleMatchingProperty(
        IReadOnlyList<IPropertySymbol> sourceProperties,
        string expectedName,
        BooleanSetting caseInsensitivePropertyMap,
        BooleanSetting ignoreUnderscoreForPropertyMap,
        bool isConstructorParameterPath,
        bool useExactNameFromAttribute,
        out IPropertySymbol match)
    {
        match = null!;
        IPropertySymbol? candidate = null;
        var matchCount = 0;

        foreach (var property in sourceProperties)
        {
            if (!NamesMatch(
                    expectedName,
                    property.GetMappablePropertyName(),
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
                return false;
            }
        }

        if (matchCount != 1 || candidate is null)
        {
            return false;
        }

        match = candidate;
        return true;
    }

    private static bool TryFindSingleMatchingField(
        IReadOnlyList<IFieldSymbol> sourceFields,
        string expectedName,
        BooleanSetting caseInsensitivePropertyMap,
        BooleanSetting ignoreUnderscoreForPropertyMap,
        bool isConstructorParameterPath,
        bool useExactNameFromAttribute,
        out IFieldSymbol match)
    {
        match = null!;
        IFieldSymbol? candidate = null;
        var matchCount = 0;

        foreach (var field in sourceFields)
        {
            if (!NamesMatch(
                    expectedName,
                    field.Name,
                    caseInsensitivePropertyMap,
                    ignoreUnderscoreForPropertyMap,
                    isConstructorParameterPath,
                    useExactNameFromAttribute))
            {
                continue;
            }

            candidate = field;
            matchCount++;

            if (matchCount > 1)
            {
                return false;
            }
        }

        if (matchCount != 1 || candidate is null)
        {
            return false;
        }

        match = candidate;
        return true;
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