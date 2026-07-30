// <copyright file="ClassToClassBenchmark.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using BenchmarkDotNet.Attributes;

using Mappa.Benchmark.Objects.Mappers;
using Mappa.Benchmark.Objects.Models;

using Mapster;

using Microsoft.Extensions.Logging.Abstractions;

namespace Mappa.Benchmark.Objects;

/// <summary>
/// Benchmark mapping class DTOs to class models.
/// </summary>
[MemoryDiagnoser]
#pragma warning disable CA1515
public class ClassToClassBenchmark
#pragma warning restore CA1515
{
    private readonly PersonClassDto input;
    private readonly AutoMapper.IMapper automapperMapper;
    private readonly MapperlyMapper mapperlyMapper;
    private readonly MappaMapper mappaMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassToClassBenchmark"/> class.
    /// </summary>
    public ClassToClassBenchmark()
    {
        this.automapperMapper = new AutoMapper.MapperConfiguration(
            cfg => cfg.AddMaps(typeof(AutomapperMapperProfile)),
#pragma warning disable CA2000
            new NullLoggerFactory()).CreateMapper();
#pragma warning restore CA2000
        this.mapperlyMapper = new();
        this.mappaMapper = new();
        this.input = new PersonClassDto
        {
            Id = 1,
            Name = "Ada Lovelace",
            Age = 36,
            Address = new AddressDto { Street = "Analytical Engine Way", City = "London", Zip = "SW1A" },
        };
    }

    /// <summary>
    /// Map using AutoMapper.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark(Baseline = true)]
    public PersonClass Automapper()
        => this.automapperMapper.Map<PersonClass>(this.input);

    /// <summary>
    /// Map using Mapperly.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark]
    public PersonClass Mapperly()
        => this.mapperlyMapper.Map(this.input);

    /// <summary>
    /// Map using Mapster.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark]
    public PersonClass Mapster()
        => this.input.Adapt<PersonClass>();

    /// <summary>
    /// Map using Mappa.
    /// </summary>
    /// <returns>The mapped model.</returns>
    [Benchmark]
    public PersonClass Mappa()
        => this.mappaMapper.Map(this.input);
}