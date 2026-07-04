// <copyright file="DescriptionEnumToEnumMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating enum-to-enum mapping by matching <see cref="System.ComponentModel.DescriptionAttribute"/> values.
/// </summary>
[Mappa]
[MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.Description)]
public sealed partial class DescriptionEnumToEnumMapper
{
    /// <summary>
    /// Map a source enum to a target enum by matching Description values.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The target enum member with the same Description.</returns>
    public partial DescribedTargetValues Map(DescribedSourceValues input);
}