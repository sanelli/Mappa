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
    /// Source and target have the same type.
    /// </summary>
    MapToSameType,

    /// <summary>
    /// Target type is <see cref="object"/>.
    /// </summary>
    MapToObject,

    /// <summary>
    /// The mapping is performed using an existing method.
    /// </summary>
    MapUsingExistingMethod,
}