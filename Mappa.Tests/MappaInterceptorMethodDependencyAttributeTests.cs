// <copyright file="MappaInterceptorMethodDependencyAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaInterceptorMethodDependencyAttribute"/>.
/// </summary>
public sealed class MappaInterceptorMethodDependencyAttributeTests
{
    /// <summary>
    /// Tests the attribute can be constructed.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ConstructorCreatesAttribute()
    {
        // Act
        var attribute = new MappaInterceptorMethodDependencyAttribute();

        // Assert
        attribute.Should().NotBeNull();
    }

    /// <summary>
    /// Tests the attribute targets methods only and does not allow multiple.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageTargetsMethodWithoutAllowMultiple()
    {
        // Act
        var attributeUsage = typeof(MappaInterceptorMethodDependencyAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }
}