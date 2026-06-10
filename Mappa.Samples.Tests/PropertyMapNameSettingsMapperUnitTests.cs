// <copyright file="PropertyMapNameSettingsMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="PropertyMapNameSettingsMapper"/>.
/// </summary>
public sealed class PropertyMapNameSettingsMapperUnitTests
{
    private readonly PropertyMapNameSettingsMapper mapper = new();

    /// <summary>
    /// Tests <see cref="PropertyMapNameSettingsMapper.MapWithClassDefaults"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWithClassDefaults()
    {
        // Arrange
        var source = new PropertyMapNameSettingsSourceModel
        {
            user_name = 42,
            PropertyB = 7,
        };

        // Act
        var actual = this.mapper.MapWithClassDefaults(source);

        // Assert
        actual.UserName.Should().Be("42");
        actual.PropertyB.Should().Be(7);
    }

    /// <summary>
    /// Tests <see cref="PropertyMapNameSettingsMapper.MapWithMethodOverrideDisablingUnderscoreMatching"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWithMethodOverrideDisablingUnderscoreMatching()
    {
        // Arrange
        var source = new PropertyMapNameSettingsSourceModel
        {
            user_name = 42,
            PropertyB = 7,
        };

        // Act
        var actual = this.mapper.MapWithMethodOverrideDisablingUnderscoreMatching(source);

        // Assert
        actual.PropertyB.Should().Be(7);
    }
}