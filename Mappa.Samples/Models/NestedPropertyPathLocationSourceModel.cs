// <copyright file="NestedPropertyPathLocationSourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source model with a nested location for three-segment property path samples.
/// </summary>
public sealed class NestedPropertyPathLocationSourceModel
{
    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    public NestedPropertyPathLocationModel Location { get; set; } = null!;
}