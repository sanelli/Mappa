// <copyright file="ArrayToListBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Collections.Mappers;
using Mappa.Benchmark.Collections.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Collections;

/// <summary>
/// Benchmark mapping arrays to lists (elements include dictionaries).
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1002
#pragma warning disable CA1515
public class ArrayToListBenchmark
#pragma warning restore CA1515
{
    private readonly CollectionItemDto[] input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayToListBenchmark"/> class.
    /// </summary>
    public ArrayToListBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddMaps(typeof(AutomapperMapperProfile)),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = CollectionDataFactory.CreateArray();
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped list.</returns>
    [Benchmark(Baseline = true)]
    public List<CollectionItem> Automapper()
        => this.automapperMapper.Map<List<CollectionItem>>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped list.</returns>
    [Benchmark]
    public List<CollectionItem> Mapperly()
        => this.mapperlyMapper.MapToList(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped list.</returns>
    [Benchmark]
    public List<CollectionItem> Mapster()
        => this.input.Adapt<List<CollectionItem>>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped list.</returns>
    [Benchmark]
    public List<CollectionItem> Mappa()
        => this.mappaMapper.MapToList(this.input);
}