// <copyright file="MappaMustMapTargetPropertyAttributeMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="MappaMustMapTargetPropertyAttributeMapper"/> and
/// <see cref="MappaMustMapAllTargetPropertiesAttributeMapper"/>.
/// </summary>
public sealed class MappaMustMapTargetPropertyAttributeMapperUnitTests
{
    private readonly MappaMustMapTargetPropertyAttributeMapper listedMapper = new();
    private readonly MappaMustMapAllTargetPropertiesAttributeMapper allPropertiesMapper = new();

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
        var actual = this.listedMapper.Map(source);

        // Assert
        actual.PropertyA.Should().Be(source.PropertyA.ToString(CultureInfo.InvariantCulture));
        actual.PropertyB.Should().Be(source.PropertyB);
    }

    /// <summary>
    /// Test <see cref="MappaMustMapAllTargetPropertiesAttributeMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWhenAllNonRequiredPropertiesMustBeMapped()
    {
        // Arrange
        var source = new MappaMustMapTargetPropertySourceModel
        {
            PropertyA = 7,
            PropertyB = 99,
        };

        // Act
        var actual = this.allPropertiesMapper.Map(source);

        // Assert
        actual.PropertyA.Should().Be(source.PropertyA.ToString(CultureInfo.InvariantCulture));
        actual.PropertyB.Should().Be(source.PropertyB);
    }
}