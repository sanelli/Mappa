// <copyright file="ConstructorMapStrategyDetectorPropertyPathContextTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Algorithm.StrategyDetectors;
using Mappa.Generator.Models;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for nested property-path context helpers on <see cref="ConstructorMapStrategyDetector"/>.
/// </summary>
public sealed class ConstructorMapStrategyDetectorPropertyPathContextTests
{
    /// <summary>
    /// Test <c>GetNestedTypeMappingPropertyPathContext</c> edge returns for null, nested-attribute scope, remaining segments, and empty remaining.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetNestedTypeMappingPropertyPathContextHandlesEdgeCases()
    {
        ConstructorMapStrategyDetector.GetNestedTypeMappingPropertyPathContext("Address", null)
            .Should().BeNull();

        var nestedAttributeScope = PropertyPathContext.CreateNestedAttributeScope("Address");
        ConstructorMapStrategyDetector.GetNestedTypeMappingPropertyPathContext("Address", nestedAttributeScope)
            .Should().BeSameAs(nestedAttributeScope);

        var remainingContext = new PropertyPathContext(
            "Location.Address.City",
            "Location.Address.City",
            ["Address", "City"],
            ["Address", "City"]);
        var descended = ConstructorMapStrategyDetector.GetNestedTypeMappingPropertyPathContext("Address", remainingContext);
        descended.Should().NotBeNull();
        descended!.RemainingTargetSegments.Should().Equal("City");
        descended.RemainingSourceSegments.Should().Equal("City");

        var passThrough = ConstructorMapStrategyDetector.GetNestedTypeMappingPropertyPathContext("ZipCode", remainingContext);
        passThrough.Should().BeSameAs(remainingContext);

        var flatOriginal = new PropertyPathContext("Address", null, [], []);
        ConstructorMapStrategyDetector.GetNestedTypeMappingPropertyPathContext("Address", flatOriginal)
            .Should().BeNull();

        var nestedOriginalEmptyRemaining = new PropertyPathContext("Address.City", "Address.City", [], []);
        var nestedScopeFromEmptyRemaining = ConstructorMapStrategyDetector.GetNestedTypeMappingPropertyPathContext(
            "Address",
            nestedOriginalEmptyRemaining);
        nestedScopeFromEmptyRemaining.Should().NotBeNull();
        nestedScopeFromEmptyRemaining!.IsNestedAttributeScope.Should().BeTrue();
        nestedScopeFromEmptyRemaining.OuterTargetSegment.Should().Be("Address");

        var nestedOriginalMismatch = new PropertyPathContext("Address.City", "Address.City", [], []);
        ConstructorMapStrategyDetector.GetNestedTypeMappingPropertyPathContext("Contact", nestedOriginalMismatch)
            .Should().BeSameAs(nestedOriginalMismatch);
    }
}