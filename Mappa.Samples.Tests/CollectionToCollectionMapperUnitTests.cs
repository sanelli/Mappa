// <copyright file="CollectionToCollectionMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;
using Mappa.Samples.Tests.Extensions;

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

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToSpan()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToSpan(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIListToSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIListToSpan()
    {
        // Arrange
        IList<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIListToSpan(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromICollectionToSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromICollectionToSpan()
    {
        // Arrange
        ICollection<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromICollectionToSpan(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToSpan()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToSpan(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToReadOnlySpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToReadOnlySpan()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToReadOnlySpan(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIListToReadOnlySpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIListToReadOnlySpan()
    {
        // Arrange
        IList<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIListToReadOnlySpan(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromICollectionToReadOnlySpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromICollectionToReadOnlySpan()
    {
        // Arrange
        ICollection<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromICollectionToReadOnlySpan(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToReadOnlySpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToReadOnlySpan()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToReadOnlySpan(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToMemory"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToMemory()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToMemory(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIListToMemory"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIListToMemory()
    {
        // Arrange
        IList<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIListToMemory(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromICollectionToMemory"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromICollectionToMemory()
    {
        // Arrange
        ICollection<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromICollectionToMemory(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToMemory"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToMemory()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToMemory(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToReadOnlyMemory"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToReadOnlyMemory()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToReadOnlyMemory(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIListToReadOnlyMemory"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIListToReadOnlyMemory()
    {
        // Arrange
        IList<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIListToReadOnlyMemory(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromICollectionToReadOnlyMemory"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromICollectionToReadOnlyMemory()
    {
        // Arrange
        ICollection<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromICollectionToReadOnlyMemory(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToReadOnlyMemory"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToReadOnlyMemory()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToReadOnlyMemory(input);

        // Assert
        actual.ShouldBeExactly([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToNonGenericTypeImplementingICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToNonGenericTypeImplementingICollection()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToNonGenericTypeImplementingICollection(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToGenericTypeImplementingICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToGenericTypeImplementingICollection()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToGenericTypeImplementingICollection(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToNonGenericTypeImplementingICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToNonGenericTypeImplementingICollection()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToNonGenericTypeImplementingICollection(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToGenericTypeImplementingICollection"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToGenericTypeImplementingICollection()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToGenericTypeImplementingICollection(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToStack"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToStack()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToStack(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToNonGenericTypeDerivedFromStack"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToNonGenericTypeDerivedFromStack()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToNonGenericTypeDerivedFromStack(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayGenericTypeDerivedFromStack"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayGenericTypeDerivedFromStack()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayGenericTypeDerivedFromStack(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToStack"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToStack()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToStack(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToNonGenericTypeDerivedFromStack"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToNonGenericTypeDerivedFromStack()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToNonGenericTypeDerivedFromStack(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableGenericTypeDerivedFromStack"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableGenericTypeDerivedFromStack()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableGenericTypeDerivedFromStack(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToQueue"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToQueue()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToQueue(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayToNonGenericTypeDerivedFromQueue"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayToNonGenericTypeDerivedFromQueue()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayToNonGenericTypeDerivedFromQueue(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromArrayGenericTypeDerivedFromQueue"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromArrayGenericTypeDerivedFromQueue()
    {
        // Arrange
        CountingValues[] input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromArrayGenericTypeDerivedFromQueue(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToQueue"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToQueue()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToQueue(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableToNonGenericTypeDerivedFromQueue"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableToNonGenericTypeDerivedFromQueue()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableToNonGenericTypeDerivedFromQueue(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Unit test <see cref="CollectionToCollectionMapper.MapFromIEnumerableGenericTypeDerivedFromQueue"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromIEnumerableGenericTypeDerivedFromQueue()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var actual = this.mapper.MapFromIEnumerableGenericTypeDerivedFromQueue(input);

        // Assert
        actual.Should().BeEquivalentTo([0, 2]);
    }
}