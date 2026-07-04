// <copyright file="DescriptionEnumToStringMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="DescriptionEnumToStringMapper"/>.
/// </summary>
public sealed class DescriptionEnumToStringMapperUnitTest
{
    private readonly DescriptionEnumToStringMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="DescriptionEnumToStringMapper.MapToString"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected Description string.</param>
    [Theory]
    [UnitTest]
    [InlineData(DescribedCountingValues.One, "First")]
    [InlineData(DescribedCountingValues.Two, "Second")]
    [InlineData(DescribedCountingValues.Three, "Third")]
    public void CanMapEnumToStringByDescription(DescribedCountingValues value, string expected)
    {
        // Act
        var actual = this.mapper.MapToString(value);

        // Assert
        actual.Should().Be(expected);
    }
}