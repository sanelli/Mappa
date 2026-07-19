// <copyright file="MappaAfterMapAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaAfterMapAttribute"/>.
/// </summary>
public sealed class MappaAfterMapAttributeTests
{
    /// <summary>
    /// Tests the method-name constructor initializes the hook name without a location.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MethodNameConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaAfterMapAttribute("AfterMap");

        // Assert
        attribute.MethodName.Should().Be("AfterMap");
        attribute.ClassType.Should().BeNull();
        attribute.FieldName.Should().BeNull();
    }

    /// <summary>
    /// Tests the class-type constructor initializes the hook name and class type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ClassTypeConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaAfterMapAttribute(typeof(MappaContext), "AfterMap");

        // Assert
        attribute.MethodName.Should().Be("AfterMap");
        attribute.ClassType.Should().Be<MappaContext>();
        attribute.FieldName.Should().BeNull();
    }

    /// <summary>
    /// Tests the field-name constructor initializes the hook name and field name.
    /// </summary>
    [Fact]
    [UnitTest]
    public void FieldNameConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaAfterMapAttribute("dependency", "AfterMap");

        // Assert
        attribute.MethodName.Should().Be("AfterMap");
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
        var attributeUsage = typeof(MappaAfterMapAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Class | AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }
}