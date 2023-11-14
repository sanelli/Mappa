// <copyright file="ArrayOrListToCollectionMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for mapper <see cref="ArrayOrListToCollectionMapper"/>.
/// </summary>
public class ArrayOrListToCollectionMapperUnitTest
{
    private static readonly int[] ExpectedZeroAndTwoArray = { 0, 2 };
    private readonly ArrayOrListToCollectionMapper mapper = new();

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapArrayToIList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapArrayToIListToIList()
    {
        // Arrange
        var input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapArrayToIList(input);

        // Assert
        var expectation = ExpectedZeroAndTwoArray;
        actual.Should().BeEquivalentTo(expectation);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapArrayToList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapArrayToIListToList()
    {
        // Arrange
        var input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapArrayToList(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapArrayToICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapArrayToIListToICollection()
    {
        // Arrange
        var input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapArrayToICollection(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapArrayToIReadOnlyCollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapArrayToIListToIReadOnlyCollection()
    {
        // Arrange
        var input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapArrayToIReadOnlyCollection(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapArrayToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapArrayToIListToIEnumerable()
    {
        // Arrange
        var input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapArrayToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapIListToIList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapIListToIListToIList()
    {
        // Arrange
        IList<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIListToIList(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapIListToList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapIListToIListToList()
    {
        // Arrange
        IList<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIListToList(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapIListToICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapIListToIListToICollection()
    {
        // Arrange
        IList<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIListToICollection(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapIListToIReadOnlyCollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapIListToIListToIReadOnlyCollection()
    {
        // Arrange
        IList<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIListToIReadOnlyCollection(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapIListToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapIListToIListToIEnumerable()
    {
        // Arrange
        IList<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIListToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapListToIList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapListToIListToIList()
    {
        // Arrange
        List<CountingValues> input = new() { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapListToIList(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapListToList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapListToIListToList()
    {
        // Arrange
        List<CountingValues> input = new() { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapListToList(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapListToICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapListToIListToICollection()
    {
        // Arrange
        List<CountingValues> input = new() { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapListToICollection(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapListToIReadOnlyCollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapListToIListToIReadOnlyCollection()
    {
        // Arrange
        List<CountingValues> input = new() { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapListToIReadOnlyCollection(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }

    /// <summary>
    /// Unit test <see cref="ArrayOrListToCollectionMapper.MapListToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapListToIListToIEnumerable()
    {
        // Arrange
        List<CountingValues> input = new() { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapListToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo(ExpectedZeroAndTwoArray);
    }
}