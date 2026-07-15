// <copyright file="ChainedSourcePropertyPathInfo.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models;

/// <summary>
/// Describes a remaining chained source property path used during code generation.
/// </summary>
internal sealed class ChainedSourcePropertyPathInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChainedSourcePropertyPathInfo"/> class.
    /// </summary>
    /// <param name="originalSourcePath">The full source path from the attribute.</param>
    /// <param name="remainingSourceSegments">The source segments still to read from the current receiver.</param>
    /// <param name="startingSourceType">The type of the current source receiver.</param>
    /// <param name="receiverPathPrefix">The dotted path prefix already consumed, used for diagnostics.</param>
    internal ChainedSourcePropertyPathInfo(
        string originalSourcePath,
        string[] remainingSourceSegments,
        ITypeSymbol startingSourceType,
        string receiverPathPrefix)
    {
        this.OriginalSourcePath = originalSourcePath;
        this.RemainingSourceSegments = remainingSourceSegments;
        this.StartingSourceType = startingSourceType;
        this.ReceiverPathPrefix = receiverPathPrefix;
    }

    /// <summary>
    /// Gets the full source path from the attribute.
    /// </summary>
    internal string OriginalSourcePath { get; }

    /// <summary>
    /// Gets the source segments still to read from the current receiver.
    /// </summary>
    internal string[] RemainingSourceSegments { get; }

    /// <summary>
    /// Gets the type of the current source receiver.
    /// </summary>
    internal ITypeSymbol StartingSourceType { get; }

    /// <summary>
    /// Gets the dotted path prefix already consumed, used for diagnostics.
    /// </summary>
    internal string ReceiverPathPrefix { get; }
}