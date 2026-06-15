// <copyright file="InvokeEmptyConstructorOnPropertyMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="InvokeEmptyConstructorOnPropertyMapper"/>.
/// </summary>
public sealed class InvokeEmptyConstructorOnPropertyMapperUnitTests
{
    private readonly InvokeEmptyConstructorOnPropertyMapper mapper = new();

    /// <summary>
    /// Unit test for <see cref="InvokeEmptyConstructorOnPropertyMapper.Map"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapClassOnProperty()
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