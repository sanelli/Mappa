// <copyright file="NestedModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1002, CA1515, CA1724, CA1815, CA1819, CA2227, SA1201, SA1402, SA1649

namespace Mappa.Benchmark.Nested.Models;

/// <summary>
/// Nested line-item DTO with a dictionary of attributes.
/// </summary>
public sealed class LineItemDto
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
/// Nested line-item target.
/// </summary>
public sealed class LineItem
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
/// Customer DTO nested under an order.
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
}

/// <summary>
/// Customer target nested under an order.
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
}

/// <summary>
/// Order DTO containing lists, arrays, hash sets, and dictionaries.
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
    /// Gets or sets the customer.
    /// </summary>
    public CustomerDto Customer { get; set; } = new();

    /// <summary>
    /// Gets or sets line items (list).
    /// </summary>
    public List<LineItemDto> LineItems { get; set; } = new();

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
}

/// <summary>
/// Order target containing lists, arrays, hash sets, and dictionaries.
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
    /// Gets or sets the customer.
    /// </summary>
    public Customer Customer { get; set; } = new();

    /// <summary>
    /// Gets or sets line items (list).
    /// </summary>
    public List<LineItem> LineItems { get; set; } = new();

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
}