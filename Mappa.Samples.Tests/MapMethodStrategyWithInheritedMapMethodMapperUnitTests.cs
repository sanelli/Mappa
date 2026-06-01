// <copyright file="MapMethodStrategyWithInheritedMapMethodMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for inherited map method samples.
/// </summary>
public sealed class MapMethodStrategyWithInheritedMapMethodMapperUnitTests
{
    private readonly MapMethodStrategyWithMapperBaseClass mapperWithMapperBaseClass = new();
    private readonly MapMethodStrategyWithDependencyPropertyBaseClass mapperWithDependencyPropertyBaseClass = new();
    private readonly MapMethodStrategyWithDependencyFieldBaseClass mapperWithDependencyFieldBaseClass = new();
    private readonly MapMethodStrategyWithInheritedDependencyPropertyMapper mapperWithInheritedDependencyProperty = new();
    private readonly MapMethodStrategyWithInheritedDependencyFieldMapper mapperWithInheritedDependencyField = new();

    /// <summary>
    /// Test mapping via a method defined on a base class of the mapper.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingMethodFromMapperBaseClass()
    {
        // Arrange
        var source = new SourceClassWithInnerClassModel
        {
            InnerModel = new()
            {
                ParamA = 33,
                ParamB = CountingValues.One,
            },
        };

        // Act
        var target = this.mapperWithMapperBaseClass.Map(source);

        // Assert
        target.InnerModel.ParamA.Should().Be($"{source.InnerModel.ParamA}");
        target.InnerModel.ParamB.Should().Be((int)CountingValues.One);
    }

    /// <summary>
    /// Test mapping via a method defined on a base class of a dependency property type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingMethodFromDependencyPropertyBaseClass()
    {
        // Arrange
        var source = new SourceClassWithInnerClassModel
        {
            InnerModel = new()
            {
                ParamA = 33,
                ParamB = CountingValues.One,
            },
        };

        // Act
        var target = this.mapperWithDependencyPropertyBaseClass.Map(source);

        // Assert
        target.InnerModel.ParamA.Should().Be($"{source.InnerModel.ParamA}");
        target.InnerModel.ParamB.Should().Be((int)CountingValues.One + 50);
    }

    /// <summary>
    /// Test mapping via a method defined on a base class of a dependency field type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingMethodFromDependencyFieldBaseClass()
    {
        // Arrange
        var source = new SourceClassWithInnerClassModel
        {
            InnerModel = new()
            {
                ParamA = 33,
                ParamB = CountingValues.One,
            },
        };

        // Act
        var target = this.mapperWithDependencyFieldBaseClass.Map(source);

        // Assert
        target.InnerModel.ParamA.Should().Be($"{source.InnerModel.ParamA}");
        target.InnerModel.ParamB.Should().Be((int)CountingValues.One + 50);
    }

    /// <summary>
    /// Test mapping via a [MappaDependency] property declared on a mapper base class.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingMappaDependencyPropertyFromMapperBaseClass()
    {
        // Arrange
        var source = new SourceClassWithInnerClassModel
        {
            InnerModel = new()
            {
                ParamA = 33,
                ParamB = CountingValues.One,
            },
        };

        // Act
        var target = this.mapperWithInheritedDependencyProperty.Map(source);

        // Assert
        target.InnerModel.ParamA.Should().Be($"{source.InnerModel.ParamA}");
        target.InnerModel.ParamB.Should().Be((int)CountingValues.One + 50);
    }

    /// <summary>
    /// Test mapping via a [MappaDependency] field declared on a mapper base class.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingMappaDependencyFieldFromMapperBaseClass()
    {
        // Arrange
        var source = new SourceClassWithInnerClassModel
        {
            InnerModel = new()
            {
                ParamA = 33,
                ParamB = CountingValues.One,
            },
        };

        // Act
        var target = this.mapperWithInheritedDependencyField.Map(source);

        // Assert
        target.InnerModel.ParamA.Should().Be($"{source.InnerModel.ParamA}");
        target.InnerModel.ParamB.Should().Be((int)CountingValues.One + 50);
    }
}