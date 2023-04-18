// <copyright file="IntegralToEnumMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="IntegralToEnumMapper"/>.
/// </summary>
public sealed class IntegralToEnumMapperUnitTest
{
    private readonly IntegralToEnumMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="IntegralToEnumMapper.MapToEnum(int)"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(0, CountingValues.One)]
    [InlineData(1, CountingValues.Two)]
    [InlineData(2, CountingValues.Three)]
    public void CanMapIntToEnum(int value, CountingValues expected)
    {
        // Act
        var actual = this.mapper.MapToEnum(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="IntegralToEnumMapper.MapToEnum(short)"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(0, CountingValues.One)]
    [InlineData(1, CountingValues.Two)]
    [InlineData(2, CountingValues.Three)]
    public void CanMapEnumToLong(short value, CountingValues expected)
    {
        // Act
        var actual = this.mapper.MapToEnum(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="IntegralToEnumMapper.MapToBackwardsEnum(int)"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(0, CountingValuesBackwards.None)]
    [InlineData(8, CountingValuesBackwards.Eight)]
    [InlineData(9, CountingValuesBackwards.Nine)]
    [InlineData(10, CountingValuesBackwards.Ten)]
    public void CanMapEnumWithCustomValuesToInt(int value, CountingValuesBackwards expected)
    {
        // Act
        var actual = this.mapper.MapToBackwardsEnum(value);

        // Assert
        actual.Should().Be(expected);
    }
}