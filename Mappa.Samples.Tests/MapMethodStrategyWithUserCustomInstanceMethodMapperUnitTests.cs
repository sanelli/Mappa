// <copyright file="MapMethodStrategyWithUserCustomInstanceMethodMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="MapMethodStrategyWithUserCustomInstanceMethodMapper"/>.
/// </summary>
public sealed class MapMethodStrategyWithUserCustomInstanceMethodMapperUnitTests
{
    private const int AValue = 101;
    private readonly MapMethodStrategyWithUserCustomInstanceMethodMapper mapper = new(AValue);

    /// <summary>
    /// Unit test for <see cref="MapMethodStrategyWithUserCustomInstanceMethodMapper.Map(SourceClassWithInnerClassModel)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapInvokingUserCustomInstanceMethod()
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

        target.InnerModel.ParamA.Should().Be($"{source.InnerModel.ParamA + AValue}");
        target.InnerModel.ParamB.Should().Be(AValue);
    }
}