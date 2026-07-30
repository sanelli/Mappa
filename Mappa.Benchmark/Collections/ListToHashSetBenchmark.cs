// <copyright file="ListToHashSetBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Collections.Mappers;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Collections;

/// <summary>
/// Benchmark mapping lists to hash sets.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class ListToHashSetBenchmark
#pragma warning restore CA1515
{
    private readonly List<int> input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListToHashSetBenchmark"/> class.
    /// </summary>
    public ListToHashSetBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddProfile(new AutomapperMapperProfile()),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = CollectionDataFactory.CreateIntList();
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped hash set.</returns>
    [Benchmark(Baseline = true)]
    public HashSet<int> Automapper()
        => this.automapperMapper.Map<HashSet<int>>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped hash set.</returns>
    [Benchmark]
    public HashSet<int> Mapperly()
        => this.mapperlyMapper.MapToHashSet(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped hash set.</returns>
    [Benchmark]
    public HashSet<int> Mapster()
        => this.input.Adapt<HashSet<int>>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped hash set.</returns>
    [Benchmark]
    public HashSet<int> Mappa()
        => this.mappaMapper.MapToHashSet(this.input);
}