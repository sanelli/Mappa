// <copyright file="PreferredInterfaceMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Runtime tests for <see cref="PreferredInterfaceMapper"/>.
/// </summary>
public sealed class PreferredInterfaceMapperUnitTests
{
    private readonly PreferredInterfaceMapper mapper = new();

    /// <summary>
    /// Implicit preference uses <see cref="IEnumerable{T}"/> of <see cref="int"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapImplicitIntUsesImplicitEnumerableOfInt()
    {
        var input = new DualEnumerable();

        var actual = this.mapper.MapImplicitInt(input);

        actual.Should().Equal(1, 2);
    }

    /// <summary>
    /// Source attribute forces the explicit <see cref="IEnumerable{T}"/> of <see cref="string"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapExplicitStringUsesExplicitEnumerableOfString()
    {
        var input = new DualEnumerable();

        var actual = this.mapper.MapExplicitString(input);

        actual.Should().Equal("a", "b");
    }

    /// <summary>
    /// Nested implicit mapping uses integer elements.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapNestedImplicitUsesIntegerItems()
    {
        var input = new PreferredInterfaceSourceDto { Items = new DualEnumerable() };

        var actual = this.mapper.MapNestedImplicit(input);

        actual.Items.Should().Equal(1, 2);
    }

    /// <summary>
    /// Nested source attribute forces string elements.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapNestedExplicitStringUsesStringItems()
    {
        var input = new PreferredInterfaceSourceDto { Items = new DualEnumerable() };

        var actual = this.mapper.MapNestedExplicitString(input);

        actual.Items.Should().Equal("a", "b");
    }

    /// <summary>
    /// Target attribute populates the string side of <see cref="DualCollection"/> (via <c>ToString</c>).
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapToDualCollectionStringsUsesStringCollectionInterface()
    {
        var input = new List<int> { 7, 8 };

        var actual = this.mapper.MapToDualCollectionStrings(input);

        actual.StringValues.Should().Equal("7", "8");
        actual.IntValues.Should().BeEmpty();
    }

    /// <summary>
    /// Unused preferred-interface attribute does not change identity list mapping.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapWithUnusedPreferredInterfaceAttributeReturnsInput()
    {
        var input = new List<int> { 9, 8 };

        var actual = this.mapper.MapWithUnusedPreferredInterfaceAttribute(input);

        actual.Should().BeSameAs(input);
    }
}