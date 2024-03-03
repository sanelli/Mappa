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
    /// Test that <see cref="MappaProtobufMapper.MapFromTimestampToDateTime"/>
    /// throws when input is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapFromTimestampToDateTimeThrowsIfInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();
        var action = () => mapper.MapFromTimestampToDateTime(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableTimestampToNullableDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableTimestampToNullableDateTime()
    {
        // Arrange
        var expected = DateTime.UtcNow;
        var timestamp = Timestamp.FromDateTime(expected);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableTimestampToNullableDateTime(timestamp);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableTimestampToNullableDateTime"/>
    /// return <c>null</c> input is <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableTimestampToNullableDateTimeWhenInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableTimestampToNullableDateTime(null);

        // Assert
        actual.Should().BeNull();
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
    /// Test that <see cref="MappaProtobufMapper.MapFromTimestampToDateTimeOffset"/>
    /// throws when input is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapFromTimestampToDateTimeOffsetThrowsIfInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();
        var action = () => mapper.MapFromTimestampToDateTimeOffset(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableTimestampToNullableDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableTimestampToNullableDateTimeOffset()
    {
        // Arrange
        var expected = DateTime.UtcNow;
        var timestamp = Timestamp.FromDateTime(expected);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableTimestampToNullableDateTimeOffset(timestamp);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableTimestampToNullableDateTimeOffset"/>
    /// return <c>null</c> input is <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableTimestampToNullableDateTimeOffsetWhenInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableTimestampToNullableDateTimeOffset(null);

        // Assert
        actual.Should().BeNull();
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
    /// Test that <see cref="MappaProtobufMapper.MapFromTimestampToDateOnly"/>
    /// throws when input is <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapFromTimestampToDateOnlyThrowsIfInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();
        var action = () => mapper.MapFromTimestampToDateOnly(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableTimestampToNullableDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableTimestampToNullableDateOnly()
    {
        // Arrange
        var expected = DateOnly.FromDateTime(DateTime.UtcNow).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var timestamp = Timestamp.FromDateTime(expected);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableTimestampToNullableDateOnly(timestamp);

        // Assert
        actual.Should().NotBeNull();
        actual!.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).Should().Be(expected);
    }

    /// <summary>
    /// Test that <see cref="MappaProtobufMapper.MapFromNullableTimestampToNullableDateOnly"/>
    /// returns <c>null</c> when input is <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapMapFromNullableTimestampToNullableDateOnlyWhenInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();
        var actual = mapper.MapFromNullableTimestampToNullableDateOnly(null);

        // Assert
        actual.Should().BeNull();
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
    /// Test that <see cref="MappaProtobufMapper.MapFromTimestampToTimeOnly"/>
    /// throws when input is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapFromTimestampToTimeOnlyThrowsIfInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();
        var action = () => mapper.MapFromTimestampToTimeOnly(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableTimestampToNullableTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableTimestampToNullableTimeOnly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var expected = DateOnly.FromDateTime(now).ToDateTime(TimeOnly.FromDateTime(now), DateTimeKind.Utc);
        var timestamp = Timestamp.FromDateTime(expected);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableTimestampToNullableTimeOnly(timestamp);

        // Assert
        actual.Should().NotBeNull();
        DateOnly.FromDateTime(now).ToDateTime(actual!.Value, DateTimeKind.Utc).Should().Be(expected);
    }

    /// <summary>
    /// Test that <see cref="MappaProtobufMapper.MapFromNullableTimestampToNullableTimeOnly"/>
    /// return <c>null</c> when input is  <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableTimestampToNullableTimeOnlyWhenInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();
        var actual = mapper.MapFromNullableTimestampToNullableTimeOnly(null);

        // Assert
        actual.Should().BeNull();
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
    /// Test <see cref="MappaProtobufMapper.MapFromNullableDateTimeToNullableTimestamp"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableDateTimeToNullableTimestamp()
    {
        // Arrange
        var expected = DateTime.UtcNow;
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableDateTimeToNullableTimestamp(expected);

        // Assert
        actual.Should().NotBeNull();
        actual!.ToDateTime().Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableDateTimeToNullableTimestamp"/>
    /// when input is <c>null</c> returns <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableDateTimeToNullableTimestampWhenInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableDateTimeToNullableTimestamp(null);

        // Assert
        actual.Should().BeNull();
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
    /// Test <see cref="MappaProtobufMapper.MapFromNullableDateTimeOffsetToNullableTimestamp"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableDateTimeOffsetToNullableTimestamp()
    {
        // Arrange
        var expected = DateTimeOffset.UtcNow;
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableDateTimeOffsetToNullableTimestamp(expected);

        // Assert
        actual.Should().NotBeNull();
        actual!.ToDateTimeOffset().Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableDateTimeOffsetToNullableTimestamp"/>
    /// when input is <c>null</c> returns <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableDateTimeOffsetToNullableTimestampWhenInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableDateTimeOffsetToNullableTimestamp(null);

        // Assert
        actual.Should().BeNull();
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

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableDateOnlyToNullableTimestamp"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableDateOnlyToNullableTimestamp()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var dateOnly = DateOnly.FromDateTime(now);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableDateOnlyToNullableTimestamp(dateOnly);

        // Assert
        actual.Should().NotBeNull();
        DateOnly.FromDateTime(actual!.ToDateTime()).Should().Be(dateOnly);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableDateOnlyToNullableTimestamp"/>
    /// when input <c>null</c> returns <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableDateOnlyToNullableTimestampWhenInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableDateOnlyToNullableTimestamp(null);

        // Assert
        actual.Should().BeNull();
    }
}