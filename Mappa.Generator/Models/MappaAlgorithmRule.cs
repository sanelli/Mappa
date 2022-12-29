// <copyright file="MappaAlgorithmRule.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Generator.Models;

/// <summary>
/// Describe the rule used by the algorithm to generate a mappa.
/// </summary>
public enum MappaAlgorithmRule
{
    /// <summary>
    /// No rule is being applied.
    /// </summary>
    None,

    /// <summary>
    /// Any object can be mapped to <see cref="object"/> when
    /// nullability is disabled in that context.
    /// </summary>
    MapToObjectWhenNullableDisabled,

    /// <summary>
    /// The mapping is performed using an existing method.
    /// </summary>
    MapUsingExistingMethod,
}