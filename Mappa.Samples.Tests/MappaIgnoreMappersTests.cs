// <copyright file="MappaIgnoreMappersTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Attributes;
using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="MappaIgnoreAttribute"/> sample mappers.
/// </summary>
public sealed class MappaIgnoreMappersTests
{
    /// <summary>
    /// Test <see cref="MappaIgnoreLocalMethodMapper"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MappaIgnoreLocalMethodMapperTest()
    {
        // Arrange
        var mapper = new MappaIgnoreLocalMethodMapper();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }

    /// <summary>
    /// Test <see cref="MappaIgnoreDependencyMethodMapper"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MappaIgnoreDependencyMethodMapperTest()
    {
        // Arrange
        var mapper = new MappaIgnoreDependencyMethodMapper();
        var source = new SourceClassModel { ParamA = 10, ParamB = CountingValues.Three };

        // Act
        var actual = mapper.Map(source);

        // Assert
        actual.ParamA.Should().Be($"{source.ParamA}");
        actual.ParamB.Should().Be((int)source.ParamB);
    }
}