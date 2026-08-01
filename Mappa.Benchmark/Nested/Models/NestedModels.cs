// <copyright file="NestedModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1002, CA1056, CA1515, CA1711, CA1724, CA1815, CA1819, CA2227, SA1201, SA1402, SA1649

namespace Mappa.Benchmark.Nested.Models;

/// <summary>
/// Source order status (maps to <see cref="NestedTargetStatus"/>).
/// </summary>
public enum NestedSourceStatus
{
    /// <summary>
    /// Unknown status.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Active status.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Inactive status.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Archived status.
    /// </summary>
    Archived = 3,
}

/// <summary>
/// Target order status.
/// </summary>
public enum NestedTargetStatus
{
    /// <summary>
    /// Unknown status.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Active status.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Inactive status.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Archived status.
    /// </summary>
    Archived = 3,
}

/// <summary>
/// Source shipping mode (maps to string).
/// </summary>
public enum NestedShippingMode
{
    /// <summary>
    /// Standard shipping.
    /// </summary>
    Standard = 0,

    /// <summary>
    /// Express shipping.
    /// </summary>
    Express = 1,

    /// <summary>
    /// Overnight shipping.
    /// </summary>
    Overnight = 2,
}

/// <summary>
/// Source priority (maps to int).
/// </summary>
public enum NestedPriority
{
    /// <summary>
    /// Low priority.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Normal priority.
    /// </summary>
    Normal = 1,

    /// <summary>
    /// High priority.
    /// </summary>
    High = 2,
}

/// <summary>
/// Coordinate DTO (nesting level 5).
/// </summary>
public sealed class CoordinateDto
{
    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    public double Longitude { get; set; }
}

/// <summary>
/// Coordinate target (nesting level 5).
/// </summary>
public sealed class Coordinate
{
    /// <summary>
    /// Gets or sets the latitude.
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude.
    /// </summary>
    public double Longitude { get; set; }
}

/// <summary>
/// Geo region DTO (nesting level 4).
/// </summary>
public sealed class GeoRegionDto
{
    /// <summary>
    /// Gets or sets the region name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the region center.
    /// </summary>
    public CoordinateDto Center { get; set; } = new();
}

/// <summary>
/// Geo region target (nesting level 4).
/// </summary>
public sealed class GeoRegion
{
    /// <summary>
    /// Gets or sets the region name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the region center.
    /// </summary>
    public Coordinate Center { get; set; } = new();
}

/// <summary>
/// Address DTO (nesting level 3).
/// </summary>
public sealed class AddressDto
{
    /// <summary>
    /// Gets or sets the street.
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the geo region.
    /// </summary>
    public GeoRegionDto Region { get; set; } = new();
}

/// <summary>
/// Address target (nesting level 3).
/// </summary>
public sealed class Address
{
    /// <summary>
    /// Gets or sets the street.
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the geo region.
    /// </summary>
    public GeoRegion Region { get; set; } = new();
}

/// <summary>
/// Polymorphic party source base.
/// </summary>
public abstract class PartyDto
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>
/// Person party source.
/// </summary>
public sealed class PersonPartyDto : PartyDto
{
    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Organization party source.
/// </summary>
public sealed class OrganizationPartyDto : PartyDto
{
    /// <summary>
    /// Gets or sets the registration number.
    /// </summary>
    public string RegistrationNumber { get; set; } = string.Empty;
}

/// <summary>
/// Polymorphic party target base.
/// </summary>
public abstract class Party
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>
/// Person party target.
/// </summary>
public sealed class PersonParty : Party
{
    /// <summary>
    /// Gets or sets the first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Organization party target.
/// </summary>
public sealed class OrganizationParty : Party
{
    /// <summary>
    /// Gets or sets the registration number.
    /// </summary>
    public string RegistrationNumber { get; set; } = string.Empty;
}

/// <summary>
/// Customer DTO (nesting level 2).
/// </summary>
public sealed class CustomerDto
{
    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets preference flags keyed by name.
    /// </summary>
    public Dictionary<string, bool> Preferences { get; set; } = new();

    /// <summary>
    /// Gets or sets the polymorphic party.
    /// </summary>
    public PartyDto Party { get; set; } = new PersonPartyDto();

    /// <summary>
    /// Gets or sets the billing address.
    /// </summary>
    public AddressDto Address { get; set; } = new();
}

/// <summary>
/// Customer target (nesting level 2).
/// </summary>
public sealed class Customer
{
    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets preference flags keyed by name.
    /// </summary>
    public Dictionary<string, bool> Preferences { get; set; } = new();

    /// <summary>
    /// Gets or sets the polymorphic party.
    /// </summary>
    public Party Party { get; set; } = new PersonParty();

