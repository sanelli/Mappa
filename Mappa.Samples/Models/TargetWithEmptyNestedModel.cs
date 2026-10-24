// <copyright file="TargetWithEmptyNestedModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// A target type that embeds an empty nested type.
/// </summary>
public sealed class TargetWithEmptyNestedModel
{
    /// <summary>
    /// Gets or sets the nested empty target.
    /// </summary>
    public NestedEmptyTargetModel Nested { get; set; } = null!;
}