// <copyright file="MappaUseConstructorAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaUseConstructorAttribute"/> and its generic forms.
/// </summary>
public sealed class MappaUseConstructorAttributeTests
{
    /// <summary>
    /// Tests the constructor-type overload initializes for the map method return type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ConstructorTypeOnlyInitializesAttributeForReturnType()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute(ConstructorType.NoParameters);

        // Assert
        attribute.TargetType.Should().BeNull();
        attribute.ConstructorType.Should().Be(ConstructorType.NoParameters);
        attribute.ParameterTypes.Should().BeEmpty();
    }

    /// <summary>
    /// Tests the constructor-type and parameter-types overload initializes the signature.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ConstructorTypeWithParameterTypesInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute(
            ConstructorType.WithParameters,
            typeof(int),
            typeof(string));

        // Assert
        attribute.TargetType.Should().BeNull();
        attribute.ConstructorType.Should().Be(ConstructorType.WithParameters);
        attribute.ParameterTypes.Should().Equal(typeof(int), typeof(string));
    }

    /// <summary>
    /// Tests the target-type overload initializes the target and constructor kind.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TargetTypeAndConstructorTypeInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute(typeof(List<int>), ConstructorType.NoParameters);

        // Assert
        attribute.TargetType.Should().Be<List<int>>();
        attribute.ConstructorType.Should().Be(ConstructorType.NoParameters);
        attribute.ParameterTypes.Should().BeEmpty();
    }

    /// <summary>
    /// Tests the target-type and parameter-types overload initializes all properties.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TargetTypeWithParameterTypesInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute(
            typeof(Dictionary<string, int>),
            ConstructorType.WithParameters,
            typeof(string),
            typeof(int));

        // Assert
        attribute.TargetType.Should().Be<Dictionary<string, int>>();
        attribute.ConstructorType.Should().Be(ConstructorType.WithParameters);
        attribute.ParameterTypes.Should().Equal(typeof(string), typeof(int));
    }

    /// <summary>
    /// Tests the single-parameter generic form initializes the root exact signature.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericOneParameterInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute<int>();

        // Assert
        attribute.TargetType.Should().BeNull();
        attribute.ConstructorType.Should().Be(ConstructorType.WithParameters);
        attribute.ParameterTypes.Should().Equal(typeof(int));
        attribute.Should().BeAssignableTo<MappaUseConstructorAttribute>();
    }

    /// <summary>
    /// Tests the two-parameter generic form initializes the root exact signature.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericTwoParametersInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute<int, string>();

        // Assert
        attribute.TargetType.Should().BeNull();
        attribute.ConstructorType.Should().Be(ConstructorType.WithParameters);
        attribute.ParameterTypes.Should().Equal(typeof(int), typeof(string));
    }

    /// <summary>
    /// Tests the eight-parameter generic form initializes the root exact signature.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericEightParametersInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute<byte, sbyte, short, ushort, int, uint, long, ulong>();

        // Assert
        attribute.TargetType.Should().BeNull();
        attribute.ConstructorType.Should().Be(ConstructorType.WithParameters);
        attribute.ParameterTypes.Should().Equal(
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong));
    }

    /// <summary>
    /// Tests the single-parameter generic form accepts an explicit target type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericOneParameterWithTargetTypeInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute<int>(typeof(List<int>));

        // Assert
        attribute.TargetType.Should().Be<List<int>>();
        attribute.ConstructorType.Should().Be(ConstructorType.WithParameters);
        attribute.ParameterTypes.Should().Equal(typeof(int));
    }

    /// <summary>
    /// Tests the two-parameter generic form accepts an explicit target type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericTwoParametersWithTargetTypeInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute<int, string>(typeof(Dictionary<int, string>));

        // Assert
        attribute.TargetType.Should().Be<Dictionary<int, string>>();
        attribute.ConstructorType.Should().Be(ConstructorType.WithParameters);
        attribute.ParameterTypes.Should().Equal(typeof(int), typeof(string));
    }

    /// <summary>
    /// Tests the eight-parameter generic form accepts an explicit target type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericEightParametersWithTargetTypeInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute<byte, sbyte, short, ushort, int, uint, long, ulong>(typeof(DateTimeOffset));

        // Assert
        attribute.TargetType.Should().Be<DateTimeOffset>();
        attribute.ConstructorType.Should().Be(ConstructorType.WithParameters);
        attribute.ParameterTypes.Should().Equal(
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong));
    }

    /// <summary>
    /// Tests the non-generic attribute supports methods and repeated declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NonGenericAttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaUseConstructorAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }

    /// <summary>
    /// Tests a generic attribute supports methods and repeated declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericAttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaUseConstructorAttribute<int, string>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }
}