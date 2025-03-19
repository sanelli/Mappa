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
}