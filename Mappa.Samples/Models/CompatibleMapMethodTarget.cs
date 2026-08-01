// <copyright file="CompatibleMapMethodTarget.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Wrapper target model with a base nested property.
/// </summary>
public sealed class CompatibleMapMethodTarget
{
    /// <summary>
    /// Gets or sets the nested base target.
    /// </summary>
    public required CompatibleMapMethodBaseTarget Property { get; set; }
}