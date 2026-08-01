// <copyright file="MappaMustMapTargetPropertyAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaMustMapTargetPropertyAttribute"/>.
/// </summary>
public sealed class MappaMustMapTargetPropertyAttributeTests
{
    /// <summary>
    /// Tests the parameterless constructor initializes an empty target property name list.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParameterlessConstructorInitializesEmptyTargetPropertyNames()
    {
        // Act
        var attribute = new MappaMustMapTargetPropertyAttribute();

        // Assert
        attribute.TargetPropertyNames.Should().BeEmpty();
    }

    /// <summary>
    /// Tests the params constructor initializes the target property names.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParamsConstructorInitializesTargetPropertyNames()
    {
        // Act
        var attribute = new MappaMustMapTargetPropertyAttribute("PropertyA", "PropertyB");

        // Assert
        attribute.TargetPropertyNames.Should().Equal("PropertyA", "PropertyB");
    }

    /// <summary>
    /// Tests the params constructor with an empty array initializes an empty target property name list.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParamsConstructorWithEmptyArrayInitializesEmptyTargetPropertyNames()
    {
        // Arrange
        string[] emptyNames = [];

        // Act
        var attribute = new MappaMustMapTargetPropertyAttribute(emptyNames);

        // Assert
        attribute.TargetPropertyNames.Should().BeEmpty();
    }

    /// <summary>
    /// Tests the params constructor treats a <c>null</c> array as an empty target property name list.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParamsConstructorWithNullArrayInitializesEmptyTargetPropertyNames()
    {
        // Arrange
        string[]? nullNames = null;

        // Act
        var attribute = new MappaMustMapTargetPropertyAttribute(nullNames);

        // Assert
        attribute.TargetPropertyNames.Should().BeEmpty();
    }

    /// <summary>
    /// Tests the attribute supports methods only and does not allow multiple declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodsAndDisallowsMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaMustMapTargetPropertyAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }
}