// <copyright file="CompatibleMapMethodDerivedTarget.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Derived target type returned by the compatible hand-written map method.
/// </summary>
public sealed class CompatibleMapMethodDerivedTarget : CompatibleMapMethodBaseTarget
{
    /// <summary>
    /// Gets or sets a label used only to distinguish the derived target type.
    /// </summary>
    public string Label { get; set; } = "derived";
}