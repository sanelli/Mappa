// <copyright file="EnumToStringMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the enum-to-string strategy.
/// </summary>
[Mappa]
public sealed partial class EnumToStringMapper
{
    /// <summary>
    /// Map an enum to a string.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The string mapped from the value.</returns>
    public partial string MapToString(CountingValues input);
}