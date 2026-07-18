// <copyright file="NestedPropertyPathLocationModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Nested location model used by nested property path samples.
/// </summary>
public sealed class NestedPropertyPathLocationModel
{
    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    public NestedPropertyPathAddressModel Address { get; set; } = null!;
}