// <copyright file="InvokeParseMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

#pragma warning disable CA1305 // Specify IFormatProvider

/// <summary>
/// Tests for parsing using the <c>ToParse</c> method.
/// </summary>
public sealed class InvokeParseMapperUnitTest
{
    private readonly ParseNumericMapper parseNumericMapper = new();
    private readonly ParseUriMapper parseUriMapper = new();

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
        var actual = this.parseNumericMapper.MapToSignedByte(input);

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
        var actual = this.parseNumericMapper.MapToShort(input);

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
        var actual = this.parseNumericMapper.MapToInteger(input);

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
        var actual = this.parseNumericMapper.MapToLong(input);

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
        var actual = this.parseNumericMapper.MapToByte(input);

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
        var actual = this.parseNumericMapper.MapToUnsignedShort(input);

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
        var actual = this.parseNumericMapper.MapToUnsignedInteger(input);

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
        var actual = this.parseNumericMapper.MapToUnsignedLong(input);

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
        var actual = this.parseNumericMapper.MapToFloat(input);

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
        var actual = this.parseNumericMapper.MapToDouble(input);

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
        var actual = this.parseNumericMapper.MapToDecimal(input);

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
        var actual = this.parseUriMapper.Map(input);

        // Assert
        actual.Should().Be(new Uri(input));
    }
}