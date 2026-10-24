// <copyright file="SourceWithEmptyNestedModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A source type that embeds an empty nested type.
/// </summary>
public sealed class SourceWithEmptyNestedModel
{
    /// <summary>
    /// Gets or sets the nested empty source.
    /// </summary>
    public NestedEmptySourceModel Nested { get; set; } = new();
}