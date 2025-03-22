// <copyright file="CollectionToCollectionMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="CollectionToCollectionMapper"/>.
/// </summary>
public sealed class CollectionToCollectionMapperUnitTests
{
    private readonly CollectionToCollectionMapper mapper = new();

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapIEnumerableToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIEnumerableToIEnumerable()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapIEnumerableToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromNonGenericTypeImplementingIEnumerableToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNonGenericTypeImplementingIEnumerableToIEnumerable()
    {
        // Arrange
        CustomCollectionImplementingIEnumerableOfCountingValues input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromNonGenericTypeImplementingIEnumerableToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromGenericTypeImplementingIEnumerableToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGenericTypeImplementingIEnumerableToIEnumerable()
    {
        // Arrange
        CustomCollectionImplementingIEnumerable<CountingValues> input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromGenericTypeImplementingIEnumerableToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToIEnumerable()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromSpanToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromSpanToIEnumerable()
    {
        // Arrange
        Span<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromSpanToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromReadOnlySpanToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromReadOnlySpanToIEnumerable()
    {
        // Arrange
        ReadOnlySpan<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromReadOnlySpanToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromMemoryToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromMemoryToIEnumerable()
    {
        // Arrange
        Memory<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapFromMemoryToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromReadOnlyMemoryToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromReadOnlyMemoryToIEnumerable()
    {
        // Arrange
        ReadOnlyMemory<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.MapFromReadOnlyMemoryToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIListToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIListToIEnumerable()
    {
        // Arrange
        IList<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIListToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromNonGenericTypeImplementingIListToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNonGenericTypeImplementingIListToIEnumerable()
    {
        // Arrange
        CustomCollectionImplementingIListOfCountingValues input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromNonGenericTypeImplementingIListToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromGenericTypeImplementingIListToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGenericTypeImplementingIListToIEnumerable()
    {
        // Arrange
        CustomCollectionImplementingIList<CountingValues> input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromGenericTypeImplementingIListToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }
}