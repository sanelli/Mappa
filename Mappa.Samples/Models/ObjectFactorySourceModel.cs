// <copyright file="ObjectFactorySourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Source model used by object factory samples.
/// </summary>
public sealed class ObjectFactorySourceModel
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a numeric value.
    /// </summary>
    public int Value { get; set; }
}