// <copyright file="MapperlyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Riok.Mapperly.Abstractions;

namespace Mappa.Benchmark.Memory.Mappers;

/// <summary>
/// Mapperly mapper for memory/array benchmarks.
/// </summary>
[Mapper]
internal sealed partial class MapperlyMapper
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