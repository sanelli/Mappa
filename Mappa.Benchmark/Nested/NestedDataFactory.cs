// <copyright file="NestedDataFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.Common;
using Mappa.Benchmark.Nested.Models;

namespace Mappa.Benchmark.Nested;

/// <summary>
/// Builds deterministic nested DTO graphs with a few hundred collection entries.
/// </summary>
internal static class NestedDataFactory
{
    /// <summary>
    /// Creates an <see cref="NestedOrderDto"/> with populated nested collections.
    /// </summary>
    /// <returns>The order DTO.</returns>
    public static NestedOrderDto CreateNestedOrder()
    {
        var lineItems = new List<LineItemDto>(BenchmarkConstants.CollectionSize);
        var coupons = new string[BenchmarkConstants.CollectionSize];
        var categories = new HashSet<string>(BenchmarkConstants.CollectionSize);
        var metadata = new Dictionary<string, string>(BenchmarkConstants.CollectionSize);
        var preferences = new Dictionary<string, bool>(BenchmarkConstants.AttributesPerItem);

        for (var index = 0; index < BenchmarkConstants.CollectionSize; index++)
        {
            var attributes = new Dictionary<string, string>(BenchmarkConstants.AttributesPerItem);
            for (var attributeIndex = 0; attributeIndex < BenchmarkConstants.AttributesPerItem; attributeIndex++)
            {
                attributes[$"attr-{attributeIndex}"] = $"value-{index}-{attributeIndex}";
            }

            lineItems.Add(new LineItemDto
            {
                Sku = $"SKU-{index}",
                Quantity = index % 10,
                Attributes = attributes,
            });
            coupons[index] = $"COUPON-{index}";
            categories.Add($"category-{index}");
            metadata[$"meta-{index}"] = $"value-{index}";
        }

        for (var preferenceIndex = 0; preferenceIndex < BenchmarkConstants.AttributesPerItem; preferenceIndex++)
        {
            preferences[$"pref-{preferenceIndex}"] = preferenceIndex % 2 == 0;
        }

        return new NestedOrderDto
        {
            Id = 42,
            Title = "Benchmark Order",
            Customer = new CustomerDto
            {
                Name = "Ada Lovelace",
                Email = "ada@example.com",
                Preferences = preferences,
            },
            LineItems = lineItems,
            Coupons = coupons,
            Categories = categories,
            Metadata = metadata,
        };
    }
}