// <copyright file="MappaMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Benchmark.Enums.Mappers;

/// <summary>
/// Mapper using <see cref="Mappa"/>.
/// </summary>
[Mappa]
public sealed partial class MappaMapper
{
    /// <summary>
    /// Map from <see cref="string"/>
    /// to <see cref="StringComparison"/>.
    /// </summary>
    /// <param name="stringComparison">The input object.</param>
    /// <returns>The output object.</returns>
    public partial StringComparison Map(string stringComparison);

    /// <summary>
    /// Map from <see cref="int"/>
    /// to <see cref="StringComparison"/>.
    /// </summary>
    /// <param name="stringComparison">The input object.</param>
    /// <returns>The output object.</returns>
    public partial StringComparison Map(int stringComparison);

    /// <summary>
    /// Map from <see cref="StringComparison"/>
    /// to <see cref="string"/>.
    /// </summary>
    /// <param name="stringComparison">The input object.</param>
    /// <returns>The output object.</returns>
    public partial string MapToString(StringComparison stringComparison);

    /// <summary>
    /// Map from <see cref="StringComparison"/>
    /// to <see cref="int"/>.
    /// </summary>
    /// <param name="stringComparison">The input object.</param>
    /// <returns>The output object.</returns>
    public partial int MapToInt(StringComparison stringComparison);
}