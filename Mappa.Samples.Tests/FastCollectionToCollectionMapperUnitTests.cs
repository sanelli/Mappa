// <copyright file="FastCollectionToCollectionMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="FastCollectionToCollectionMapper"/>.
/// </summary>
public sealed class FastCollectionToCollectionMapperUnitTests
{
    private readonly FastCollectionToCollectionMapper mapper = new();

    /// <summary>
    /// Tests <see cref="FastCollectionToCollectionMapper.MapArrayToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapArrayToArray()
    {
        // Arrange
        CountingValues[] source = [CountingValues.Three, CountingValues.Two, CountingValues.One];

        // Act
        var actual = this.mapper.MapArrayToArray(source);

        // Assert
        actual.Should().BeEquivalentTo([2, 1, 0]);
    }

    /// <summary>
    /// Tests <see cref="FastCollectionToCollectionMapper.MapArrayToList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapArrayToList()
    {
        // Arrange
        CountingValues[] source = [CountingValues.Three, CountingValues.Two, CountingValues.One];

        // Act
        var actual = this.mapper.MapArrayToList(source);

        // Assert
        actual.Should().BeEquivalentTo([2, 1, 0]);
    }

    /// <summary>
    /// Tests <see cref="FastCollectionToCollectionMapper.MapListToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapListToArray()
    {
        // Arrange
        List<CountingValues> source = [CountingValues.Three, CountingValues.Two, CountingValues.One];

        // Act
        var actual = this.mapper.MapListToArray(source);

        // Assert
        actual.Should().BeEquivalentTo([2, 1, 0]);
    }

    /// <summary>
    /// Tests <see cref="FastCollectionToCollectionMapper.MapListToList"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapListToList()
    {
        // Arrange
        List<CountingValues> source = [CountingValues.Three, CountingValues.Two, CountingValues.One];

        // Act
        var actual = this.mapper.MapListToList(source);

        // Assert
        actual.Should().BeEquivalentTo([2, 1, 0]);
    }
}