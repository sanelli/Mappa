// <copyright file="MappaMapEnumMemberAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaMapEnumMemberAttribute{TEnum}"/>
/// and <see cref="MappaMapEnumMemberAttribute{TEnum, TOtherEnum}"/>.
/// </summary>
public sealed class MappaMapEnumMemberAttributeTests
{
    private enum SampleStatus
    {
        One,
        Two,
    }

    private enum OtherSampleKind
    {
        Alpha,
        Beta,
    }

    /// <summary>
    /// Tests the integral pairing constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IntegerConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaMapEnumMemberAttribute<SampleStatus>(SampleStatus.Two, 42);

        // Assert
        attribute.EnumValue.Should().Be(SampleStatus.Two);
        attribute.IntegerValue.Should().Be(42);
        attribute.StringValue.Should().BeNull();
    }

    /// <summary>
    /// Tests the string pairing constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void StringConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaMapEnumMemberAttribute<SampleStatus>(SampleStatus.One, "one");

        // Assert
        attribute.EnumValue.Should().Be(SampleStatus.One);
        attribute.IntegerValue.Should().BeNull();
        attribute.StringValue.Should().Be("one");
    }

    /// <summary>
    /// Tests the enum-to-enum pairing constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EnumToEnumConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaMapEnumMemberAttribute<SampleStatus, OtherSampleKind>(SampleStatus.Two, OtherSampleKind.Beta);

        // Assert
        attribute.EnumValue.Should().Be(SampleStatus.Two);
        attribute.OtherEnumValue.Should().Be(OtherSampleKind.Beta);
    }

    /// <summary>
    /// Tests the attribute supports methods and repeated declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var oneTypeParameterUsage = typeof(MappaMapEnumMemberAttribute<SampleStatus>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();
        var twoTypeParameterUsage = typeof(MappaMapEnumMemberAttribute<SampleStatus, OtherSampleKind>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        oneTypeParameterUsage.ValidOn.Should().Be(AttributeTargets.Method);
        oneTypeParameterUsage.AllowMultiple.Should().BeTrue();
        twoTypeParameterUsage.ValidOn.Should().Be(AttributeTargets.Method);
        twoTypeParameterUsage.AllowMultiple.Should().BeTrue();
    }
}