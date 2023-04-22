// <copyright file="NullableToNullableMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using enum-to-enum strategy.
/// </summary>
[Mappa]
public sealed partial class NullableToNullableMapper
{
    /// <summary>
    /// Map an enum to another enum.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The integer mapped from the value.</returns>
    public partial int? Map(CountingValues? input);
}