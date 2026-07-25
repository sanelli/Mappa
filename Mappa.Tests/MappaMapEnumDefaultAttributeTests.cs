// <copyright file="MappaMapEnumDefaultAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaMapEnumDefaultAttribute{TEnum}"/>.
/// </summary>
public sealed class MappaMapEnumDefaultAttributeTests
{
    private enum SampleStatus
    {
        One,
        Two,
    }

    /// <summary>
    /// Tests the behaviour-only constructor initializes the attribute without a default value.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BehaviorConstructorInitializesAttributeWithoutDefaultValue()
    {
        // Act
        var attribute = new MappaMapEnumDefaultAttribute<SampleStatus>(MappaMapEnumDefaultBehavior.Throw);

        // Assert
        attribute.Behavior.Should().Be(MappaMapEnumDefaultBehavior.Throw);
        attribute.EnumDefaultValue.Should().BeNull();
        attribute.IntegerDefaultValue.Should().BeNull();
        attribute.StringDefaultValue.Should().BeNull();
        attribute.HasDefaultValue.Should().BeFalse();
    }

    /// <summary>
    /// Tests the enum default constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumDefaultConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaMapEnumDefaultAttribute<SampleStatus>(
            MappaMapEnumDefaultBehavior.UseDefaultValue,
            SampleStatus.Two);

        // Assert
        attribute.Behavior.Should().Be(MappaMapEnumDefaultBehavior.UseDefaultValue);
        attribute.EnumDefaultValue.Should().Be(SampleStatus.Two);
        attribute.IntegerDefaultValue.Should().BeNull();
        attribute.StringDefaultValue.Should().BeNull();
        attribute.HasDefaultValue.Should().BeTrue();
    }

    /// <summary>
    /// Tests the integral default constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IntegerDefaultConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaMapEnumDefaultAttribute<SampleStatus>(
            MappaMapEnumDefaultBehavior.UseDefaultValue,
            42);

        // Assert
        attribute.Behavior.Should().Be(MappaMapEnumDefaultBehavior.UseDefaultValue);
        attribute.EnumDefaultValue.Should().BeNull();
        attribute.IntegerDefaultValue.Should().Be(42);
        attribute.StringDefaultValue.Should().BeNull();
        attribute.HasDefaultValue.Should().BeTrue();
    }

    /// <summary>
    /// Tests the string default constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void StringDefaultConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaMapEnumDefaultAttribute<SampleStatus>(
            MappaMapEnumDefaultBehavior.UseDefaultValue,
            "fallback");

        // Assert
        attribute.Behavior.Should().Be(MappaMapEnumDefaultBehavior.UseDefaultValue);
        attribute.EnumDefaultValue.Should().BeNull();
        attribute.IntegerDefaultValue.Should().BeNull();
        attribute.StringDefaultValue.Should().Be("fallback");
        attribute.HasDefaultValue.Should().BeTrue();
    }

    /// <summary>
    /// Tests the attribute supports methods and repeated declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaMapEnumDefaultAttribute<SampleStatus>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }
}