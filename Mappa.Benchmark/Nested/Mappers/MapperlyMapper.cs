// <copyright file="MapperlyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.Nested.Models;

using Riok.Mapperly.Abstractions;

namespace Mappa.Benchmark.Nested.Mappers;

/// <summary>
/// Mapperly mapper for nested DTO benchmarks.
/// </summary>
[Mapper]
internal sealed partial class MapperlyMapper
{
    /// <summary>
    /// Maps an order DTO.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    public partial NestedOrder Map(NestedOrderDto source);
}