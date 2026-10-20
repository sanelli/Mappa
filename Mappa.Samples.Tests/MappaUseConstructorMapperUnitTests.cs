// <copyright file="MappaUseConstructorMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Runtime tests for <see cref="MappaUseConstructorMapper"/>.
/// </summary>
public sealed class MappaUseConstructorMapperUnitTests
{
    private readonly MappaUseConstructorMapper mapper = new();

    /// <summary>
    /// Forces the empty constructor over the mapping constructor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapForceEmptyUsesEmptyConstructor()
    {
        var input = new UseConstructorSource { PropertyA = 42, PropertyB = "hello" };

        var actual = this.mapper.MapForceEmpty(input);

        actual.ConstructorUsed.Should().Be("empty");
        actual.PropertyA.Should().Be("42");
        actual.PropertyB.Should().Be("hello");
    }

    /// <summary>
    /// Forces the parameterized constructor over the empty constructor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapForceParameterizedUsesParameterizedConstructor()
    {
        var input = new UseConstructorSource { PropertyA = 7, PropertyB = "world" };

        var actual = this.mapper.MapForceParameterized(input);

        actual.ConstructorUsed.Should().Be("parameterized");
        actual.PropertyB.Should().Be("world");
    }

    /// <summary>
    /// Forces the exact <c>(int, string)</c> constructor via the generic attribute.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapExactSignatureUsesIntStringConstructor()
    {
        var input = new UseConstructorSource { PropertyA = 3, PropertyB = "exact" };

        var actual = this.mapper.MapExactSignature(input);

        actual.ConstructorUsed.Should().Be("int-string");
        actual.PropertyA.Should().Be(3);
        actual.PropertyB.Should().Be("exact");
    }

    /// <summary>
    /// Typed attribute forces the nested target parameterized constructor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapNestedTypedUsesParameterizedNestedConstructor()
    {
        var input = new UseConstructorNestedOuterSource
        {
            Nested = new UseConstructorNestedInnerSource { PropertyA = "nested-typed" },
        };

        var actual = this.mapper.MapNestedTyped(input);

        actual.Nested.ConstructorUsed.Should().Be("parameterized");
        actual.Nested.PropertyA.Should().Be("nested-typed");
    }
}