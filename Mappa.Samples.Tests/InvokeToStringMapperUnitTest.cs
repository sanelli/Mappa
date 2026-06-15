// <copyright file="InvokeToStringMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using System.Globalization;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="InvokeToStringMapper"/>.
/// </summary>
public sealed class InvokeToStringMapperUnitTest
{
    private readonly InvokeToStringMapper mapper = new();
    private readonly InvokeToStringMapperWithFormatSettingsOnMethod mapperWithFormatSettingsOnMethod = new();
    private readonly InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod mapperWithFormatAndInvariantCultureSettingsOnMethod = new();
    private readonly InvokeToStringMapperWithInvariantCultureSettingsOnMethod mapperWithInvariantCultureSettingsOnMethod = new();
    private readonly InvokeToStringMapperWithCurrentCultureSettingsOnMethod mapperWithCurrentCultureSettingsOnMethod = new();
    private readonly InvokeToStringMapperWithCustomCultureSettingsOnMethod mapperWithCustomCultureSettingsOnMethod = new();
    private readonly InvokeToStringMapperWithFormatSettingsOnClass mapperWithFormatSettingsOnClass = new();
    private readonly InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass mapperWithFormatAndInvariantCultureSettingsOnClass = new();
    private readonly InvokeToStringMapperWithInvariantCultureSettingsOnClass mapperWithInvariantCultureSettingsOnClass = new();
    private readonly InvokeToStringMapperWithCurrentCultureSettingsOnClass mapperWithCurrentCultureSettingsOnClass = new();
    private readonly InvokeToStringMapperWithCustomCultureSettingsOnClass mapperWithCustomCultureSettingsOnClass = new();
    private readonly InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass mapperWithMethodSettingsSupersedingTheClassSettings = new();
    private readonly InvokeToStringNumericMapperWithFormatSettingsOnMethod numericMapperWithFormatSettingsOnMethod = new();
    private readonly InvokeToStringNumericMapperWithFormatAndInvariantCultureSettingsOnMethod numericMapperWithFormatAndInvariantCultureSettingsOnMethod = new();
    private readonly InvokeToStringNumericMapperWithInvariantCultureSettingsOnMethod numericMapperWithInvariantCultureSettingsOnMethod = new();

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

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithFormatAndInvariantCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTime(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateTimeFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithFormatAndInvariantCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateTimeOffset.UtcNow;

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateTimeOffsetFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithFormatAndInvariantCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnMethod.MapDateOnly(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateOnlyFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithFormatAndInvariantCultureSettingsOnMethod()
    {
        // Arrange
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnMethod.MapTimeSpan(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.TimeSpanFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethod.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithFormatAndInvariantCultureSettingsOnMethod()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnMethod.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.GuidFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithInvariantCultureSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithInvariantCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnMethod.MapDateTime(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithInvariantCultureSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithInvariantCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateTimeOffset.UtcNow;

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnMethod.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithInvariantCultureSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithInvariantCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnMethod.MapDateOnly(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithInvariantCultureSettingsOnMethod.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithInvariantCultureSettingsOnMethod()
    {
        // Arrange
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnMethod.MapTimeSpan(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithInvariantCultureSettingsOnMethod.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithInvariantCultureSettingsOnMethod()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnMethod.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCurrentCultureSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithCurrentCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnMethod.MapDateTime(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCurrentCultureSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithCurrentCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateTimeOffset.UtcNow;

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnMethod.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCurrentCultureSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithCurrentCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnMethod.MapDateOnly(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCurrentCultureSettingsOnMethod.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithCurrentCultureSettingsOnMethod()
    {
        // Arrange
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnMethod.MapTimeSpan(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCurrentCultureSettingsOnMethod.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithCurrentCultureSettingsOnMethod()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnMethod.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCustomCultureSettingsOnMethod.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithCustomCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnMethod.MapDateTime(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.GetCultureInfo(InvokeToStringStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCustomCultureSettingsOnMethod.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithCustomCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateTimeOffset.UtcNow;

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnMethod.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.GetCultureInfo(InvokeToStringStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCustomCultureSettingsOnMethod.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithCustomCultureSettingsOnMethod()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnMethod.MapDateOnly(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.GetCultureInfo(InvokeToStringStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCustomCultureSettingsOnMethod.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithCustomCultureSettingsOnMethod()
    {
        // Arrange
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnMethod.MapTimeSpan(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCustomCultureSettingsOnMethod.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithCustomCultureSettingsOnMethod()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnMethod.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatSettingsOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithFormatSettingsOnClass()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithFormatSettingsOnClass.MapDateTime(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateTimeFormat));
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatSettingsOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithFormatSettingsOnClass()
    {
        // Arrange
        var input = DateTimeOffset.UtcNow;

        // Act
        var actual = this.mapperWithFormatSettingsOnClass.MapDateTimeOffset(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateTimeOffsetFormat));
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatSettingsOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithFormatSettingsOnClass()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapperWithFormatSettingsOnClass.MapDateOnly(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateOnlyFormat));
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatSettingsOnClass.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithFormatSettingsOnClass()
    {
        // Arrange
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapperWithFormatSettingsOnClass.MapTimeSpan(input);

        // Assert
 #pragma warning disable CA1305
        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.TimeSpanFormat));
 #pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatSettingsOnClass.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithFormatSettingsOnClass()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapperWithFormatSettingsOnClass.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.GuidFormat));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithFormatAndInvariantCultureSettingsOnClass()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTime(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateTimeFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithFormatAndInvariantCultureSettingsOnClass()
    {
        // Arrange
        var input = DateTimeOffset.UtcNow;

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnClass.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateTimeOffsetFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithFormatAndInvariantCultureSettingsOnClass()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnClass.MapDateOnly(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateOnlyFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithFormatAndInvariantCultureSettingsOnClass()
    {
        // Arrange
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnClass.MapTimeSpan(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.TimeSpanFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnClass.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithFormatAndInvariantCultureSettingsOnClass()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapperWithFormatAndInvariantCultureSettingsOnClass.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.GuidFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithInvariantCultureSettingsOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithInvariantCultureSettingsOnClass()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnClass.MapDateTime(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithInvariantCultureSettingsOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithInvariantCultureSettingsOnClass()
    {
        // Arrange
        var input = DateTimeOffset.UtcNow;

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnClass.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithInvariantCultureSettingsOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithInvariantCultureSettingsOnClass()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnClass.MapDateOnly(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithInvariantCultureSettingsOnClass.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithInvariantCultureSettingsOnClass()
    {
        // Arrange
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnClass.MapTimeSpan(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithInvariantCultureSettingsOnClass.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithInvariantCultureSettingsOnClass()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapperWithInvariantCultureSettingsOnClass.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCurrentCultureSettingsOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithCurrentCultureSettingsOnClass()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnClass.MapDateTime(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCurrentCultureSettingsOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithCurrentCultureSettingsOnClass()
    {
        // Arrange
        var input = DateTimeOffset.UtcNow;

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnClass.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCurrentCultureSettingsOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithCurrentCultureSettingsOnClass()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnClass.MapDateOnly(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.CurrentCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCurrentCultureSettingsOnClass.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithCurrentCultureSettingsOnClass()
    {
        // Arrange
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnClass.MapTimeSpan(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCurrentCultureSettingsOnClass.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithCurrentCultureSettingsOnClass()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapperWithCurrentCultureSettingsOnClass.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCustomCultureSettingsOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithCustomCultureSettingsOnClass()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnClass.MapDateTime(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.GetCultureInfo(InvokeToStringStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCustomCultureSettingsOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithCustomCultureSettingsOnClass()
    {
        // Arrange
        var input = DateTimeOffset.UtcNow;

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnClass.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.GetCultureInfo(InvokeToStringStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCustomCultureSettingsOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithCustomCultureSettingsOnClass()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnClass.MapDateOnly(input);

        // Assert
        actual.Should().Be(input.ToString(CultureInfo.GetCultureInfo(InvokeToStringStrategySettings.CultureName)));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCustomCultureSettingsOnClass.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithCustomCultureSettingsOnClass()
    {
        // Arrange
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnClass.MapTimeSpan(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithCustomCultureSettingsOnClass.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithCustomCultureSettingsOnClass()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapperWithCustomCultureSettingsOnClass.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString());
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass.MapDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeToStringWithFormatAndInvariantCultureSettingsOnMethodSupersedingSettingsOnClass()
    {
        // Arrange
        var input = DateTime.UtcNow;

        // Act
        var actual = this.mapperWithMethodSettingsSupersedingTheClassSettings.MapDateTime(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateTimeFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass.MapDateTimeOffset"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateTimeOffsetToStringWithFormatAndInvariantCultureSettingsOnMethodSupersedingSettingsOnClass()
    {
        // Arrange
        var input = DateTimeOffset.UtcNow;

        // Act
        var actual = this.mapperWithMethodSettingsSupersedingTheClassSettings.MapDateTimeOffset(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateTimeOffsetFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass.MapDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDateOnlyToStringWithFormatAndInvariantCultureSettingsOnMethodSupersedingSettingsOnClass()
    {
        // Arrange
        var input = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var actual = this.mapperWithMethodSettingsSupersedingTheClassSettings.MapDateOnly(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DateOnlyFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass.MapTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapTimeSpanToStringWithFormatAndInvariantCultureSettingsOnMethodSupersedingSettingsOnClass()
    {
        // Arrange
        var input = TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(2)).Add(TimeSpan.FromSeconds(3));

        // Act
        var actual = this.mapperWithMethodSettingsSupersedingTheClassSettings.MapTimeSpan(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.TimeSpanFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringMapperWithFormatAndInvariantCultureSettingsOnMethodSupersedingTheOnesOnClass.MapGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapGuidToStringWithFormatAndInvariantCultureSettingsOnMethodSupersedingSettingsOnClass()
    {
        // Arrange
        var input = Guid.NewGuid();

        // Act
        var actual = this.mapperWithMethodSettingsSupersedingTheClassSettings.MapGuid(input);

        // Assert
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.GuidFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringNumericMapperWithFormatSettingsOnMethod.MapInt"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIntToStringWithNumericFormatSettingsOnMethod()
    {
        const int input = 100;

        var actual = this.numericMapperWithFormatSettingsOnMethod.MapInt(input);

#pragma warning disable CA1305
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.IntFormat));
#pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringNumericMapperWithFormatSettingsOnMethod.MapDecimal"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDecimalToStringWithNumericFormatSettingsOnMethod()
    {
        const decimal input = 100.5m;

        var actual = this.numericMapperWithFormatSettingsOnMethod.MapDecimal(input);

#pragma warning disable CA1305
        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DecimalFormat));
#pragma warning restore CA1305
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringNumericMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDecimal"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapDecimalToStringWithNumericFormatAndInvariantCultureSettingsOnMethod()
    {
        const decimal input = 100.5m;

        var actual = this.numericMapperWithFormatAndInvariantCultureSettingsOnMethod.MapDecimal(input);

        actual.Should().Be(input.ToString(InvokeToStringStrategySettings.DecimalFormat, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Unit test for <see cref="InvokeToStringNumericMapperWithInvariantCultureSettingsOnMethod.MapInt"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapIntToStringWithInvariantCultureSettingsOnMethodForNumericTypes()
    {
        const int input = 100;

        var actual = this.numericMapperWithInvariantCultureSettingsOnMethod.MapInt(input);

        actual.Should().Be(input.ToString(CultureInfo.InvariantCulture));
    }
}