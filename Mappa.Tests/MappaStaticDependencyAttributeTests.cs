// <copyright file="MappaStaticDependencyAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaStaticDependencyAttribute"/>.
/// </summary>
public sealed class MappaStaticDependencyAttributeTests
{
    /// <summary>
    /// Tests <see cref="MappaStaticDependencyAttribute.Dependency"/> returns the type passed to the constructor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void DependencyReturnsTypePassedToConstructor()
    {
        // Act
        var attribute = new MappaStaticDependencyAttribute(typeof(MappaContext));

        // Assert
        attribute.Dependency.Should().Be<MappaContext>();
    }
}