// <copyright file="EnumToIntegralMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the enum-to-string strategy.
/// </summary>
[Mappa]
public sealed partial class EnumToIntegralMapper
{
    /// <summary>
    /// Map an enum to a integer.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The integer mapped from the value.</returns>
    public partial int MapToInteger(CountingValues input);

    /// <summary>
    /// Map an enum with custom values to a integer.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The integer mapped from the value.</returns>
    public partial int MapToInteger(CountingValuesBackwards input);

    /// <summary>
    /// Map an enum to a long.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The long mapped from the value.</returns>
    public partial long MapToLong(CountingValues input);
}