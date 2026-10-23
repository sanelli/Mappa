// <copyright file="MappaSettingsAttributeTransformerTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for value-transformer settings on <see cref="MappaSettingsAttribute"/>.
/// </summary>
public sealed class MappaSettingsAttributeTransformerTests
{
    /// <summary>
    /// Tests <see cref="MappaSettingsAttribute.MappaTransformerFromDependency"/> defaults to
    /// <see cref="BooleanSetting.Undefined"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MappaTransformerFromDependencyDefaultsToUndefined()
    {
        // Act
        var attribute = new MappaSettingsAttribute();

        // Assert
        attribute.MappaTransformerFromDependency.Should().Be(BooleanSetting.Undefined);
    }

    /// <summary>
    /// Tests <see cref="MappaSettingsAttribute.MappaTransformerFromDependency"/> can be set.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MappaTransformerFromDependencyCanBeSet()
    {
        // Act
        var attribute = new MappaSettingsAttribute
        {
            MappaTransformerFromDependency = BooleanSetting.Enable,
        };

        // Assert
        attribute.MappaTransformerFromDependency.Should().Be(BooleanSetting.Enable);
    }
}