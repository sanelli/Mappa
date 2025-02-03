// <copyright file="EnumToStringBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Enums.Mappers;

using Mapster;

namespace Mappa.Benchmark.Enums;

/// <summary>
/// Benchmark to test the <see cref="Enum"/>
/// to <see cref="string"/> mapper.
/// </summary>
[MemoryDiagnoser]
internal class EnumToStringBenchmark
{
    private const StringComparison Input = StringComparison.InvariantCulture;

    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToStringBenchmark"/> class.
    /// </summary>
    public EnumToStringBenchmark()
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
    public string Automapper()
        => this.automapperMapper.Map<string>(Input);

    /// <summary>
    /// Map using <see cref="Riok.Mapperly"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
    public string Mapperly()
        => this.mapperlyMapper.MapToString(Input);

    /// <summary>
    /// Map using <see cref="Mapster"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark]
#pragma warning disable CA1822 // Member 'Mapster' does not access instance data and can be marked as static
    public string Mapster()
#pragma warning restore CA1822
        => Input.Adapt<string>();

    /// <summary>
    /// Map using <see cref="Mappa.Attributes"/>.
    /// </summary>
    /// <returns>The mapper model.</returns>
    [Benchmark(Baseline = true)]
    public string Mappa()
        => this.mappaMapper.MapToString(Input);
}