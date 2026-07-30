// <copyright file="MemoryToArrayBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Collections;
using Mappa.Benchmark.Memory.Mappers;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Memory;

/// <summary>
/// Benchmark mapping <see cref="Memory{T}"/> to arrays.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class MemoryToArrayBenchmark
#pragma warning restore CA1515
{
    private readonly Memory<int> input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryToArrayBenchmark"/> class.
    /// </summary>
    public MemoryToArrayBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddMaps(typeof(AutomapperMapperProfile)),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = CollectionDataFactory.CreateIntArray();
        TypeAdapterConfig<Memory<int>, int[]>.NewConfig().MapWith(memory => memory.ToArray());
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped array.</returns>
    [Benchmark(Baseline = true)]
    public int[] Automapper()
        => this.automapperMapper.Map<int[]>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped array.</returns>
    [Benchmark]
    public int[] Mapperly()
        => this.mapperlyMapper.MapToArray(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped array.</returns>
    [Benchmark]
    public int[] Mapster()
        => this.input.Adapt<int[]>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped array.</returns>
    [Benchmark]
    public int[] Mappa()
        => this.mappaMapper.MapToArray(this.input);
}