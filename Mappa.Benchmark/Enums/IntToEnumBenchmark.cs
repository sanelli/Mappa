// <copyright file="IntToEnumBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Enums.Mappers;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Enums;

/// <summary>
/// Benchmark to test the <see cref="int"/>
/// to <see cref="Enum"/> mapper.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class IntToEnumBenchmark
#pragma warning restore CA1515
{
    private readonly int input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntToEnumBenchmark"/> class.
    /// </summary>
    public IntToEnumBenchmark()
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
        this.input = EnumDataFactory.CreateStringComparisonValue();
    }

    /// <summary>
    /// Map using <see cref="AutoMapper"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark(Baseline = true)]
    public StringComparison Automapper()
        => this.automapperMapper.Map<StringComparison>(this.input);

    /// <summary>
    /// Map using <see cref="Riok.Mapperly"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public StringComparison Mapperly()
        => this.mapperlyMapper.Map(this.input);

    /// <summary>
    /// Map using <see cref="Mapster"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public StringComparison Mapster()
        => this.input.Adapt<StringComparison>();

    /// <summary>
    /// Map using <see cref="Mappa.Attributes"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public StringComparison Mappa()
        => this.mappaMapper.Map(this.input);
}