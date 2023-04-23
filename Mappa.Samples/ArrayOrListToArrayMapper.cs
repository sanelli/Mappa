// <copyright file="ArrayOrListToArrayMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using nullable-to-nullable strategy.
/// </summary>
[Mappa]
public sealed partial class ArrayOrListToArrayMapper
{
    /// <summary>
    /// Map an array of enum to an array of integer.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The integer mapped from the value.</returns>
    public partial int[] Map(CountingValues[] input);

    /// <summary>
    /// Map an array of nullable enum to an array of nullable integer.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The integer mapped from the value.</returns>
    public partial int?[] Map(CountingValues?[] input);

    /// <summary>
    /// Map an <see cref="IList{T}"/> of enum to an array of integer.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The integer mapped from the value.</returns>
    public partial int[] Map(IList<CountingValues> input);

    /// <summary>
    /// Map an <see cref="List{T}"/> of enum to an array of integer.
    /// </summary>
    /// <param name="input">The input enum value.</param>
    /// <returns>The integer mapped from the value.</returns>
    public partial int[] Map(List<CountingValues> input);
}