// <copyright file="MappaMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;

namespace Mappa.Benchmark.Memory.Mappers;

/// <summary>
/// Mappa mapper for memory/array benchmarks.
/// </summary>
[Mappa]
internal sealed partial class MappaMapper
{
    /// <summary>
    /// Maps <see cref="Memory{T}"/> to an array.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial int[] MapToArray(Memory<int> source);

    /// <summary>
    /// Maps an array to <see cref="Memory{T}"/>.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial Memory<int> MapToMemory(int[] source);
}