// <copyright file="CaseInsensitiveEnumToEnumMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating case-insensitive enum-to-enum mapping via <see cref="MappaSettingsAttribute"/>.
/// </summary>
[Mappa]
[MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Enable)]
public sealed partial class CaseInsensitiveEnumToEnumMapper
{
    /// <summary>
    /// Map a source enum to a target enum using case-insensitive member name matching.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The target enum member with a matching name.</returns>
    public partial CaseInsensitiveTargetValues Map(CaseInsensitiveSourceValues input);
}