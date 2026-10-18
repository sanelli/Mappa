// <copyright file="MappaSourcePreferredInterfaceImplementationAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaSourcePreferredInterfaceImplementationAttribute"/> and
/// <see cref="MappaSourcePreferredInterfaceImplementationAttribute{TImplementingType, TClosedInterface}"/>.
/// </summary>
public sealed class MappaSourcePreferredInterfaceImplementationAttributeTests
{
    /// <summary>
    /// Tests the non-generic constructor initializes the implementing type and preferred closed interface.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NonGenericConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaSourcePreferredInterfaceImplementationAttribute(
            typeof(List<int>),
            typeof(IEnumerable<int>));

        // Assert
        attribute.ImplementingType.Should().Be<List<int>>();
        attribute.PreferredClosedInterface.Should().Be<IEnumerable<int>>();
    }

    /// <summary>
    /// Tests the generic empty constructor initializes the implementing type and preferred closed interface.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaSourcePreferredInterfaceImplementationAttribute<List<int>, IEnumerable<int>>();

        // Assert
        attribute.ImplementingType.Should().Be<List<int>>();
        attribute.PreferredClosedInterface.Should().Be<IEnumerable<int>>();
        attribute.Should().BeAssignableTo<MappaSourcePreferredInterfaceImplementationAttribute>();
    }

    /// <summary>
    /// Tests the non-generic attribute supports methods and repeated declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NonGenericAttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaSourcePreferredInterfaceImplementationAttribute)
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
        var attributeUsage = typeof(MappaSourcePreferredInterfaceImplementationAttribute<List<int>, IEnumerable<int>>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }
}