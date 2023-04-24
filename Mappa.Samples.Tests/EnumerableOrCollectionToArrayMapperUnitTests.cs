// <copyright file="EnumerableOrCollectionToArrayMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="EnumerableOrCollectionToArrayMapper"/>.
/// </summary>
public sealed class EnumerableOrCollectionToArrayMapperUnitTests
{
    private readonly EnumerableOrCollectionToArrayMapper mapper = new();

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToArrayMapper.Map(System.Collections.Generic.IEnumerable{Mappa.Samples.Models.CountingValues})"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIEnumerableToArray()
    {
        // Arrange
        IEnumerable<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.Map(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToArrayMapper.Map(System.Collections.Generic.ICollection{Mappa.Samples.Models.CountingValues})"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapICollectionToArray()
    {
        // Arrange
        ICollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.Map(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }

    /// <summary>
    /// Unit test <see cref="EnumerableOrCollectionToArrayMapper.Map(System.Collections.Generic.IReadOnlyCollection{Mappa.Samples.Models.CountingValues})"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIReadOnlyCollectionToArray()
    {
        // Arrange
        IReadOnlyCollection<CountingValues> input = new[] { CountingValues.One, CountingValues.Three };

        // Act
        var actual = this.mapper.Map(input);

        // Assert
        actual.Should().BeEquivalentTo(new[] { 0, 2 });
    }
}