// <copyright file="CaseInsensitiveEnumMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating case-insensitive string-to-enum mapping via <see cref="MappaSettingsAttribute"/>.
/// </summary>
[Mappa]
[MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Enable)]
public sealed partial class CaseInsensitiveEnumMapper
{
    /// <summary>
    /// Map a string to an enum using case-insensitive member name matching.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The enum mapped from the string.</returns>
    public partial CountingValues MapToEnum(string input);
}