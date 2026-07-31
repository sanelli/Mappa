// <copyright file="MappaObjectFactoryMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for object factory sample mappers.
/// </summary>
public sealed class MappaObjectFactoryMapperTests
{
    /// <summary>
    /// Test a parameterless factory keeps factory-set fields and fills properties from the source.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingEmptyParameterFactory()
    {
        // Arrange
        var mapper = new MappaObjectFactoryEmptyParameterMapper();
        var input = new ObjectFactorySourceModel
        {
            Name = "Ada",
            Value = 5,
        };

        // Act
        var actual = mapper.Map(input);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Value.Should().Be(5);
        actual.FactoryTag.Should().Be("empty-parameter");
    }

    /// <summary>
    /// Test a context-only factory reads the context and fills properties from the source.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingContextParameterFactory()
    {
        // Arrange
        var mapper = new MappaObjectFactoryContextParameterMapper();
        var input = new ObjectFactorySourceModel
        {
            Name = "Ada",
            Value = 5,
        };
        MappaContext context = new Dictionary<string, object>
        {
            ["factory-tag"] = "from-context",
        };

        // Act
        var actual = mapper.Map(input, context);

        // Assert
        actual.Name.Should().Be("Ada");
        actual.Value.Should().Be(5);
        actual.FactoryTag.Should().Be("from-context");
    }

    /// <summary>
    /// Test a source-and-context factory fully produces the target without property assignment.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingSourceAndContextFactory()
    {
        // Arrange
        var mapper = new MappaObjectFactorySourceAndContextMapper();
        var input = new ObjectFactorySourceModel
        {
            Name = "Ada",
            Value = 5,
        };
        MappaContext context = new Dictionary<string, object>
        {
            ["suffix"] = "ctx",
        };

        // Act
        var actual = mapper.Map(input, context);

        // Assert
        actual.Name.Should().Be("Ada-ctx");
        actual.Value.Should().Be(205);
        actual.FactoryTag.Should().Be("source-and-context");
    }

    /// <summary>
    /// Test a source-only factory fully produces the target without property assignment.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingSourceParameterFactory()
    {
        // Arrange
        var mapper = new MappaObjectFactorySourceParameterMapper();
        var input = new ObjectFactorySourceModel
        {
            Name = "Ada",
            Value = 5,
        };

        // Act
        var actual = mapper.Map(input);

        // Assert
        actual.Name.Should().Be("Ada-source");
        actual.Value.Should().Be(105);
        actual.FactoryTag.Should().Be("source");
    }

    /// <summary>
    /// Test a multi-parameter factory maps parameters from source properties like a non-empty constructor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingParameterizedFactory()
    {
        // Arrange
        var mapper = new MappaObjectFactoryParameterizedMapper();
        var input = new ObjectFactorySourceModel
        {
            Name = "Ada",
            Value = 5,
        };

        // Act
        var actual = mapper.Map(input);

        // Assert
        actual.Name.Should().Be("Ada-parameterized");
        actual.Value.Should().Be(55);
        actual.FactoryTag.Should().Be("parameterized");
    }
}