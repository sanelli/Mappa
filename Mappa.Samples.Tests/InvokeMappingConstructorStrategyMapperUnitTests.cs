// <copyright file="InvokeMappingConstructorStrategyMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="InvokeMappingConstructorStrategyMapper"/>.
/// </summary>
public sealed class InvokeMappingConstructorStrategyMapperUnitTests
{
    private readonly InvokeMappingConstructorStrategyMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="InvokeMappingConstructorStrategyMapper.MapToClassWithSingleMappingConstructor"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToClassWithSingleMappingConstructor()
    {
        // Arrange
        var source = new SourceClassModel { ParamA = 123, ParamB = CountingValues.Three };

        // Act
        var target = this.mapper.MapToClassWithSingleMappingConstructor(source);

        // Arrange
        target.ParamA.Should().Be(source.ParamA);
        target.ParamB.Should().Be(source.ParamB);
    }

    /// <summary>
    /// Unit test for <see cref="InvokeMappingConstructorStrategyMapper.MapToClassWithSingleMappingConstructorRequiringStrategy"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToClassWithSingleMappingConstructorRequiringStrategy()
    {
        // Arrange
        var source = CountingValues.Three;

        // Act
        var target = this.mapper.MapToClassWithSingleMappingConstructorRequiringStrategy(source);

        // Arrange
        target.ParamA.Should().Be((int)source);
        target.ParamB.Should().Be(source);
    }

    /// <summary>
    /// Unit test for <see cref="InvokeMappingConstructorStrategyMapper.MapToClassWithMultipleMappingConstructors"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapToClassWithMultipleMappingConstructors()
    {
        // Arrange
        var source = CountingValues.Three;

        // Act
        var target = this.mapper.MapToClassWithMultipleMappingConstructors(source);

        // Arrange
        target.ParamA.Should().Be((int)source);
        target.ParamB.Should().Be(source);
    }
}