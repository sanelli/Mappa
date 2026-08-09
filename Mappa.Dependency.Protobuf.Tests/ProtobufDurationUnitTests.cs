// <copyright file="ProtobufDurationUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Security.Cryptography;

using AwesomeAssertions;

using Google.Protobuf.WellKnownTypes;

using Xunit.OpenCategories.V3;

namespace Mappa.Dependency.Protobuf.Tests;

/// <summary>
/// Tests related to mapping to an from <see cref="Timestamp"/>.
/// </summary>
public sealed class ProtobufDurationUnitTests
{
    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromTimeSpanToDuration"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromTimeSpanToDuration()
    {
        // Arrange
        var timespan = TimeSpan.FromMilliseconds(RandomNumberGenerator.GetInt32(0, 100));
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromTimeSpanToDuration(timespan);

        // Assert
        actual.ToTimeSpan().Should().Be(timespan);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableTimeSpanToNullableDuration"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableTimeSpanToNullableDuration()
    {
        // Arrange
        var timespan = TimeSpan.FromMilliseconds(RandomNumberGenerator.GetInt32(0, 100));
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableTimeSpanToNullableDuration(timespan);

        // Assert
        actual.Should().NotBeNull();
        actual.ToTimeSpan().Should().Be(timespan);
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableTimeSpanToNullableDuration"/>
    /// return <c>null</c> when input is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableTimeSpanToNullableDurationWhenInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableTimeSpanToNullableDuration(null);

        // Assert
        actual.Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromDurationToTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromDurationToTimeSpan()
    {
        // Arrange
        var expected = TimeSpan.FromMilliseconds(RandomNumberGenerator.GetInt32(0, 100));
        var duration = Duration.FromTimeSpan(expected);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromDurationToTimeSpan(duration);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Test that <see cref="MappaProtobufMapper.MapFromDurationToTimeSpan"/>
    /// throws when input is <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapFromDurationToTimeSpanThrowsIfInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();
        var action = () => mapper.MapFromDurationToTimeSpan(null!);

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Test <see cref="MappaProtobufMapper.MapFromNullableDurationToNullableTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableDurationToNullableTimeSpan()
    {
        // Arrange
        var expected = TimeSpan.FromMilliseconds(RandomNumberGenerator.GetInt32(0, 100));
        var duration = Duration.FromTimeSpan(expected);
        var mapper = new MappaProtobufMapper();

        // Act
        var actual = mapper.MapFromNullableDurationToNullableTimeSpan(duration);

        // Assert
        actual.Should().NotBeNull();
        actual.Value.Should().Be(expected);
    }

    /// <summary>
    /// Test that <see cref="MappaProtobufMapper.MapFromNullableDurationToNullableTimeSpan"/>
    /// return <c>null</c> when input is <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFromNullableDurationToNullableTimeSpanWhenInputIsNull()
    {
        // Arrange
        var mapper = new MappaProtobufMapper();
        var actual = mapper.MapFromNullableDurationToNullableTimeSpan(null);

        // Assert
        actual.Should().BeNull();
    }
}