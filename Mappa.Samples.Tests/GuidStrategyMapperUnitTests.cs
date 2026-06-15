// <copyright file="GuidStrategyMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="GuidStrategyMapper"/>.
/// </summary>
public sealed class GuidStrategyMapperUnitTests
{
    private readonly GuidStrategyMapper mapper = new();

    /// <summary>
    /// Unit tests for <see cref="GuidStrategyMapper.MapFromGuidToArray"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGuidToArray()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapper.MapFromGuidToArray(input);

        // Assert
        new Guid(actual).Should().Be(input);
    }

    /// <summary>
    /// Unit tests for <see cref="GuidStrategyMapper.MapFromGuidToSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGuidToSpan()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapper.MapFromGuidToSpan(input);

        // Assert
        new Guid(actual).Should().Be(input);
    }

    /// <summary>
    /// Unit tests for <see cref="GuidStrategyMapper.MapFromGuidToReadOnlySpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGuidToReadOnlySpan()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapper.MapFromGuidToReadOnlySpan(input);

        // Assert
        new Guid(actual).Should().Be(input);
    }

    /// <summary>
    /// Unit tests for <see cref="GuidStrategyMapper.MapFromGuidToMemory"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGuidToMemory()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapper.MapFromGuidToMemory(input);

        // Assert
        new Guid(actual.Span).Should().Be(input);
    }

    /// <summary>
    /// Unit tests for <see cref="GuidStrategyMapper.MapFromGuidToReadOnlyMemory"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromGuidToReadOnlyMemory()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapper.MapFromGuidToReadOnlyMemory(input);

        // Assert
        new Guid(actual.Span).Should().Be(input);
    }

    /// <summary>
    /// Unit tests for <see cref="GuidStrategyMapper.MapArrayToGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapArrayToGuid()
    {
        // Arrange
        var expected = Guid.NewGuid();
        byte[] input = expected.ToByteArray();

        // Act
        var actual = this.mapper.MapArrayToGuid(input);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit tests for <see cref="GuidStrategyMapper.MapSpanToGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapSpanToGuid()
    {
        // Arrange
        var expected = Guid.NewGuid();
        Span<byte> input = expected.ToByteArray();

        // Act
        var actual = this.mapper.MapSpanToGuid(input);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit tests for <see cref="GuidStrategyMapper.MapReadOnlySpanToGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapReadOnlySpanToGuid()
    {
        // Arrange
        var expected = Guid.NewGuid();
        ReadOnlySpan<byte> input = expected.ToByteArray();

        // Act
        var actual = this.mapper.MapReadOnlySpanToGuid(input);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit tests for <see cref="GuidStrategyMapper.MapMemoryToGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMemoryToGuid()
    {
        // Arrange
        var expected = Guid.NewGuid();
        Memory<byte> input = expected.ToByteArray();

        // Act
        var actual = this.mapper.MapMemoryToGuid(input);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit tests for <see cref="GuidStrategyMapper.MapReadOnlyMemoryToGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapReadOnlyMemoryToGuid()
    {
        // Arrange
        var expected = Guid.NewGuid();
        ReadOnlyMemory<byte> input = expected.ToByteArray();

        // Act
        var actual = this.mapper.MapReadOnlyMemoryToGuid(input);

        // Assert
        actual.Should().Be(expected);
    }
}