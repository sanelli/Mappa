// <copyright file="MappaProtobufPolymorphismSetupAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaProtobufPolymorphismSetupAttribute"/>.
/// </summary>
public sealed class MappaProtobufPolymorphismSetupAttributeTests
{
    /// <summary>
    /// Tests the single-property constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SinglePropertyConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaProtobufPolymorphismSetupAttribute("ResultCase");

        // Assert
        attribute.OneOfCasePropertyName.Should().Be("ResultCase");
        attribute.TargetOneOfCasePropertyName.Should().BeNull();
        attribute.SourceOneOfCasePropertyName.Should().BeNull();
    }

    /// <summary>
    /// Tests the target-and-source constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TargetAndSourceConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaProtobufPolymorphismSetupAttribute("TargetCase", "SourceCase");

        // Assert
        attribute.OneOfCasePropertyName.Should().BeNull();
        attribute.TargetOneOfCasePropertyName.Should().Be("TargetCase");
        attribute.SourceOneOfCasePropertyName.Should().Be("SourceCase");
    }

    /// <summary>
    /// Tests the attribute targets methods and does not allow multiple declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodsWithoutMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaProtobufPolymorphismSetupAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }
}