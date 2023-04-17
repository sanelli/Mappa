// <copyright file="EnumToIntegralMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="EnumToIntegralMapper"/>.
/// </summary>
public sealed class EnumToIntegralMapperUnitTest
{
    private readonly EnumToIntegralMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="EnumToIntegralMapper.MapToInteger(Mappa.Samples.Models.CountingValues)"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(CountingValues.One, 0)]
    [InlineData(CountingValues.Two, 1)]
    [InlineData(CountingValues.Three, 2)]
    public void CanMapEnumToInt(CountingValues value, int expected)
    {
        // Act
        var actual = this.mapper.MapToInteger(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="EnumToIntegralMapper.MapToLong(Mappa.Samples.Models.CountingValues)"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(CountingValues.One, 0L)]
    [InlineData(CountingValues.Two, 1L)]
    [InlineData(CountingValues.Three, 2L)]
    public void CanMapEnumToLong(CountingValues value, long expected)
    {
        // Act
        var actual = this.mapper.MapToLong(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="EnumToIntegralMapper.MapToInteger(Mappa.Samples.Models.CountingValues)"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(CountingValuesBackwards.None, 0)]
    [InlineData(CountingValuesBackwards.Eight, 8)]
    [InlineData(CountingValuesBackwards.Nine, 9)]
    [InlineData(CountingValuesBackwards.Ten, 10)]
    public void CanMapEnumWithCustomValuesToInt(CountingValuesBackwards value, int expected)
    {
        // Act
        var actual = this.mapper.MapToInteger(value);

        // Assert
        actual.Should().Be(expected);
    }
}