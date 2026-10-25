// <copyright file="MappaInterceptorAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaInterceptorAttribute"/>.
/// </summary>
public sealed class MappaInterceptorAttributeTests
{
    /// <summary>
    /// Tests the default constructor uses <c>MappaMap</c> as the method name.
    /// </summary>
    [Fact]
    [UnitTest]
    public void DefaultConstructorUsesMappaMapMethodName()
    {
        // Act
        var attribute = new MappaInterceptorAttribute();

        // Assert
        attribute.MethodName.Should().Be("MappaMap");
    }

    /// <summary>
    /// Tests the method-name constructor stores the custom name.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MethodNameConstructorStoresCustomName()
    {
        // Act
        var attribute = new MappaInterceptorAttribute("MapInline");

        // Assert
        attribute.MethodName.Should().Be("MapInline");
    }

    /// <summary>
    /// Tests the attribute targets classes only and does not allow multiple.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageTargetsClassWithoutAllowMultiple()
    {
        // Act
        var attributeUsage = typeof(MappaInterceptorAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Class);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }
}