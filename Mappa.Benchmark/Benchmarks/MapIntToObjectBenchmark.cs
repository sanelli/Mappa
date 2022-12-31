// <copyright file="MapIntToObjectBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Mappers;
using Mappa.Samples;

using Mapster;

namespace Mappa.Benchmark.Benchmarks;

/// <summary>
/// The benchmark to map <see cref="int"/> to <see cref="object"/>.
/// </summary>
[MemoryDiagnoser]
public class MapIntToObjectBenchmark
{
    private const int InputInteger = 17;
    private readonly IdentityStrategyMapper mappaMapper;
    private readonly AutoMapper.IMapper automapperMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapIntToObjectBenchmark"/> class.
    /// </summary>
    public MapIntToObjectBenchmark()
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
    public object MappaWithNullableDisabled()
    {
        return this.mappaMapper.MapIntToObjectWhenNullableIsDisabled(InputInteger);
    }
#nullable restore

#nullable enable
    /// <summary>
    /// Uses the Mappa mapper when nullable is enabled.
    /// </summary>
    /// <returns>The mapped object.</returns>
    [Benchmark(Baseline = true)]
    public object? MappaWithNullableEnabled()
    {
        return this.mappaMapper.MapIntToNullableObjectWhenNullableIsEnabled(InputInteger);
    }
#nullable restore

    /// <summary>
    /// Uses the AutoMapper.
    /// </summary>
    /// <returns>The mapped object.</returns>
    [Benchmark]
    public object AutoMapper()
    {
        return this.automapperMapper.Map<object>(InputInteger);
    }

    /// <summary>
    /// Uses the Mapster mapper.
    /// </summary>
    /// <returns>The mapped object.</returns>
    [Benchmark]
#pragma warning disable CA1822
    public object Mapster()
#pragma warning restore CA1822
    {
        return InputInteger.Adapt<object>();
    }
}