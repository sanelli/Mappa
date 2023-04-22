// <copyright file="ArrayToArrayMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="ArrayToArrayMapper"/>.
/// </summary>
public sealed class ArrayToArrayMapperUnitTest
{
    private readonly ArrayToArrayMapper mapper = new();

    /// <summary>
    /// Test data for <see cref="CanMapArrayToArray"/>.
    /// </summary>
    /// <returns>The test data for <see cref="CanMapArrayToArray"/>.</returns>
    public static IEnumerable<object[]> CanMapArrayToArrayWithNullableValuesTestData()
    {
        yield return new object[]
        {
            new CountingValues?[] { CountingValues.One },
            new int?[] { 0 },
        };

        yield return new object[]
        {
            new CountingValues?[] { CountingValues.One, null, CountingValues.Three },
            new int?[] { 0, null, 2 },
        };

        yield return new object[]
        {
            Array.Empty<CountingValues?>(),
            Array.Empty<int?>(),
        };
    }

    /// <summary>
    /// Unit test for <see cref="ArrayToArrayMapper.Map(Mappa.Samples.Models.CountingValues[])"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [InlineData(new[] { CountingValues.One }, new[] { 0 })]
    [InlineData(new[] { CountingValues.One, CountingValues.Two }, new[] { 0, 1 })]
    [InlineData(new CountingValues[0], new int[0])]
    public void CanMapArrayToArray(CountingValues[] value, int[] expected)
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Unit test for <see cref="ArrayToArrayMapper.Map(Mappa.Samples.Models.CountingValues?[])"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [MemberData(nameof(CanMapArrayToArrayWithNullableValuesTestData))]
    public void CanMapArrayToArrayWithNullableValues(CountingValues?[] value, int?[] expected)
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}