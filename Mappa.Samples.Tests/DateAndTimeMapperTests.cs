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
}