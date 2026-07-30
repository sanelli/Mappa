// <copyright file="IQueryableProjectionBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AutoMapper.QueryableExtensions;

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Common;
using Mappa.Benchmark.Queryable.Mappers;
using Mappa.Benchmark.Queryable.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Queryable;

/// <summary>
/// Benchmark IQueryable projection with forced enumeration.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1002
#pragma warning disable CA1515
#pragma warning disable S101
public class IQueryableProjectionBenchmark
#pragma warning restore S101
#pragma warning restore CA1515
{
    private readonly IQueryable<ProjectionOrder> input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="IQueryableProjectionBenchmark"/> class.
    /// </summary>
    public IQueryableProjectionBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddMaps(typeof(AutomapperMapperProfile)),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();

        var orders = new List<ProjectionOrder>(BenchmarkConstants.CollectionSize);
        for (var index = 0; index < BenchmarkConstants.CollectionSize; index++)
        {
            orders.Add(new ProjectionOrder
            {
                Id = index,
                Name = $"Order-{index}",
                CustomerName = $"Customer-{index}",
            });
        }

        this.input = orders.AsQueryable();
        TypeAdapterConfig<ProjectionOrder, ProjectionOrderDto>.NewConfig()
            .Map(destination => destination.Title, source => source.Name);
    }

    /// <summary>
    /// Map using AutoMapper ProjectTo.
    /// </summary>
    /// <returns>The projected list.</returns>
    [Benchmark(Baseline = true)]
    public List<ProjectionOrderDto> Automapper()
        => this.input.ProjectTo<ProjectionOrderDto>(this.automapperMapper.ConfigurationProvider).ToList();

    /// <summary>
    /// Map using Mapperly projection.
    /// </summary>
    /// <returns>The projected list.</returns>
    [Benchmark]
    public List<ProjectionOrderDto> Mapperly()
        => this.mapperlyMapper.Project(this.input).ToList();

    /// <summary>
    /// Map using Mapster ProjectToType.
    /// </summary>
    /// <returns>The projected list.</returns>
    [Benchmark]
    public List<ProjectionOrderDto> Mapster()
        => this.input.ProjectToType<ProjectionOrderDto>().ToList();

    /// <summary>
    /// Map using Mappa projection.
    /// </summary>
    /// <returns>The projected list.</returns>
    [Benchmark]
    public List<ProjectionOrderDto> Mappa()
        => this.mappaMapper.Project(this.input).ToList();
}