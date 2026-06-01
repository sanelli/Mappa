// <copyright file="PolymorphicMethodMapMapperWithBaseClassUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for polymorphic map methods inherited from base classes.
/// </summary>
public sealed class PolymorphicMethodMapMapperWithBaseClassUnitTests
{
    private static readonly PolymorphicMethodMapMapperWithMapperBaseClass PolymorphicMethodMapMapperWithMapperBaseClass = new();
    private static readonly PolymorphicMethodMapMapperWithDependencyPropertyBaseClass PolymorphicMethodMapMapperWithDependencyPropertyBaseClass = new();
    private static readonly PolymorphicMethodMapMapperWithDependencyFieldBaseClass PolymorphicMethodMapMapperWithDependencyFieldBaseClass = new();

    /// <summary>
    /// Test mapping via a polymorphic method defined on a base class of the mapper.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingPolymorphicMethodFromMapperBaseClass()
    {
        // Arrange
        var source = new Models.Polymorphism.One.SourceWithDependency
        {
            NumericProperty = 125,
            NestedProperty = new Models.Polymorphism.One.SourceThirdClass
            {
                NumericProperty = 456,
                GuidProperty = Guid.NewGuid(),
                Numbers = ["7", "8", "9"],
            },
        };

        // Act
        var target = PolymorphicMethodMapMapperWithMapperBaseClass.Map(source);

        // Assert
        target.NumericProperty.Should().Be(125L);
        target.NestedProperty.Should().NotBeNull();
        target.NestedProperty.NumericProperty.Should().Be(456);
        target.NestedProperty.GuidProperty.Should().Be(source.NestedProperty.GuidProperty.ToString());
        target.NestedProperty.Numbers.Should().BeEquivalentTo([7L, 8L, 9L]);
    }

    /// <summary>
    /// Test mapping via a polymorphic method defined on a base class of a dependency property type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingPolymorphicMethodFromDependencyPropertyBaseClass()
    {
        // Arrange
        var source = new Models.Polymorphism.One.SourceWithDependency
        {
            NumericProperty = 125,
            NestedProperty = new Models.Polymorphism.One.SourceThirdClass
            {
                NumericProperty = 456,
                GuidProperty = Guid.NewGuid(),
                Numbers = ["7", "8", "9"],
            },
        };

        // Act
        var target = PolymorphicMethodMapMapperWithDependencyPropertyBaseClass.Map(source);

        // Assert
        target.NumericProperty.Should().Be(125L);
        target.NestedProperty.Should().NotBeNull();
        target.NestedProperty.NumericProperty.Should().Be(456);
        target.NestedProperty.GuidProperty.Should().Be(source.NestedProperty.GuidProperty.ToString());
        target.NestedProperty.Numbers.Should().BeEquivalentTo([7L, 8L, 9L]);
    }

    /// <summary>
    /// Test mapping via a polymorphic method defined on a base class of a dependency field type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingPolymorphicMethodFromDependencyFieldBaseClass()
    {
        // Arrange
        var source = new Models.Polymorphism.One.SourceWithDependency
        {
            NumericProperty = 125,
            NestedProperty = new Models.Polymorphism.One.SourceThirdClass
            {
                NumericProperty = 456,
                GuidProperty = Guid.NewGuid(),
                Numbers = ["7", "8", "9"],
            },
        };

        // Act
        var target = PolymorphicMethodMapMapperWithDependencyFieldBaseClass.Map(source);

        // Assert
        target.NumericProperty.Should().Be(125L);
        target.NestedProperty.Should().NotBeNull();
        target.NestedProperty.NumericProperty.Should().Be(456);
        target.NestedProperty.GuidProperty.Should().Be(source.NestedProperty.GuidProperty.ToString());
        target.NestedProperty.Numbers.Should().BeEquivalentTo([7L, 8L, 9L]);
    }
}