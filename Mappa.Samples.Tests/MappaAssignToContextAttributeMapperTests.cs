// <copyright file="MappaAssignToContextAttributeMapperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using FluentAssertions;

using Mappa.Samples.Models;

using Xunit;
using Xunit.Categories;

namespace Mappa.Samples.Tests;

/// <summary>
/// Tests for <see cref="MappaAssignToContextAttributeMapper"/>.
/// </summary>
public sealed class MappaAssignToContextAttributeMapperTests
{
    /// <summary>
    /// Test <see cref="MappaAssignToContextAttributeMapper.Map(Mappa.Samples.Models.SourceClassModel,Mappa.MappaContext)"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CanMapAndAssignMappedPropertyToContext()
    {
        // Arrange
        var mapper = new MappaAssignToContextAttributeMapper();
        var input = new SourceClassModel { ParamA = 13, ParamB = CountingValues.Three };
        MappaContext context = new Dictionary<string, object>();

        // Act
        var actual = mapper.Map(input, context);

        // Assert
        actual.ParamA.Should().Be("13");
        actual.ParamB.Should().Be((int)CountingValues.Three);
        context["ParamA"].Should().Be("13");
    }
}