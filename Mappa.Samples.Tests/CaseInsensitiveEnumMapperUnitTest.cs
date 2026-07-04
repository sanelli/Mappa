// <copyright file="CaseInsensitiveEnumMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="CaseInsensitiveEnumMapper"/>.
/// </summary>
public sealed class CaseInsensitiveEnumMapperUnitTest
{
    private readonly CaseInsensitiveEnumMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="CaseInsensitiveEnumMapper.MapToEnum"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData("one", CountingValues.One)]
    [InlineData("ONE", CountingValues.One)]
    [InlineData("One", CountingValues.One)]
    [InlineData("two", CountingValues.Two)]
    [InlineData("THREE", CountingValues.Three)]
    public void CanMapStringToEnumCaseInsensitively(string value, CountingValues expected)
    {
        // Act
        var actual = this.mapper.MapToEnum(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="CaseInsensitiveEnumMapper.MapToEnum"/> when the input is invalid.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ThrowsWhenInputDoesNotMatchAnyEnumMemberName()
    {
        // Act
        var action = () => this.mapper.MapToEnum("invalid");

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}