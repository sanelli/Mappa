// <copyright file="EnumToStringMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="EnumToStringMapper"/>.
/// </summary>
public sealed class EnumToStringMapperUnitTest
{
    private readonly EnumToStringMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="EnumToStringMapper.MapToString"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(CountingValues.One, nameof(CountingValues.One))]
    [InlineData(CountingValues.Two, nameof(CountingValues.Two))]
    [InlineData(CountingValues.Three, nameof(CountingValues.Three))]
    public void CanMapEnumToString(CountingValues value, string expected)
    {
        // Act
        var actual = this.mapper.MapToString(value);

        // Assert
        actual.Should().Be(expected);
    }
}