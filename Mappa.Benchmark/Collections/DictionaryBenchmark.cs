// <copyright file="DictionaryBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Collections.Mappers;
using Mappa.Benchmark.Collections.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Collections;

/// <summary>
/// Benchmark mapping dictionaries (values include nested dictionaries).
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class DictionaryBenchmark
#pragma warning restore CA1515
{
    private readonly Dictionary<string, CollectionItemDto> input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryBenchmark"/> class.
    /// </summary>
    public DictionaryBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddMaps(typeof(AutomapperMapperProfile)),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = CollectionDataFactory.CreateDictionary();
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped dictionary.</returns>
    [Benchmark(Baseline = true)]
    public Dictionary<string, CollectionItem> Automapper()
        => this.automapperMapper.Map<Dictionary<string, CollectionItem>>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped dictionary.</returns>
    [Benchmark]
    public Dictionary<string, CollectionItem> Mapperly()
        => this.mapperlyMapper.Map(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped dictionary.</returns>
    [Benchmark]
    public Dictionary<string, CollectionItem> Mapster()
        => this.input.Adapt<Dictionary<string, CollectionItem>>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped dictionary.</returns>
    [Benchmark]
    public Dictionary<string, CollectionItem> Mappa()
        => this.mappaMapper.Map(this.input);
}