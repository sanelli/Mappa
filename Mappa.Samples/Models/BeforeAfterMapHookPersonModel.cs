// <copyright file="BeforeAfterMapHookPersonModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Mutable person model used by before/after map hook samples.
/// </summary>
public sealed class BeforeAfterMapHookPersonModel
{
    /// <summary>
    /// Gets or sets the person name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a score mutated by before-map hooks.
    /// </summary>
    public int Score { get; set; }
}