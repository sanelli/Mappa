// <copyright file="MappaMapper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Benchmark.Polymorphism.Models;

namespace Mappa.Benchmark.Polymorphism.Mappers;

/// <summary>
/// Mappa mapper for polymorphic benchmarks.
/// </summary>
[Mappa]
internal sealed partial class MappaMapper
{
    /// <summary>
    /// Maps a polymorphic animal DTO.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The target.</returns>
    [MappaTypeMapping(typeof(Dog), typeof(DogDto))]
    [MappaTypeMapping(typeof(Cat), typeof(CatDto))]
    [MappaTypeMapping(typeof(Bird), typeof(BirdDto))]
    public partial Animal Map(AnimalDto source);
}