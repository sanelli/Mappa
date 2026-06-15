// <copyright file="PragmaWarningSettingMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="PragmaWarningSettingMapper"/>.
/// </summary>
public sealed class PragmaWarningSettingMapperUnitTests
{
    private readonly PragmaWarningSettingMapper mapper = new();

    /// <summary>
    /// Map <see cref="PragmaWarningSettingMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMap()
    {
        // Arrange
        int input = 100;

        // Act
        var actual = this.mapper.Map(input);

        // Assert
        actual.Should().Be(input);
    }
}