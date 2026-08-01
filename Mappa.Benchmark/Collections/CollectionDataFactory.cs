// <copyright file="CollectionDataFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Bogus;

using Mappa.Benchmark.Collections.Models;
using Mappa.Benchmark.Common;

namespace Mappa.Benchmark.Collections;

/// <summary>
/// Builds deterministic collection inputs with Bogus (fixed seed).
/// </summary>
internal static class CollectionDataFactory
{
    /// <summary>
    /// Creates a list of <see cref="CollectionItemDto"/> with nested dictionaries.
    /// </summary>
    /// <returns>The list.</returns>
    public static List<CollectionItemDto> CreateList()
    {
        BenchmarkSeed.Apply();
        return CreateItemFaker().Generate(BenchmarkConstants.CollectionSize);
    }

    /// <summary>
    /// Creates an array of <see cref="CollectionItemDto"/> with nested dictionaries.
    /// </summary>
    /// <returns>The array.</returns>
    public static CollectionItemDto[] CreateArray()
    {
        BenchmarkSeed.Apply();
        return CreateItemFaker().Generate(BenchmarkConstants.CollectionSize).ToArray();
    }

    /// <summary>
    /// Creates a dictionary of <see cref="CollectionItemDto"/> with nested dictionaries.
    /// </summary>
    /// <returns>The dictionary.</returns>
    public static Dictionary<string, CollectionItemDto> CreateDictionary()
    {
        BenchmarkSeed.Apply();
        var faker = new Faker();
        var itemFaker = CreateItemFaker();
        var dictionary = new Dictionary<string, CollectionItemDto>(BenchmarkConstants.CollectionSize);
        for (var index = 0; index < BenchmarkConstants.CollectionSize; index++)
        {
            dictionary[$"{faker.Random.AlphaNumeric(8)}-{index}"] = itemFaker.Generate();
        }

        return dictionary;
    }

    /// <summary>
    /// Creates a list of integers.
    /// </summary>
    /// <returns>The list.</returns>
    public static List<int> CreateIntList()
    {
        BenchmarkSeed.Apply();
        var faker = new Faker();
        return faker.Make(BenchmarkConstants.CollectionSize, () => faker.Random.Int(0, 10_000)).ToList();
    }

    /// <summary>
    /// Creates an integer array.
    /// </summary>
    /// <returns>The array.</returns>
    public static int[] CreateIntArray()
    {
        BenchmarkSeed.Apply();
        var faker = new Faker();
        return faker.Make(BenchmarkConstants.CollectionSize, () => faker.Random.Int(0, 10_000)).ToArray();
    }

    private static Faker<CollectionItemDto> CreateItemFaker()
    {
        return new Faker<CollectionItemDto>()
            .StrictMode(true)
            .RuleFor(item => item.Id, faker => faker.Random.Int(1, 1_000_000))
            .RuleFor(item => item.Name, faker => faker.Commerce.ProductName())
            .RuleFor(item => item.Attributes, faker => CreateAttributes(faker));
    }

    private static Dictionary<string, string> CreateAttributes(Faker faker)
    {
        var attributes = new Dictionary<string, string>(BenchmarkConstants.AttributesPerItem);
        for (var attributeIndex = 0; attributeIndex < BenchmarkConstants.AttributesPerItem; attributeIndex++)
        {
            attributes[$"{faker.Database.Column()}-{attributeIndex}"] = faker.Lorem.Word();
        }

        return attributes;
    }
}