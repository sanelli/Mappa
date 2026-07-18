// <copyright file="PropertyPathHelpersUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for nested property-path helpers.
/// </summary>
public sealed class PropertyPathHelpersUnitTests
{
    /// <summary>
    /// Test <see cref="PropertyPath.Parse"/> returns an empty path for null, whitespace, and empty segments.
    /// </summary>
    /// <param name="path">The path to parse.</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(".")]
    [InlineData("Foo.")]
    [InlineData(".Bar")]
    [InlineData("Foo..Bar")]
    [UnitTest]
    public void ParseReturnsEmptyPathForInvalidInput(string? path)
    {
        var parsed = PropertyPath.Parse(path);

        parsed.Segments.Should().BeEmpty();
        parsed.IsNested.Should().BeFalse();
        parsed.GetFirstSegment().Should().BeNull();
        parsed.GetRemainingSegments().Should().BeEmpty();
    }

    /// <summary>
    /// Test <see cref="PropertyPath.Parse"/> splits a valid nested path.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ParseSplitsValidNestedPath()
    {
        var parsed = PropertyPath.Parse("Location.Address.City");

        parsed.Segments.Should().Equal("Location", "Address", "City");
        parsed.IsNested.Should().BeTrue();
        parsed.GetFirstSegment().Should().Be("Location");
        parsed.GetRemainingSegments().Should().Equal("Address", "City");
        parsed.ToDotSeparatedString().Should().Be("Location.Address.City");
    }

