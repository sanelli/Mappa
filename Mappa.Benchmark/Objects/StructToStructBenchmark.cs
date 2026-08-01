// <copyright file="StructToStructBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Objects.Mappers;
using Mappa.Benchmark.Objects.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Objects;

/// <summary>
/// Benchmark mapping struct DTOs to struct models.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class StructToStructBenchmark
#pragma warning restore CA1515
{
    private readonly PersonStructDto input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="StructToStructBenchmark"/> class.
    /// </summary>
    public StructToStructBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddProfile(new AutomapperMapperProfile()),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = ObjectDataFactory.CreatePersonStructDto();
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark(Baseline = true)]
    public PersonStruct Automapper()
        => this.automapperMapper.Map<PersonStruct>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark]
    public PersonStruct Mapperly()
        => this.mapperlyMapper.Map(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark]
    public PersonStruct Mapster()
        => this.input.Adapt<PersonStruct>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark]
    public PersonStruct Mappa()
        => this.mappaMapper.Map(this.input);
}