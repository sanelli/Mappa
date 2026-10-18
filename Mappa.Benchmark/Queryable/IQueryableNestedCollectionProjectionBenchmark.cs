// <copyright file="IQueryableNestedCollectionProjectionBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper.QueryableExtensions;

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Queryable.Mappers;
using Mappa.Benchmark.Queryable.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Queryable;

/// <summary>
/// Benchmark IQueryable projection with nested collection mapping and forced enumeration.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1002
#pragma warning disable CA1515
#pragma warning disable S101
public class IQueryableNestedCollectionProjectionBenchmark
#pragma warning restore S101
#pragma warning restore CA1515
{
    private readonly IQueryable<ProjectionOrder> input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="IQueryableNestedCollectionProjectionBenchmark"/> class.
    /// </summary>
    public IQueryableNestedCollectionProjectionBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddProfile(new AutomapperMapperProfile()),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();

        this.input = QueryableDataFactory.CreateOrdersWithChildren().AsQueryable();
        TypeAdapterConfig<ProjectionOrder, ProjectionOrderDto>.NewConfig()
            .Map(destination => destination.Title, source => source.Name);
    }

    /// <summary>
    /// Map using AutoMapper ProjectTo (nested children).
    /// </summary>
    /// <returns>The projected list.</returns>
    [Benchmark(Baseline = true)]
    public List<ProjectionOrderDto> Automapper()
        => this.input.ProjectTo<ProjectionOrderDto>(this.automapperMapper.ConfigurationProvider).ToList();

    /// <summary>
    /// Map using Mapperly projection (nested children).
    /// </summary>
    /// <returns>The projected list.</returns>
    [Benchmark]
    public List<ProjectionOrderDto> Mapperly()
        => this.mapperlyMapper.Project(this.input).ToList();

    /// <summary>
    /// Map using Mapster ProjectToType (nested children).
    /// </summary>
    /// <returns>The projected list.</returns>
    [Benchmark]
    public List<ProjectionOrderDto> Mapster()
        => this.input.ProjectToType<ProjectionOrderDto>().ToList();

    /// <summary>
    /// Map using Mappa projection (nested children).
    /// </summary>
    /// <returns>The projected list.</returns>
    [Benchmark]
    public List<ProjectionOrderDto> Mappa()
        => this.mappaMapper.Project(this.input).ToList();
}