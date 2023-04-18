// <copyright file="StringToEnumMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the enum-to-string strategy.
/// </summary>
[Mappa]
public sealed partial class StringToEnumMapper
{
    /// <summary>
    /// Map an enum to a string.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The enum mapped from the string.</returns>
    public partial CountingValues MapToEnum(string input);
}