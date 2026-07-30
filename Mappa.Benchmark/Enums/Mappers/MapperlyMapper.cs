// <copyright file="MapperlyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.Enums.Models;

using Riok.Mapperly.Abstractions;

namespace Mappa.Benchmark.Enums.Mappers;

/// <summary>
/// Mapper using Mapperly.
/// </summary>
[Mapper]
internal sealed partial class MapperlyMapper
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
    /// Map from <see cref="SourceStatus"/> to <see cref="TargetStatus"/>.
    /// </summary>
    /// <param name="status">The input enum.</param>
    /// <returns>The output enum.</returns>
    public partial TargetStatus Map(SourceStatus status);

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