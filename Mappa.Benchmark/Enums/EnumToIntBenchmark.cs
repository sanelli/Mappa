// <copyright file="EnumToIntBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Enums.Mappers;

using Mapster;

namespace Mappa.Benchmark.Enums;

/// <summary>
/// Benchmark to test the <see cref="Enum"/>
/// to <see cref="int"/> mapper.
/// </summary>
[MemoryDiagnoser]
internal class EnumToIntBenchmark
{
    private const StringComparison Input = StringComparison.InvariantCulture;

    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToIntBenchmark"/> class.
    /// </summary>
    public EnumToIntBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(AutomapperMapperProfile));
        }).CreateMapper();

        this.mapperlyMapper = new();

        this.mappaMapper = new();
    }

    /// <summary>
    /// Map using <see cref="AutoMapper"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public int Automapper()
        => this.automapperMapper.Map<int>(Input);

    /// <summary>
    /// Map using <see cref="Riok.Mapperly"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public int Mapperly()
        => this.mapperlyMapper.MapToInt(Input);

    /// <summary>
    /// Map using <see cref="Mapster"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
#pragma warning disable CA1822 // Member 'Mapster' does not access instance data and can be marked as static
    public int Mapster()
#pragma warning restore CA1822
        => Input.Adapt<int>();

    /// <summary>
    /// Map using <see cref="Mappa.Attributes"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark(Baseline = true)]
    public int Mappa()
        => this.mappaMapper.MapToInt(Input);
}