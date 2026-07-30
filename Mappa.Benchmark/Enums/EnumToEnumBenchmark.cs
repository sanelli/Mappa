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
    private const SourceStatus Input = SourceStatus.Active;

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
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped enum.</returns>
    [Benchmark(Baseline = true)]
    public TargetStatus Automapper()
        => this.automapperMapper.Map<TargetStatus>(Input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped enum.</returns>
    [Benchmark]
    public TargetStatus Mapperly()
        => this.mapperlyMapper.Map(Input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped enum.</returns>
    [Benchmark]
#pragma warning disable CA1822
    public TargetStatus Mapster()
#pragma warning restore CA1822
        => Input.Adapt<TargetStatus>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped enum.</returns>
    [Benchmark]
    public TargetStatus Mappa()
        => this.mappaMapper.Map(Input);
}