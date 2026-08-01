// <copyright file="CompatibleMapMethodDerivedSource.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Derived source type used as a nested property when demonstrating compatible map-method reuse.
/// </summary>
public sealed class CompatibleMapMethodDerivedSource : CompatibleMapMethodBaseSource
{
    /// <summary>
    /// Gets or sets a label used only to distinguish the derived source type.
    /// </summary>
    public string Label { get; set; } = "derived";
}