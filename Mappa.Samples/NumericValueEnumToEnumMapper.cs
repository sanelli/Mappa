// <copyright file="NumericValueEnumToEnumMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating numeric-value enum-to-enum mapping via <see cref="MappaSettingsAttribute"/>.
/// </summary>
[Mappa]
[MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.NumericValue)]
public sealed partial class NumericValueEnumToEnumMapper
{
    /// <summary>
    /// Map an enum to another enum by matching underlying numeric values.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The enum mapped by numeric value.</returns>
    public partial CountingValuesFromTwo Map(CountingValues input);
}