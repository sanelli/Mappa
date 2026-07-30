// <copyright file="NestedDtoBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Nested.Mappers;
using Mappa.Benchmark.Nested.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Nested;

/// <summary>
/// Benchmark mapping nested DTOs that contain lists, arrays, hash sets, and dictionaries.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class NestedDtoBenchmark
#pragma warning restore CA1515
{
    private readonly NestedOrderDto input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="NestedDtoBenchmark"/> class.
    /// </summary>
    public NestedDtoBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddProfile(new AutomapperMapperProfile()),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = NestedDataFactory.CreateNestedOrder();
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped nested order.</returns>
    [Benchmark(Baseline = true)]
    public NestedOrder Automapper()
        => this.automapperMapper.Map<NestedOrder>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped nested order.</returns>
    [Benchmark]
    public NestedOrder Mapperly()
        => this.mapperlyMapper.Map(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped nested order.</returns>
    [Benchmark]
    public NestedOrder Mapster()
        => this.input.Adapt<NestedOrder>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped nested order.</returns>
    [Benchmark]
    public NestedOrder Mappa()
        => this.mappaMapper.Map(this.input);
}