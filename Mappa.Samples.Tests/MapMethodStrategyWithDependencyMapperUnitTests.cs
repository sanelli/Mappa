// <copyright file="MapMethodStrategyWithDependencyMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="MapMethodStrategyWithDependencyMapper"/>.
/// </summary>
public sealed class MapMethodStrategyWithDependencyMapperUnitTests
{
    private readonly MapMethodStrategyWithDependencyMapper withDependencyMapper = new();

    /// <summary>
    /// Unit test for <see cref="MapMethodStrategyWithDependencyMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapInvokingAMethodOnADependencyClass()
    {
        // Arrange
        var source = new SourceClassWithMultipleFieldsForDependencyModel
        {
            InnerModel = new()
            {
                ParamA = 33,
                ParamB = CountingValues.One,
            },
            Property1 = 1,
            Property2 = 2,
            Property3 = 3,
            Property4 = 4,
            Property5 = 5,
            Property6 = 6,
            Property7 = 7,
        };

        // Act
        var target = this.withDependencyMapper.Map(source);

        // Assert
        target.InnerModel.ParamA.Should().Be($"{source.InnerModel.ParamA}");
        target.InnerModel.ParamB.Should().Be((int)CountingValues.One);
        target.Property1.Should().Be($"{source.Property1 + 1}");
        target.Property2.Should().Be($"{source.Property2 + 2}");
        target.Property3.Should().Be($"{source.Property3 + 3}");
        target.Property4.Should().Be($"{source.Property4 + 4}");
        target.Property5.Should().Be($"{source.Property5 + 5}");
        target.Property6.Should().Be($"{source.Property6 + 6}");
    }
}