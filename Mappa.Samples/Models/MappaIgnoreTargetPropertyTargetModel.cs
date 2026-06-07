// <copyright file="MappaIgnoreTargetPropertyTargetModel.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples.Models;

/// <summary>
/// Target model for <see cref="MappaIgnoreTargetPropertyAttribute"/> samples.
/// </summary>
public sealed class MappaIgnoreTargetPropertyTargetModel
{
    /// <summary>
    /// Gets or sets the property that is mapped from the source.
    /// </summary>
    public string MappedProperty { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a property that is excluded from empty-constructor mapping.
    /// </summary>
    public long IgnoredProperty { get; set; }
}