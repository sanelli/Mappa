// <copyright file="NumericValueEnumToEnumMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="NumericValueEnumToEnumMapper"/>.
/// </summary>
public sealed class NumericValueEnumToEnumMapperUnitTest
{
    private readonly NumericValueEnumToEnumMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="NumericValueEnumToEnumMapper.Map(Mappa.Samples.Models.CountingValues)"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(CountingValues.One, CountingValuesFromTwo.Two)]
    [InlineData(CountingValues.Two, CountingValuesFromTwo.Three)]
    [InlineData(CountingValues.Three, CountingValuesFromTwo.Four)]
    public void CanMapEnumToEnumByNumericValue(CountingValues value, CountingValuesFromTwo expected)
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="NumericValueEnumToEnumMapper.Map(Mappa.Samples.Models.CountingValues)"/>
    /// when the input value has no matching target enum value.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ThrowsWhenInputValueHasNoMatchingTargetEnumValue()
    {
        // Act
        var action = () => this.mapper.Map((CountingValues)99);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}