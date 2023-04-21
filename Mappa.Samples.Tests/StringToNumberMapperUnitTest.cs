// <copyright file="StringToNumberMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="StringToNumberMapper"/>.
/// </summary>
public sealed class StringToNumberMapperUnitTest
{
    private readonly StringToNumberMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="StringToNumberMapper.MapToByte"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData("100", 100)]
    public void CanMapStringToByte(string value, byte expected)
    {
        // Act
        var actual = this.mapper.MapToByte(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="StringToNumberMapper.MapToShort"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData("100", 100)]
    public void CanMapStringToShort(string value, byte expected)
    {
        // Act
        var actual = this.mapper.MapToShort(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="StringToNumberMapper.MapToInteger"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData("100", 100)]
    public void CanMapStringToInteger(string value, int expected)
    {
        // Act
        var actual = this.mapper.MapToInteger(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="StringToNumberMapper.MapToLong"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData("100", 100L)]
    public void CanMapStringToLong(string value, long expected)
    {
        // Act
        var actual = this.mapper.MapToLong(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="StringToNumberMapper.MapToFloat"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData("100", 100.00f)]
    public void CanMapStringToFloat(string value, float expected)
    {
        // Act
        var actual = this.mapper.MapToFloat(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="StringToNumberMapper.MapToDouble"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData("100", 100.00)]
    public void CanMapStringToDouble(string value, double expected)
    {
        // Act
        var actual = this.mapper.MapToDouble(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="StringToNumberMapper.MapToDecimal"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData("100", 100)]
    public void CanMapStringToDecimal(string value, decimal expected)
    {
        // Act
        var actual = this.mapper.MapToDecimal(value);

        // Assert
        actual.Should().Be(expected);
    }
}