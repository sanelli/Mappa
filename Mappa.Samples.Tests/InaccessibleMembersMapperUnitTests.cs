// <copyright file="InaccessibleMembersMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="InaccessibleMembersMapper"/>.
/// </summary>
public sealed class InaccessibleMembersMapperUnitTests
{
    private readonly InaccessibleMembersMapper mapper = new();

    /// <summary>
    /// Test mapping private source members and private target constructor/setters.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapPrivateSourceAndTargetMembers()
    {
        // Arrange
        var source = new InaccessibleMembersSourceModel("Ada", 36);

        // Act
        var actual = this.mapper.Map(source);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Age.Should().Be(36);
    }
}