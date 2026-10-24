// <copyright file="MapMethodMappingAttributesValidatorNullSubstituteTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MapMethodMappingAttributesValidator.ValidateMappaNullSubstituteAttributes"/>.
/// </summary>
public sealed class MapMethodMappingAttributesValidatorNullSubstituteTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test mode, locator, duplicate, and AssignFromConstant conflict diagnostics.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ValidateMappaNullSubstituteAttributesReportsExpectedDiagnostics()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public string? Value { get; set; }
                                  public string? Other { get; set; }
                                  public string? Both { get; set; }
                                  public string? Duplicate { get; set; }
                                  public string? Constant { get; set; }
                              }

                              public class Target
                              {
                                  public string Value { get; set; }
                                  public string Other { get; set; }
                                  public string Both { get; set; }
                                  public string Duplicate { get; set; }
                                  public string Constant { get; set; }
                              }

                              public static class Helpers
                              {
                                  public static string Substitute(string? value) => value ?? string.Empty;
                              }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  private readonly object helper = new();

                                  [MappaNullSubstitute(nameof(Target.Value), nameof(Source.Value))]
                                  [MappaNullSubstitute(nameof(Target.Both), nameof(Source.Both), AssignValue = "A", InvokeMethod = "Substitute")]
                                  [MappaNullSubstitute(nameof(Target.Other), nameof(Source.Other), InvokeMethod = "Substitute", InvokeMethodType = typeof(Helpers), InvokeMethodSource = "helper")]
                                  [MappaNullSubstitute(nameof(Target.Duplicate), nameof(Source.Duplicate), AssignValue = "A")]
                                  [MappaNullSubstitute(nameof(Target.Duplicate), nameof(Source.Duplicate), AssignValue = "B")]
                                  [MappaAssignFromConstant(nameof(Target.Constant), "C")]
                                  [MappaNullSubstitute(nameof(Target.Constant), nameof(Source.Constant), AssignValue = "D")]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var syntaxTree = compilation.SyntaxTrees[0];
        var methodDeclarationSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(method => method.Identifier.Text == "Map");
        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        var mapMethod = new MapMethod(methodDeclarationSyntax, semanticModel, nullableEnabled: true, TestContext.Current.CancellationToken);
        var context = new RecordingMapAlgorithmContext(mapMethod, syntaxTree);

        context.ValidateMappaNullSubstituteAttributes();

        context.Diagnostics.Should().Contain(diagnostic =>
            diagnostic.Descriptor.Equals(MappaDiagnosticDescriptors.NullSubstituteModeMissingOrAmbiguous));
        context.Diagnostics.Should().Contain(diagnostic =>
            diagnostic.Descriptor.Equals(MappaDiagnosticDescriptors.NullSubstituteInvokeMethodTypeAndSourceConflict));
        context.Diagnostics.Should().Contain(diagnostic =>
            diagnostic.Descriptor.Equals(MappaDiagnosticDescriptors.TooManyNullSubstituteAttributesForTheSameTargetProperty));
        context.Diagnostics.Should().Contain(diagnostic =>
            diagnostic.Descriptor.Equals(MappaDiagnosticDescriptors.NullSubstituteIgnoredBecauseAssignFromConstant));
    }

    private sealed class RecordingMapAlgorithmContext : MappaMapAlgorithmContext
    {
        public RecordingMapAlgorithmContext(MapMethod mapMethod, SyntaxTree syntaxTree)
        {
            this.MapMethod = mapMethod;
            this.SourceType = mapMethod.SourceType;
            this.TargetType = mapMethod.TargetType;
            this.ParentSymbol = mapMethod.TargetType;
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

        internal override MapMethod? MapMethod { get; }

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

        internal override bool TryGetCompatibleMethod(ITypeSymbol targetType, ITypeSymbol sourceType, Compilation compilation, out MapMethod? mapMethod)
        {
            mapMethod = null;
            return false;
        }

        internal override void ReportDiagnostic(Diagnostic diagnostic)
            => this.Diagnostics.Add(diagnostic);

        internal override Location? GetLocation() => null;
    }
}