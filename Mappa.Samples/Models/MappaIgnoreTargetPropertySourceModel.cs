// <copyright file="MappaIgnoreTargetPropertySourceModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples.Models;

/// <summary>
/// Source model for <see cref="MappaIgnoreTargetPropertyAttribute"/> samples.
/// </summary>
public sealed class MappaIgnoreTargetPropertySourceModel
{
    /// <summary>
    /// Gets or sets the property that is mapped to the target.
    /// </summary>
    public int MappedProperty { get; set; }
}