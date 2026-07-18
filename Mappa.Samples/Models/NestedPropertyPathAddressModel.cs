// <copyright file="NestedPropertyPathAddressModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Nested address model used by nested property path samples.
/// </summary>
public sealed class NestedPropertyPathAddressModel
{
    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the zip code.
    /// </summary>
    public string ZipCode { get; set; } = string.Empty;
}