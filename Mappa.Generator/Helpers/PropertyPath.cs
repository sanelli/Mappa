// <copyright file="PropertyPath.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Helpers;

/// <summary>
/// A dot-separated property path.
/// </summary>
internal readonly struct PropertyPath
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyPath"/> struct.
    /// </summary>
    /// <param name="segments">The path segments.</param>
    private PropertyPath(string[] segments)
    {
        this.Segments = segments;
    }

    /// <summary>
    /// Gets the path segments.
    /// </summary>
    internal string[] Segments { get; }

    /// <summary>
    /// Gets a value indicating whether the path contains nested segments.
    /// </summary>
    internal bool IsNested => this.Segments.Length > 1;

    /// <summary>
    /// Parses a dot-separated property path.
    /// </summary>
    /// <param name="path">The path to parse.</param>
    /// <returns>The parsed path.</returns>
    internal static PropertyPath Parse(string? path)
    {
        if (path is null || string.IsNullOrWhiteSpace(path))
        {
            return new PropertyPath([]);
        }

        var segments = path.Split('.');
        for (var index = 0; index < segments.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(segments[index]))
            {
                return new PropertyPath([]);
            }
        }

        return new PropertyPath(segments);
    }

    /// <summary>
    /// Gets a path containing the specified remaining segments.
    /// </summary>
    /// <param name="remainingSegments">The remaining segments.</param>
    /// <returns>The path.</returns>
    internal static PropertyPath FromRemainingSegments(string[] remainingSegments)
        => new(remainingSegments);

    /// <summary>
    /// Gets the first segment of the path, if any.
    /// </summary>
    /// <returns>The first segment, or <c>null</c> when the path is empty.</returns>
    internal string? GetFirstSegment()
        => this.Segments.Length > 0 ? this.Segments[0] : null;

    /// <summary>
    /// Gets the remaining segments after the first one.
    /// </summary>
    /// <returns>The remaining segments.</returns>
    internal string[] GetRemainingSegments()
        => this.Segments.Length > 1 ? this.Segments.Skip(1).ToArray() : [];

    /// <summary>
    /// Joins the segments into a dot-separated path.
    /// </summary>
    /// <returns>The joined path.</returns>
    internal string ToDotSeparatedString()
        => string.Join(".", this.Segments);

    /// <summary>
    /// Determines whether the path ends with the specified suffix segments.
    /// </summary>
    /// <param name="suffixSegments">The suffix segments.</param>
    /// <returns><c>true</c> when the suffix matches; otherwise, <c>false</c>.</returns>
    internal bool EndsWith(string[] suffixSegments)
    {
        if (suffixSegments.Length > this.Segments.Length)
        {
            return false;
        }

        var offset = this.Segments.Length - suffixSegments.Length;
        for (var index = 0; index < suffixSegments.Length; index++)
        {
            if (!this.Segments[offset + index].Equals(suffixSegments[index], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}