// <copyright file="MappaProtobufTypeMappingDefaultAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaProtobufTypeMappingDefaultAttribute"/>.
/// </summary>
public sealed class MappaProtobufTypeMappingDefaultAttributeTests
{
    /// <summary>
    /// Tests the behaviour-only constructor initializes Throw.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BehaviorConstructorInitializesThrow()
    {
        // Act
        var attribute = new MappaProtobufTypeMappingDefaultAttribute(MappaProtobufTypeMappingDefaultBehavior.Throw);

        // Assert
        attribute.Behavior.Should().Be(MappaProtobufTypeMappingDefaultBehavior.Throw);
        attribute.ExceptionType.Should().BeNull();
        attribute.TargetOneOfCaseName.Should().BeNull();
    }

    /// <summary>
    /// Tests the behaviour-only constructor initializes NoMap.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BehaviorConstructorInitializesNoMap()
    {
        // Act
        var attribute = new MappaProtobufTypeMappingDefaultAttribute(MappaProtobufTypeMappingDefaultBehavior.NoMap);

        // Assert
        attribute.Behavior.Should().Be(MappaProtobufTypeMappingDefaultBehavior.NoMap);
        attribute.ExceptionType.Should().BeNull();
        attribute.TargetOneOfCaseName.Should().BeNull();
    }

    /// <summary>
    /// Tests the behaviour-and-exception-type constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BehaviorAndExceptionTypeConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaProtobufTypeMappingDefaultAttribute(
            MappaProtobufTypeMappingDefaultBehavior.Throw,
            typeof(InvalidOperationException));

        // Assert
        attribute.Behavior.Should().Be(MappaProtobufTypeMappingDefaultBehavior.Throw);
        attribute.ExceptionType.Should().Be<InvalidOperationException>();
        attribute.TargetOneOfCaseName.Should().BeNull();
    }

    /// <summary>
    /// Tests the behaviour-and-case-name constructor initializes MapTo.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BehaviorAndCaseNameConstructorInitializesMapTo()
    {
        // Act
        var attribute = new MappaProtobufTypeMappingDefaultAttribute(
            MappaProtobufTypeMappingDefaultBehavior.MapTo,
            "Person");

        // Assert
        attribute.Behavior.Should().Be(MappaProtobufTypeMappingDefaultBehavior.MapTo);
        attribute.TargetOneOfCaseName.Should().Be("Person");
        attribute.ExceptionType.Should().BeNull();
    }

    /// <summary>
    /// Tests the attribute targets methods and does not allow multiple declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodsWithoutMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaProtobufTypeMappingDefaultAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }
}