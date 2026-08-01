// <copyright file="MappaAllowInaccessibleTargetMembersAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaAllowInaccessibleTargetMembersAttribute"/>.
/// </summary>
public sealed class MappaAllowInaccessibleTargetMembersAttributeTests
{
    /// <summary>
    /// Tests the parameterless constructor initializes an empty member name list
    /// and defaults both allow flags to <c>true</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParameterlessConstructorInitializesEmptyMemberNamesAndDefaultFlags()
    {
        // Act
        var attribute = new MappaAllowInaccessibleTargetMembersAttribute();

        // Assert
        attribute.MemberNames.Should().BeEmpty();
        attribute.AllowProperties.Should().BeTrue();
        attribute.AllowConstructors.Should().BeTrue();
    }

    /// <summary>
    /// Tests the params constructor initializes the member names and keeps default flags.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParamsConstructorInitializesMemberNamesAndDefaultFlags()
    {
        // Act
        var attribute = new MappaAllowInaccessibleTargetMembersAttribute("PropertyA", "PropertyB");

        // Assert
        attribute.MemberNames.Should().Equal("PropertyA", "PropertyB");
        attribute.AllowProperties.Should().BeTrue();
        attribute.AllowConstructors.Should().BeTrue();
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
        var attribute = new MappaAllowInaccessibleTargetMembersAttribute(emptyNames);

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
        var attribute = new MappaAllowInaccessibleTargetMembersAttribute(nullNames);

        // Assert
        attribute.MemberNames.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that <see cref="MappaAllowInaccessibleTargetMembersAttribute.AllowProperties"/>
    /// and <see cref="MappaAllowInaccessibleTargetMembersAttribute.AllowConstructors"/> can be set.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AllowFlagsCanBeSetIndependently()
    {
        // Act
        var attribute = new MappaAllowInaccessibleTargetMembersAttribute
        {
            AllowProperties = false,
            AllowConstructors = true,
        };

        // Assert
        attribute.AllowProperties.Should().BeFalse();
        attribute.AllowConstructors.Should().BeTrue();

        // Act
        attribute.AllowProperties = true;
        attribute.AllowConstructors = false;

        // Assert
        attribute.AllowProperties.Should().BeTrue();
        attribute.AllowConstructors.Should().BeFalse();
    }

    /// <summary>
    /// Tests the attribute supports methods only and does not allow multiple declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodsAndDisallowsMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaAllowInaccessibleTargetMembersAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeFalse();
    }
}