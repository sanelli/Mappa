// <copyright file="PreventEnumerableCountMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Tests.Extensions;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="PreventEnumerableCountMapper"/>.
/// </summary>
public sealed class PreventEnumerableCountMapperUnitTests
{
    private readonly PreventEnumerableCountMapper mapper = new();

    /// <summary>
    /// Tests <see cref="PreventEnumerableCountMapper.MapEnumerableToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapEnumerableToArray()
    {
        // Arrange
        IEnumerable<CountingValues> source = [CountingValues.Three, CountingValues.Two, CountingValues.One];

        // Act
        var actual = this.mapper.MapEnumerableToArray(source);

        // Assert
        actual.Should().BeEquivalentTo([2, 1, 0]);
    }

    /// <summary>
    /// Tests <see cref="PreventEnumerableCountMapper.MapEnumerableToSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapEnumerableToSpan()
    {
        // Arrange
        IEnumerable<CountingValues> source = [CountingValues.Three, CountingValues.Two, CountingValues.One];

        // Act
        var actual = this.mapper.MapEnumerableToSpan(source);

        // Assert
        actual.ShouldBeExactly([2, 1, 0]);
    }
}