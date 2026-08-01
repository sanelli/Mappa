// <copyright file="NestedDataFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Benchmark.Common;
using Mappa.Benchmark.Nested.Models;

namespace Mappa.Benchmark.Nested;

/// <summary>
/// Builds deterministic nested DTO graphs with mixed collections (~100 entries).
/// </summary>
internal static class NestedDataFactory
{
    /// <summary>
    /// Creates an <see cref="NestedOrderDto"/> with populated nested collections.
    /// </summary>
    /// <returns>The order DTO.</returns>
    public static NestedOrderDto CreateNestedOrder()
    {
        var size = BenchmarkConstants.NestedCollectionSize;
        var lineItems = new List<LineItemBaseDto>(size);
        var coupons = new string[size];
        var categories = new HashSet<string>(size);
        var metadata = new Dictionary<string, string>(size);
        var pendingSkus = new Queue<string>(size);
        var recentTags = new Stack<string>(size);
        var scores = new int[size];
        var weights = new int[size];
        var notes = new List<string>(size);
        var preferences = new Dictionary<string, bool>(BenchmarkConstants.AttributesPerItem);

        for (var index = 0; index < size; index++)
        {
            var attributes = new Dictionary<string, string>(BenchmarkConstants.AttributesPerItem);
            for (var attributeIndex = 0; attributeIndex < BenchmarkConstants.AttributesPerItem; attributeIndex++)
            {
                attributes[$"attr-{attributeIndex}"] = $"value-{index}-{attributeIndex}";
            }

            if (index % 2 == 0)
            {
                lineItems.Add(new PhysicalLineItemDto
                {
                    Sku = $"SKU-P-{index}",
                    Quantity = index % 10,
                    Attributes = attributes,
                    WeightKg = 0.5 + (index % 5),
                });
            }
            else
            {
                lineItems.Add(new DigitalLineItemDto
                {
                    Sku = $"SKU-D-{index}",
                    Quantity = index % 10,
                    Attributes = attributes,
                    DownloadUrl = $"https://example.com/dl/{index}",
                });
            }

            coupons[index] = $"COUPON-{index}";
            categories.Add($"category-{index}");
            metadata[$"meta-{index}"] = $"value-{index}";
            pendingSkus.Enqueue($"pending-{index}");
            recentTags.Push($"tag-{index}");
            scores[index] = index;
            weights[index] = size - index;
            notes.Add($"note-{index}");
        }

        for (var preferenceIndex = 0; preferenceIndex < BenchmarkConstants.AttributesPerItem; preferenceIndex++)
        {
            preferences[$"pref-{preferenceIndex}"] = preferenceIndex % 2 == 0;
        }

        return new NestedOrderDto
        {
            Id = 42,
            Title = "Benchmark Order",
            Status = NestedSourceStatus.Active,
            ShippingMode = NestedShippingMode.Express,
            Priority = NestedPriority.High,
            Customer = new CustomerDto
            {
                Name = "Ada Lovelace",
                Email = "ada@example.com",
                Preferences = preferences,
                Party = new PersonPartyDto
                {
                    DisplayName = "Ada Lovelace",
                    FirstName = "Ada",
                    LastName = "Lovelace",
                },
                Address = new AddressDto
                {
                    Street = "1 Analytical Engine Way",
                    City = "London",
                    Region = new GeoRegionDto
                    {
                        Name = "Greater London",
                        CountryCode = "GB",
                        Center = new CoordinateDto
                        {
                            Latitude = 51.5074,
                            Longitude = -0.1278,
                        },
                    },
                },
            },
            LineItems = lineItems,
            Coupons = coupons,
            Categories = categories,
            Metadata = metadata,
            PendingSkus = pendingSkus,
            RecentTags = recentTags,
            Scores = scores,
            Weights = weights,
            Notes = notes,
        };
    }
}