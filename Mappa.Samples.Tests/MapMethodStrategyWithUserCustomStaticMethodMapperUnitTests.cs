// <copyright file="MapMethodStrategyWithUserCustomStaticMethodMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="MapMethodStrategyWithUserCustomStaticMethodMapper"/>.
/// </summary>
public sealed class MapMethodStrategyWithUserCustomStaticMethodMapperUnitTests
{
    private readonly MapMethodStrategyWithUserCustomStaticMethodMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="MapMethodStrategyWithUserCustomStaticMethodMapper.Map(SourceClassWithInnerClassModel)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapInvokingUserCustomStaticMethod()
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

        target.InnerModel.ParamA.Should().Be($"{source.InnerModel.ParamA + 100}");
        target.InnerModel.ParamB.Should().Be(17);
    }
}