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
    private readonly MapMethodStrategyWithDependencyMapper withDependencyMapper = new(new());

    /// <summary>
    /// Unit test for <see cref="MapMethodStrategyWithDependencyMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapInvokingAMethodOnADependencyClass()
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
        var target = this.withDependencyMapper.Map(source);

        target.InnerModel.ParamA.Should().Be($"{source.InnerModel.ParamA}");
        target.InnerModel.ParamB.Should().Be((int)CountingValues.One);
    }
}