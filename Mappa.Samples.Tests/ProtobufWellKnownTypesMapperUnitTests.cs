// <copyright file="ProtobufWellKnownTypesMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="ProtobufWellKnownTypesMapper"/>.
/// </summary>
public sealed class ProtobufWellKnownTypesMapperUnitTests
{
    private readonly ProtobufWellKnownTypesMapper mapper = new();

    /// <summary>
    /// Tests nested Timestamp mapping via <see cref="ProtobufWellKnownTypesMapper.MapTimestamp"/>.
    /// </summary>
    [Fact]
    [Bug("#121")]
    [UnitTest]
    public void CanMapTimestampOnRecord()
    {
        // Arrange
        var timestamp = new DateTime(1984, 06, 03, 14, 22, 00, DateTimeKind.Utc);
        var source = new MappaDependencySourceRecord(timestamp);

        // Act
        var target = this.mapper.MapTimestamp(source);

        // Assert
        target.TimeStamp.ToDateTime().Should().Be(timestamp);
    }

    /// <summary>
    /// Tests <see cref="DateTime"/> ↔ <see cref="Timestamp"/> mapping.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeAndTimestamp()
    {
        // Arrange
        var dateTime = new DateTime(2020, 01, 02, 03, 04, 05, DateTimeKind.Utc);

        // Act
        var timestamp = this.mapper.MapDateTimeToTimestamp(dateTime);
        var roundTrip = this.mapper.MapTimestampToDateTime(timestamp);

        // Assert
        roundTrip.Should().Be(dateTime);
    }

    /// <summary>
    /// Tests <see cref="TimeSpan"/> ↔ <see cref="Duration"/> mapping.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanAndDuration()
    {
        // Arrange
        var timeSpan = TimeSpan.FromMinutes(42);

        // Act
        var duration = this.mapper.MapTimeSpanToDuration(timeSpan);
        var roundTrip = this.mapper.MapDurationToTimeSpan(duration);

        // Assert
        roundTrip.Should().Be(timeSpan);
    }

    /// <summary>
    /// Tests representative protobuf scalar wrappers.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapScalarWrappers()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3 };

        // Act / Assert
        this.mapper.MapStringValueToString(this.mapper.MapStringToStringValue("hello")).Should().Be("hello");
        this.mapper.MapBoolValueToBool(this.mapper.MapBoolToBoolValue(true)).Should().BeTrue();
        this.mapper.MapBytesValueToBytes(this.mapper.MapBytesToBytesValue(bytes)).Should().Equal(bytes);
        this.mapper.MapInt32ValueToInt(this.mapper.MapIntToInt32Value(33)).Should().Be(33);
        this.mapper.MapDoubleValueToDouble(this.mapper.MapDoubleToDoubleValue(1.5)).Should().Be(1.5);
    }
}