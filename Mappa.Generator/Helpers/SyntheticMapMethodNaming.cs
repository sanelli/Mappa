// <copyright file="SyntheticMapMethodNaming.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text;

using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Naming helpers for generator-synthesized private map methods.
/// </summary>
internal static class SyntheticMapMethodNaming
{
    /// <summary>
    /// Allocates a unique private map method name for the given type pair that does not collide
    /// with existing map methods or members on <paramref name="classContext"/>.
    /// </summary>
    /// <param name="classContext">The mapper class context.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="targetType">The target type.</param>
    /// <returns>A unique method name.</returns>
    internal static string AllocateName(
        MappaClassGeneratorContext classContext,
        ITypeSymbol sourceType,
        ITypeSymbol targetType)
    {
        var baseName = $"Map__{SanitizeTypeName(sourceType)}__To__{SanitizeTypeName(targetType)}";
        if (!IsNameTaken(classContext, baseName))
        {
            return baseName;
        }

        var suffix = 1;
        while (IsNameTaken(classContext, $"{baseName}_{suffix}"))
        {
            suffix++;
        }

        return $"{baseName}_{suffix}";
    }

    /// <summary>
    /// Sanitizes a type display name into an identifier-safe fragment.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The sanitized name fragment.</returns>
    internal static string SanitizeTypeName(ITypeSymbol type)
        => SanitizeTypeName(type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));

    /// <summary>
    /// Sanitizes a display name into an identifier-safe fragment.
    /// </summary>
    /// <param name="displayName">The display name.</param>
    /// <returns>The sanitized name fragment.</returns>
    internal static string SanitizeTypeName(string displayName)
    {
        var builder = new StringBuilder(displayName.Length);
        foreach (var character in displayName)
        {
            if (char.IsLetterOrDigit(character) || character == '_')
            {
                builder.Append(character);
            }
            else
            {
                builder.Append('_');
            }
        }

        if (builder.Length == 0 || !IsIdentifierStart(builder[0]))
        {
            builder.Insert(0, '_');
        }

        return builder.ToString();
    }

    private static bool IsNameTaken(MappaClassGeneratorContext classContext, string methodName)
    {
        if (classContext.MapMethods.Any(mapMethod =>
                string.Equals(mapMethod.MethodName, methodName, StringComparison.Ordinal)))
        {
            return true;
        }

        return classContext.ClassSymbol.GetMembers(methodName).Length > 0;
    }

    private static bool IsIdentifierStart(char character)
        => char.IsLetter(character) || character == '_';
}