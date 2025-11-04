// <copyright file="ContainersWithCapacityConstructorMapperTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Xunit;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for mapper <see cref="ContainersWithCapacityConstructorMapper"/>.
/// </summary>
public sealed class ContainersWithCapacityConstructorMapperTest
{
    private readonly ContainersWithCapacityConstructorMapper mapper = new();

    /// <summary>
    /// Tests for <see cref="ContainersWithCapacityConstructorMapper.MapFromArrayToCustomCollection"/>.
    /// </summary>
    [Fact]
    public void MapFromArrayToCustomCollectionTest()
    {
        // Arrange
        int[] input = [1, 2, 3];

        // Act
        var actual = this.mapper.MapFromArrayToCustomCollection(input);

        // Assert
        actual.Should().BeEquivalentTo("1", "2", "3");
    }

    /// <summary>
    /// Tests for <see cref="ContainersWithCapacityConstructorMapper.MapFromEnumerableToCustomCollection"/>.
    /// </summary>
    [Fact]
    public void MapFromEnumerableToCustomCollectionTest()
    {
        // Arrange
        IEnumerable<int> input = [1, 2, 3];

        // Act
        var actual = this.mapper.MapFromEnumerableToCustomCollection(input);

        // Assert
        actual.Should().BeEquivalentTo("1", "2", "3");
    }

    /// <summary>
    /// Tests for <see cref="ContainersWithCapacityConstructorMapper.MapFromArrayToCustomSet"/>.
    /// </summary>
    [Fact]
    public void MapFromArrayToCustomSetTest()
    {
        // Arrange
        int[] input = [1, 2, 3];

        // Act
        var actual = this.mapper.MapFromArrayToCustomSet(input);

        // Assert
        actual.Should().BeEquivalentTo("1", "2", "3");
    }

    /// <summary>
    /// Tests for <see cref="ContainersWithCapacityConstructorMapper.MapFromEnumerableToCustomSet"/>.
    /// </summary>
    [Fact]
    public void MapFromEnumerableToCustomSetTest()
    {
        // Arrange
        IEnumerable<int> input = [1, 2, 3];

        // Act
        var actual = this.mapper.MapFromEnumerableToCustomSet(input);

        // Assert
        actual.Should().BeEquivalentTo("1", "2", "3");
    }

    /// <summary>
    /// Tests for <see cref="ContainersWithCapacityConstructorMapper.MapFromArrayToCustomStack"/>.
    /// </summary>
    [Fact]
    public void MapFromArrayToCustomStackTest()
    {
        // Arrange
        int[] input = [1, 2, 3];

        // Act
        var actual = this.mapper.MapFromArrayToCustomStack(input);

        // Assert
        actual.Should().BeEquivalentTo("1", "2", "3");
    }

    /// <summary>
    /// Tests for <see cref="ContainersWithCapacityConstructorMapper.MapFromEnumerableToCustomStack"/>.
    /// </summary>
    [Fact]
    public void MapFromEnumerableToCustomStackTest()
    {
        // Arrange
        IEnumerable<int> input = [1, 2, 3];

        // Act
        var actual = this.mapper.MapFromEnumerableToCustomStack(input);

        // Assert
        actual.Should().BeEquivalentTo("1", "2", "3");
    }

    /// <summary>
    /// Tests for <see cref="ContainersWithCapacityConstructorMapper.MapFromArrayToCustomQueue"/>.
    /// </summary>
    [Fact]
    public void MapFromArrayToCustomQueueTest()
    {
        // Arrange
        int[] input = [1, 2, 3];

        // Act
        var actual = this.mapper.MapFromArrayToCustomQueue(input);

        // Assert
        actual.Should().BeEquivalentTo("1", "2", "3");
    }

    /// <summary>
    /// Tests for <see cref="ContainersWithCapacityConstructorMapper.MapFromEnumerableToCustomQueue"/>.
    /// </summary>
    [Fact]
    public void MapFromEnumerableToCustomQueueTest()
    {
        // Arrange
        IEnumerable<int> input = [1, 2, 3];

        // Act
        var actual = this.mapper.MapFromEnumerableToCustomQueue(input);

        // Assert
        actual.Should().BeEquivalentTo("1", "2", "3");
    }

    /// <summary>
    /// Tests for <see cref="ContainersWithCapacityConstructorMapper.MapFromArrayToCustomBlockingCollection"/>.
    /// </summary>
    [Fact]
    public void MapFromArrayToCustomBlockingCollectionTest()
    {
        // Arrange
        int[] input = [1, 2, 3];

        // Act
        var actual = this.mapper.MapFromArrayToCustomBlockingCollection(input);

        // Assert
        actual.Should().BeEquivalentTo("1", "2", "3");
    }

    /// <summary>
    /// Tests for <see cref="ContainersWithCapacityConstructorMapper.MapFromEnumerableToCustomBlockingCollection"/>.
    /// </summary>
    [Fact]
    public void MapFromEnumerableToCustomBlockingCollectionTest()
    {
        // Arrange
        IEnumerable<int> input = [1, 2, 3];

        // Act
        var actual = this.mapper.MapFromEnumerableToCustomBlockingCollection(input);

        // Assert
        actual.Should().BeEquivalentTo("1", "2", "3");
    }
}