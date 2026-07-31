// <copyright file="ObjectFactoryTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Models;

/// <summary>
/// Target model used by object factory samples.
/// </summary>
public sealed class ObjectFactoryTargetModel
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a numeric value.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets or sets a tag identifying which factory produced the instance.
    /// </summary>
    public string FactoryTag { get; set; } = string.Empty;
}