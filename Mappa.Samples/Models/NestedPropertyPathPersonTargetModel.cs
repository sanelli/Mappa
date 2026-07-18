// <copyright file="NestedPropertyPathPersonTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target person model with a nested address for nested property path samples.
/// </summary>
public sealed class NestedPropertyPathPersonTargetModel
{
    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    public NestedPropertyPathAddressModel Address { get; set; } = null!;
}