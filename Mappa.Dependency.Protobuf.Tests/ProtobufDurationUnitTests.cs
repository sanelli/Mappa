// <copyright file="ProtobufDurationUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Security.Cryptography;

using FluentAssertions;

using Google.Protobuf.WellKnownTypes;

using Xunit.Categories;

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
    /// throws when input is null.
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
}