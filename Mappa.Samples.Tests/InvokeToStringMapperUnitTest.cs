// <copyright file="InvokeToStringMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="InvokeToStringMapper"/>.
/// </summary>
public sealed class InvokeToStringMapperUnitTest
{
    private readonly InvokeToStringMapper mapper = new();
    private readonly InvokeToStringMapperWithFormatSettingsOnMethod mapperWithFormatSettingsOnMethod = new();

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapper.MapInt"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIntToString()
    {
        // Arrange
        var input = 100;

        // Act
        var actual = this.mapper.MapInt(input);

        // Assert
#pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString());
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapper.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToString()
    {
        // Arrange
        var dateTime = DateTime.UtcNow;

        // Act
        var actual = this.mapper.MapDateTime(dateTime);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(dateTime.ToString());
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapper.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToString()
    {
        // Arrange
        var dateTimeOffset = DateTimeOffset.Now;

        // Act
        var actual = this.mapper.MapDateTimeOffset(dateTimeOffset);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(dateTimeOffset.ToString());
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapper.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToString()
    {
        // Arrange
        var dateOnly = DateOnly.FromDateTime(DateTime.Now);

        // Act
        var actual = this.mapper.MapDateOnly(dateOnly);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(dateOnly.ToString());
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapper.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToString()
    {
        // Arrange
        var timeOnly = TimeOnly.FromDateTime(DateTime.Now);

        // Act
        var actual = this.mapper.MapTimeOnly(timeOnly);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(timeOnly.ToString());
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapper.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToString()
    {
        // Arrange
        var timespan = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapper.MapTimeSpan(timespan);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(timespan.ToString());
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapper.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToString()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var actual = this.mapper.MapGuid(guid);

        // Assert
        actual.Should().Be(guid.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        var datetime = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapDateTime(datetime);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(datetime.ToString(InvokeToStringStrategySettings.DateTimeFormat));
 #pragma warning restore CA1305
    }
}