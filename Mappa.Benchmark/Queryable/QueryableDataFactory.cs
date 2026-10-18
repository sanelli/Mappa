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
    /// Creates a list of projection orders without nested children.
    /// </summary>
    /// <returns>The orders.</returns>
    public static List<ProjectionOrder> CreateOrders()
        => CreateOrders(includeChildren: false);

    /// <summary>
    /// Creates a list of projection orders with nested children.
    /// </summary>
    /// <returns>The orders.</returns>
    public static List<ProjectionOrder> CreateOrdersWithChildren()
        => CreateOrders(includeChildren: true);

    private static List<ProjectionOrder> CreateOrders(bool includeChildren)
    {
        BenchmarkSeed.Apply();
        var childFaker = new Faker<ProjectionChild>()
            .StrictMode(true)
            .RuleFor(child => child.Id, faker => faker.Random.Int(1, 1_000_000))
            .RuleFor(child => child.Label, faker => faker.Commerce.ProductAdjective());

        return new Faker<ProjectionOrder>()
            .StrictMode(true)
            .RuleFor(order => order.Id, faker => faker.Random.Int(1, 1_000_000))
            .RuleFor(order => order.Name, faker => faker.Commerce.ProductName())
            .RuleFor(order => order.CustomerName, faker => faker.Person.FullName)
            .RuleFor(
                order => order.Children,
                _ => includeChildren ? childFaker.Generate(3) : [])
            .Generate(BenchmarkConstants.CollectionSize);
    }
}