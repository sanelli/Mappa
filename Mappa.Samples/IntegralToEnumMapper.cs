// <copyright file="IntegralToEnumMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the number-to-enum strategy.
/// </summary>
[Mappa]
public sealed partial class IntegralToEnumMapper
{
    /// <summary>
    /// Map an integer to an enum.
    /// </summary>
    /// <param name="input">The input integer value.</param>
    /// <returns>The enum mapped from the value.</returns>
    public partial CountingValues MapToEnum(int input);

    /// <summary>
    /// Map a short to an enum.
    /// </summary>
    /// <param name="input">The input short value.</param>
    /// <returns>The enum mapped from the value.</returns>
    public partial CountingValues MapToEnum(short input);

    /// <summary>
    /// Map an integer to an enum with custom values.
    /// </summary>
    /// <param name="input">The input int value.</param>
    /// <returns>The enum mapped from the value.</returns>
    public partial CountingValuesBackwards MapToBackwardsEnum(int input);
}