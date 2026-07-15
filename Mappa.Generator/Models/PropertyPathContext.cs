// <copyright file="PropertyPathContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Tracks the remaining target and source property paths while mapping nested attribute paths.
/// </summary>
internal sealed class PropertyPathContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyPathContext"/> class.
    /// </summary>
    /// <param name="originalTargetPath">The full target path from the attribute.</param>
    /// <param name="originalSourcePath">The full source path from the attribute, if any.</param>
    /// <param name="remainingTargetSegments">The target segments still to resolve at the current level.</param>
    /// <param name="remainingSourceSegments">The source segments still to resolve at the current level.</param>
    /// <param name="isNestedAttributeScope">Whether the context represents a nested attribute scope.</param>
    /// <param name="outerTargetSegment">The outer target member name for nested attribute scopes.</param>
    internal PropertyPathContext(
        string originalTargetPath,
        string? originalSourcePath,
        string[] remainingTargetSegments,
        string[] remainingSourceSegments,
        bool isNestedAttributeScope = false,
        string? outerTargetSegment = null)
    {
        this.OriginalTargetPath = originalTargetPath;
        this.OriginalSourcePath = originalSourcePath;
        this.RemainingTargetSegments = remainingTargetSegments;
        this.RemainingSourceSegments = remainingSourceSegments;
        this.IsNestedAttributeScope = isNestedAttributeScope;
        this.OuterTargetSegment = outerTargetSegment;
    }

    /// <summary>
    /// Gets the full target path from the attribute.
    /// </summary>
    internal string OriginalTargetPath { get; }

    /// <summary>
    /// Gets the full source path from the attribute, if any.
    /// </summary>
    internal string? OriginalSourcePath { get; }

    /// <summary>
    /// Gets the target segments still to resolve at the current nested level.
    /// </summary>
    internal string[] RemainingTargetSegments { get; }

    /// <summary>
    /// Gets the source segments still to resolve at the current nested level.
    /// </summary>
    internal string[] RemainingSourceSegments { get; }

    /// <summary>
    /// Gets a value indicating whether the remaining target path has a single segment.
    /// </summary>
    internal bool IsLeafTargetMapping => this.RemainingTargetSegments.Length == 1;

    /// <summary>
    /// Gets a value indicating whether the context represents a nested attribute scope for an outer target member.
    /// </summary>
    internal bool IsNestedAttributeScope { get; }

    /// <summary>
    /// Gets the outer target member name when <see cref="IsNestedAttributeScope"/> is <c>true</c>.
    /// </summary>
    internal string? OuterTargetSegment { get; }

    /// <summary>
    /// Creates a nested attribute scope for mapping multiple nested paths under the same outer target member.
    /// </summary>
    /// <param name="outerTargetSegment">The outer target member name.</param>
    /// <returns>The property path context.</returns>
    internal static PropertyPathContext CreateNestedAttributeScope(string outerTargetSegment)
        => new(outerTargetSegment, null, [], [], true, outerTargetSegment);

    /// <summary>
    /// Creates a new context after descending one nested level.
    /// </summary>
    /// <returns>The descended context.</returns>
    internal PropertyPathContext DescendOneLevel()
    {
        return new PropertyPathContext(
            this.OriginalTargetPath,
            this.OriginalSourcePath,
            this.RemainingTargetSegments.Skip(1).ToArray(),
            this.RemainingSourceSegments.Length > 0 ? this.RemainingSourceSegments.Skip(1).ToArray() : []);
    }
}