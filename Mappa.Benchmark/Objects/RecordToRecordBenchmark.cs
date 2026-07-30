// <copyright file="RecordToRecordBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Objects.Mappers;
using Mappa.Benchmark.Objects.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Objects;

/// <summary>
/// Benchmark mapping record DTOs to record models.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class RecordToRecordBenchmark
#pragma warning restore CA1515
{
    private readonly PersonRecordDto input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordToRecordBenchmark"/> class.
    /// </summary>
    public RecordToRecordBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddMaps(typeof(AutomapperMapperProfile)),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = new PersonRecordDto(
            1,
            "Ada Lovelace",
            36,
            new AddressDto { Street = "Analytical Engine Way", City = "London", Zip = "SW1A" });
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark(Baseline = true)]
    public PersonRecord Automapper()
        => this.automapperMapper.Map<PersonRecord>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark]
    public PersonRecord Mapperly()
        => this.mapperlyMapper.Map(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark]
    public PersonRecord Mapster()
        => this.input.Adapt<PersonRecord>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark]
    public PersonRecord Mappa()
        => this.mappaMapper.Map(this.input);
}