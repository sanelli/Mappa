// <copyright file="PolymorphicMethodMapMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for Polymorphic Map Method strategy.
/// </summary>
public sealed class PolymorphicMethodMapMapperUnitTests
{
    private static readonly PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeMapper PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeMapper = new();
    private static readonly PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingDefaultAttributeMapper PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingDefaultAttributeMapper = new();
    private static readonly PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeUsingFieldDependencyMapper PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeUsingFieldDependencyMapper = new();

    /// <summary>
    /// Test <see cref="PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeMapper.Map(Models.Polymorphism.One.SourceWithDependency)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    private void TestPolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeMapper()
    {
        // Arrange
        var source = new Models.Polymorphism.One.SourceWithDependency
        {
            NumericProperty = 125,
            NestedProperty = new Models.Polymorphism.One.SourceThirdClass
            {
                NumericProperty = 456, GuidProperty = Guid.NewGuid(), Numbers = ["7", "8", "9"],
            },
        };

        // Act
        var target = PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeMapper.Map(source);

        // Assert
        target.NumericProperty.Should().Be(125L);
        target.NestedProperty.Should().NotBeNull();
        target.NestedProperty.NumericProperty.Should().Be(456);
        target.NestedProperty.GuidProperty.Should().Be(source.NestedProperty.GuidProperty.ToString());
        target.NestedProperty.Numbers.Should().BeEquivalentTo([7, 8, 9]);
    }

    /// <summary>
    /// Test <see cref="PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingDefaultAttributeMapper.Map(Models.Polymorphism.One.SourceWithDependencyWithSourceBaseClass)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    private void TestPolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingDefaultAttributeMapper()
    {
        // Arrange
        var source = new Models.Polymorphism.One.SourceWithDependencyWithSourceBaseClass
        {
            NumericProperty = 125,
            NestedProperty = new Models.Polymorphism.One.SourceBaseClass
            {
                NumericProperty = 456,
            },
        };

        // Act
        var target = PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingDefaultAttributeMapper.Map(source);

        // Assert
        target.NumericProperty.Should().Be(125L);
        target.NestedProperty.Should().NotBeNull();
        target.NestedProperty.Should().BeOfType<Models.Polymorphism.One.TargetUnmappedBaseClass>();
        target.NestedProperty.NumericProperty.Should().Be(456);
    }

    /// <summary>
    /// Test <see cref="Samples.PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeUsingFieldDependencyMapper.Map(Models.Polymorphism.One.SourceWithDependency)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    private void TestPolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeMapperUsingFieldDependency()
    {
        // Arrange
        var source = new Models.Polymorphism.One.SourceWithDependency
        {
            NumericProperty = 125,
            NestedProperty = new Models.Polymorphism.One.SourceThirdClass
            {
                NumericProperty = 456, GuidProperty = Guid.NewGuid(), Numbers = ["7", "8", "9"],
            },
        };

        // Act
        var target = PolymorphicMethodMapMapperIdentifiedViaMappaTypeMappingAttributeUsingFieldDependencyMapper.Map(source);

        // Assert
        target.NumericProperty.Should().Be(125L);
        target.NestedProperty.Should().NotBeNull();
        target.NestedProperty.NumericProperty.Should().Be(456);
        target.NestedProperty.GuidProperty.Should().Be(source.NestedProperty.GuidProperty.ToString());
        target.NestedProperty.Numbers.Should().BeEquivalentTo([7, 8, 9]);
    }

    /// <summary>
    /// Test <see cref="NonStaticPolymorphicMethodNotInvokedByStaticContextMapper.Map(Models.Polymorphism.One.SourceWithDependency)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    private void TestNonStaticPolymorphicMethodNotInvokedByStaticContextMapper()
    {
        // Arrange
        var source = new Models.Polymorphism.One.SourceWithDependency
        {
            NumericProperty = 125,
            NestedProperty = new Models.Polymorphism.One.SourceThirdClass
            {
                NumericProperty = 456, GuidProperty = Guid.NewGuid(), Numbers = ["7", "8", "9"],
            },
        };

        // Act
        var target = NonStaticPolymorphicMethodNotInvokedByStaticContextMapper.Map(source);

        // Assert
        target.NumericProperty.Should().Be(125L);
        target.NestedProperty.Should().NotBeNull();
        target.NestedProperty.NumericProperty.Should().Be(456);
        target.NestedProperty.GuidProperty.Should().Be(source.NestedProperty.GuidProperty.ToString());
        target.NestedProperty.Numbers.Should().BeEquivalentTo([7, 8, 9]);
    }
}