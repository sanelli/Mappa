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
    /// Tests the three-parameter generic form initializes the root exact signature.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericThreeParametersInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute<int, string, bool>();

        // Assert
        attribute.TargetType.Should().BeNull();
        attribute.ConstructorType.Should().Be(ConstructorType.WithParameters);
        attribute.ParameterTypes.Should().Equal(typeof(int), typeof(string), typeof(bool));
    }

    /// <summary>
    /// Tests the four-parameter generic form initializes the root exact signature.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericFourParametersInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute<int, string, bool, double>();

        // Assert
        attribute.ParameterTypes.Should().Equal(typeof(int), typeof(string), typeof(bool), typeof(double));
    }

    /// <summary>
    /// Tests the five-parameter generic form initializes the root exact signature.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericFiveParametersInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute<int, string, bool, double, float>();

        // Assert
        attribute.ParameterTypes.Should().HaveCount(5);
        attribute.ParameterTypes[^1].Should().Be<float>();
    }

    /// <summary>
    /// Tests the six-parameter generic form initializes the root exact signature.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericSixParametersInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute<int, string, bool, double, float, long>();

        // Assert
        attribute.ParameterTypes.Should().HaveCount(6);
        attribute.ParameterTypes[^1].Should().Be<long>();
    }

    /// <summary>
    /// Tests the seven-parameter generic form initializes the root exact signature.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericSevenParametersInitializesAttribute()
    {
        // Act
        var attribute = new MappaUseConstructorAttribute<int, string, bool, double, float, long, short>();

        // Assert
        attribute.ParameterTypes.Should().HaveCount(7);
        attribute.ParameterTypes[^1].Should().Be<short>();
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
    /// Tests null parameterTypes arrays are normalized to empty.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NullParameterTypesAreNormalizedToEmpty()
    {
        // Act
        var rootForm = new MappaUseConstructorAttribute(ConstructorType.WithParameters, null!);
        var typedForm = new MappaUseConstructorAttribute(typeof(List<int>), ConstructorType.WithParameters, null!);

        // Assert
        rootForm.ParameterTypes.Should().BeEmpty();
        typedForm.TargetType.Should().Be<List<int>>();
        typedForm.ParameterTypes.Should().BeEmpty();
    }

    /// <summary>
    /// Tests the three-to-seven parameter generics accept an explicit target type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericThreeToSevenParametersWithTargetTypeInitializeAttribute()
    {
        // Act
        var three = new MappaUseConstructorAttribute<int, string, bool>(typeof(Uri));
        var four = new MappaUseConstructorAttribute<int, string, bool, double>(typeof(Uri));
        var five = new MappaUseConstructorAttribute<int, string, bool, double, float>(typeof(Uri));
        var six = new MappaUseConstructorAttribute<int, string, bool, double, float, long>(typeof(Uri));
        var seven = new MappaUseConstructorAttribute<int, string, bool, double, float, long, short>(typeof(Uri));

        // Assert
        three.TargetType.Should().Be<Uri>();
        three.ParameterTypes.Should().HaveCount(3);
        four.ParameterTypes.Should().HaveCount(4);
        five.ParameterTypes.Should().HaveCount(5);
        six.ParameterTypes.Should().HaveCount(6);
        seven.ParameterTypes.Should().HaveCount(7);
        seven.TargetType.Should().Be<Uri>();
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