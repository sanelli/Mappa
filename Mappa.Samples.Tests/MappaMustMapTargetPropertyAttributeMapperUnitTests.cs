// <copyright file="MappaMustMapTargetPropertyAttributeMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="MappaMustMapTargetPropertyAttributeMapper"/>.
/// </summary>
public sealed class MappaMustMapTargetPropertyAttributeMapperUnitTests
{
    private readonly MappaMustMapTargetPropertyAttributeMapper mapper = new();

    /// <summary>
    /// Test <see cref="MappaMustMapTargetPropertyAttributeMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWhenListedPropertiesMustBeMapped()
    {
        // Arrange
        var source = new MappaMustMapTargetPropertySourceModel
        {
            PropertyA = 17,
            PropertyB = 42,
        };

        // Act
        var actual = this.mapper.Map(source);

        // Assert
        actual.PropertyA.Should().Be(source.PropertyA.ToString(CultureInfo.InvariantCulture));
        actual.PropertyB.Should().Be(source.PropertyB);
    }
}