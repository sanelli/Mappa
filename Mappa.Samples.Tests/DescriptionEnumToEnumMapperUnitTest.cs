// <copyright file="DescriptionEnumToEnumMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="DescriptionEnumToEnumMapper"/>.
/// </summary>
public sealed class DescriptionEnumToEnumMapperUnitTest
{
    private readonly DescriptionEnumToEnumMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="DescriptionEnumToEnumMapper.Map"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(DescribedSourceValues.Alpha, DescribedTargetValues.First)]
    [InlineData(DescribedSourceValues.Beta, DescribedTargetValues.Second)]
    public void CanMapEnumToEnumByDescription(DescribedSourceValues value, DescribedTargetValues expected)
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().Be(expected);
    }

    /// <summary>
    /// Unit test for <see cref="DescriptionEnumToEnumMapper.Map"/> when the input value has no matching target.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ThrowsWhenInputValueHasNoMatchingTargetEnumValue()
    {
        // Act
        var action = () => this.mapper.Map((DescribedSourceValues)99);

        // Assert
        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}