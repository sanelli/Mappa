// <copyright file="StringToEnumMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="StringToEnumMapper"/>.
/// </summary>
public sealed class StringToEnumMapperUnitTest
{
    private readonly StringToEnumMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="StringToEnumMapper.MapToEnum"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(nameof(CountingValues.One), CountingValues.One)]
    [InlineData(nameof(CountingValues.Two), CountingValues.Two)]
    [InlineData(nameof(CountingValues.Three), CountingValues.Three)]
    public void CanMapStringToEnum(string value, CountingValues expected)
    {
        // Act
        var actual = this.mapper.MapToEnum(value);

        // Assert
        actual.Should().Be(expected);
    }
}