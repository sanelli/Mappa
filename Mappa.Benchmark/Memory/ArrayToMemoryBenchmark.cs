// <copyright file="ArrayToMemoryBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Collections;
using Mappa.Benchmark.Memory.Mappers;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Memory;

/// <summary>
/// Benchmark mapping arrays to <see cref="Memory{T}"/>.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class ArrayToMemoryBenchmark
#pragma warning restore CA1515
{
    private readonly int[] input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayToMemoryBenchmark"/> class.
    /// </summary>
    public ArrayToMemoryBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddMaps(typeof(AutomapperMapperProfile)),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = CollectionDataFactory.CreateIntArray();
        TypeAdapterConfig<int[], Memory<int>>.NewConfig().MapWith(array => new Memory<int>(array));
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped memory.</returns>
    [Benchmark(Baseline = true)]
    public Memory<int> Automapper()
        => this.automapperMapper.Map<Memory<int>>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped memory.</returns>
    [Benchmark]
    public Memory<int> Mapperly()
        => this.mapperlyMapper.MapToMemory(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped memory.</returns>
    [Benchmark]
    public Memory<int> Mapster()
        => this.input.Adapt<Memory<int>>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped memory.</returns>
    [Benchmark]
    public Memory<int> Mappa()
        => this.mappaMapper.MapToMemory(this.input);
}