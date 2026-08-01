// <copyright file="MapMethodMappingAttributesValidatorNestedPathTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for nested-path validation helpers on <see cref="MapMethodMappingAttributesValidator"/>.
/// </summary>
public sealed class MapMethodMappingAttributesValidatorNestedPathTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MapMethodMappingAttributesValidator.ShouldValidateAttributeTargetPathAtCurrentLevel"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ShouldValidateAttributeTargetPathAtCurrentLevelHandlesNestedScopeAndRemainingSegments()
    {
        var nestedScope = PropertyPathContext.CreateNestedAttributeScope("Address");
        MapMethodMappingAttributesValidator.ShouldValidateAttributeTargetPathAtCurrentLevel(
                PropertyPath.Parse("Address.City"),
                nestedScope)
            .Should().BeTrue();
        MapMethodMappingAttributesValidator.ShouldValidateAttributeTargetPathAtCurrentLevel(
                PropertyPath.Parse("Contact.Name"),
                nestedScope)
            .Should().BeFalse();
        MapMethodMappingAttributesValidator.ShouldValidateAttributeTargetPathAtCurrentLevel(
                PropertyPath.Parse("City"),
                nestedScope)
            .Should().BeFalse();

        var remaining = new PropertyPathContext(
            "Location.Address.City",
            "Location.Address.City",
            ["Address", "City"],
            ["Address", "City"]);
        MapMethodMappingAttributesValidator.ShouldValidateAttributeTargetPathAtCurrentLevel(
                PropertyPath.Parse("Location.Address.City"),
                remaining)
            .Should().BeTrue();
        MapMethodMappingAttributesValidator.ShouldValidateAttributeTargetPathAtCurrentLevel(
                PropertyPath.Parse("Location.Address.ZipCode"),
                remaining)
            .Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="MapMethodMappingAttributesValidator.ValidateTargetPathSegments"/> empty, single, multi, and missing paths.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ValidateTargetPathSegmentsReportsDiagnosticsForMissingSegments()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Address
                              {
                                  public string City { get; set; }
                              }

                              public class Location
                              {
                                  public Address Address { get; set; }
                              }

                              public class Source
                              {
                                  public Location Location { get; set; }
                              }

                              public class Target
                              {
                                  public Location Location { get; set; }
                              }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  [MappaUseProperty("Location.Address.City", "Location.Address.City")]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var locationType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Location")!;
        var addressType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Address")!;
        var methodSyntax = compilation.SyntaxTrees
            .SelectMany(tree => tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>())
            .Single(method => method.Identifier.Text == "Map");
        var context = new RecordingMapAlgorithmContext(locationType, locationType, compilation.SyntaxTrees[0]);

        MapMethodMappingAttributesValidator.ValidateTargetPathSegments(
            context,
            methodSyntax,
            "Map",
            locationType.ToDisplayString(),
            nameof(MappaUsePropertyAttribute),
            "Location.Address.City",
            [],
            locationType);
        context.Diagnostics.Should().BeEmpty();

        MapMethodMappingAttributesValidator.ValidateTargetPathSegments(
            context,
            methodSyntax,
            "Map",
            addressType.ToDisplayString(),
            nameof(MappaUsePropertyAttribute),
            "Location.Address.City",
            ["City"],
            addressType);
        context.Diagnostics.Should().BeEmpty();

        MapMethodMappingAttributesValidator.ValidateTargetPathSegments(
            context,
            methodSyntax,
            "Map",
            addressType.ToDisplayString(),
            nameof(MappaUsePropertyAttribute),
            "Location.Address.Missing",
            ["Missing"],
            addressType);
        context.Diagnostics.Should().ContainSingle()
            .Which.Id.Should().Be(MappaDiagnosticDescriptors.MappingAttributeTargetPropertyOrParameterDoesNotExist.Id);

        context.Diagnostics.Clear();
        MapMethodMappingAttributesValidator.ValidateTargetPathSegments(
            context,
            methodSyntax,
            "Map",
            locationType.ToDisplayString(),
            nameof(MappaUsePropertyAttribute),
            "Location.Address.City",
            ["Address", "City"],
            locationType);
        context.Diagnostics.Should().BeEmpty();

        MapMethodMappingAttributesValidator.ValidateTargetPathSegments(
            context,
            methodSyntax,
            "Map",
            locationType.ToDisplayString(),
            nameof(MappaUsePropertyAttribute),
            "Location.Address.Missing",
            ["Address", "Missing"],
            locationType);
        context.Diagnostics.Should().ContainSingle()
            .Which.Id.Should().Be(MappaDiagnosticDescriptors.MappingAttributeTargetPropertyOrParameterDoesNotExist.Id);
    }

    private sealed class RecordingMapAlgorithmContext : MappaMapAlgorithmContext
    {
        public RecordingMapAlgorithmContext(ITypeSymbol sourceType, ITypeSymbol targetType, SyntaxTree syntaxTree)
        {
            this.SourceType = sourceType;
            this.TargetType = targetType;
            this.ParentSymbol = targetType;
            this.AlgorithmSettings = new MappaMapAlgorithmContextSettings();
            var globalOptions = new MappaGlobalOptions(
                TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
                syntaxTree);
            this.MappaUserSettings = new MappaUserSettings(globalOptions);
        }

        public List<Diagnostic> Diagnostics { get; } = [];

        internal override ISymbol ParentSymbol { get; }

        internal override ITypeSymbol SourceType { get; }

        internal override ITypeSymbol TargetType { get; }

        internal override MapMethod? MapMethod => null;

        internal override MappaMapAlgorithmContextSettings AlgorithmSettings { get; }

        internal override MappaUserSettings MappaUserSettings { get; }

        internal override bool HasErrorDiagnostics => this.Diagnostics.Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

        internal override bool IsNullableEnabled() => true;

        internal override bool TryGetMethod(ITypeSymbol targetType, ITypeSymbol sourceType, out MapMethod mapMethod)
        {
            mapMethod = null!;
            return false;
        }

        internal override bool TryGetPolymorphicMethod(ITypeSymbol targetType, ITypeSymbol sourceType, IMappaUserSettings mappaUserSettings, out MapMethod mapMethod)
        {
            mapMethod = null!;
            return false;
        }

        internal override bool TryGetCompatibleMethod(ITypeSymbol targetType, ITypeSymbol sourceType, Compilation compilation, out MapMethod mapMethod)
        {
            mapMethod = null!;
            return false;
        }

        internal override void ReportDiagnostic(Diagnostic diagnostic)
            => this.Diagnostics.Add(diagnostic);

        internal override Location? GetLocation() => null;
    }
}