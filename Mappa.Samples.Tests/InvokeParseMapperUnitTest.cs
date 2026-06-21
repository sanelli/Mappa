// <copyright file="InvokeParseMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using System.Globalization;

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
    private readonly ParseMapperWithInvariantCultureSettingsOnMethod mapperWithInvariantCultureSettingsOnMethod = new();
    private readonly ParseMapperWithCurrentCultureSettingsOnMethod mapperWithCurrentCultureSettingsOnMethod = new();
    private readonly ParseMapperWithCustomCultureSettingsOnMethod mapperWithCustomCultureSettingsOnMethod = new();
    private readonly ParseMapperWithFormatSettingsOnClass mapperWithFormatSettingsOnClass = new();
    private readonly ParseMapperWithFormatAndInvariantCultureSettingsOnClass mapperWithFormatAndInvariantCultureSettingsOnClass = new();
    private readonly ParseMapperWithInvariantCultureSettingsOnClass mapperWithInvariantCultureSettingsOnClass = new();
    private readonly ParseMapperWithCurrentCultureSettingsOnClass mapperWithCurrentCultureSettingsOnClass = new();
    private readonly ParseMapperWithCustomCultureSettingsOnClass mapperWithCustomCultureSettingsOnClass = new();
    private readonly ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod mapperWithSettingsOnClassSupersededBySettingsOnMethod = new();
    private readonly ParseMapperWithDateTimeStyleSettingsOnMethod mapperWithDateTimeStyleSettingsOnMethod = new();
    private readonly ParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod mapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod = new();
    private readonly ParseMapperWithDateTimeStyleSettingsOnClass mapperWithDateTimeStyleSettingsOnClass = new();
    private readonly ParseMapperWithNumberStyleSettingsOnMethod mapperWithNumberStyleSettingsOnMethod = new();
    private readonly ParseMapperWithInvariantCultureAndNumberStyleSettingsOnMethod mapperWithInvariantCultureAndNumberStyleSettingsOnMethod = new();
    private readonly ParseMapperWithNumberStyleSettingsOnClass mapperWithNumberStyleSettingsOnClass = new();
    private readonly ParseNumericMapperWithInvariantCultureSettingsOnMethod numericMapperWithInvariantCultureSettingsOnMethod = new();
    private readonly ParseNumericMapperWithCustomCultureSettingsOnMethod numericMapperWithCustomCultureSettingsOnMethod = new();

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
        const string input = "2025+02+01";

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.ParseExact(input, InvokeParseStrategySettings.DateOnlyFormat));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnMethod.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithFormatSettingsOnMethod()
    {
        // Arrange
        const string input = "22+20+05";

        // Act
        var actual = this.mapperWithFormatSettingsOnMethod.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.ParseExact(input, InvokeParseStrategySettings.TimeOnlyFormat));
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

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithInvariantCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnMethod.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithInvariantCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnMethod.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithInvariantCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01";

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnMethod.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnMethod.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithInvariantCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "22:20:05";

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnMethod.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnMethod.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithInvariantCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "0:18:30:00.0000000";

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnMethod.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnMethod.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithInvariantCultureSettingsOnMethod()
    {
        // Arrange
        string input = Guid.NewGuid().ToString();

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnMethod.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithCurrentCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnMethod.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithCurrentCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnMethod.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithCurrentCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01";

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnMethod.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnMethod.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithCurrentCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "22:20:05";

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnMethod.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnMethod.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithCurrentCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "0:18:30:00.0000000";

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnMethod.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnMethod.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithCurrentCultureSettingsOnMethod()
    {
        // Arrange
        string input = Guid.NewGuid().ToString();

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnMethod.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithCustomCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnMethod.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithCustomCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnMethod.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithCustomCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "2025-02-01";

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnMethod.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnMethod.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithCustomCultureSettingsOnMethod()
    {
        // Arrange
        const string input = "22:20:05";

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnMethod.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnMethod.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithCustomCultureSettingsOnMethod()
    {
        // Arrange
        var input = TimeSpan.FromDays(2).Add(TimeSpan.FromHours(1)).Add(TimeSpan.FromMinutes(30)).ToString("G", CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName));

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnMethod.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnMethod.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithCustomCultureSettingsOnMethod()
    {
        // Arrange
        string input = Guid.NewGuid().ToString();

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnMethod.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithFormatSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithFormatSettingsOnClass.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithFormatSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithFormatSettingsOnClass.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithFormatSettingsOnClass()
    {
        // Arrange
        const string input = "2025+02+01";

        // Act
        var actual = this.mapperWithFormatSettingsOnClass.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.ParseExact(input, InvokeParseStrategySettings.DateOnlyFormat));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnClass.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithFormatSettingsOnClass()
    {
        // Arrange
        const string input = "22+20+05";

        // Act
        var actual = this.mapperWithFormatSettingsOnClass.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.ParseExact(input, InvokeParseStrategySettings.TimeOnlyFormat));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnClass.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithFormatSettingsOnClass()
    {
        // Arrange
        const string input = "0:18:30:00.0000000";

        // Act
        var actual = this.mapperWithFormatSettingsOnClass.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatSettingsOnClass.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithFormatSettingsOnClass()
    {
        // Arrange
        string input = Guid.NewGuid().ToString(InvokeParseStrategySettings.GuidFormat);

        // Act
        var actual = this.mapperWithFormatSettingsOnClass.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.ParseExact(input, InvokeParseStrategySettings.GuidFormat));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithFormatAndInvariantCultureSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithFormatAndInvariantCultureSettingsOnClass()
    {
        // Arrange
        const string input = "01-02-2025 34:17:22";

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.ParseExact(input, InvokeParseStrategySettings.DateTimeOffsetFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithFormatAndInvariantCultureSettingsOnClass()
    {
        // Arrange
        const string input = "2025+02+01";

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnClass.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.ParseExact(input, InvokeParseStrategySettings.DateOnlyFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithFormatAndInvariantCultureSettingsOnClass()
    {
        // Arrange
        const string input = "22+20+05";

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnClass.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.ParseExact(input, InvokeParseStrategySettings.TimeOnlyFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithFormatAndInvariantCultureSettingsOnClass()
    {
        // Arrange
        const string input = "0:18:30:00.0000000";

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnClass.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.ParseExact(input, InvokeParseStrategySettings.TimeSpanFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatAndInvariantCultureSettingsOnClass.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithFormatAndInvariantCultureSettingsOnClass()
    {
        // Arrange
        string input = Guid.NewGuid().ToString(InvokeParseStrategySettings.GuidFormat);

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnClass.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.ParseExact(input, InvokeParseStrategySettings.GuidFormat));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithInvariantCultureSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnClass.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithInvariantCultureSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnClass.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithInvariantCultureSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01";

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnClass.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnClass.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithInvariantCultureSettingsOnClass()
    {
        // Arrange
        const string input = "22:20:05";

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnClass.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnClass.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithInvariantCultureSettingsOnClass()
    {
        // Arrange
        const string input = "0:18:30:00.0000000";

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnClass.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureSettingsOnClass.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithInvariantCultureSettingsOnClass()
    {
        // Arrange
        string input = Guid.NewGuid().ToString();

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnClass.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithCurrentCultureSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnClass.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithCurrentCultureSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnClass.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithCurrentCultureSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01";

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnClass.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnClass.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithCurrentCultureSettingsOnClass()
    {
        // Arrange
        const string input = "22:20:05";

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnClass.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnClass.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithCurrentCultureSettingsOnClass()
    {
        // Arrange
        const string input = "0:18:30:00.0000000";

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnClass.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCurrentCultureSettingsOnClass.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithCurrentCultureSettingsOnClass()
    {
        // Arrange
        string input = Guid.NewGuid().ToString();

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnClass.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.Parse(input, CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithCustomCultureSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnClass.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithCustomCultureSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnClass.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithCustomCultureSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01";

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnClass.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnClass.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithCustomCultureSettingsOnClass()
    {
        // Arrange
        const string input = "22:20:05";

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnClass.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnClass.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithCustomCultureSettingsOnClass()
    {
        // Arrange
        var input = TimeSpan.FromDays(2).Add(TimeSpan.FromHours(1)).Add(TimeSpan.FromMinutes(30)).ToString("G", CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName));

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnClass.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithCustomCultureSettingsOnClass.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithCustomCultureSettingsOnClass()
    {
        // Arrange
        string input = Guid.NewGuid().ToString();

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnClass.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithFormatAndInvariantCultureSettingsOnMethodSupersedsSettingsOnClass()
    {
        // Arrange
        const string input = "2025-02-01 22:17:34";

        // Act
        var actual = this.mapperWithSettingsOnClassSupersededBySettingsOnMethod.MapDateTime(input);

        // Assert
        actual.Should().Be(DateTime.Parse(input));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithFormatAndInvariantCultureSettingsOnMethodSupersedsSettingsOnClass()
    {
        // Arrange
        const string input = "01-02-2025 34:17:22";

        // Act
        var actual = this.mapperWithSettingsOnClassSupersededBySettingsOnMethod.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(DateTimeOffset.ParseExact(input, InvokeParseStrategySettings.DateTimeOffsetFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithFormatAndInvariantCultureSettingsOnMethodSupersedsSettingsOnClass()
    {
        // Arrange
        const string input = "2025+02+01";

        // Act
        var actual = this.mapperWithSettingsOnClassSupersededBySettingsOnMethod.MapDateOnly(input);

        // Assert
        actual.Should().Be(DateOnly.ParseExact(input, InvokeParseStrategySettings.DateOnlyFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyToStringWithFormatAndInvariantCultureSettingsOnMethodSupersedsSettingsOnClass()
    {
        // Arrange
        const string input = "22+20+05";

        // Act
        var actual = this.mapperWithSettingsOnClassSupersededBySettingsOnMethod.MapTimeOnly(input);

        // Assert
        actual.Should().Be(TimeOnly.ParseExact(input, InvokeParseStrategySettings.TimeOnlyFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithFormatAndInvariantCultureSettingsOnMethodSupersedsSettingsOnClass()
    {
        // Arrange
        const string input = "0:18:30:00.0000000";

        // Act
        var actual = this.mapperWithSettingsOnClassSupersededBySettingsOnMethod.MapTimeSpan(input);

        // Assert
        actual.Should().Be(TimeSpan.ParseExact(input, InvokeParseStrategySettings.TimeSpanFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithSettingsOnClassSupersededBySettingsOnMethod.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithFormatAndInvariantCultureSettingsOnMethodSupersedsSettingsOnClass()
    {
        // Arrange
        string input = Guid.NewGuid().ToString(InvokeParseStrategySettings.GuidFormat);

        // Act
        var actual = this.mapperWithSettingsOnClassSupersededBySettingsOnMethod.MapGuid(input);

        // Assert
        actual.Should().Be(Guid.ParseExact(input, InvokeParseStrategySettings.GuidFormat));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithDateTimeStyleSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeWithDateTimeStyleSettingsOnMethod()
    {
        const string input = InvokeParseStrategySettings.DateTimeInputWithWhiteSpaces;

        var actual = this.mapperWithDateTimeStyleSettingsOnMethod.MapDateTime(input);

        actual.Should().Be(DateTime.Parse(input, null, InvokeParseStrategySettings.DateTimeStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithDateTimeStyleSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetWithDateTimeStyleSettingsOnMethod()
    {
        const string input = InvokeParseStrategySettings.DateTimeOffsetInputWithWhiteSpaces;

        var actual = this.mapperWithDateTimeStyleSettingsOnMethod.MapDateTimeOffset(input);

        actual.Should().Be(DateTimeOffset.Parse(input, null, InvokeParseStrategySettings.DateTimeOffsetStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithDateTimeStyleSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyWithDateTimeStyleSettingsOnMethod()
    {
        const string input = InvokeParseStrategySettings.DateOnlyInputWithWhiteSpaces;

        var actual = this.mapperWithDateTimeStyleSettingsOnMethod.MapDateOnly(input);

        actual.Should().Be(DateOnly.Parse(input, null, InvokeParseStrategySettings.DateOnlyStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithDateTimeStyleSettingsOnMethod.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyWithDateTimeStyleSettingsOnMethod()
    {
        const string input = InvokeParseStrategySettings.TimeOnlyInputWithWhiteSpaces;

        var actual = this.mapperWithDateTimeStyleSettingsOnMethod.MapTimeOnly(input);

        actual.Should().Be(TimeOnly.Parse(input, null, InvokeParseStrategySettings.TimeOnlyStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod()
    {
        const string input = InvokeParseStrategySettings.DateTimeInputWithWhiteSpaces;

        var actual = this.mapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapDateTime(input);

        actual.Should().Be(DateTime.ParseExact(
            input,
            InvokeParseStrategySettings.DateTimeFormat,
            CultureInfo.InvariantCulture,
            InvokeParseStrategySettings.DateTimeStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod()
    {
        const string input = " 01-02-2025 34:17:22 ";

        var actual = this.mapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapDateTimeOffset(input);

        actual.Should().Be(DateTimeOffset.ParseExact(
            input,
            InvokeParseStrategySettings.DateTimeOffsetFormat,
            CultureInfo.InvariantCulture,
            InvokeParseStrategySettings.DateTimeOffsetStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod()
    {
        const string input = " 2025+02+01 ";

        var actual = this.mapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapDateOnly(input);

        actual.Should().Be(DateOnly.ParseExact(
            input,
            InvokeParseStrategySettings.DateOnlyFormat,
            CultureInfo.InvariantCulture,
            InvokeParseStrategySettings.DateOnlyStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod()
    {
        const string input = " 22+20+05 ";

        var actual = this.mapperWithFormatInvariantCultureAndDateTimeStyleSettingsOnMethod.MapTimeOnly(input);

        actual.Should().Be(TimeOnly.ParseExact(
            input,
            InvokeParseStrategySettings.TimeOnlyFormat,
            CultureInfo.InvariantCulture,
            InvokeParseStrategySettings.TimeOnlyStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithDateTimeStyleSettingsOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeWithDateTimeStyleSettingsOnClass()
    {
        const string input = InvokeParseStrategySettings.DateTimeInputWithWhiteSpaces;

        var actual = this.mapperWithDateTimeStyleSettingsOnClass.MapDateTime(input);

        actual.Should().Be(DateTime.Parse(input, null, InvokeParseStrategySettings.DateTimeStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithDateTimeStyleSettingsOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetWithDateTimeStyleSettingsOnClass()
    {
        const string input = InvokeParseStrategySettings.DateTimeOffsetInputWithWhiteSpaces;

        var actual = this.mapperWithDateTimeStyleSettingsOnClass.MapDateTimeOffset(input);

        actual.Should().Be(DateTimeOffset.Parse(input, null, InvokeParseStrategySettings.DateTimeOffsetStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithDateTimeStyleSettingsOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyWithDateTimeStyleSettingsOnClass()
    {
        const string input = InvokeParseStrategySettings.DateOnlyInputWithWhiteSpaces;

        var actual = this.mapperWithDateTimeStyleSettingsOnClass.MapDateOnly(input);

        actual.Should().Be(DateOnly.Parse(input, null, InvokeParseStrategySettings.DateOnlyStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithDateTimeStyleSettingsOnClass.MapTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeOnlyWithDateTimeStyleSettingsOnClass()
    {
        const string input = InvokeParseStrategySettings.TimeOnlyInputWithWhiteSpaces;

        var actual = this.mapperWithDateTimeStyleSettingsOnClass.MapTimeOnly(input);

        actual.Should().Be(TimeOnly.Parse(input, null, InvokeParseStrategySettings.TimeOnlyStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithNumberStyleSettingsOnMethod.MapToInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToIntegerWithNumberStyleSettingsOnMethod()
    {
        const string input = InvokeParseStrategySettings.IntInputWithThousandsAndParentheses;

        var actual = this.mapperWithNumberStyleSettingsOnMethod.MapToInteger(input);

        actual.Should().Be(int.Parse(input, InvokeParseStrategySettings.IntStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithNumberStyleSettingsOnMethod.MapToDecimal"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToDecimalWithNumberStyleSettingsOnMethod()
    {
        const string input = InvokeParseStrategySettings.DecimalInputWithThousandsAndParentheses;

        var actual = this.mapperWithNumberStyleSettingsOnMethod.MapToDecimal(input);

        actual.Should().Be(decimal.Parse(input, InvokeParseStrategySettings.DecimalStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureAndNumberStyleSettingsOnMethod.MapToInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToIntegerWithInvariantCultureAndNumberStyleSettingsOnMethod()
    {
        const string input = InvokeParseStrategySettings.IntInputWithThousandsAndParentheses;

        var actual = this.mapperWithInvariantCultureAndNumberStyleSettingsOnMethod.MapToInteger(input);

        actual.Should().Be(int.Parse(input, InvokeParseStrategySettings.IntStyle, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithInvariantCultureAndNumberStyleSettingsOnMethod.MapToDecimal"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToDecimalWithInvariantCultureAndNumberStyleSettingsOnMethod()
    {
        const string input = InvokeParseStrategySettings.DecimalInputWithThousandsAndParentheses;

        var actual = this.mapperWithInvariantCultureAndNumberStyleSettingsOnMethod.MapToDecimal(input);

        actual.Should().Be(decimal.Parse(input, InvokeParseStrategySettings.DecimalStyle, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithNumberStyleSettingsOnClass.MapToInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToIntegerWithNumberStyleSettingsOnClass()
    {
        const string input = InvokeParseStrategySettings.IntInputWithThousandsAndParentheses;

        var actual = this.mapperWithNumberStyleSettingsOnClass.MapToInteger(input);

        actual.Should().Be(int.Parse(input, InvokeParseStrategySettings.IntStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseMapperWithNumberStyleSettingsOnClass.MapToDecimal"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToDecimalWithNumberStyleSettingsOnClass()
    {
        const string input = InvokeParseStrategySettings.DecimalInputWithThousandsAndParentheses;

        var actual = this.mapperWithNumberStyleSettingsOnClass.MapToDecimal(input);

        actual.Should().Be(decimal.Parse(input, InvokeParseStrategySettings.DecimalStyle));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapperWithInvariantCultureSettingsOnMethod.MapToInteger"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToIntegerWithInvariantCultureSettingsOnMethod()
    {
        const string input = "100";

        var actual = this.numericMapperWithInvariantCultureSettingsOnMethod.MapToInteger(input);

        actual.Should().Be(int.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapperWithInvariantCultureSettingsOnMethod.MapToDecimal"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToDecimalWithInvariantCultureSettingsOnMethod()
    {
        const string input = "100.50";

        var actual = this.numericMapperWithInvariantCultureSettingsOnMethod.MapToDecimal(input);

        actual.Should().Be(decimal.Parse(input, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="ParseNumericMapperWithCustomCultureSettingsOnMethod.MapToDecimal"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToDecimalWithCustomCultureSettingsOnMethod()
    {
        const string input = "100,50";

        var actual = this.numericMapperWithCustomCultureSettingsOnMethod.MapToDecimal(input);

        actual.Should().Be(decimal.Parse(input, CultureInfo.GetCultureInfo(InvokeParseStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="CustomClassWithStaticParse"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapCustomClassWithStaticParse()
    {
        // Arrange
        const string input = "this-is-a-string";

        // Act
        var actual = this.mapperWithoutAnySettings.MapCustomClassWithStaticParse(input);

        // Assert
        actual.TheString.Should().Be(input);
    }
}