// <copyright file="PolymorphicBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Polymorphism.Mappers;
using Mappa.Benchmark.Polymorphism.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Polymorphism;

/// <summary>
/// Benchmark polymorphic mapping across three derived types.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class PolymorphicBenchmark
#pragma warning restore CA1515
{
    private readonly AnimalDto input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolymorphicBenchmark"/> class.
    /// </summary>
    public PolymorphicBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddMaps(typeof(AutomapperMapperProfile)),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = new DogDto { Name = "Rex", Trained = true };
        TypeAdapterConfig<AnimalDto, Animal>.NewConfig()
            .Include<DogDto, Dog>()
            .Include<CatDto, Cat>()
            .Include<BirdDto, Bird>();
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped animal.</returns>
    [Benchmark(Baseline = true)]
    public Animal Automapper()
        => this.automapperMapper.Map<Animal>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped animal.</returns>
    [Benchmark]
    public Animal Mapperly()
        => this.mapperlyMapper.Map(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped animal.</returns>
    [Benchmark]
    public Animal Mapster()
        => this.input.Adapt<Animal>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped animal.</returns>
    [Benchmark]
    public Animal Mappa()
        => this.mappaMapper.Map(this.input);
}