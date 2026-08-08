// <copyright file="ReferenceReusingSharedDagBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.ReferenceHandling.Mappers;
using Mappa.Benchmark.ReferenceHandling.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.ReferenceHandling;

/// <summary>
/// Benchmarks mapping a DAG where left and right children share one source instance.
/// Complements the cycle bench by measuring shared-reference reuse without a cycle.
/// Not part of the SVG chart subset.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class ReferenceReusingSharedDagBenchmark
#pragma warning restore CA1515
{
    private readonly RootSource input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceReusingSharedDagBenchmark"/> class.
    /// </summary>
    public ReferenceReusingSharedDagBenchmark()
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
        this.input = ReferenceHandlingDataFactory.CreateSharedDag();

        TypeAdapterConfig<RootSource, RootTarget>.NewConfig().PreserveReference(true);
        TypeAdapterConfig<NodeSource, NodeTarget>.NewConfig().PreserveReference(true);
    }

    /// <summary>
    /// Map using AutoMapper with preserve-references.
    /// </summary>
    /// <returns>The mapped root.</returns>
    [Benchmark(Baseline = true)]
    public RootTarget Automapper()
        => this.automapperMapper.Map<RootTarget>(this.input);

    /// <summary>
    /// Map using Mapperly with reference handling.
    /// </summary>
    /// <returns>The mapped root.</returns>
    [Benchmark]
    public RootTarget Mapperly()
        => this.mapperlyMapper.MapRoot(this.input);

    /// <summary>
    /// Map using Mapster with preserve-reference.
    /// </summary>
    /// <returns>The mapped root.</returns>
    [Benchmark]
    public RootTarget Mapster()
        => this.input.Adapt<RootTarget>();

    /// <summary>
    /// Map using Mappa with <c>ReferenceReusing</c>.
    /// </summary>
    /// <returns>The mapped root.</returns>
    [Benchmark]
    public RootTarget Mappa()
        => this.mappaMapper.MapRoot(this.input, new MappaContext());
}