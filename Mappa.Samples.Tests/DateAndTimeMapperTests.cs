// <copyright file="DateAndTimeMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Xunit;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="DateAndTimeMapper"/>.
/// </summary>
public sealed class DateAndTimeMapperTests
{
    private readonly DateAndTimeMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapDateTimeToDateOnly"/>.
    /// </summary>
    [Fact]
    public void CanMapFromDateTimeToDateOnly()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapper.MapDateTimeToDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.FromDateTime(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapDateTimeToTimeOnly"/>.
    /// </summary>
    [Fact]
    public void CanMapFromDateTimeToTimeOnly()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapper.MapDateTimeToTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.FromDateTime(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapDateTimeToLong"/>.
    /// </summary>
    [Fact]
    public void CanMapFromDateTimeToLong()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapper.MapDateTimeToLong(input);

        // Assert
        actual.Should().Be((long)input.Subtract(DateTime.UnixEpoch).TotalSeconds);
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapDateOnlyToDateTime"/>.
    /// </summary>
    [Fact]
    public void CanMapFromDateOnlyToDateTime()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapper.MapDateOnlyToDateTime(input);

        // Assert
        actual.Should().Be(new DateTime(input, TimeOnly.MinValue, DateTimeKind.Utc));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapLongToDateTime"/>.
    /// </summary>
    [Fact]
    public void CanMapFromLongToDateTime()
    {
        // Arrange
        const long input = 100L;

        // Act
        var actual = this.mapper.MapLongToDateTime(input);

        // Assert
        actual.Should().Be(DateTime.UnixEpoch.AddSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapUintToDateTime"/>.
    /// </summary>
    [Fact]
    public void CanMapFromUintToDateTime()
    {
        // Arrange
        const uint input = 100;

        // Act
        var actual = this.mapper.MapUintToDateTime(input);

        // Assert
        actual.Should().Be(DateTime.UnixEpoch.AddSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapIntToDateTime"/>.
    /// </summary>
    [Fact]
    public void CanMapFromIntToDateTime()
    {
        // Arrange
        const int input = 100;

        // Act
        var actual = this.mapper.MapIntToDateTime(input);

        // Assert
        actual.Should().Be(DateTime.UnixEpoch.AddSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapUShortToDateTime"/>.
    /// </summary>
    [Fact]
    public void CanMapFromUShortToDateTime()
    {
        // Arrange
        const ushort input = 100;

        // Act
        var actual = this.mapper.MapUShortToDateTime(input);

        // Assert
        actual.Should().Be(DateTime.UnixEpoch.AddSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapShortToDateTime"/>.
    /// </summary>
    [Fact]
    public void CanMapFromShortToDateTime()
    {
        // Arrange
        const short input = 100;

        // Act
        var actual = this.mapper.MapShortToDateTime(input);

        // Assert
        actual.Should().Be(DateTime.UnixEpoch.AddSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapSByteToDateTime"/>.
    /// </summary>
    [Fact]
    public void CanMapFromSByteToDateTime()
    {
        // Arrange
        const sbyte input = 100;

        // Act
        var actual = this.mapper.MapSByteToDateTime(input);

        // Assert
        actual.Should().Be(DateTime.UnixEpoch.AddSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapByteToDateTime"/>.
    /// </summary>
    [Fact]
    public void CanMapFromByteToDateTime()
    {
        // Arrange
        const byte input = 100;

        // Act
        var actual = this.mapper.MapByteToDateTime(input);

        // Assert
        actual.Should().Be(DateTime.UnixEpoch.AddSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapDateOnlyToLong"/>.
    /// </summary>
    [Fact]
    public void CanMapFromDateOnlyToLong()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapper.MapDateOnlyToLong(input);

        // Assert
        actual.Should().Be((long)new DateTime(input, TimeOnly.MinValue, DateTimeKind.Utc).ToUniversalTime().Subtract(DateTime.UnixEpoch).TotalSeconds);
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapTimeSpanToDouble"/>.
    /// </summary>
    [Fact]
    public void CanMapFromTimeSpanToDouble()
    {
        // Arrange
        var input = TimeSpan.FromMilliseconds(1234);

        // Act
        var actual = this.mapper.MapTimeSpanToDouble(input);

        // Assert
        actual.Should().Be(input.TotalSeconds);
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapDoubleToTimeSpan"/>.
    /// </summary>
    [Fact]
    public void CanMapFromDoubleToTimeSpan()
    {
        // Arrange
        const double input = 100;

        // Act
        var actual = this.mapper.MapDoubleToTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.FromSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapFloatToTimeSpan"/>.
    /// </summary>
    [Fact]
    public void CanMapFromFloatToTimeSpan()
    {
        // Arrange
        const float input = 100;

        // Act
        var actual = this.mapper.MapFloatToTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.FromSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapULongToTimeSpan"/>.
    /// </summary>
    [Fact]
    public void CanMapFromULongToTimeSpan()
    {
        // Arrange
        const ulong input = 100;

        // Act
        var actual = this.mapper.MapULongToTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.FromSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapLongToTimeSpan"/>.
    /// </summary>
    [Fact]
    public void CanMapFromLongToTimeSpan()
    {
        // Arrange
        const long input = 100;

        // Act
        var actual = this.mapper.MapLongToTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.FromSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapUintToTimeSpan"/>.
    /// </summary>
    [Fact]
    public void CanMapFromUintToTimeSpan()
    {
        // Arrange
        const uint input = 100;

        // Act
        var actual = this.mapper.MapUintToTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.FromSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapIntToTimeSpan"/>.
    /// </summary>
    [Fact]
    public void CanMapFromIntToTimeSpan()
    {
        // Arrange
        const int input = 100;

        // Act
        var actual = this.mapper.MapIntToTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.FromSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapUShortToTimeSpan"/>.
    /// </summary>
    [Fact]
    public void CanMapFromUShortToTimeSpan()
    {
        // Arrange
        const ushort input = 100;

        // Act
        var actual = this.mapper.MapUShortToTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.FromSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapShortToTimeSpan"/>.
    /// </summary>
    [Fact]
    public void CanMapFromShortToTimeSpan()
    {
        // Arrange
        const short input = 100;

        // Act
        var actual = this.mapper.MapShortToTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.FromSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapSByteToTimeSpan"/>.
    /// </summary>
    [Fact]
    public void CanMapFromSByteToTimeSpan()
    {
        // Arrange
        const sbyte input = 100;

        // Act
        var actual = this.mapper.MapSByteToTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.FromSeconds(input));
    }

    /// <summary>
    /// Unit test for <see cref="DateAndTimeMapper.MapByteToTimeSpan"/>.
    /// </summary>
    [Fact]
    public void CanMapFromByteToTimeSpan()
    {
        // Arrange
        const byte input = 100;

        // Act
        var actual = this.mapper.MapByteToTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.FromSeconds(input));
    }
}