// <copyright file="PropertyMapNameSettingsMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating property map name <see cref="MappaSettingsAttribute"/> settings.
/// </summary>
[Mappa]
[MappaSettings(
    CaseInsensitivePropertyMap = BooleanSetting.Enable,
    IgnoreUnderscoreForPropertyMap = BooleanSetting.Enable)]
public sealed partial class PropertyMapNameSettingsMapper
{
    /// <summary>
    /// Map using class-level property map name settings.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <returns>The mapped model.</returns>
    public partial PropertyMapNameSettingsTargetModel MapWithClassDefaults(PropertyMapNameSettingsSourceModel input);

    /// <summary>
    /// Map with method-level override disabling underscore-insensitive matching.
    /// </summary>
    /// <param name="input">The input model.</param>
    /// <returns>The mapped model.</returns>
    [MappaSettings(IgnoreUnderscoreForPropertyMap = BooleanSetting.Disable)]
    public partial PropertyMapNameSettingsPartialTargetModel MapWithMethodOverrideDisablingUnderscoreMatching(PropertyMapNameSettingsSourceModel input);
}