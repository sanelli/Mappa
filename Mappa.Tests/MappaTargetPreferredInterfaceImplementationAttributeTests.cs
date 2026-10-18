// <copyright file="MappaTargetPreferredInterfaceImplementationAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaTargetPreferredInterfaceImplementationAttribute"/> and
/// <see cref="MappaTargetPreferredInterfaceImplementationAttribute{TImplementingType, TClosedInterface}"/>.
/// </summary>
public sealed class MappaTargetPreferredInterfaceImplementationAttributeTests
{
    /// <summary>
    /// Tests the non-generic constructor initializes the implementing type and preferred closed interface.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NonGenericConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaTargetPreferredInterfaceImplementationAttribute(
            typeof(List<string>),
            typeof(ICollection<string>));

        // Assert
        attribute.ImplementingType.Should().Be<List<string>>();
        attribute.PreferredClosedInterface.Should().Be<ICollection<string>>();
    }

    /// <summary>
    /// Tests the generic empty constructor initializes the implementing type and preferred closed interface.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaTargetPreferredInterfaceImplementationAttribute<List<string>, ICollection<string>>();

        // Assert
        attribute.ImplementingType.Should().Be<List<string>>();
        attribute.PreferredClosedInterface.Should().Be<ICollection<string>>();
        attribute.Should().BeAssignableTo<MappaTargetPreferredInterfaceImplementationAttribute>();
    }

    /// <summary>
    /// Tests the non-generic attribute supports methods and repeated declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NonGenericAttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaTargetPreferredInterfaceImplementationAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }

    /// <summary>
    /// Tests the generic attribute supports methods and repeated declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericAttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaTargetPreferredInterfaceImplementationAttribute<List<string>, ICollection<string>>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }
}