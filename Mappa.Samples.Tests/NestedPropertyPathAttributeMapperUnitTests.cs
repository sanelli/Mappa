// <copyright file="NestedPropertyPathAttributeMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for nested property path attribute sample mappers.
/// </summary>
public sealed class NestedPropertyPathAttributeMapperUnitTests
{
    /// <summary>
    /// Test <see cref="NestedPropertyPathAttributeMapper.MapWithTwoSegmentUseProperty"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWithTwoSegmentUseProperty()
    {
        // Arrange
        var mapper = new NestedPropertyPathAttributeMapper();
        var source = new NestedPropertyPathPersonSourceModel
        {
            Address = new NestedPropertyPathAddressModel
            {
                City = "Rome",
                ZipCode = "00100",
            },
        };

        // Act
        var actual = mapper.MapWithTwoSegmentUseProperty(source);

        // Assert
        actual.Address.City.Should().Be("Rome");
        actual.Address.ZipCode.Should().Be("00100");
    }

    /// <summary>
    /// Test <see cref="NestedPropertyPathThreeSegmentUsePropertyAttributeMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWithThreeSegmentUseProperty()
    {
        // Arrange
        var mapper = new NestedPropertyPathThreeSegmentUsePropertyAttributeMapper();
        var source = new NestedPropertyPathLocationSourceModel
        {
            Location = new NestedPropertyPathLocationModel
            {
                Address = new NestedPropertyPathAddressModel
                {
                    City = "Milan",
                    ZipCode = "20100",
                },
            },
        };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.Address.City.Should().Be("Milan");
        actual.Address.ZipCode.Should().Be("20100");
    }

    /// <summary>
    /// Test <see cref="NestedPropertyPathNestedSourceOnFlatTargetAttributeMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWithNestedSourceOnFlatTarget()
    {
        // Arrange
        var mapper = new NestedPropertyPathNestedSourceOnFlatTargetAttributeMapper();
        var source = new NestedPropertyPathLocationSourceModel
        {
            Location = new NestedPropertyPathLocationModel
            {
                Address = new NestedPropertyPathAddressModel
                {
                    City = "Turin",
                    ZipCode = "10100",
                },
            },
        };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.City.Should().Be("Turin");
    }

    /// <summary>
    /// Test <see cref="NestedPropertyPathInvokeMethodAttributeMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWithInvokeMethodOnNestedTarget()
    {
        // Arrange
        var mapper = new NestedPropertyPathInvokeMethodAttributeMapper();
        var source = new NestedPropertyPathPersonSourceModel
        {
            Address = new NestedPropertyPathAddressModel
            {
                City = "Naples",
                ZipCode = "80100",
            },
        };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.Address.City.Should().Be("NAPLES");
        actual.Address.ZipCode.Should().Be("80100");
    }

    /// <summary>
    /// Test <see cref="NestedPropertyPathAssignFromConstantAttributeMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWithAssignFromConstantOnNestedTarget()
    {
        // Arrange
        var mapper = new NestedPropertyPathAssignFromConstantAttributeMapper();
        var source = new NestedPropertyPathPersonSourceModel
        {
            Address = new NestedPropertyPathAddressModel
            {
                City = "Rome",
                ZipCode = "00100",
            },
        };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.Address.City.Should().Be("London");
        actual.Address.ZipCode.Should().Be("00100");
    }

    /// <summary>
    /// Test <see cref="NestedPropertyPathAssignFromContextAttributeMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWithAssignFromContextOnNestedTarget()
    {
        // Arrange
        var mapper = new NestedPropertyPathAssignFromContextAttributeMapper();
        var source = new NestedPropertyPathPersonSourceModel
        {
            Address = new NestedPropertyPathAddressModel
            {
                City = "Rome",
                ZipCode = "00100",
            },
        };
        MappaContext context = new Dictionary<string, object> { ["city"] = "Florence" };

        // Act
        var actual = mapper.Map(source, context);

        // Assert
        actual.Address.City.Should().Be("Florence");
        actual.Address.ZipCode.Should().Be("00100");
    }

    /// <summary>
    /// Test <see cref="NestedPropertyPathAssignToContextAttributeMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWithAssignToContextOnNestedTarget()
    {
        // Arrange
        var mapper = new NestedPropertyPathAssignToContextAttributeMapper();
        var source = new NestedPropertyPathPersonSourceModel
        {
            Address = new NestedPropertyPathAddressModel
            {
                City = "Venice",
                ZipCode = "30100",
            },
        };
        MappaContext context = new Dictionary<string, object>();

        // Act
        var actual = mapper.Map(source, context);

        // Assert
        actual.Address.City.Should().Be("Venice");
        actual.Address.ZipCode.Should().Be("30100");
        context["MappedCity"].Should().Be("Venice");
    }

    /// <summary>
    /// Test <see cref="NestedPropertyPathIgnoreTargetPropertyAttributeMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWithIgnoreNestedTargetProperty()
    {
        // Arrange
        var mapper = new NestedPropertyPathIgnoreTargetPropertyAttributeMapper();
        var source = new NestedPropertyPathPersonSourceModel
        {
            Address = new NestedPropertyPathAddressModel
            {
                City = "Bologna",
                ZipCode = "40100",
            },
        };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.Address.City.Should().Be("Bologna");
        actual.Address.ZipCode.Should().Be(string.Empty);
    }
}