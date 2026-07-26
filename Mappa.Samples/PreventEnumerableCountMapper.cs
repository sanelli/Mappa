// <copyright file="PreventEnumerableCountMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper demonstrating <see cref="MappaSettingsAttribute.PreventEnumerableCount"/> for
/// <see cref="IEnumerable{T}"/> sources mapped to fixed-size collection targets.
/// </summary>
[Mappa]
[MappaSettings(PreventEnumerableCount = BooleanSetting.Enable)]
public sealed partial class PreventEnumerableCountMapper
{
    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to an <see cref="Array"/> of <see cref="int"/> without calling <c>Enumerable.Count</c>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial int[] MapEnumerableToArray(IEnumerable<CountingValues> input);

    /// <summary>
    /// Map an <see cref="IEnumerable{T}"/> of <see cref="CountingValues"/>
    /// to a <see cref="Span{T}"/> of <see cref="int"/> without calling <c>Enumerable.Count</c>.
    /// </summary>
    /// <param name="input">The input collection.</param>
    /// <returns>The output collection.</returns>
    public partial Span<int> MapEnumerableToSpan(IEnumerable<CountingValues> input);
}