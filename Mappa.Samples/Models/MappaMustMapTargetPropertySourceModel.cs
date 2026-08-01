// <copyright file="MappaMustMapTargetPropertySourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples.Models;

/// <summary>
/// Source model for <see cref="MappaMustMapTargetPropertyAttribute"/> samples.
/// </summary>
public sealed class MappaMustMapTargetPropertySourceModel
{
    /// <summary>
    /// Gets or sets the first property that must be mapped.
    /// </summary>
    public int PropertyA { get; set; }

    /// <summary>
    /// Gets or sets the second property that must be mapped.
    /// </summary>
    public int PropertyB { get; set; }
}