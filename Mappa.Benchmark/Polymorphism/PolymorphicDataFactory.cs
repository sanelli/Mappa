// <copyright file="PolymorphicDataFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Bogus;

using Mappa.Benchmark.Common;
using Mappa.Benchmark.Polymorphism.Models;

namespace Mappa.Benchmark.Polymorphism;

/// <summary>
/// Builds deterministic polymorphic inputs with Bogus (fixed seed).
/// </summary>
internal static class PolymorphicDataFactory
{
    /// <summary>
    /// Creates a polymorphic animal DTO (dog, cat, or bird).
    /// </summary>
    /// <returns>The animal DTO.</returns>
    public static AnimalDto CreateAnimalDto()
    {
        BenchmarkSeed.Apply();
        var faker = new Faker();
        return faker.Random.Int(0, 2) switch
        {
            0 => new Faker<DogDto>()
                .StrictMode(true)
                .RuleFor(dog => dog.Name, f => f.Name.FirstName())
                .RuleFor(dog => dog.Trained, f => f.Random.Bool())
                .Generate(),
            1 => new Faker<CatDto>()
                .StrictMode(true)
                .RuleFor(cat => cat.Name, f => f.Name.FirstName())
                .RuleFor(cat => cat.Lives, f => f.Random.Int(1, 9))
                .Generate(),
            _ => new Faker<BirdDto>()
                .StrictMode(true)
                .RuleFor(bird => bird.Name, f => f.Name.FirstName())
                .RuleFor(bird => bird.CanFly, f => f.Random.Bool())
                .Generate(),
        };
    }
}