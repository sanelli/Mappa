// <copyright file="EnumToEnumMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="EnumToEnumMapper"/>.
/// </summary>
public sealed class EnumToEnumMapperUnitTest
{
    private readonly EnumToEnumMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="EnumToEnumMapper.Map(Mappa.Samples.Models.CountingValues)"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(CountingValues.Two, CountingValuesFromTwo.Two)]
    [InlineData(CountingValues.Three, CountingValuesFromTwo.Three)]
    public void CanMapEnumToEnum(CountingValues value, CountingValuesFromTwo expected)
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="EnumToEnumMapper.Map(Mappa.Samples.Models.CountingValues)"/>
    /// to ensure <see cref="ArgumentException"/> is thrown when input cannot be mapped.
    /// </summary>
    /// <param name="value">The value to map.</param>
    [Theory]
    [UnitTest]
    [InlineData(CountingValues.One)]
    public void EnumToEnumMapThrowsWhenInputCannotBeMapped(CountingValues value)
    {
        // Arrange
        var act = () => this.mapper.Map(value);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}