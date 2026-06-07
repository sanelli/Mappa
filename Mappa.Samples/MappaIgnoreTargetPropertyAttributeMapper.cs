// <copyright file="MappaIgnoreTargetPropertyAttributeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper used to demonstrate <see cref="MappaIgnoreTargetPropertyAttribute"/>.
/// </summary>
[Mappa]
public sealed partial class MappaIgnoreTargetPropertyAttributeMapper
{
    /// <summary>
    /// Map from <see cref="MappaIgnoreTargetPropertySourceModel"/> to
    /// <see cref="MappaIgnoreTargetPropertyTargetModel"/> while ignoring
    /// <see cref="MappaIgnoreTargetPropertyTargetModel.IgnoredProperty"/>.
    /// </summary>
    /// <param name="source">The source model.</param>
    /// <returns>The target model.</returns>
    [MappaIgnoreTargetProperty(nameof(MappaIgnoreTargetPropertyTargetModel.IgnoredProperty))]
    public partial MappaIgnoreTargetPropertyTargetModel Map(MappaIgnoreTargetPropertySourceModel source);
}