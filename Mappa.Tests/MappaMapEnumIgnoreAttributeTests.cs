// <copyright file="MappaMapEnumIgnoreAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaMapEnumIgnoreAttribute{TEnum}"/>.
/// </summary>
public sealed class MappaMapEnumIgnoreAttributeTests
{
    private enum SampleStatus
    {
        One,
        Two,
    }

    /// <summary>
    /// Tests the constructor initializes the ignored enum member.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaMapEnumIgnoreAttribute<SampleStatus>(SampleStatus.Two);

        // Assert
        attribute.EnumValue.Should().Be(SampleStatus.Two);
    }

    /// <summary>
    /// Tests the attribute supports methods and repeated declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaMapEnumIgnoreAttribute<SampleStatus>)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }
}