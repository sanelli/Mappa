// <copyright file="DescriptionEnumToStringMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating enum-to-string mapping by <see cref="System.ComponentModel.DescriptionAttribute"/>.
/// </summary>
[Mappa]
[MappaSettings(EnumStringMapSetting = EnumStringMapSetting.Description)]
public sealed partial class DescriptionEnumToStringMapper
{
    /// <summary>
    /// Map an enum to a string using each member's Description value.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The Description string for the enum member.</returns>
    public partial string MapToString(DescribedCountingValues input);
}