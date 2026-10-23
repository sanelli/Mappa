// <copyright file="MappaApplyTransformerAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaApplyTransformerAttribute"/>.
/// </summary>
public sealed class MappaApplyTransformerAttributeTests
{
    /// <summary>
    /// Tests the method-name constructor initializes the method name without a location or source member.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MethodNameConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaApplyTransformerAttribute("Transform");

        // Assert
        attribute.MethodName.Should().Be("Transform");
        attribute.ClassType.Should().BeNull();
        attribute.SourcePropertyName.Should().BeNull();
        attribute.FieldName.Should().BeNull();
    }

    /// <summary>
    /// Tests the class-type constructor initializes the method name and class type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ClassTypeConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaApplyTransformerAttribute(typeof(MappaContext), "Transform");

        // Assert
        attribute.MethodName.Should().Be("Transform");
        attribute.ClassType.Should().Be<MappaContext>();
        attribute.SourcePropertyName.Should().BeNull();
        attribute.FieldName.Should().BeNull();
    }

    /// <summary>
    /// Tests the source-property constructor initializes the method name and source property path.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SourcePropertyConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaApplyTransformerAttribute("Address.City", "Transform");

        // Assert
        attribute.MethodName.Should().Be("Transform");
        attribute.ClassType.Should().BeNull();
        attribute.SourcePropertyName.Should().Be("Address.City");
        attribute.FieldName.Should().BeNull();
    }

    /// <summary>
    /// Tests the source-property and class-type constructor initializes all location properties.
    /// </summary>
    [Fact]
    [UnitTest]
    public void SourcePropertyAndClassTypeConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaApplyTransformerAttribute("Name", typeof(MappaContext), "Transform");

        // Assert
        attribute.MethodName.Should().Be("Transform");
        attribute.ClassType.Should().Be<MappaContext>();
        attribute.SourcePropertyName.Should().Be("Name");
        attribute.FieldName.Should().BeNull();
    }

    /// <summary>
    /// Tests <see cref="MappaApplyTransformerAttribute.FieldName"/> can be set via object initializer.
    /// </summary>
    [Fact]
    [UnitTest]
    public void FieldNameCanBeSetViaObjectInitializer()
    {
        // Act
        var attribute = new MappaApplyTransformerAttribute("Transform")
        {
            FieldName = "dependency",
        };

        // Assert
        attribute.MethodName.Should().Be("Transform");
        attribute.FieldName.Should().Be("dependency");
        attribute.ClassType.Should().BeNull();
        attribute.SourcePropertyName.Should().BeNull();
    }

    /// <summary>
    /// Tests the attribute targets methods and allows multiple declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaApplyTransformerAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }
}