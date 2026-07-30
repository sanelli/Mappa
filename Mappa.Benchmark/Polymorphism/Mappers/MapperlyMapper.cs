// <copyright file="MapperlyMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.Polymorphism.Models;

using Riok.Mapperly.Abstractions;

namespace Mappa.Benchmark.Polymorphism.Mappers;

/// <summary>
/// Mapperly mapper for polymorphic benchmarks.
/// </summary>
[Mapper]
internal sealed partial class MapperlyMapper
{
    /// <summary>
    /// Maps a polymorphic animal DTO.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    [MapDerivedType(typeof(DogDto), typeof(Dog))]
    [MapDerivedType(typeof(CatDto), typeof(Cat))]
    [MapDerivedType(typeof(BirdDto), typeof(Bird))]
    public partial Animal Map(AnimalDto source);
}