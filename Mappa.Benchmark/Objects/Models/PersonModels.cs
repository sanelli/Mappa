// <copyright file="PersonModels.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

#pragma warning disable CA1002, CA1515, CA1724, CA1815, CA1819, CA2227, SA1201, SA1402, SA1649

namespace Mappa.Benchmark.Objects.Models;

/// <summary>
/// Nested address DTO used by class/record/struct benchmarks.
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
    /// Gets or sets the postal code.
    /// </summary>
    public string Zip { get; set; } = string.Empty;
}

/// <summary>
/// Nested address target model.
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
    /// Gets or sets the postal code.
    /// </summary>
    public string Zip { get; set; } = string.Empty;
}

/// <summary>
/// Class-based person DTO.
/// </summary>
public sealed class PersonClassDto
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the age.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    public AddressDto Address { get; set; } = new();
}

/// <summary>
/// Class-based person target.
/// </summary>
public sealed class PersonClass
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the age.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Gets or sets the address.
    /// </summary>
    public Address Address { get; set; } = new();
}

/// <summary>
/// Record-based person DTO.
/// </summary>
/// <param name="Id">The identifier.</param>
/// <param name="Name">The display name.</param>
/// <param name="Age">The age.</param>
/// <param name="Address">The address.</param>
public sealed record PersonRecordDto(int Id, string Name, int Age, AddressDto Address);

/// <summary>
/// Record-based person target.
/// </summary>
/// <param name="Id">The identifier.</param>
/// <param name="Name">The display name.</param>
/// <param name="Age">The age.</param>
/// <param name="Address">The address.</param>
public sealed record PersonRecord(int Id, string Name, int Age, Address Address);

/// <summary>
/// Struct-based person DTO.
/// </summary>
public struct PersonStructDto
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonStructDto"/> struct.
    /// </summary>
    public PersonStructDto()
    {
        this.Id = 0;
        this.Name = string.Empty;
        this.Age = 0;
        this.Street = string.Empty;
        this.City = string.Empty;
        this.Zip = string.Empty;
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the age.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Gets or sets the street.
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string Zip { get; set; }
}

/// <summary>
/// Struct-based person target.
/// </summary>
public struct PersonStruct
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonStruct"/> struct.
    /// </summary>
    public PersonStruct()
    {
        this.Id = 0;
        this.Name = string.Empty;
        this.Age = 0;
        this.Street = string.Empty;
        this.City = string.Empty;
        this.Zip = string.Empty;
    }

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the age.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Gets or sets the street.
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string Zip { get; set; }
}