// <copyright file="NullableToNullableMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="NullableToNullableMapper"/>.
/// </summary>
public sealed class NullableToNullableMapperUnitTest
{
    private readonly NullableToNullableMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="NullableToNullableMapper.Map(Mappa.Samples.Models.CountingValues?)"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(CountingValues.One, 0)]
    [InlineData(CountingValues.Two, 1)]
    [InlineData(CountingValues.Three, 2)]
    [InlineData(null, null)]
    public void CanMapNullableToNullable(CountingValues? value, int? expected)
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="NullableToNullableMapper.MapToNonNullable"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(CountingValues.One, 0)]
    [InlineData(CountingValues.Two, 1)]
    [InlineData(CountingValues.Three, 2)]
    public void CanMapNullableToNonNullable(CountingValues? value, int? expected)
    {
        // Act
        var actual = this.mapper.MapToNonNullable(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="NullableToNullableMapper.MapToNonNullable"/>
    /// to throw when input is null.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapNullableToNonNullableThrowsWhenInputIsNull()
    {
        // Act
        var action = () => this.mapper.MapToNonNullable(null);

        // Assert
        action.Should().Throw<NullReferenceException>();
    }

    /// <summary>
    /// Unit test for <see cref="NullableToNullableMapper.Map(Mappa.Samples.Models.CountingValues)"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(CountingValues.One, 0)]
    [InlineData(CountingValues.Two, 1)]
    [InlineData(CountingValues.Three, 2)]
    public void CanMapNonNullableToNullable(CountingValues value, int? expected)
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }
}