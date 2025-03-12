// <copyright file="ReadOnlyTargetCollectionMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="ReadOnlyTargetCollectionMapper"/>.
/// </summary>
public sealed class ReadOnlyTargetCollectionMapperUnitTests
{
    private readonly ReadOnlyTargetCollectionMapper mapper = new();

    /// <summary>
    /// Test <see cref="ReadOnlyTargetCollectionMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMap()
    {
        // Arrange
        var source = new SourceClassWithCollections(
            [1, 2, 3],
            [4, 5, 6],
            [7, 8, 9],
            [10, 11, 12],
            [13, 14, 15],
            [16, 17, 18],
            new Dictionary<int, string>
            {
                [19] = "119",
                [20] = "120",
                [21] = "121",
            },
            new Dictionary<int, string>
            {
                [22] = "122",
                [23] = "123",
                [24] = "124",
            });

        // Act
        var actual = this.mapper.Map(source);

        // Assert
        actual.PropertyA.Should().BeEquivalentTo("1", "2", "3");
        actual.PropertyB.Should().BeEquivalentTo("4", "5", "6");
        actual.PropertyC.Should().BeEquivalentTo("7", "8", "9");
        actual.PropertyD.Should().BeEquivalentTo("10", "11", "12");
        actual.PropertyE.Should().BeEquivalentTo("13", "14", "15");
        actual.PropertyF.Should().BeEquivalentTo("16", "17", "18");
        actual.PropertyG.Should().HaveCount(3);
        actual.PropertyG["19"].Should().Be(119);
        actual.PropertyG["20"].Should().Be(120);
        actual.PropertyG["21"].Should().Be(121);
        actual.PropertyH.Should().HaveCount(3);
        actual.PropertyH["22"].Should().Be(122);
        actual.PropertyH["23"].Should().Be(123);
        actual.PropertyH["24"].Should().Be(124);
    }
}