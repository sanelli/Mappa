// <copyright file="StringToSystemEntitiesMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using System.Globalization;

using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="StringToSystemEntitiesMapper"/>.
/// </summary>
public sealed class StringToSystemEntitiesMapperUnitTest
{
    private readonly StringToSystemEntitiesMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="StringToSystemEntitiesMapper.MapToDateTime"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToDateTime()
    {
        // Arrange
        var expected = DateTime.UtcNow;
        var input = expected.ToString("yyyy-MM-dd HH:mm:ss.fffffff", DateTimeFormatInfo.CurrentInfo);

        // Act
        var actual = this.mapper.MapToDateTime(input);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="StringToSystemEntitiesMapper.MapToTimeSpan"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToTimeSpan()
    {
        // Arrange
        var expected = DateTime.UtcNow - DateTime.UtcNow.AddHours(7).AddMinutes(13).AddSeconds(17);
        var input = expected.ToString();

        // Act
        var actual = this.mapper.MapToTimeSpan(input);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="StringToSystemEntitiesMapper.MapToTimeOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToTimeOnly()
    {
        // Arrange
        var expected = TimeOnly.FromDateTime(DateTime.UtcNow);
        var input = expected.ToString("HH:mm:ss.fffffff", DateTimeFormatInfo.CurrentInfo);

        // Act
        var actual = this.mapper.MapToTimeOnly(input);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="StringToSystemEntitiesMapper.MapToDateOnly"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToDateOnly()
    {
        // Arrange
        var expected = DateOnly.FromDateTime(DateTime.UtcNow);
        var input = expected.ToString("yyyy-MM-dd", DateTimeFormatInfo.CurrentInfo);

        // Act
        var actual = this.mapper.MapToDateOnly(input);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="StringToSystemEntitiesMapper.MapToGuid"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToGuid()
    {
        // Arrange
        var expected = Guid.NewGuid();
        var input = expected.ToString("N", DateTimeFormatInfo.CurrentInfo);

        // Act
        var actual = this.mapper.MapToGuid(input);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="StringToSystemEntitiesMapper.MapToUri"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapStringToUri()
    {
        // Arrange
        var expected = new Uri("https://github.com/sanelli/Mappa", UriKind.Absolute);
        var input = expected.ToString();

        // Act
        var actual = this.mapper.MapToUri(input);

        // Assert
        actual.Should().Be(expected);
    }
}