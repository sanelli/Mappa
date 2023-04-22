// <copyright file="StringToDateTimeMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Attributes;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the string-to-datetime strategy.
/// </summary>
[Mappa]
public sealed partial class StringToDateTimeMapper
{
    /// <summary>
    /// Map a string to <see cref="DateTime"/>.
    /// </summary>
    /// <param name="input">The input string value.</param>
    /// <returns>The mapped numeric value value.</returns>
    public partial DateTime Map(string input);
}