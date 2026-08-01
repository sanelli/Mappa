// <copyright file="ObjectDataFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Bogus;

using Mappa.Benchmark.Common;
using Mappa.Benchmark.Objects.Models;

namespace Mappa.Benchmark.Objects;

/// <summary>
/// Builds deterministic object-graph inputs with Bogus (fixed seed).
/// </summary>
internal static class ObjectDataFactory
{
    /// <summary>
    /// Creates a class-based person DTO.
    /// </summary>
    /// <returns>The person DTO.</returns>
    public static PersonClassDto CreatePersonClassDto()
    {
        BenchmarkSeed.Apply();
        return CreatePersonClassFaker().Generate();
    }

    /// <summary>
    /// Creates a record-based person DTO.
    /// </summary>
    /// <returns>The person DTO.</returns>
    public static PersonRecordDto CreatePersonRecordDto()
    {
        BenchmarkSeed.Apply();
        var address = CreateAddressFaker().Generate();
        return new Faker<PersonRecordDto>()
            .CustomInstantiator(faker => new PersonRecordDto(
                faker.Random.Int(1, 1_000_000),
                faker.Person.FullName,
                faker.Random.Int(18, 90),
                address))
            .Generate();
    }

    /// <summary>
    /// Creates a struct-based person DTO.
    /// </summary>
    /// <returns>The person DTO.</returns>
    public static PersonStructDto CreatePersonStructDto()
    {
        BenchmarkSeed.Apply();
        var faker = new Faker();
        return new PersonStructDto
        {
            Id = faker.Random.Int(1, 1_000_000),
            Name = faker.Person.FullName,
            Age = faker.Random.Int(18, 90),
            Street = faker.Address.StreetAddress(),
            City = faker.Address.City(),
            Zip = faker.Address.ZipCode(),
        };
    }

    private static Faker<AddressDto> CreateAddressFaker()
    {
        return new Faker<AddressDto>()
            .StrictMode(true)
            .RuleFor(address => address.Street, faker => faker.Address.StreetAddress())
            .RuleFor(address => address.City, faker => faker.Address.City())
            .RuleFor(address => address.Zip, faker => faker.Address.ZipCode());
    }

    private static Faker<PersonClassDto> CreatePersonClassFaker()
    {
        var addressFaker = CreateAddressFaker();
        return new Faker<PersonClassDto>()
            .StrictMode(true)
            .RuleFor(person => person.Id, faker => faker.Random.Int(1, 1_000_000))
            .RuleFor(person => person.Name, faker => faker.Person.FullName)
            .RuleFor(person => person.Age, faker => faker.Random.Int(18, 90))
            .RuleFor(person => person.Address, _ => addressFaker.Generate());
    }
}