// <copyright file="TupleToTupleMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Samples.Models;

namespace Mappa.Samples;

/// <summary>
/// Mapper using the tuple-to-tuple strategy.
/// </summary>
[Mappa]
public sealed partial class TupleToTupleMapper
{
    /// <summary>
    /// Map a <see cref="Tuple"/> to <see cref="Tuple"/>.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapper dictionary.</returns>
    public partial Tuple<string, string, string> MapSystemTupleToSystemTuple(Tuple<int, CountingValues, long> input);

    /// <summary>
    /// Map a <see cref="Tuple"/> with un-named elements to <see cref="Tuple"/>
    /// with un-named elements.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapper dictionary.</returns>
    public partial (string, string, string) MapTupleToTuple((int, CountingValues, long) input);

    /// <summary>
    /// Map a <see cref="Tuple"/> with named elements to <see cref="Tuple"/>
    /// with named elements.
    /// </summary>
    /// <param name="input">The input dictionary.</param>
    /// <returns>The mapper dictionary.</returns>
    public partial (string First, string Second, string Third) MapTupleWithNamesElementsToTupleWithNamesElements((int Alpha, CountingValues Beta, long Gamma) input);
}