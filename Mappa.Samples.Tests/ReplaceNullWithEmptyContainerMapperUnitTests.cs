// <copyright file="ReplaceNullWithEmptyContainerMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="ReplaceNullWithEmptyContainerMapper"/>.
/// </summary>
public sealed class ReplaceNullWithEmptyContainerMapperUnitTests
{
    private readonly ReplaceNullWithEmptyContainerMapper mapper = new();

    /// <summary>
    /// Test a null source list is mapped to an empty target list.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapReplacesNullListWithEmptyList()
    {
        var target = this.mapper.Map(null);

        target.Should().NotBeNull();
        target.Should().BeEmpty();
    }

    /// <summary>
    /// Test a non-null source list is mapped element-wise.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapMapsNonNullList()
    {
        var target = this.mapper.Map([1, 2]);

        target.Should().Equal("1", "2");
    }
}