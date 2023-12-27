// <copyright file="ProtobufTimestampUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Google.Protobuf.WellKnownTypes;

using Xunit.Categories;

namespace Mappa.Dependency.Protobuf.Tests;

/// <summary>
/// Tests related to mapping to an from <see cref="Timestamp"/>.
/// </summary>
public sealed class ProtobufTimestampUnitTests
{
    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromTimestampToDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromTimestampToDateTime()
    {
        // Arrange
        var expected = DateTime.UtcNow;
        var timestamp = Timestamp.FromDateTime(expected);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromTimestampToDateTime(timestamp);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromTimestampToDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromTimestampToDateTimeOffset()
    {
        // Arrange
        var expected = DateTimeOffset.UtcNow;
        var timestamp = Timestamp.FromDateTimeOffset(expected);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromTimestampToDateTimeOffset(timestamp);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromTimestampToDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromTimestampToDateOnly()
    {
        // Arrange
        var expected = DateOnly.FromDateTime(DateTime.UtcNow).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var timestamp = Timestamp.FromDateTime(expected);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromTimestampToDateOnly(timestamp);

        // Assert
        actual.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromTimestampToTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromTimestampToTimeOnly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var expected = DateOnly.FromDateTime(now).ToDateTime(TimeOnly.FromDateTime(now), DateTimeKind.Utc);
        var timestamp = Timestamp.FromDateTime(expected);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromTimestampToTimeOnly(timestamp);

        // Assert
        DateOnly.FromDateTime(now).ToDateTime(actual, DateTimeKind.Utc).Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromDateTimeToTimestamp"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromDateTimeToTimestamp()
    {
        // Arrange
        var expected = DateTime.UtcNow;
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromDateTimeToTimestamp(expected);

        // Assert
        actual.ToDateTime().Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromDateTimeOffsetToTimestamp"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromDateTimeOffsetToTimestamp()
    {
        // Arrange
        var expected = DateTimeOffset.UtcNow;
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromDateTimeOffsetToTimestamp(expected);

        // Assert
        actual.ToDateTimeOffset().Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromDateOnlyToTimestamp"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromDateOnlyToTimestamp()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dateOnly = DateOnly.FromDateTime(now);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromDateOnlyToTimestamp(dateOnly);

        // Assert
        DateOnly.FromDateTime(actual.ToDateTime()).Should().Be(dateOnly);
    }
}