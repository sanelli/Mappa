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
    /// Tests <see cref="MappaContext"/> can be created from a dictionary
    /// or set list of key-value pairs.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanCreateMappaContextFromDictionary()
    {
        // Arrange
        var dictionary = new Dictionary<string, object>
        {
            ["foo"] = "bar",
        };

        KeyValuePair<string, object>[] pairs = [new("foo", "bar")];

        // Act
        var contextFromDictionary = new MappaContext(dictionary);
        var contextFromPairs = new MappaContext(pairs);
        var contentFromFancyConstructor = new MappaContext { ["foo"] = "bar" };

        // Assert
        contextFromDictionary["foo"].Should().Be("bar");
        contextFromPairs["foo"].Should().Be("bar");
        contentFromFancyConstructor["foo"].Should().Be("bar");
    }

    /// <summary>
    /// Tests <see cref="MappaContext"/> can be created from by using the conversion methods.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanCreateMappaFromConversion()
    {
        // Arrange
        var dictionary = new Dictionary<string, object>
        {
            ["foo"] = "bar",
        };

        KeyValuePair<string, object>[] pairs = [new("foo", "bar")];

        // Act
        MappaContext contextFromDictionary = dictionary;
        MappaContext contextFromPairs = pairs;
        MappaContext contextFromDictionary2 = MappaContext.ToMappaContext(dictionary);
        MappaContext contextFromPairs2 = MappaContext.ToMappaContext(pairs);

        // Assert
        contextFromDictionary["foo"].Should().Be("bar");
        contextFromPairs["foo"].Should().Be("bar");
        contextFromDictionary2["foo"].Should().Be("bar");
        contextFromPairs2["foo"].Should().Be("bar");
    }

    /// <summary>
    /// Tests <see cref="MappaContext"/> value can be set.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanAddValuesViaContext()
    {
        // Act
        var context = new MappaContext { ["foo"] = "bar" };
        context.Add("ijk", "xyz");
        context["zap"] = "bli";
        context["foo"] = "abc";

        // Assert
        context["ijk"].Should().Be("xyz");
        context["foo"].Should().Be("abc");
        context["zap"].Should().Be("bli");
        context.Keys.Should().BeEquivalentTo("foo", "ijk", "zap");
    }

    /// <summary>
    /// Tests <see cref="MappaContext"/> value can be obtained.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanGetValuesViaContext()
    {
        // Act
        var context = new MappaContext { ["foo"] = "bar" };
        var throwing = () => context["ijk"];

        // Assert
        context["foo"].Should().Be("bar");
        throwing.Should().Throw<KeyNotFoundException>();
        context.TryGetValue("foo", out _).Should().BeTrue();
        context.TryGetValue("ijk", out _).Should().BeFalse();
        context.TryGetValue<string>("foo", out _).Should().BeTrue();
        context.TryGetValue<int>("foo", out _).Should().BeFalse();
        context.TryGetValue<string>("ijk", out _).Should().BeFalse();
    }
}