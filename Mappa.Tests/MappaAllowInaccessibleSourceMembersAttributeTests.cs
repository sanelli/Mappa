// <copyright file="MappaAllowInaccessibleSourceMembersAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaAllowInaccessibleSourceMembersAttribute"/>.
/// </summary>
public sealed class MappaAllowInaccessibleSourceMembersAttributeTests
{
    /// <summary>
    /// Tests the parameterless constructor initializes an empty member name list.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParameterlessConstructorInitializesEmptyMemberNames()
    {
        // Act
        var attribute = new MappaAllowInaccessibleSourceMembersAttribute();

        // Assert
        attribute.MemberNames.Should().BeEmpty();
    }

    /// <summary>
    /// Tests the params constructor initializes the member names.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParamsConstructorInitializesMemberNames()
    {
        // Act
        var attribute = new MappaAllowInaccessibleSourceMembersAttribute("PropertyA", "PropertyB");

        // Assert
        attribute.MemberNames.Should().Equal("PropertyA", "PropertyB");
    }

    /// <summary>
    /// Tests the params constructor with an empty array initializes an empty member name list.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParamsConstructorWithEmptyArrayInitializesEmptyMemberNames()
    {
        // Arrange
        string[] emptyNames = [];

        // Act
        var attribute = new MappaAllowInaccessibleSourceMembersAttribute(emptyNames);

        // Assert
        attribute.MemberNames.Should().BeEmpty();
    }

    /// <summary>
    /// Tests the params constructor treats a <c>null</c> array as an empty member name list.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParamsConstructorWithNullArrayInitializesEmptyMemberNames()
    {
        // Arrange
        string[]? nullNames = null;

        // Act
        var attribute = new MappaAllowInaccessibleSourceMembersAttribute(nullNames);

        // Assert
        attribute.MemberNames.Should().BeEmpty();
    }

    /// <summary>
    /// Tests the attribute supports methods only and does not allow multiple declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodsAndDisallowsMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaAllowInaccessibleSourceMembersAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }
}