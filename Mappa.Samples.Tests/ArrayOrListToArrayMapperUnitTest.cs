// <copyright file="ArrayOrListToArrayMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the <see cref="ArrayOrListToArrayMapper"/>.
/// </summary>
public sealed class ArrayOrListToArrayMapperUnitTest
{
    private readonly ArrayOrListToArrayMapper mapper = new();

    /// <summary>
    /// Test data for <see cref="CanMapArrayToArrayWithNullableValues"/>.
    /// </summary>
    /// <returns>The test data for <see cref="CanMapArrayToArrayWithNullableValues"/>.</returns>
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
    /// Test data for <see cref="CanMapIListToArrayTestData"/>.
    /// </summary>
    /// <returns>The test data for <see cref="CanMapIListToArrayTestData"/>.</returns>
    public static IEnumerable<object[]> CanMapIListToArrayTestData()
    {
        yield return new object[]
        {
            new[] { CountingValues.One },
            new[] { 0 },
        };

        yield return new object[]
        {
            new[] { CountingValues.One, CountingValues.Three },
            new[] { 0, 2 },
        };

        yield return new object[]
        {
            Array.Empty<CountingValues>(),
            Array.Empty<int>(),
        };
    }

    /// <summary>
    /// Test data for <see cref="CanMapListToArray"/>.
    /// </summary>
    /// <returns>The test data for <see cref="CanMapListToArray"/>.</returns>
    public static IEnumerable<object[]> CanMapListToArrayTestData()
    {
        yield return new object[]
        {
            new List<CountingValues> { CountingValues.One },
            new[] { 0 },
        };

        yield return new object[]
        {
            new List<CountingValues> { CountingValues.One, CountingValues.Three },
            new[] { 0, 2 },
        };

        yield return new object[]
        {
            new List<CountingValues>(),
            Array.Empty<int>(),
        };
    }

    /// <summary>
    /// Returns test data for <see cref="CanMapArrayToArray"/>.
    /// </summary>
    /// <returns>The test data for <see cref="CanMapArrayToArray"/>.</returns>
    public static IEnumerable<object[]> CanMapArrayToArrayTestData()
    {
        yield return new object[] { new[] { CountingValues.One }, new[] { 0 } };
        yield return new object[] { new[] { CountingValues.One, CountingValues.Two }, new[] { 0, 1 } };
        yield return new object[] { Array.Empty<CountingValues>(), Array.Empty<int>() };
    }

    /// <summary>
    /// Unit test for <see cref="ArrayOrListToArrayMapper.Map(Mappa.Samples.Models.CountingValues[])"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [MemberData(nameof(CanMapArrayToArrayTestData))]
    public void CanMapArrayToArray(CountingValues[] value, int[] expected)
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Unit test for <see cref="ArrayOrListToArrayMapper.Map(Mappa.Samples.Models.CountingValues?[])"/>.
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

    /// <summary>
    /// Unit test for <see cref="ArrayOrListToArrayMapper.Map(IList{CountingValues})"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [MemberData(nameof(CanMapIListToArrayTestData))]
    public void CanMapIListToArray(IList<CountingValues> value, int[] expected)
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Unit test for <see cref="ArrayOrListToArrayMapper.Map(List{CountingValues})"/>.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="expected">The expected value of the mapping.</param>
    [Theory]
    [UnitTest]
    [MemberData(nameof(CanMapListToArrayTestData))]
#pragma warning disable CA1002 //Change 'List ' in 'ArrayOrListToArrayMapperUnitTest.CanMapListToArray(List , int?[])' to use 'Collection ', 'ReadOnlyCollection ' or 'KeyedCollection '
    public void CanMapListToArray(List<CountingValues> value, int[] expected)
#pragma warning restore CA1002 //Change 'List ' in 'ArrayOrListToArrayMapperUnitTest.CanMapListToArray(List , int?[])' to use 'Collection ', 'ReadOnlyCollection ' or 'KeyedCollection '
    {
        // Act
        var actual = this.mapper.Map(value);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}