// <copyright file="MapFieldsMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="MapFieldsMapper"/>.
/// </summary>
public sealed class MapFieldsMapperUnitTests
{
    private readonly MapFieldsMapper mapper = new();

    /// <summary>
    /// Tests <see cref="MapFieldsMapper.MapFieldsToFields"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFieldsToFields()
    {
        // Arrange
        var source = new MapFieldsSourceModel
        {
            Name = "Ada",
            Age = 36,
        };

        // Act
        var actual = this.mapper.MapFieldsToFields(source);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Age.Should().Be(36);
    }

    /// <summary>
    /// Tests <see cref="MapFieldsMapper.MapFieldsToProperties"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapFieldsToProperties()
    {
        // Arrange
        var source = new MapFieldsSourceModel
        {
            Name = "Ada",
            Age = 36,
        };

        // Act
        var actual = this.mapper.MapFieldsToProperties(source);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Age.Should().Be(36);
    }
}