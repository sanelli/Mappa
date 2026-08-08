// <copyright file="BreakCompileTimeCyclesSettingsIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Models;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="Mappa.Attributes.MappaSettingsAttribute.BreakCompileTimeCycles"/>
/// settings layering (method vs class vs <c>.editorconfig</c>; inherit/<c>Undefined</c>).
/// </summary>
public sealed class BreakCompileTimeCyclesSettingsIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string Ns = "Mappa.Generator.Tests.UnitTests.SourceCode";
    private const string RootSourceType = $"{Ns}.RootSource";
    private const string RootTargetType = $"{Ns}.RootTarget";
    private const string ASourceType = $"{Ns}.ASource";
    private const string ATargetType = $"{Ns}.ATarget";
    private const string BSourceType = $"{Ns}.BSource";
    private const string BTargetType = $"{Ns}.BTarget";
    private const string SyntheticMethodName = "Map__ASource__To__ATarget";

    private const string MutualCycleTypes = """
                                            public class ASource
                                            {
                                                public int Id { get; set; }
                                                public BSource Child { get; set; } = null!;
                                            }

                                            public class BSource
                                            {
                                                public int Id { get; set; }
                                                public ASource Child { get; set; } = null!;
                                            }

                                            public class ATarget
                                            {
                                                public int Id { get; set; }
                                                public BTarget Child { get; set; } = null!;
                                            }

                                            public class BTarget
                                            {
                                                public int Id { get; set; }
                                                public ATarget Child { get; set; } = null!;
                                            }

                                            public class RootSource
                                            {
                                                public ASource Child { get; set; } = null!;
                                            }

                                            public class RootTarget
                                            {
                                                public ATarget Child { get; set; } = null!;
                                            }
                                            """;

    private static readonly SyntaxKind[] ClassModifiers =
    [
        SyntaxKind.PublicKeyword,
        SyntaxKind.SealedKeyword,
        SyntaxKind.PartialKeyword,
    ];

    private static readonly SyntaxKind[] PublicPartialMethodModifiers =
    [
        SyntaxKind.PublicKeyword,
        SyntaxKind.PartialKeyword,
    ];

    private static readonly SyntaxKind[] PrivateMethodModifiers =
    [
        SyntaxKind.PrivateKeyword,
    ];

    /// <summary>
    /// BreakCompileTimeCycles enabled via <c>.editorconfig</c> synthesizes a private map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task BreakCompileTimeCyclesEnabledInEditorConfigSynthesizesPrivateMapMethod()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.breakcompiletimecycles = enable
                                    """;

        var sourceCode = BuildMutualCycleMapper(classSettings: null, methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        AssertCycleAutoBroken(generatedResults);
    }

    /// <summary>
    /// BreakCompileTimeCycles enabled on the class synthesizes a private map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task BreakCompileTimeCyclesEnabledOnClassSynthesizesPrivateMapMethod()
    {
        var sourceCode = BuildMutualCycleMapper(
            classSettings: "BreakCompileTimeCycles = BooleanSetting.Enable",
            methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertCycleAutoBroken(generatedResults);
    }

    /// <summary>
    /// Method-level BreakCompileTimeCycles Disable overrides class Enable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task BreakCompileTimeCyclesMethodDisableOverridesClassEnable()
    {
        var sourceCode = BuildMutualCycleMapper(
            classSettings: "BreakCompileTimeCycles = BooleanSetting.Enable",
            methodSettings: "BreakCompileTimeCycles = BooleanSetting.Disable");
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertMappingCycleStillReported(generatedResults);
    }

    /// <summary>
    /// Method-level BreakCompileTimeCycles Enable overrides class Disable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task BreakCompileTimeCyclesMethodEnableOverridesClassDisable()
    {
        var sourceCode = BuildMutualCycleMapper(
            classSettings: "BreakCompileTimeCycles = BooleanSetting.Disable",
            methodSettings: "BreakCompileTimeCycles = BooleanSetting.Enable");
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertCycleAutoBroken(generatedResults);
    }

    /// <summary>
    /// Method-level BreakCompileTimeCycles Undefined inherits class Enable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task BreakCompileTimeCyclesMethodUndefinedInheritsClassEnable()
    {
        var sourceCode = BuildMutualCycleMapper(
            classSettings: "BreakCompileTimeCycles = BooleanSetting.Enable",
            methodSettings: "BreakCompileTimeCycles = BooleanSetting.Undefined");
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertCycleAutoBroken(generatedResults);
    }

    /// <summary>
    /// Class-level BreakCompileTimeCycles Disable overrides <c>.editorconfig</c> enable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task BreakCompileTimeCyclesClassDisableOverridesEditorConfigEnable()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.breakcompiletimecycles = enable
                                    """;

        var sourceCode = BuildMutualCycleMapper(
            classSettings: "BreakCompileTimeCycles = BooleanSetting.Disable",
            methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        AssertMappingCycleStillReported(generatedResults);
    }

    private static string BuildMutualCycleMapper(string? classSettings, string? methodSettings)
    {
        var classAttribute = classSettings is null ? string.Empty : $"[MappaSettings({classSettings})]";
        var methodAttribute = methodSettings is null ? string.Empty : $"[MappaSettings({methodSettings})]";
        return $$"""
                 #nullable enable
                 using Mappa.Attributes;

                 namespace {{Ns}};

                 {{MutualCycleTypes}}

                 [Mappa]
                 {{classAttribute}}
                 public sealed partial class Mapper
                 {
                     {{methodAttribute}}
                     public partial RootTarget Map(RootSource input);
                 }
                 #nullable restore
                 """;
    }

    private static void AssertCycleAutoBroken(GeneratedResults generatedResults)
    {
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappingCycleAutoBroken,
                ASourceType,
                ATargetType,
                SyntheticMethodName)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                "Mapper",
                ClassModifiers,
                "Map",
                PublicPartialMethodModifiers,
                false,
                RootTargetType,
                NullableAnnotation.NotAnnotated,
                "input",
                RootSourceType,
                NullableAnnotation.NotAnnotated,
                2,
                AssertRootMapInvokesSynthetic)
            .HaveMapMethod(
                "Mapper",
                ClassModifiers,
                SyntheticMethodName,
                PrivateMethodModifiers,
                false,
                ATargetType,
                NullableAnnotation.NotAnnotated,
                "source",
                ASourceType,
                NullableAnnotation.NotAnnotated,
                2,
                AssertSyntheticMapSelfInvokes);
    }

    private static void AssertMappingCycleStillReported(GeneratedResults generatedResults)
    {
        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappingCycleDetected, ASourceType, ATargetType)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, BTargetType, "Child")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                RootTargetType,
                NullableAnnotation.NotAnnotated,
                RootSourceType,
                NullableAnnotation.NotAnnotated,
                AssertRootMapOmittingCyclicBackEdge);
    }

    private static void AssertRootMapInvokesSynthetic(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(10)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                ASourceType,
                "__mappa_tmp_1",
                init => init.BeMemberAccessExpressionSyntax("input.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_2",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Id")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                BSourceType,
                "__mappa_tmp_3",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_4",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Id")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                ASourceType,
                "__mappa_tmp_5",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                ATargetType,
                "__mappa_tmp_6",
                init => init.BeInvocationExpressionSyntax(
                    $"this.{SyntheticMethodName}",
                    argument => argument.BeIdentifierNameSyntax("__mappa_tmp_5"))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                BTargetType,
                "__mappa_tmp_7",
                init => init.BeObjectCreationExpressionSyntax(
                    BTargetType,
                    ("Id", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_4")),
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_6")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                ATargetType,
                "__mappa_tmp_8",
                init => init.BeObjectCreationExpressionSyntax(
                    ATargetType,
                    ("Id", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2")),
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_7")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                RootTargetType,
                "__mappa_tmp_9",
                init => init.BeObjectCreationExpressionSyntax(
                    RootTargetType,
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_8")))))
            .HasNextSyntaxNode(node => node.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_9")));
    }

    private static void AssertSyntheticMapSelfInvokes(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(8)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_10",
                init => init.BeMemberAccessExpressionSyntax("source.Id")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                BSourceType,
                "__mappa_tmp_11",
                init => init.BeMemberAccessExpressionSyntax("source.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_12",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_11.Id")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                ASourceType,
                "__mappa_tmp_13",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_11.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                ATargetType,
                "__mappa_tmp_14",
                init => init.BeInvocationExpressionSyntax(
                    $"this.{SyntheticMethodName}",
                    argument => argument.BeIdentifierNameSyntax("__mappa_tmp_13"))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                BTargetType,
                "__mappa_tmp_15",
                init => init.BeObjectCreationExpressionSyntax(
                    BTargetType,
                    ("Id", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_12")),
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_14")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                ATargetType,
                "__mappa_tmp_16",
                init => init.BeObjectCreationExpressionSyntax(
                    ATargetType,
                    ("Id", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_10")),
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_15")))))
            .HasNextSyntaxNode(node => node.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_16")));
    }

    private static void AssertRootMapOmittingCyclicBackEdge(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(8)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                ASourceType,
                "__mappa_tmp_1",
                init => init.BeMemberAccessExpressionSyntax("input.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_2",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Id")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                BSourceType,
                "__mappa_tmp_3",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_4",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Id")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                BTargetType,
                "__mappa_tmp_5",
                init => init.BeObjectCreationExpressionSyntax(
                    BTargetType,
                    ("Id", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_4")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                ATargetType,
                "__mappa_tmp_6",
                init => init.BeObjectCreationExpressionSyntax(
                    ATargetType,
                    ("Id", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2")),
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_5")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                RootTargetType,
                "__mappa_tmp_7",
                init => init.BeObjectCreationExpressionSyntax(
                    RootTargetType,
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_6")))))
            .HasNextSyntaxNode(node => node.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_7")));
    }
}