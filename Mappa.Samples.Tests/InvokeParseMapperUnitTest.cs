// <copyright file="InvokeParseMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using System.Globalization;

using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

#pragma warning disable CA1305 // Specify IFormatProvider
#pragma warning disable S6580 // Use a format provider when parsing date and time.

/// <summary>
/// Tests for parsing using the <c>ToParse</c> method.
/// </summary>
public sealed class InvokeParseMapperUnitTest
{
    private readonly ParseNumericMapper numericMapper = new();
    private readonly ParseUriMapper uriMapper = new();
    private readonly ParseMapperWithoutAnySettings mapperWithoutAnySettings = new();
    private readonly ParseMapperWithFormatSettingsOnMethod mapperWithFormatSettingsOnMethod = new();
    private readonly ParseMapperWithFormatAndInvariantCultureSettingsOnMethod mapperWithFormatAndInvariantCultureSettingsOnMethod = new();

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapper.MapToSignedByte"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToSignedByte()
    {
        // Arrange
        const string input = "100";

        // Act
        var actual = this.numericMapper.MapToSignedByte(input);

        // Assert
        actual.Should().Be(sbyte.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapper.MapToShort"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToShort()
    {
        // Arrange
        const string input = "100";

        // Act
        var actual = this.numericMapper.MapToShort(input);

        // Assert
        actual.Should().Be(short.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapper.MapToInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToInteger()
    {
        // Arrange
        const string input = "100";

        // Act
        var actual = this.numericMapper.MapToInteger(input);

        // Assert
        actual.Should().Be(int.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapper.MapToLong"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToLong()
    {
        // Arrange
        const string input = "100";

        // Act
        var actual = this.numericMapper.MapToLong(input);

        // Assert
        actual.Should().Be(long.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapper.MapToByte"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToByte()
    {
        // Arrange
        const string input = "100";

        // Act
        var actual = this.numericMapper.MapToByte(input);

        // Assert
        actual.Should().Be(byte.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapper.MapToUnsignedShort"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToUnsignedShort()
    {
        // Arrange
        const string input = "100";

        // Act
        var actual = this.numericMapper.MapToUnsignedShort(input);

        // Assert
        actual.Should().Be(ushort.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapper.MapToUnsignedInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToUnsignedInteger()
    {
        // Arrange
        const string input = "100";

        // Act
        var actual = this.numericMapper.MapToUnsignedInteger(input);

        // Assert
        actual.Should().Be(uint.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapper.MapToUnsignedLong"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToUnsignedLong()
    {
        // Arrange
        const string input = "100";

        // Act
        var actual = this.numericMapper.MapToUnsignedLong(input);

        // Assert
        actual.Should().Be(ulong.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapper.MapToFloat"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToFloat()
    {
        // Arrange
        const string input = "100";

        // Act
        var actual = this.numericMapper.MapToFloat(input);

        // Assert
        actual.Should().Be(float.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapper.MapToDouble"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToDouble()
    {
        // Arrange
        const string input = "100";

        // Act
        var actual = this.numericMapper.MapToDouble(input);

        // Assert
        actual.Should().Be(double.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapper.MapToDecimal"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToDecimal()
    {
        // Arrange
        const string input = "100";

        // Act
        var actual = this.numericMapper.MapToDecimal(input);

        // Assert
        actual.Should().Be(decimal.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseUriMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToUri()
    {
        // Arrange
        const string input = "http://localhost:5000";

        // Act
        var actual = this.uriMapper.Map(input);

        // Assert
        actual.Should().Be(new Uri(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithoutAnySettings.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithoutAnySettings()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithoutAnySettings.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithoutAnySettings.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithoutAnySettings()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithoutAnySettings.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithoutAnySettings.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithoutAnySettings()
    {
        // Arrange
        const string input = "2025-02-01";

        // Act
        var actual = this.mapperWithoutAnySettings.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithoutAnySettings.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithoutAnySettings()
    {
        // Arrange
        const string input = "22:20:05";

        // Act
        var actual = this.mapperWithoutAnySettings.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithoutAnySettings.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithoutAnySettings()
    {
        // Arrange
        const string input = "22:20:05";

        // Act
        var actual = this.mapperWithoutAnySettings.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithoutAnySettings.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithoutAnySettings()
    {
        // Arrange
        string input = Guid.NewGuid().ToString("N");

        // Act
        var actual = this.mapperWithoutAnySettings.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01";

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnMethod.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        const string input = "22:20:05";

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnMethod.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        const string input = "0:18:30:00.0000000";

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnMethod.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        string input = Guid.NewGuid().ToString(InvokeParseStrategySettings.GuidFormat);

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.ParseExact(input, InvokeParseStrategySettings.GuidFormat));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithFormatAndInvariantCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithFormatAndInvariantCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "01-02-2025 34:17:22";

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.ParseExact(input, InvokeParseStrategySettings.DateTimeOffsetFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithFormatAndInvariantCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "2025+02+01";

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.ParseExact(input, InvokeParseStrategySettings.DateOnlyFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithFormatAndInvariantCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "22+20+05";

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnMethod.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.ParseExact(input, InvokeParseStrategySettings.TimeOnlyFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithFormatAndInvariantCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "0:18:30:00.0000000";

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnMethod.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.ParseExact(input, InvokeParseStrategySettings.TimeSpanFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnMethod.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithFormatAndInvariantCultureSettingsOnMethod()
    {
        // Arrange
        string input = Guid.NewGuid().ToString(InvokeParseStrategySettings.GuidFormat);

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnMethod.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.ParseExact(input, InvokeParseStrategySettings.GuidFormat));
    }
}