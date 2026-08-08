// <copyright file="ReferenceReusingCycleBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.ReferenceHandling.Mappers;
using Mappa.Benchmark.ReferenceHandling.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.ReferenceHandling;

/// <summary>
/// Benchmarks mapping a closed Person↔Address cycle with reference preservation enabled.
/// Worth including: exercises <c>ReferenceReusing</c> / equivalent competitor features on a real cycle
/// (not covered by existing nested-DTO or collection benches). Not part of the SVG chart subset.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class ReferenceReusingCycleBenchmark
#pragma warning restore CA1515
{
    private readonly PersonSource input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceReusingCycleBenchmark"/> class.
    /// </summary>
    public ReferenceReusingCycleBenchmark()
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
        this.input = ReferenceHandlingDataFactory.CreateClosedCycle();

        TypeAdapterConfig<PersonSource, PersonTarget>.NewConfig().PreserveReference(true);
        TypeAdapterConfig<AddressSource, AddressTarget>.NewConfig().PreserveReference(true);
    }

    /// <summary>
    /// Map using AutoMapper with preserve-references.
    /// </summary>
    /// <returns>The mapped person.</returns>
    [Benchmark(Baseline = true)]
    public PersonTarget Automapper()
        => this.automapperMapper.Map<PersonTarget>(this.input);

    /// <summary>
    /// Map using Mapperly with reference handling.
    /// </summary>
    /// <returns>The mapped person.</returns>
    [Benchmark]
    public PersonTarget Mapperly()
        => this.mapperlyMapper.MapPerson(this.input);

    /// <summary>
    /// Map using Mapster with preserve-reference.
    /// </summary>
    /// <returns>The mapped person.</returns>
    [Benchmark]
    public PersonTarget Mapster()
        => this.input.Adapt<PersonTarget>();

    /// <summary>
    /// Map using Mappa with <c>ReferenceReusing</c>.
    /// </summary>
    /// <returns>The mapped person.</returns>
    [Benchmark]
    public PersonTarget Mappa()
        => this.mappaMapper.MapPerson(this.input, new MappaContext());
}