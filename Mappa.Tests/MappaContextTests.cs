// <copyright file="MappaContextTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Tests;

/// <summary>
/// Unit  tests for <see cref="MappaContext"/>.
/// </summary>
public sealed class MappaContextTests
{
    /// <summary>
    /// Tests <see cref="MappaContext"/> can be created from a dictionary.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanCreateMappaContextFromDictionary()
    {
        // Arrange
        MappaContext context = new(new Dictionary<string, object>
        {
            ["foo"] = "bar",
        });

        // Assert
        context["foo"].Should().Be("bar");
    }
}