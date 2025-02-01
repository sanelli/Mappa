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
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapper.MapDateTime(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString());
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
        var input = DateTimeOffset.Now;

        // Act
        var actual = this.mapper.MapDateTimeOffset(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString());
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
        var input = DateOnly.FromDateTime(DateTime.Now);

        // Act
        var actual = this.mapper.MapDateOnly(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString());
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
        var input = TimeOnly.FromDateTime(DateTime.Now);

        // Act
        var actual = this.mapper.MapTimeOnly(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString());
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
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapper.MapTimeSpan(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString());
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
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapper.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapDateTime(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateTimeFormat));
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        var input = DateTimeOffset.UtcNow;

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapDateTimeOffset(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateTimeOffsetFormat));
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapDateOnly(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateOnlyFormat));
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatSettingsOnMethod.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapTimeSpan(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.TimeSpanFormat));
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatSettingsOnMethod.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.GuidFormat));
    }
}