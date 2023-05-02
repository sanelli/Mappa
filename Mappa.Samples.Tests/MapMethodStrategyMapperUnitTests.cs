// <copyright file="MapMethodStrategyMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="InvokeEmptyConstructorOnPropertyMapper"/>.
/// </summary>
public sealed class MapMethodStrategyMapperUnitTests
{
    private readonly MapMethodStrategyMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="MapMethodStrategyMapper.Map(SourceClassWithInnerClassModel)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapInvokingAnotherMappingMethod()
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
        var target = this.mapper.Map(source);

        target.InnerModel.ParamA.Should().Be($"{source.InnerModel.ParamA}");
        target.InnerModel.ParamB.Should().Be((int)CountingValues.One);
    }
}