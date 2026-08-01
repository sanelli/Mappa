// <copyright file="InaccessibleMembersMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for inaccessible-members sample mappers.
/// </summary>
public sealed class InaccessibleMembersMapperUnitTests
{
    /// <summary>
    /// Test mapping all private source members and private target constructor/setters.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapPrivateSourceAndTargetMembers()
    {
        // Arrange
        var source = new InaccessibleMembersSourceModel("Ada", 36);
        var mapper = new InaccessibleMembersMapper();

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Age.Should().Be(36);
    }

    /// <summary>
    /// Test mapping with named inaccessible properties and a private target constructor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapNamedInaccessiblePropertiesAndPrivateConstructor()
    {
        // Arrange
        var source = new InaccessibleMembersSourceModel("Ada", 36);
        var mapper = new InaccessibleMembersNamedPropertiesAndConstructorMapper();

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Age.Should().Be(36);
    }

    /// <summary>
    /// Test mapping that only uses the private target constructor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingPrivateConstructorOnly()
    {
        // Arrange
        var source = new InaccessibleMembersSourceModel("Ada", 36);
        var mapper = new InaccessibleMembersConstructorOnlyMapper();

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Age.Should().Be(36);
    }

    /// <summary>
    /// Test mapping only a whitelisted inaccessible target property, excluding Age.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapNamedInaccessiblePropertiesOnlyExcludingAge()
    {
        // Arrange
        var source = new InaccessibleMembersSourceModel("Ada", 36);
        var mapper = new InaccessibleMembersNamedPropertiesOnlyMapper();

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Age.Should().Be(0);
    }
}