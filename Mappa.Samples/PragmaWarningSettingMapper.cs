// <copyright file="PragmaWarningSettingMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Mapper user to showcase the <see cref="MappaSettingsAttribute.PragmaWarning"/> setting.
/// </summary>
[Mappa]
public sealed partial class PragmaWarningSettingMapper
{
    /// <summary>
    /// Map from <see cref="int"/> to <see cref="long"/> and any
    /// warning is suppressed.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The mapped value.</returns>
    [MappaSettings(PragmaWarning = PragmaWarningSetting.Disable)]
    public partial long Map(int input);
}