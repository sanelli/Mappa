// <copyright file="MappaTypeMappingAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaTypeMappingAttribute"/> and
/// <see cref="MappaTypeMappingAttribute{TTarget, TSource}"/>.
/// </summary>
public sealed class MappaTypeMappingAttributeTests
{
    /// <summary>
    /// Tests the non-generic constructor initializes target and source types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NonGenericConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaTypeMappingAttribute(typeof(string), typeof(int));

        // Assert
        attribute.TargetType.Should().Be<string>();
        attribute.SourceType.Should().Be<int>();
    }

    /// <summary>
    /// Tests the generic empty constructor initializes target and source types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GenericConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaTypeMappingAttribute<string, int>();

        // Assert
        attribute.TargetType.Should().Be<string>();
        attribute.SourceType.Should().Be<int>();
        attribute.Should().BeAssignableTo<MappaTypeMappingAttribute>();
    }

    /// <summary>
    /// Tests the non-generic attribute supports methods and repeated declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NonGenericAttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaTypeMappingAttribute)
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
        var attributeUsage = typeof(MappaTypeMappingAttribute<string, int>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }
}