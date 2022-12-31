// <copyright file="MapStringToStringBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Mappers;
using Mappa.Samples;

using Mapster;

namespace Mappa.Benchmark.Benchmarks;

/// <summary>
/// The benchmark to map <see cref="string"/> to <see cref="string"/>.
/// </summary>
[MemoryDiagnoser]
public class MapStringToStringBenchmark
{
    private const string InputString = "This is the input message";
    private readonly IdentityStrategyMapper mappaMapper;
    private readonly AutoMapper.IMapper automapperMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapStringToStringBenchmark"/> class.
    /// </summary>
    public MapStringToStringBenchmark()
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
    public string MappaWithNullableDisabled()
    {
        return this.mappaMapper.MapStringToStringWhenNullableIsDisabled(InputString);
    }
#nullable restore

#nullable enable
    /// <summary>
    /// Uses the Mappa mapper when nullable is enabled.
    /// </summary>
    /// <returns>The mapped object.</returns>
    [Benchmark]
    public string? MappaWithNullableEnabled()
    {
        return this.mappaMapper.MapStringToStringWhenNullableIsEnabled(InputString);
    }
#nullable restore

    /// <summary>
    /// Uses the AutoMapper.
    /// </summary>
    /// <returns>The mapped object.</returns>
    [Benchmark]
    public string AutoMapper()
    {
        return this.automapperMapper.Map<string>(InputString);
    }

    /// <summary>
    /// Uses the Mapster mapper.
    /// </summary>
    /// <returns>The mapped object.</returns>
    [Benchmark]
#pragma warning disable CA1822
    public string Mapster()
#pragma warning restore CA1822
    {
        return InputString.Adapt<string>();
    }
}