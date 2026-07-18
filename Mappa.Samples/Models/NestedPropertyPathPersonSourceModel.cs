// <copyright file="NestedPropertyPathPersonSourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source person model with a nested address for nested property path samples.
/// </summary>
public sealed class NestedPropertyPathPersonSourceModel
{
    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    public NestedPropertyPathAddressModel Address { get; set; } = null!;
}