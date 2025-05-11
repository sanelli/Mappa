// <copyright file="TupleToTupleMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="TupleToTupleMapper"/>.
/// </summary>
public sealed class TupleToTupleMapperUnitTests
{
    private readonly TupleToTupleMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="TupleToTupleMapper.MapSystemTupleToSystemTuple"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapSystemTupleToSystemTuple()
    {
        // Arrange
        Tuple<int, CountingValues, long> input = new(3, CountingValues.Three, 30L);

        // Act
        var actual = this.mapper.MapSystemTupleToSystemTuple(input);

        // Assert
        actual.Item1.Should().Be("3");
        actual.Item2.Should().Be("Three");
        actual.Item3.Should().Be("30");
    }

    /// <summary>
    /// Unit test for <see cref="TupleToTupleMapper.MapTupleToTuple"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTupleToTuple()
    {
        // Arrange
        var input = (3, CountingValues.Three, 30L);

        // Act
        var actual = this.mapper.MapTupleToTuple(input);

        // Assert
        actual.Item1.Should().Be("3");
        actual.Item2.Should().Be("Three");
        actual.Item3.Should().Be("30");
    }

    /// <summary>
    /// Unit test for <see cref="TupleToTupleMapper.MapTupleWithNamesElementsToTupleWithNamesElements"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTupleWithNamesElementsToTupleWithNamesElements()
    {
        // Arrange
        (int Alpha, CountingValues Beta, long Gamma) input = (3, CountingValues.Three, 30L);

        // Act
        var actual = this.mapper.MapTupleWithNamesElementsToTupleWithNamesElements(input);

        // Assert
        actual.First.Should().Be("3");
        actual.Second.Should().Be("Three");
        actual.Third.Should().Be("30");
    }

    /// <summary>
    /// Unit test for <see cref="TupleToTupleMapper.MapSystemValueTupleToSystemValueTuple"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapSystemValueTupleToSystemValueTuple()
    {
        // Arrange
        ValueTuple<int, CountingValues, long, string> input = new(3, CountingValues.Three, 30L, "Stefano");

        // Act
        var actual = this.mapper.MapSystemValueTupleToSystemValueTuple(input);

        // Assert
        actual.Item1.Should().Be("3");
        actual.Item2.Should().Be("Three");
        actual.Item3.Should().Be("30");
        actual.Item4.Should().Be("Stefano");
    }
}