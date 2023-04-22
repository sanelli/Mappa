// <copyright file="InvokeToStringMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the invoke-to-string strategy.
/// </summary>
[Mappa]
public sealed partial class InvokeToStringMapper
{
    /// <summary>
    /// Map an integer to a string.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The string mapped from the value.</returns>
    public partial string Map(int input);
}