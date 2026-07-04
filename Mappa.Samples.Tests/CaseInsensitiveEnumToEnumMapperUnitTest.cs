// <copyright file="CaseInsensitiveEnumToEnumMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="CaseInsensitiveEnumToEnumMapper"/>.
/// </summary>
public sealed class CaseInsensitiveEnumToEnumMapperUnitTest
{
    private readonly CaseInsensitiveEnumToEnumMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="CaseInsensitiveEnumToEnumMapper.Map"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(CaseInsensitiveSourceValues.ONe, CaseInsensitiveTargetValues.one)]
    [InlineData(CaseInsensitiveSourceValues.Two, CaseInsensitiveTargetValues.Two)]
    public void CanMapEnumToEnumCaseInsensitively(CaseInsensitiveSourceValues value, CaseInsensitiveTargetValues expected)
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="CaseInsensitiveEnumToEnumMapper.Map"/> when the input value has no matching target.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ThrowsWhenInputValueHasNoMatchingTargetEnumMemberName()
    {
        // Act
        var action = () => this.mapper.Map((CaseInsensitiveSourceValues)99);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}