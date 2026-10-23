// <copyright file="MappaTransformerAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaTransformerAttribute"/>.
/// </summary>
public sealed class MappaTransformerAttributeTests
{
    /// <summary>
    /// Tests the attribute can be constructed and targets methods only.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageTargetsMethodsAndDoesNotAllowMultiple()
    {
        // Act
        var attribute = new MappaTransformerAttribute();
        var attributeUsage = typeof(MappaTransformerAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attribute.Should().NotBeNull();
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }
}