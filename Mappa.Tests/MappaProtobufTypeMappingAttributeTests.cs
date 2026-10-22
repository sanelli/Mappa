// <copyright file="MappaProtobufTypeMappingAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using AwesomeAssertions;

using Mappa.Attributes;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Tests;

/// <summary>
/// Unit tests for <see cref="MappaProtobufTypeMappingAttribute"/>.
/// </summary>
public sealed class MappaProtobufTypeMappingAttributeTests
{
    /// <summary>
    /// Tests the proto→POCO constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ProtoToPocoConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaProtobufTypeMappingAttribute(typeof(string), "Person");

        // Assert
        attribute.TargetType.Should().Be<string>();
        attribute.SourceOneOfCaseName.Should().Be("Person");
        attribute.TargetOneOfCaseName.Should().BeNull();
        attribute.SourceType.Should().BeNull();
    }

    /// <summary>
    /// Tests the POCO→proto constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void PocoToProtoConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaProtobufTypeMappingAttribute("Error", typeof(int));

        // Assert
        attribute.TargetOneOfCaseName.Should().Be("Error");
        attribute.SourceType.Should().Be<int>();
        attribute.SourceOneOfCaseName.Should().BeNull();
        attribute.TargetType.Should().BeNull();
    }

    /// <summary>
    /// Tests the proto→proto constructor initializes the attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ProtoToProtoConstructorInitializesAttribute()
    {
        // Act
        var attribute = new MappaProtobufTypeMappingAttribute("TargetPerson", "SourcePerson");

        // Assert
        attribute.TargetOneOfCaseName.Should().Be("TargetPerson");
        attribute.SourceOneOfCaseName.Should().Be("SourcePerson");
        attribute.SourceType.Should().BeNull();
        attribute.TargetType.Should().BeNull();
    }

    /// <summary>
    /// Tests the attribute supports methods and repeated declarations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AttributeUsageSupportsMethodsAndMultipleDeclarations()
    {
        // Act
        var attributeUsage = typeof(MappaProtobufTypeMappingAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .OfType<AttributeUsageAttribute>()
            .Single();

        // Assert
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Method);
        attributeUsage.AllowMultiple.Should().BeTrue();
    }
}