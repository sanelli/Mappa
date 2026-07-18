// <copyright file="NestedPropertyPathCityTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Flat target model used when mapping a nested source path into a single property.
/// </summary>
public sealed class NestedPropertyPathCityTargetModel
{
    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string City { get; set; } = string.Empty;
}