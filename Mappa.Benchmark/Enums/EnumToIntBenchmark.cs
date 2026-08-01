// <copyright file="EnumToIntBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Enums.Mappers;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Enums;

/// <summary>
/// Benchmark to test the <see cref="Enum"/>
/// to <see cref="int"/> mapper.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class EnumToIntBenchmark
#pragma warning restore CA1515
{
    private readonly StringComparison input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToIntBenchmark"/> class.
    /// </summary>
    public EnumToIntBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile(new AutomapperMapperProfile());
            },
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000

        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = EnumDataFactory.CreateStringComparison();
    }

    /// <summary>
    /// Map using <see cref="AutoMapper"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark(Baseline = true)]
    public int Automapper()
        => this.automapperMapper.Map<int>(this.input);

    /// <summary>
    /// Map using <see cref="Riok.Mapperly"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public int Mapperly()
        => this.mapperlyMapper.MapToInt(this.input);

    /// <summary>
    /// Map using <see cref="Mapster"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public int Mapster()
        => this.input.Adapt<int>();

    /// <summary>
    /// Map using <see cref="Mappa.Attributes"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public int Mappa()
        => this.mappaMapper.MapToInt(this.input);
}