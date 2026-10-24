// <copyright file="NullSubstituteTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model for null-substitute mapping samples.
/// </summary>
public sealed class NullSubstituteTargetModel
{
    /// <summary>
    /// Gets or sets the mapped name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the mapped alias.
    /// </summary>
    public string Alias { get; set; } = string.Empty;
}