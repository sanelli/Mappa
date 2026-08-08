// <copyright file="MappaTypeMappingDefaultAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaTypeMappingDefaultAttribute"/> and
/// <see cref="MappaTypeMappingDefaultAttribute{TDefault}"/>.
/// </summary>
public sealed class MappaTypeMappingDefaultAttributeTests
{
    /// <summary>
    /// Tests the behavior-and-type constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BehaviorAndTypeConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaTypeMappingDefaultAttribute(
            MappaTypeMappingDefaultBehavior.MapSourceType,
            typeof(string));

        // Assert
        attribute.Behavior.Should().Be(MappaTypeMappingDefaultBehavior.MapSourceType);
        attribute.Type.Should().Be<string>();
        attribute.MethodName.Should().BeNull();
    }

    /// <summary>
    /// Tests the behavior-only constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BehaviorConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaTypeMappingDefaultAttribute(MappaTypeMappingDefaultBehavior.Throw);

        // Assert
        attribute.Behavior.Should().Be(MappaTypeMappingDefaultBehavior.Throw);
        attribute.Type.Should().BeNull();
        attribute.MethodName.Should().BeNull();
    }

    /// <summary>
    /// Tests the method-name constructor initializes the attribute for invoke-method behaviour.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MethodNameConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaTypeMappingDefaultAttribute("DefaultMap");

        // Assert
        attribute.Behavior.Should().Be(MappaTypeMappingDefaultBehavior.InvokeMethod);
        attribute.Type.Should().BeNull();
        attribute.MethodName.Should().Be("DefaultMap");
    }

    /// <summary>
    /// Tests the type-and-method-name constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TypeAndMethodNameConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaTypeMappingDefaultAttribute(typeof(string), "DefaultMap");

        // Assert
        attribute.Behavior.Should().Be(MappaTypeMappingDefaultBehavior.InvokeMethod);
        attribute.Type.Should().Be<string>();
        attribute.MethodName.Should().Be("DefaultMap");
    }

    /// <summary>
    /// Tests the generic empty constructor initializes MapSourceType behaviour and the default type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaTypeMappingDefaultAttribute<string>();

        // Assert
        attribute.Behavior.Should().Be(MappaTypeMappingDefaultBehavior.MapSourceType);
        attribute.Type.Should().Be<string>();
        attribute.MethodName.Should().BeNull();
        attribute.Should().BeAssignableTo<MappaTypeMappingDefaultAttribute>();
    }

    /// <summary>
    /// Tests the non-generic attribute targets methods and does not allow multiple declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NonGenericAttributeUsageSupportsMethodsWithoutMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaTypeMappingDefaultAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }

    /// <summary>
    /// Tests the generic attribute targets methods and does not allow multiple declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericAttributeUsageSupportsMethodsWithoutMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaTypeMappingDefaultAttribute<string>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }
}