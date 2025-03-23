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

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromICollectionToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromICollectionToIEnumerable()
    {
        // Arrange
        ICollection<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromICollectionToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromNonGenericTypeImplementingICollectionToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNonGenericTypeImplementingICollectionToIEnumerable()
    {
        // Arrange
        CustomCollectionImplementingICollectionOfCountingValues input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromNonGenericTypeImplementingICollectionToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromGenericTypeImplementingICollectionToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGenericTypeImplementingICollectionToIEnumerable()
    {
        // Arrange
        CustomCollectionImplementingICollection<CountingValues> input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromGenericTypeImplementingICollectionToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIReadOnlyCollectionToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIReadOnlyCollectionToIEnumerable()
    {
        // Arrange
        IReadOnlyCollection<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIReadOnlyCollectionToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromNonGenericTypeImplementingIReadOnlyCollectionToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNonGenericTypeImplementingIReadOnlyCollectionToIEnumerable()
    {
        // Arrange
        CustomCollectionImplementingIReadOnlyCollectionOfCountingValues input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromNonGenericTypeImplementingIReadOnlyCollectionToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromGenericTypeImplementingIReadOnlyCollectionToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGenericTypeImplementingIReadOnlyCollectionToIEnumerable()
    {
        // Arrange
        CustomCollectionImplementingIReadOnlyCollection<CountingValues> input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromGenericTypeImplementingIReadOnlyCollectionToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToList()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToList(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToList()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToList(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToIList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToIList()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToIList(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToIList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToIList()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToIList(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToICollection()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToICollection(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToICollection()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToICollection(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToIReadOnlyCollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToIReadOnlyCollection()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToIReadOnlyCollection(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToIReadOnlyCollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToIReadOnlyCollection()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToIReadOnlyCollection(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromStackToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromStackToIEnumerable()
    {
        // Arrange
        Stack<CountingValues> input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromStackToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromQueueToIEnumerable"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromQueueToIEnumerable()
    {
        // Arrange
        Queue<CountingValues> input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromQueueToIEnumerable(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToArray()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToArray()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromNonGenericTypeImplementingIEnumerableToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNonGenericTypeImplementingIEnumerableToArray()
    {
        // Arrange
        CustomCollectionImplementingIEnumerableOfCountingValues input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromNonGenericTypeImplementingIEnumerableToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromGenericTypeImplementingIEnumerableToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGenericTypeImplementingIEnumerableToArray()
    {
        // Arrange
        CustomCollectionImplementingIEnumerable<CountingValues> input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromGenericTypeImplementingIEnumerableToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromICollectionToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromICollectionToArray()
    {
        // Arrange
        ICollection<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromICollectionToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromNonGenericTypeImplementingICollectionToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNonGenericTypeImplementingICollectionToArray()
    {
        // Arrange
        CustomCollectionImplementingICollectionOfCountingValues input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromNonGenericTypeImplementingICollectionToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromGenericTypeImplementingICollectionToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGenericTypeImplementingICollectionToArray()
    {
        // Arrange
        CustomCollectionImplementingICollection<CountingValues> input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromGenericTypeImplementingICollectionToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIReadOnlyCollectionToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIReadOnlyCollectionToArray()
    {
        // Arrange
        IReadOnlyCollection<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIReadOnlyCollectionToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromNonGenericTypeImplementingIReadOnlyCollectionToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNonGenericTypeImplementingIReadOnlyCollectionToArray()
    {
        // Arrange
        CustomCollectionImplementingIReadOnlyCollectionOfCountingValues input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromNonGenericTypeImplementingIReadOnlyCollectionToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromGenericTypeImplementingIReadOnlyCollectionToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGenericTypeImplementingIReadOnlyCollectionToArray()
    {
        // Arrange
        CustomCollectionImplementingIReadOnlyCollection<CountingValues> input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromGenericTypeImplementingIReadOnlyCollectionToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromStackToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromStackToArray()
    {
        // Arrange
        Stack<CountingValues> input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromStackToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromQueueToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromQueueToArray()
    {
        // Arrange
        Queue<CountingValues> input = new([CountingValues.One, CountingValues.Three]);

        // Act
        var actual = this.mapper.MapFromQueueToArray(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }
}