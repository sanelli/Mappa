// <copyright file="DescriptionStringToEnumMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating string-to-enum mapping by <see cref="System.ComponentModel.DescriptionAttribute"/>.
/// </summary>
[Mappa]
[MappaSettings(EnumStringMapSetting = EnumStringMapSetting.Description)]
public sealed partial class DescriptionStringToEnumMapper
{
    /// <summary>
    /// Map a string to an enum by matching Description values.
    /// </summary>
    /// <param name="input">The input Description string.</param>
    /// <returns>The enum member with the matching Description.</returns>
    public partial DescribedCountingValues MapToEnum(string input);
}