// <copyright file="MappaObjectFactoryAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaObjectFactoryAttribute"/>.
/// </summary>
public sealed class MappaObjectFactoryAttributeTests
{
    /// <summary>
    /// Tests the method-name constructor initializes the target type and factory name without a location.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MethodNameConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaObjectFactoryAttribute(typeof(MappaContext), "CreateTarget");

        // Assert
        attribute.TargetType.Should().Be<MappaContext>();
        attribute.MethodName.Should().Be("CreateTarget");
        attribute.ClassType.Should().BeNull();
        attribute.FieldName.Should().BeNull();
    }

    /// <summary>
    /// Tests the class-type constructor initializes the target type, factory name, and class type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ClassTypeConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaObjectFactoryAttribute(typeof(string), typeof(MappaContext), "CreateTarget");

        // Assert
        attribute.TargetType.Should().Be<string>();
        attribute.MethodName.Should().Be("CreateTarget");
        attribute.ClassType.Should().Be<MappaContext>();
        attribute.FieldName.Should().BeNull();
    }

    /// <summary>
    /// Tests the field-name constructor initializes the target type, factory name, and field name.
    /// </summary>
    [Fact]
    [UnitTest]
    public void FieldNameConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaObjectFactoryAttribute(typeof(MappaContext), "dependency", "CreateTarget");

        // Assert
        attribute.TargetType.Should().Be<MappaContext>();
        attribute.MethodName.Should().Be("CreateTarget");
        attribute.ClassType.Should().BeNull();
        attribute.FieldName.Should().Be("dependency");
    }

    /// <summary>
    /// Tests the attribute supports classes, methods, and repeated declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsClassesMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaObjectFactoryAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Class | AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }
}