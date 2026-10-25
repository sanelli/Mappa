// <copyright file="MappaInterceptorExtensionsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaInterceptorExtensions"/>.
/// </summary>
public sealed class MappaInterceptorExtensionsTests
{
    /// <summary>
    /// Tests <see cref="MappaInterceptorExtensions.With{T}"/> returns the same reference.
    /// </summary>
    [Fact]
    [UnitTest]
    public void WithReturnsSameReference()
    {
        // Arrange
        var input = new object();

        // Act
        var result = input.With();

        // Assert
        result.Should().BeSameAs(input);
    }

    /// <summary>
    /// Tests <see cref="MappaInterceptorExtensions.With{T}"/> returns the same value for value types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void WithReturnsSameValueForValueTypes()
    {
        // Arrange
        const int input = 42;

        // Act
        var result = input.With(
            mappaSettings: new MappaSettingsAttribute(),
            mappaStaticDependencies: [new MappaStaticDependencyAttribute(typeof(string))],
            mappaMethodAttributes: [new MappaIgnoreTargetPropertyAttribute("Name")]);

        // Assert
        result.Should().Be(input);
    }
}