// <copyright file="NestedDataFactory.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Bogus;

using Mappa.Benchmark.Common;
using Mappa.Benchmark.Nested.Models;

namespace Mappa.Benchmark.Nested;

/// <summary>
/// Builds deterministic nested DTO graphs with Bogus (fixed seed) and mixed collections (~100 entries).
/// </summary>
internal static class NestedDataFactory
{
    /// <summary>
    /// Creates an <see cref="NestedOrderDto"/> with populated nested collections.
    /// </summary>
    /// <returns>The order DTO.</returns>
    public static NestedOrderDto CreateNestedOrder()
    {
        BenchmarkSeed.Apply();

        var size = BenchmarkConstants.NestedCollectionSize;
        var attributeCount = BenchmarkConstants.AttributesPerItem;

        var coordinateFaker = new Faker<CoordinateDto>()
            .StrictMode(true)
            .RuleFor(coordinate => coordinate.Latitude, faker => faker.Address.Latitude())
            .RuleFor(coordinate => coordinate.Longitude, faker => faker.Address.Longitude());

        var geoRegionFaker = new Faker<GeoRegionDto>()
            .StrictMode(true)
            .RuleFor(region => region.Name, faker => faker.Address.County())
            .RuleFor(region => region.CountryCode, faker => faker.Address.CountryCode())
            .RuleFor(region => region.Center, _ => coordinateFaker.Generate());

        var addressFaker = new Faker<AddressDto>()
            .StrictMode(true)
            .RuleFor(address => address.Street, faker => faker.Address.StreetAddress())
            .RuleFor(address => address.City, faker => faker.Address.City())
            .RuleFor(address => address.Region, _ => geoRegionFaker.Generate());

        var personPartyFaker = new Faker<PersonPartyDto>()
            .StrictMode(true)
            .RuleFor(party => party.DisplayName, faker => faker.Person.FullName)
            .RuleFor(party => party.FirstName, faker => faker.Person.FirstName)
            .RuleFor(party => party.LastName, faker => faker.Person.LastName);

        var organizationPartyFaker = new Faker<OrganizationPartyDto>()
            .StrictMode(true)
            .RuleFor(party => party.DisplayName, faker => faker.Company.CompanyName())
            .RuleFor(party => party.RegistrationNumber, faker => faker.Random.AlphaNumeric(10));

        var customerFaker = new Faker<CustomerDto>()
            .StrictMode(true)
            .RuleFor(customer => customer.Name, faker => faker.Person.FullName)
            .RuleFor(customer => customer.Email, faker => faker.Internet.Email())
            .RuleFor(customer => customer.Preferences, faker => CreateBoolDictionary(faker, attributeCount))
            .RuleFor(
                customer => customer.Party,
                faker => faker.Random.Bool()
                    ? personPartyFaker.Generate()
                    : organizationPartyFaker.Generate())
            .RuleFor(customer => customer.Address, _ => addressFaker.Generate());

        var physicalLineItemFaker = new Faker<PhysicalLineItemDto>()
            .StrictMode(true)
            .RuleFor(item => item.Sku, faker => faker.Commerce.Ean13())
            .RuleFor(item => item.Quantity, faker => faker.Random.Int(1, 20))
            .RuleFor(item => item.Attributes, faker => CreateStringDictionary(faker, attributeCount))
            .RuleFor(item => item.WeightKg, faker => faker.Random.Double(0.1, 25.0));

        var digitalLineItemFaker = new Faker<DigitalLineItemDto>()
            .StrictMode(true)
            .RuleFor(item => item.Sku, faker => faker.Commerce.Ean13())
            .RuleFor(item => item.Quantity, faker => faker.Random.Int(1, 20))
            .RuleFor(item => item.Attributes, faker => CreateStringDictionary(faker, attributeCount))
            .RuleFor(item => item.DownloadUrl, faker => faker.Internet.Url());

        var orderFaker = new Faker<NestedOrderDto>()
            .StrictMode(true)
            .RuleFor(order => order.Id, faker => faker.Random.Int(1, 100_000))
            .RuleFor(order => order.Title, faker => faker.Commerce.ProductName())
            .RuleFor(order => order.Status, faker => faker.PickRandom<NestedSourceStatus>())
            .RuleFor(order => order.ShippingMode, faker => faker.PickRandom<NestedShippingMode>())
            .RuleFor(order => order.Priority, faker => faker.PickRandom<NestedPriority>())
            .RuleFor(order => order.BillingStatus, faker => faker.PickRandom<NestedTargetStatus>().ToString())
            .RuleFor(order => order.ArchiveStatus, faker => (int)faker.PickRandom<NestedTargetStatus>())
            .RuleFor(order => order.Customer, _ => customerFaker.Generate())
            .RuleFor(order => order.LineItems, faker => CreateLineItems(faker, size, physicalLineItemFaker, digitalLineItemFaker))
            .RuleFor(order => order.Coupons, faker => faker.Make(size, () => faker.Commerce.Ean8()).ToArray())
            .RuleFor(order => order.Categories, faker => faker.Make(size, () => faker.Commerce.Categories(1)[0]).ToHashSet())
            .RuleFor(order => order.Metadata, faker => CreateStringDictionary(faker, size))
            .RuleFor(order => order.PendingSkus, faker => new Queue<string>(faker.Make(size, () => faker.Commerce.Ean13())))
            .RuleFor(order => order.RecentTags, faker => new Stack<string>(faker.Make(size, () => faker.Lorem.Word())))
            .RuleFor(order => order.Scores, faker => faker.Make(size, () => faker.Random.Int(0, 1000)).ToArray())
            .RuleFor(order => order.Weights, faker => faker.Make(size, () => faker.Random.Int(0, 1000)).ToArray())
            .RuleFor(order => order.Notes, faker => faker.Make(size, () => faker.Lorem.Sentence()));

        return orderFaker.Generate();
    }

    private static List<LineItemBaseDto> CreateLineItems(
        Faker faker,
        int size,
        Faker<PhysicalLineItemDto> physicalLineItemFaker,
        Faker<DigitalLineItemDto> digitalLineItemFaker)
    {
        var lineItems = new List<LineItemBaseDto>(size);
        for (var index = 0; index < size; index++)
        {
            lineItems.Add(faker.Random.Bool()
                ? physicalLineItemFaker.Generate()
                : digitalLineItemFaker.Generate());
        }

        return lineItems;
    }

    private static Dictionary<string, string> CreateStringDictionary(Faker faker, int count)
    {
        var dictionary = new Dictionary<string, string>(count);
        for (var index = 0; index < count; index++)
        {
            dictionary[$"{faker.Database.Column()}-{index}"] = faker.Lorem.Word();
        }

        return dictionary;
    }

    private static Dictionary<string, bool> CreateBoolDictionary(Faker faker, int count)
    {
        var dictionary = new Dictionary<string, bool>(count);
        for (var index = 0; index < count; index++)
        {
            dictionary[$"{faker.Database.Column()}-{index}"] = faker.Random.Bool();
        }

        return dictionary;
    }
}