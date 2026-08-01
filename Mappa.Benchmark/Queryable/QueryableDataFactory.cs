// <copyright file="QueryableDataFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Bogus;

using Mappa.Benchmark.Common;
using Mappa.Benchmark.Queryable.Models;

namespace Mappa.Benchmark.Queryable;

/// <summary>
/// Builds deterministic IQueryable projection inputs with Bogus (fixed seed).
/// </summary>
internal static class QueryableDataFactory
{
    /// <summary>
    /// Creates a list of projection orders.
    /// </summary>
    /// <returns>The orders.</returns>
    public static List<ProjectionOrder> CreateOrders()
    {
        BenchmarkSeed.Apply();
        return new Faker<ProjectionOrder>()
            .StrictMode(true)
            .RuleFor(order => order.Id, faker => faker.Random.Int(1, 1_000_000))
            .RuleFor(order => order.Name, faker => faker.Commerce.ProductName())
            .RuleFor(order => order.CustomerName, faker => faker.Person.FullName)
            .Generate(BenchmarkConstants.CollectionSize);
    }
}