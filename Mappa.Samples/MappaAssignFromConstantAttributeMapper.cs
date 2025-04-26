// <copyright file="MappaAssignFromConstantAttributeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using <see cref="MappaAssignFromConstantAttribute"/>.
/// </summary>
// TODO [#53] Complete mapping MappaAssignFromConstantTargetClassModel of my adding missing attributes.
// TODO [#53] Add mapping to record to test mapping via constructor parameters.
[Mappa]
public sealed partial class MappaAssignFromConstantAttributeMapper
{
    /// <summary>
    /// Tests that a mapping can happen where properties are mapped using <see cref="MappaAssignFromConstantAttribute"/>.
    /// Target model is a class.
    /// </summary>
    /// <param name="o">The input unused object.</param>
    /// <returns>The mapped object.</returns>
    [MappaAssignFromConstant(nameof(MappaAssignFromConstantTargetClassModel.SbyteProperty), 13)]
    public partial MappaAssignFromConstantTargetClassModel MapForClassModel(object o);
}