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
    private const int Input = (int)StringComparison.InvariantCulture;

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
                    cfg.AddMaps(typeof(AutomapperMapperProfile));
                },
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000

        this.mapperlyMapper = new();

        this.mappaMapper = new();
    }

    /// <summary>
    /// Map using <see cref="AutoMapper"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark(Baseline = true)]
    public StringComparison Automapper()
        => this.automapperMapper.Map<StringComparison>(Input);

    /// <summary>
    /// Map using <see cref="Riok.Mapperly"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public StringComparison Mapperly()
        => this.mapperlyMapper.Map(Input);

    /// <summary>
    /// Map using <see cref="Mapster"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
#pragma warning disable CA1822 // Member 'Mapster' does not access instance data and can be marked as static
    public StringComparison Mapster()
#pragma warning restore CA1822
        => Input.Adapt<StringComparison>();

    /// <summary>
    /// Map using <see cref="Mappa.Attributes"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public StringComparison Mappa()
        => this.mappaMapper.Map(Input);
}