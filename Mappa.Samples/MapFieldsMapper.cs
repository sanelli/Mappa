// <copyright file="MapFieldsMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaSettingsAttribute.MapSourceFields"/>
/// and <see cref="MappaSettingsAttribute.MapTargetFields"/>.
/// </summary>
[Mappa]
[MappaSettings(
    MapSourceFields = BooleanSetting.Enable,
    MapTargetFields = BooleanSetting.Enable)]
public sealed partial class MapFieldsMapper
{
    /// <summary>
    /// Maps source fields to target fields when both field settings are enabled.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <returns>The mapped model.</returns>
    public partial MapFieldsTargetModel MapFieldsToFields(MapFieldsSourceModel input);

    /// <summary>
    /// Maps source fields to target properties when <see cref="MappaSettingsAttribute.MapSourceFields"/> is enabled.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <returns>The mapped model.</returns>
    public partial MapFieldsPropertyTargetModel MapFieldsToProperties(MapFieldsSourceModel input);
}