    /// <summary>
    /// Test <see cref="PropertyPath.EndsWith"/> matching and mismatching suffix segments.
    /// </summary>
    [Fact]
    [UnitTest]
    public void EndsWithMatchesAndRejectsSuffixSegments()
    {
        var path = PropertyPath.Parse("Location.Address.City");

        path.EndsWith(["Address", "City"]).Should().BeTrue();
        path.EndsWith(["City"]).Should().BeTrue();
        path.EndsWith(["Location", "Address", "City"]).Should().BeTrue();
        path.EndsWith(["Zip"]).Should().BeFalse();
        path.EndsWith(["Address", "Zip"]).Should().BeFalse();
        path.EndsWith(["A", "B", "C", "D"]).Should().BeFalse();
        path.EndsWith([]).Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="PropertyPathContext.DescendOneLevel"/> trims target and source remaining segments.
    /// </summary>
    [Fact]
    [UnitTest]
    public void DescendOneLevelTrimsRemainingTargetAndSourceSegments()
    {
        var context = new PropertyPathContext(
            "Location.Address.City",
            "Root.Location.Address.City",
            ["Address", "City"],
            ["Location", "Address", "City"]);

        var descended = context.DescendOneLevel();

        descended.OriginalTargetPath.Should().Be("Location.Address.City");
        descended.OriginalSourcePath.Should().Be("Root.Location.Address.City");
        descended.RemainingTargetSegments.Should().Equal("City");
        descended.RemainingSourceSegments.Should().Equal("Address", "City");
        descended.IsLeafTargetMapping.Should().BeTrue();
        descended.IsNestedAttributeScope.Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="PropertyPathContext.DescendOneLevel"/> with empty remaining source segments.
    /// </summary>
    [Fact]
    [UnitTest]
    public void DescendOneLevelKeepsEmptySourceSegmentsEmpty()
    {
        var context = new PropertyPathContext(
            "Location.Address.City",
            null,
            ["Address", "City"],
            []);

        var descended = context.DescendOneLevel();

        descended.RemainingTargetSegments.Should().Equal("City");
        descended.RemainingSourceSegments.Should().BeEmpty();
        descended.OriginalSourcePath.Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="PropertyPathAttributeMatching.MatchesTargetMember"/> with remaining-segment context.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MatchesTargetMemberUsesRemainingTargetSegments()
    {
        var context = new PropertyPathContext(
            "Location.Address.City",
            "Location.Address.City",
            ["Address", "City"],
            ["Address", "City"]);

        PropertyPathAttributeMatching.MatchesTargetMember(
            "Location.Address.City",
            "Address",
            context,
            StringComparison.Ordinal).Should().BeTrue();

        PropertyPathAttributeMatching.MatchesTargetMember(
            "Location.Address.City",
            "City",
            context,
            StringComparison.Ordinal).Should().BeFalse();

        PropertyPathAttributeMatching.MatchesTargetMember(
            string.Empty,
            "Address",
            context,
            StringComparison.Ordinal).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="PropertyPathAttributeMatching.MatchesTargetMember"/> nested attribute scope matching.
    /// </summary>
    [Fact]
    [UnitTest]
    public void MatchesTargetMemberUsesNestedAttributeScope()
    {
        var context = PropertyPathContext.CreateNestedAttributeScope("Address");

        PropertyPathAttributeMatching.MatchesTargetMember(
            "Address.City",
            "City",
            context,
            StringComparison.Ordinal).Should().BeTrue();

        PropertyPathAttributeMatching.MatchesTargetMember(
            "Address.City",
            "ZipCode",
            context,
            StringComparison.Ordinal).Should().BeFalse();

        PropertyPathAttributeMatching.MatchesTargetMember(
            "Contact.Name",
            "Name",
            context,
            StringComparison.Ordinal).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="PropertyPathAttributeMatching.GetFirstSourceSegment"/> for null, whitespace, and valid paths.
    /// </summary>
    /// <param name="sourcePath">The source path.</param>
    /// <param name="expected">The expected first segment.</param>
    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("   ", null)]
    [InlineData("Location.Address.City", "Location")]
    [InlineData("City", "City")]
    [UnitTest]
    public void GetFirstSourceSegmentReturnsExpectedValue(string? sourcePath, string? expected)
    {
        PropertyPathAttributeMatching.GetFirstSourceSegment(sourcePath).Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="PropertyPathAttributeMatching.GetExpectedSourcePropertyNameForCurrentLevel"/> leaf and intermediate cases.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetExpectedSourcePropertyNameForCurrentLevelHandlesLeafAndIntermediate()
    {
        var context = new PropertyPathContext(
            "Address.City",
            "Location.Address.City",
            ["City"],
            ["Address", "City"]);

        PropertyPathAttributeMatching.GetExpectedSourcePropertyNameForCurrentLevel(
            "Location.Address.City",
            context,
            isLeafTargetMapping: true).Should().BeNull();

        PropertyPathAttributeMatching.GetExpectedSourcePropertyNameForCurrentLevel(
            "Location.Address.City",
            context,
            isLeafTargetMapping: false).Should().Be("Address");

        var emptyRemainingSource = new PropertyPathContext(
            "Address.City",
            "Location.Address.City",
            ["City"],
            []);

        PropertyPathAttributeMatching.GetExpectedSourcePropertyNameForCurrentLevel(
            "Location.Address.City",
            emptyRemainingSource,
            isLeafTargetMapping: false).Should().BeNull();

        PropertyPathAttributeMatching.GetExpectedSourcePropertyNameForCurrentLevel(
            null,
            null,
            isLeafTargetMapping: false).Should().BeNull();

        PropertyPathAttributeMatching.GetExpectedSourcePropertyNameForCurrentLevel(
            "City",
            null,
            isLeafTargetMapping: false).Should().Be("City");
    }

    /// <summary>
    /// Test <see cref="PropertyPathAttributeMatching.CreatePropertyPathContext"/> builds remaining segments.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CreatePropertyPathContextBuildsRemainingSegments()
    {
        var context = PropertyPathAttributeMatching.CreatePropertyPathContext(
            "Location.Address.City",
            "Root.Location.Address.City");

        context.RemainingTargetSegments.Should().Equal("Address", "City");
        context.RemainingSourceSegments.Should().Equal("Location", "Address", "City");
        context.IsLeafTargetMapping.Should().BeFalse();

        var withoutSource = PropertyPathAttributeMatching.CreatePropertyPathContext("Address.City", null);
        withoutSource.OriginalSourcePath.Should().BeNull();
        withoutSource.RemainingSourceSegments.Should().BeEmpty();
    }
}