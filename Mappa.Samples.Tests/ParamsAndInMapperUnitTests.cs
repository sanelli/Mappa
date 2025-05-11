// <copyright file="ParamsAndInMapperUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Unit tests for <see cref="ParamsAndInMapper"/>.
/// </summary>
public sealed class ParamsAndInMapperUnitTests
{
    private readonly ParamsAndInMapper mapper = new();

    /// <summary>
    /// Test <see cref="ParamsAndInMapper.MapWithIn"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapWithIn()
    {
        // Arrange
        var source = new SourceClassModel()
        {
            ParamA = 17,
            ParamB = CountingValues.Three,
        };

        // Act
        var actual = this.mapper.MapWithIn(source);

        // Assert
        actual.ParamA.Should().Be("17");
        actual.ParamB.Should().Be(2);
    }

    /// <summary>
    /// Test <see cref="ParamsAndInMapper.MapWithParams"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TestMapWithParams()
    {
        // Arrange
        var source1 = new SourceClassModel
        {
            ParamA = 17,
            ParamB = CountingValues.Three,
        };

        var source2 = new SourceClassModel
        {
            ParamA = 13,
            ParamB = CountingValues.One,
        };

        // Act
        var actual = this.mapper.MapWithParams(source1, source2);

        // Assert
        actual.Should().HaveCount(2);
        actual[0].ParamA.Should().Be("17");
        actual[0].ParamB.Should().Be(2);
        actual[1].ParamA.Should().Be("13");
        actual[1].ParamB.Should().Be(0);
    }

    /// <summary>
    /// Test <see cref="ParamsAndInMapper.MapWithInOnContext"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MapWithInOnContext()
    {
        // Arrange
        var source = new SourceRecordModel(17, CountingValues.Three);
        var context = new MappaContext { ["paramB"] = 33, };

        // Act
        var actual = this.mapper.MapWithInOnContext(source, context);

        // Assert
        actual.ParamA.Should().Be("17");
        actual.ParamB.Should().Be(33);
    }
}