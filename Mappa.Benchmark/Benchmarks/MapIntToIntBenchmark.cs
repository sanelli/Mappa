// <copyright file="MapIntToIntBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Mappers;
using Mappa.Samples;

using Mapster;

namespace Mappa.Benchmark.Benchmarks;

/// <summary>
/// The benchmark to map <see cref="int"/> to <see cref="int"/>.
/// </summary>
[MemoryDiagnoser]
public class MapIntToIntBenchmark
{
    private const int InputInteger = 17;
    private readonly IdentityStrategyMapper mappaMapper;
    private readonly AutoMapper.IMapper automapperMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapIntToIntBenchmark"/> class.
    /// </summary>
    public MapIntToIntBenchmark()
    {
        // Create the Mappa mapper.
        this.mappaMapper = new();

        // Create the AutoMapper mapper.
        var autoMapperConfiguration = new AutoMapper
            .MapperConfiguration(mapperConfigurationExpression => mapperConfigurationExpression
                .AddProfile(typeof(AutoMapperProfile)));
        this.automapperMapper = autoMapperConfiguration.CreateMapper();
    }

#nullable disable
    /// <summary>
    /// Uses the Mappa mapper when nullable is disabled.
    /// </summary>
    /// <returns>The mapped object.</returns>
    [Benchmark(Baseline = true)]
    public int MappaWithNullableDisabled()
    {
        return this.mappaMapper.MapIntToIntWhenNullableIsDisabled(InputInteger);
    }
#nullable restore

    /// <summary>
    /// Uses the AutoMapper.
    /// </summary>
    /// <returns>The mapped object.</returns>
    [Benchmark]
    public int AutoMapper()
    {
        return this.automapperMapper.Map<int>(InputInteger);
    }

    /// <summary>
    /// Uses the Mapster mapper.
    /// </summary>
    /// <returns>The mapped object.</returns>
    [Benchmark]
#pragma warning disable CA1822
    public int Mapster()
#pragma warning restore CA1822
    {
        return InputInteger.Adapt<int>();
    }
}