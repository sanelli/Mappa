// <copyright file="EnumToEnumBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Enums.Mappers;
using Mappa.Benchmark.Enums.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Enums;

/// <summary>
/// Benchmark mapping enums to enums.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class EnumToEnumBenchmark
#pragma warning restore CA1515
{
    private readonly SourceStatus input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToEnumBenchmark"/> class.
    /// </summary>
    public EnumToEnumBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddProfile(new AutomapperMapperProfile()),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = EnumDataFactory.CreateSourceStatus();
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped enum.</returns>
    [Benchmark(Baseline = true)]
    public TargetStatus Automapper()
        => this.automapperMapper.Map<TargetStatus>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped enum.</returns>
    [Benchmark]
    public TargetStatus Mapperly()
        => this.mapperlyMapper.Map(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped enum.</returns>
    [Benchmark]
    public TargetStatus Mapster()
        => this.input.Adapt<TargetStatus>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped enum.</returns>
    [Benchmark]
    public TargetStatus Mappa()
        => this.mappaMapper.Map(this.input);
}