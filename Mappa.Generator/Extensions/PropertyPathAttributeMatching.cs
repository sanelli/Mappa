// <copyright file="PropertyPathAttributeMatching.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Helpers for matching mapping attributes against property paths.
/// </summary>
internal static class PropertyPathAttributeMatching
{
    /// <summary>
    /// Determines whether an attribute target path matches the current target member at the given mapping level.
    /// </summary>
    /// <param name="attributeTargetPath">The attribute target path.</param>
    /// <param name="memberName">The current target member name.</param>
    /// <param name="propertyPathContext">The active property path context, if any.</param>
    /// <param name="stringComparison">The string comparison to use.</param>
    /// <returns><c>true</c> when the attribute applies at the current level.</returns>
    internal static bool MatchesTargetMember(
        string attributeTargetPath,
        string memberName,
        PropertyPathContext? propertyPathContext,
        StringComparison stringComparison)
    {
        var targetPath = PropertyPath.Parse(attributeTargetPath);
        if (targetPath.Segments.Length == 0)
        {
            return false;
        }

        if (propertyPathContext is null)
        {
            return MatchesTargetMemberAtRoot(targetPath, memberName, stringComparison);
        }

        if (propertyPathContext.IsNestedAttributeScope)
        {
            return MatchesTargetMemberInNestedAttributeScope(targetPath, memberName, propertyPathContext, stringComparison);
        }

        return MatchesTargetMemberForRemainingSegments(targetPath, memberName, propertyPathContext, stringComparison);
    }

    /// <summary>
    /// Creates a property path context for a matched attribute target and source path.
    /// </summary>
    /// <param name="targetPath">The attribute target path.</param>
    /// <param name="sourcePath">The attribute source path, if any.</param>
    /// <returns>The property path context.</returns>
    internal static PropertyPathContext CreatePropertyPathContext(string targetPath, string? sourcePath)
    {
        var parsedTargetPath = PropertyPath.Parse(targetPath);
        PropertyPath parsedSourcePath;
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            parsedSourcePath = PropertyPath.Parse(string.Empty);
        }
        else
        {
            parsedSourcePath = PropertyPath.Parse(sourcePath);
        }

        return new PropertyPathContext(
            targetPath,
            string.IsNullOrWhiteSpace(sourcePath) ? null : sourcePath,
            parsedTargetPath.GetRemainingSegments(),
            parsedSourcePath.GetRemainingSegments());
    }

    /// <summary>
    /// Gets the first source segment to pair with the outer target member, if any.
    /// </summary>
    /// <param name="sourcePath">The attribute source path.</param>
    /// <returns>The first source segment, if any.</returns>
    internal static string? GetFirstSourceSegment(string? sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            return null;
        }

        return PropertyPath.Parse(sourcePath).GetFirstSegment();
    }

    /// <summary>
    /// Gets the expected source property name for the current mapping level.
    /// </summary>
    /// <param name="sourcePath">The attribute source path.</param>
    /// <param name="propertyPathContext">The active property path context, if any.</param>
    /// <param name="isLeafTargetMapping">Whether the current target mapping is a leaf.</param>
    /// <returns>The source property name for name matching, if any.</returns>
    internal static string? GetExpectedSourcePropertyNameForCurrentLevel(
        string? sourcePath,
        PropertyPathContext? propertyPathContext,
        bool isLeafTargetMapping)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
        {
            return null;
        }

        if (propertyPathContext is not null)
        {
            if (isLeafTargetMapping)
            {
                return null;
            }

            return propertyPathContext.RemainingSourceSegments.Length > 0
                ? propertyPathContext.RemainingSourceSegments[0]
                : null;
        }

        var parsedSourcePath = PropertyPath.Parse(sourcePath);
        if (parsedSourcePath.Segments.Length <= 1)
        {
            return parsedSourcePath.GetFirstSegment();
        }

        return parsedSourcePath.GetFirstSegment();
    }

    private static bool MatchesTargetMemberAtRoot(
        PropertyPath targetPath,
        string memberName,
        StringComparison stringComparison)
        => targetPath.GetFirstSegment() is string firstSegment
           && firstSegment.Equals(memberName, stringComparison);

    private static bool MatchesTargetMemberInNestedAttributeScope(
        PropertyPath targetPath,
        string memberName,
        PropertyPathContext propertyPathContext,
        StringComparison stringComparison)
        => propertyPathContext.OuterTargetSegment is string outerTargetSegment
           && targetPath.Segments.Length >= 2
           && targetPath.Segments[0].Equals(outerTargetSegment, stringComparison)
           && targetPath.Segments[targetPath.Segments.Length - 1].Equals(memberName, stringComparison);

    private static bool MatchesTargetMemberForRemainingSegments(
        PropertyPath targetPath,
        string memberName,
        PropertyPathContext propertyPathContext,
        StringComparison stringComparison)
        => propertyPathContext.RemainingTargetSegments.Length > 0
           && propertyPathContext.RemainingTargetSegments[0].Equals(memberName, stringComparison)
           && targetPath.EndsWith(propertyPathContext.RemainingTargetSegments);
}