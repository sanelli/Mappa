// <copyright file="EnumerableConcreteTypeMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the enumerable concrete type sample mappers.
/// </summary>
public sealed class EnumerableConcreteTypeMapperUnitTest
{
    private readonly EnumerableConcreteTypeListMapper listMapper = new();
    private readonly EnumerableConcreteTypeArrayMapper arrayMapper = new();
    private readonly EnumerableConcreteTypeExplicitListMapper explicitListMapper = new();
    private readonly EnumerableConcreteTypeArrayInterfaceMapper arrayInterfaceMapper = new();

    /// <summary>
    /// Test default mapping to <see cref="IEnumerable{T}"/> returns a <see cref="List{T}"/> at runtime.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapToIEnumerableReturnsListAtRuntime()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var result = this.listMapper.Map(input);

        // Assert
        var listResult = Assert.IsType<List<int>>(result);
        listResult.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Test mapping to <see cref="IEnumerable{T}"/> with <see cref="EnumerableConcreteTypeSetting.Array"/> returns <see cref="int"/>[] at runtime.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapToIEnumerableWithArraySettingReturnsArrayAtRuntime()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var result = this.arrayMapper.Map(input);

        // Assert
        var arrayResult = Assert.IsType<int[]>(result);
        arrayResult.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Test mapping to concrete <see cref="List{T}"/> remains a list when array setting is enabled.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapToListWithArraySettingReturnsListAtRuntime()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var result = this.explicitListMapper.Map(input);

        // Assert
        var listResult = Assert.IsType<List<int>>(result);
        Assert.IsNotType<int[]>(result);
        listResult.Should().BeEquivalentTo([0, 2]);
    }

    /// <summary>
    /// Test mapping to <see cref="ICollection{T}"/> with array setting returns <see cref="int"/>[] at runtime.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapToICollectionWithArraySettingReturnsArrayAtRuntime()
    {
        // Arrange
        IEnumerable<CountingValues> input = [CountingValues.One, CountingValues.Three];

        // Act
        var result = this.arrayInterfaceMapper.Map(input);

        // Assert
        var arrayResult = Assert.IsType<int[]>(result);
        arrayResult.Should().BeEquivalentTo([0, 2]);
    }
}