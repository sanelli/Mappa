// <copyright file="EnumToStringBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Enums.Mappers;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Enums;

/// <summary>
/// Benchmark to test the <see cref="Enum"/>
/// to <see cref="string"/> mapper.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class EnumToStringBenchmark
#pragma warning restore CA1515
{
    private readonly StringComparison input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToStringBenchmark"/> class.
    /// </summary>
    public EnumToStringBenchmark()
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
    public string Automapper()
        => this.automapperMapper.Map<string>(this.input);

    /// <summary>
    /// Map using <see cref="Riok.Mapperly"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public string Mapperly()
        => this.mapperlyMapper.MapToString(this.input);

    /// <summary>
    /// Map using <see cref="Mapster"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public string Mapster()
        => this.input.Adapt<string>();

    /// <summary>
    /// Map using <see cref="Mappa.Attributes"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public string Mappa()
        => this.mappaMapper.MapToString(this.input);
}