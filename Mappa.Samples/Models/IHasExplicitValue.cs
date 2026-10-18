// <copyright file="IHasExplicitValue.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Interface with a scalar property used for explicit-implementation samples.
/// </summary>
public interface IHasExplicitValue
{
    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    int Value { get; set; }
}