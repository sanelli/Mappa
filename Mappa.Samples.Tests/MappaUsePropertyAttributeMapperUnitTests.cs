// <copyright file="MappaUsePropertyAttributeMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="MappaUsePropertyAttributeMapper"/>.
/// </summary>
public sealed class MappaUsePropertyAttributeMapperUnitTests
{
    private readonly MappaUsePropertyAttributeMapper mapper = new();

    /// <summary>
    /// Test <see cref="MappaUsePropertyAttributeMapper.MapWithEmptyConstructor"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapWithEmptyConstructor()
    {
        // Arrange
        var source = new SourceClassModel
        {
            ParamA = 17, ParamB = CountingValues.Three,
        };

        // Act
        var actual = this.mapper.MapWithEmptyConstructor(source);

        // Assert
        actual.ParamA.Should().Be(source.ParamB.ToString());
        actual.ParamB.Should().Be(source.ParamA);
    }

    /// <summary>
    /// Test <see cref="MappaUsePropertyAttributeMapper.MapWithConstructorWithParameters"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingConstructorWithParameters()
    {
        // Arrange
        var source = new SourceRecordModel(17, CountingValues.Three);

        // Act
        var actual = this.mapper.MapWithConstructorWithParameters(source);

        // Assert
        actual.ParamA.Should().Be(source.ParamB.ToString());
        actual.ParamB.Should().Be(source.ParamA);
    }
}