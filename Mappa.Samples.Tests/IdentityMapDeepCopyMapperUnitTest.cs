// <copyright file="IdentityMapDeepCopyMapperUnitTest.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Samples.Models;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for the identity map deep copy sample mappers.
/// </summary>
public sealed class IdentityMapDeepCopyMapperUnitTest
{
    private readonly IdentityMapDeepCopyShallowMapper shallowMapper = new();
    private readonly IdentityMapDeepCopyDeepMapper deepMapper = new();
    private readonly IdentityMapDeepCopyNestedMapper nestedMapper = new();
    private readonly IdentityMapDeepCopyNestedStructMapper nestedStructMapper = new();

    /// <summary>
    /// Test shallow identity mapping returns the same root and nested references.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapShallowCopyReturnsSameRootAndNestedReferences()
    {
        // Arrange
        var source = CreatePerson();

        // Act
        var result = this.shallowMapper.Map(source);

        // Assert
        Assert.True(ReferenceEquals(source, result));
        Assert.True(ReferenceEquals(source.Child, result.Child));
    }

    /// <summary>
    /// Test deep identity mapping clones the root but shares nested references.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapDeepCopyReturnsNewRootWithSharedNestedReference()
    {
        // Arrange
        var source = CreatePerson();

        // Act
        var result = this.deepMapper.Map(source);

        // Assert
        Assert.False(ReferenceEquals(source, result));
        Assert.True(ReferenceEquals(source.Child, result.Child));
    }

    /// <summary>
    /// Test nested deep identity mapping clones the root and nested references.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapNestedDeepCopyReturnsNewRootAndNestedReferences()
    {
        // Arrange
        var source = CreatePerson();

        // Act
        var result = this.nestedMapper.Map(source);

        // Assert
        Assert.False(ReferenceEquals(source, result));
        Assert.False(ReferenceEquals(source.Child, result.Child));
    }

    /// <summary>
    /// Test nested deep identity mapping on a struct clones the nested reference field.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapNestedDeepCopyStructReturnsStructWithClonedNestedReference()
    {
        // Arrange
        var source = new IdentityMapDeepCopyStruct
        {
            Child = new IdentityMapDeepCopyChild { Name = "nested" },
        };

        // Act
        var result = this.nestedStructMapper.Map(source);

        // Assert
        Assert.False(ReferenceEquals(source.Child, result.Child));
        result.Child.Name.Should().Be(source.Child.Name);
    }

    private static IdentityMapDeepCopyPerson CreatePerson()
        => new()
        {
            Child = new IdentityMapDeepCopyChild { Name = "nested" },
        };
}