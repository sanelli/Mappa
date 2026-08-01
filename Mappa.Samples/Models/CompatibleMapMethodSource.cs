// <copyright file="CompatibleMapMethodSource.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Wrapper source model with a derived nested property.
/// </summary>
public sealed class CompatibleMapMethodSource
{
    /// <summary>
    /// Gets or sets the nested derived source.
    /// </summary>
    public CompatibleMapMethodDerivedSource Property { get; set; } = new();
}