// <copyright file="ListToArrayBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Collections.Mappers;
using Mappa.Benchmark.Collections.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Collections;

/// <summary>
/// Benchmark mapping lists to arrays (elements include dictionaries).
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class ListToArrayBenchmark
#pragma warning restore CA1515
{
    private readonly List<CollectionItemDto> input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListToArrayBenchmark"/> class.
    /// </summary>
    public ListToArrayBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddMaps(typeof(AutomapperMapperProfile)),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = CollectionDataFactory.CreateList();
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped array.</returns>
    [Benchmark(Baseline = true)]
    public CollectionItem[] Automapper()
        => this.automapperMapper.Map<CollectionItem[]>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped array.</returns>
    [Benchmark]
    public CollectionItem[] Mapperly()
        => this.mapperlyMapper.MapToArray(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped array.</returns>
    [Benchmark]
    public CollectionItem[] Mapster()
        => this.input.Adapt<CollectionItem[]>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped array.</returns>
    [Benchmark]
    public CollectionItem[] Mappa()
        => this.mappaMapper.MapToArray(this.input);
}