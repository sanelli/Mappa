// <copyright file="MappaAssignFromContextAttributeMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="MappaAssignFromContextAttributeMapper"/>.
/// </summary>
public sealed class MappaAssignFromContextAttributeMapperTests
{
    /// <summary>
    /// Test <see cref="MappaAssignFromContextAttributeMapper.Map(Mappa.Samples.Models.SourceClassModel,Mappa.MappaContext)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingMappaContext()
    {
        // Arrange
        var mapper = new MappaAssignFromContextAttributeMapper();
        var input = new SourceClassModel { ParamA = 13, ParamB = CountingValues.Three };

        // Act
        MappaContext context = new Dictionary<string, object> { ["CustomValue"] = "Use the custom value" };
        var actual = mapper.Map(input, context);

        // Assert
        actual.ParamA.Should().Be((string)context["CustomValue"]);
        actual.ParamB.Should().Be((int)CountingValues.Three);
    }

    /// <summary>
    /// Test <see cref="MappaAssignFromContextAttributeMapper.Map(Mappa.Samples.Models.SourceClassWithInnerClassModel,Mappa.MappaContext)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapUsingNestedMappaContext()
    {
        // Arrange
        var mapper = new MappaAssignFromContextAttributeMapper();
        var input = new SourceClassWithInnerClassModel { InnerModel = new SourceClassModel { ParamA = 13, ParamB = CountingValues.Three } };

        // Act
        MappaContext context = new Dictionary<string, object> { ["CustomValue"] = "Use the custom value" };
        var actual = mapper.Map(input, context);

        // Assert
        actual.InnerModel.ParamA.Should().Be((string)context["CustomValue"]);
        actual.InnerModel.ParamB.Should().Be((int)CountingValues.Three);
    }
}