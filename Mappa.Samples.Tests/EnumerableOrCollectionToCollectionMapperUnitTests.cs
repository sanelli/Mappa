// <copyright file="EnumerableOrCollectionToCollectionMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="EnumerableOrCollectionToCollectionMapper"/>.
/// </summary>
public sealed class EnumerableOrCollectionToCollectionMapperUnitTests
{
    private readonly EnumerableOrCollectionToCollectionMapper mapper = new();

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapIEnumerableToIList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIEnumerableToIList()
    {
        // Arrange
        IEnumerable<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIEnumerableToIList(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapIEnumerableToList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIEnumerableToList()
    {
        // Arrange
        IEnumerable<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIEnumerableToList(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapIEnumerableToICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIEnumerableToICollection()
    {
        // Arrange
        IEnumerable<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIEnumerableToICollection(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapIEnumerableToIReadOnlyCollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIEnumerableToIReadOnlyCollection()
    {
        // Arrange
        IEnumerable<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIEnumerableToIReadOnlyCollection(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapIEnumerableToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIEnumerableToIEnumerable()
    {
        // Arrange
        IEnumerable<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIEnumerableToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapICollectionToIList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapICollectionToIList()
    {
        // Arrange
        ICollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapICollectionToIList(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapICollectionToList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapICollectionToList()
    {
        // Arrange
        ICollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapICollectionToList(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapICollectionToICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapICollectionToICollection()
    {
        // Arrange
        ICollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapICollectionToICollection(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapICollectionToIReadOnlyCollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapICollectionToIReadOnlyCollection()
    {
        // Arrange
        ICollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapICollectionToIReadOnlyCollection(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapICollectionToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapICollectionToIEnumerable()
    {
        // Arrange
        ICollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapICollectionToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapIReadOnlyCollectionToIList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIReadOnlyCollectionToIList()
    {
        // Arrange
        IReadOnlyCollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIReadOnlyCollectionToIList(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapIReadOnlyCollectionToList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIReadOnlyCollectionToList()
    {
        // Arrange
        IReadOnlyCollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIReadOnlyCollectionToList(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapIReadOnlyCollectionToICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIReadOnlyCollectionToICollection()
    {
        // Arrange
        IReadOnlyCollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIReadOnlyCollectionToICollection(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapIReadOnlyCollectionToIReadOnlyCollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIReadOnlyCollectionToIReadOnlyCollection()
    {
        // Arrange
        IReadOnlyCollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIReadOnlyCollectionToIReadOnlyCollection(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToCollectionMapper.MapIReadOnlyCollectionToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIReadOnlyCollectionToIEnumerable()
    {
        // Arrange
        IReadOnlyCollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapIReadOnlyCollectionToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }
}