    /// <summary>
    /// Gets or sets the billing address.
    /// </summary>
    public Address Address { get; set; } = new();
}

/// <summary>
/// Polymorphic line-item source base.
/// </summary>
public abstract class LineItemBaseDto
{
    /// <summary>
    /// Gets or sets the SKU.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets item attributes.
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = new();
}

/// <summary>
/// Physical line-item source.
/// </summary>
public sealed class PhysicalLineItemDto : LineItemBaseDto
{
    /// <summary>
    /// Gets or sets the weight in kilograms.
    /// </summary>
    public double WeightKg { get; set; }
}

/// <summary>
/// Digital line-item source.
/// </summary>
public sealed class DigitalLineItemDto : LineItemBaseDto
{
    /// <summary>
    /// Gets or sets the download URL.
    /// </summary>
    public string DownloadUrl { get; set; } = string.Empty;
}

/// <summary>
/// Polymorphic line-item target base.
/// </summary>
public abstract class LineItemBase
{
    /// <summary>
    /// Gets or sets the SKU.
    /// </summary>
    public string Sku { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets item attributes.
    /// </summary>
    public Dictionary<string, string> Attributes { get; set; } = new();
}

/// <summary>
/// Physical line-item target.
/// </summary>
public sealed class PhysicalLineItem : LineItemBase
{
    /// <summary>
    /// Gets or sets the weight in kilograms.
    /// </summary>
    public double WeightKg { get; set; }
}

/// <summary>
/// Digital line-item target.
/// </summary>
public sealed class DigitalLineItem : LineItemBase
{
    /// <summary>
    /// Gets or sets the download URL.
    /// </summary>
    public string DownloadUrl { get; set; } = string.Empty;
}

/// <summary>
/// Order DTO containing deep nesting, polymorphism, and mixed collections.
/// </summary>
public sealed class NestedOrderDto
{
    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the order title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the order status (enum → different enum).
    /// </summary>
    public NestedSourceStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the shipping mode (enum → string).
    /// </summary>
    public NestedShippingMode ShippingMode { get; set; }

    /// <summary>
    /// Gets or sets the priority (enum → int).
    /// </summary>
    public NestedPriority Priority { get; set; }

    /// <summary>
    /// Gets or sets the customer.
    /// </summary>
    public CustomerDto Customer { get; set; } = new();

    /// <summary>
    /// Gets or sets polymorphic line items (list).
    /// </summary>
    public List<LineItemBaseDto> LineItems { get; set; } = new();

    /// <summary>
    /// Gets or sets coupon codes (array).
    /// </summary>
    public string[] Coupons { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets category tags (hash set).
    /// </summary>
    public HashSet<string> Categories { get; set; } = new();

    /// <summary>
    /// Gets or sets order-level metadata (dictionary).
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets pending SKUs (queue).
    /// </summary>
    public Queue<string> PendingSkus { get; set; } = new();

    /// <summary>
    /// Gets or sets recent tags (stack).
    /// </summary>
    public Stack<string> RecentTags { get; set; } = new();

    /// <summary>
    /// Gets or sets score values (<see cref="Memory{T}"/> → array).
    /// </summary>
    public Memory<int> Scores { get; set; }

    /// <summary>
    /// Gets or sets weight values (<see cref="ReadOnlyMemory{T}"/>).
    /// </summary>
    public ReadOnlyMemory<int> Weights { get; set; }

    /// <summary>
    /// Gets or sets free-form notes (maps into a get-only list on the target).
    /// </summary>
    public List<string> Notes { get; set; } = new();
}

/// <summary>
/// Order target containing deep nesting, polymorphism, and mixed collections.
/// </summary>
public sealed class NestedOrder
{
    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the order title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the order status.
    /// </summary>
    public NestedTargetStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the shipping mode as a string.
    /// </summary>
    public string ShippingMode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the priority as an integer.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets the customer.
    /// </summary>
    public Customer Customer { get; set; } = new();

    /// <summary>
    /// Gets or sets polymorphic line items (list).
    /// </summary>
    public List<LineItemBase> LineItems { get; set; } = new();

    /// <summary>
    /// Gets or sets coupon codes (array).
    /// </summary>
    public string[] Coupons { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets category tags (hash set).
    /// </summary>
    public HashSet<string> Categories { get; set; } = new();

    /// <summary>
    /// Gets or sets order-level metadata (dictionary).
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets pending SKUs (queue).
    /// </summary>
    public Queue<string> PendingSkus { get; set; } = new();

    /// <summary>
    /// Gets or sets recent tags (stack).
    /// </summary>
    public Stack<string> RecentTags { get; set; } = new();

    /// <summary>
    /// Gets or sets score values as an array.
    /// </summary>
    public int[] Scores { get; set; } = Array.Empty<int>();

    /// <summary>
    /// Gets or sets weight values.
    /// </summary>
    public ReadOnlyMemory<int> Weights { get; set; }

    /// <summary>
    /// Gets free-form notes (get-only list populated post-construction).
    /// </summary>
    public List<string> Notes { get; } = new();
}