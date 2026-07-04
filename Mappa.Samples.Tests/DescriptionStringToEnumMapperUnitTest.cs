// <copyright file="DescriptionStringToEnumMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="DescriptionStringToEnumMapper"/>.
/// </summary>
public sealed class DescriptionStringToEnumMapperUnitTest
{
    private readonly DescriptionStringToEnumMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="DescriptionStringToEnumMapper.MapToEnum"/>.
    /// </summary>
    /// <param name="value">The Description string to map.</param>
    /// <param name="expected">The expected enum value.</param>
    [Theory]
    [UnitTest]
    [InlineData("First", DescribedCountingValues.One)]
    [InlineData("Second", DescribedCountingValues.Two)]
    [InlineData("Third", DescribedCountingValues.Three)]
    public void CanMapStringToEnumByDescription(string value, DescribedCountingValues expected)
    {
        // Act
        var actual = this.mapper.MapToEnum(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="DescriptionStringToEnumMapper.MapToEnum"/> when the input is invalid.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ThrowsWhenInputDoesNotMatchAnyDescription()
    {
        // Act
        var action = () => this.mapper.MapToEnum("invalid");

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}