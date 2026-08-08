// <copyright file="MaxCompileTimeDepthIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Models;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="Mappa.Attributes.MappaSettingsAttribute.MaxCompileTimeDepth"/>.
/// </summary>
public sealed class MaxCompileTimeDepthIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string Ns = "Mappa.Generator.Tests.UnitTests.SourceCode";
    private const string Level0SourceType = $"{Ns}.Level0Source";
    private const string Level0TargetType = $"{Ns}.Level0Target";
    private const string Level1SourceType = $"{Ns}.Level1Source";
    private const string Level1TargetType = $"{Ns}.Level1Target";
    private const string Level2SourceType = $"{Ns}.Level2Source";
    private const string Level2TargetType = $"{Ns}.Level2Target";
    private const string Level4SourceType = $"{Ns}.Level4Source";
    private const string Level4TargetType = $"{Ns}.Level4Target";
    private const string NestedSourceType = $"{Ns}.NestedSource";
    private const string NestedTargetType = $"{Ns}.NestedTarget";
    private const string SourceType = $"{Ns}.Source";
    private const string TargetType = $"{Ns}.Target";
    private const string NodeSourceType = $"{Ns}.NodeSource";
    private const string NodeTargetType = $"{Ns}.NodeTarget";

    private const string ThreeLevelTypes = """
                                           public class Level2Source
                                           {
                                               public int Value { get; set; }
                                           }

                                           public class Level2Target
                                           {
                                               public int Value { get; set; }
                                           }

                                           public class Level1Source
                                           {
                                               public Level2Source Child { get; set; } = null!;
                                           }

                                           public class Level1Target
                                           {
                                               public Level2Target Child { get; set; } = null!;
                                           }

                                           public class Level0Source
                                           {
                                               public Level1Source Child { get; set; } = null!;
                                           }

                                           public class Level0Target
                                           {
                                               public Level1Target Child { get; set; } = null!;
                                           }
                                           """;

    private const string FiveLevelTypes = """
                                          public class Level4Source
                                          {
                                              public int Value { get; set; }
                                          }

                                          public class Level4Target
                                          {
                                              public int Value { get; set; }
                                          }

                                          public class Level3Source
                                          {
                                              public Level4Source Child { get; set; } = null!;
                                          }

                                          public class Level3Target
                                          {
                                              public Level4Target Child { get; set; } = null!;
                                          }

                                          public class Level2Source
                                          {
                                              public Level3Source Child { get; set; } = null!;
                                          }

                                          public class Level2Target
                                          {
                                              public Level3Target Child { get; set; } = null!;
                                          }

                                          public class Level1Source
                                          {
                                              public Level2Source Child { get; set; } = null!;
                                          }

                                          public class Level1Target
                                          {
                                              public Level2Target Child { get; set; } = null!;
                                          }

                                          public class Level0Source
                                          {
                                              public Level1Source Child { get; set; } = null!;
                                          }

                                          public class Level0Target
                                          {
                                              public Level1Target Child { get; set; } = null!;
                                          }
                                          """;

    /// <summary>
    /// Default effective MaxCompileTimeDepth (<c>50</c>) allows ordinary nested graphs.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DefaultMaxCompileTimeDepthAllowsOrdinaryThreeLevelNesting()
    {
        var sourceCode = BuildMapper(ThreeLevelTypes, "Level0Target", "Level0Source", methodSettings: null, classSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertThreeLevelSuccess(generatedResults);
    }

    /// <summary>
    /// Method MaxCompileTimeDepth of <c>-1</c> inherits the default and allows nesting.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NegativeMethodMaxCompileTimeDepthInheritsDefaultAndAllowsNesting()
    {
        var sourceCode = BuildMapper(ThreeLevelTypes, "Level0Target", "Level0Source", methodSettings: "MaxCompileTimeDepth = -1", classSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertThreeLevelSuccess(generatedResults);
    }

    /// <summary>
    /// Nesting that reaches exactly MaxCompileTimeDepth succeeds.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NestingThatEqualsMaxCompileTimeDepthSucceeds()
    {
        // Level0 (0) → Level1 (1) → Level2 (2) → int (3); limit 3 allows depth == limit.
        var sourceCode = BuildMapper(ThreeLevelTypes, "Level0Target", "Level0Source", methodSettings: "MaxCompileTimeDepth = 3", classSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertThreeLevelSuccess(generatedResults);
    }

    /// <summary>
    /// Nesting that exceeds MaxCompileTimeDepth reports MP00076 and emits a NoMap stub.
    /// Also covers method-level setting and property nesting via <c>TypeMapIdentifierWithMapMethodAlgorithm</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NestingThatExceedsMaxCompileTimeDepthReportsError()
    {
        var sourceCode = BuildMapper(ThreeLevelTypes, "Level0Target", "Level0Source", methodSettings: "MaxCompileTimeDepth = 1", classSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertDepthExceeded(generatedResults, Level2SourceType, Level2TargetType, expectedDepth: 1, Level0TargetType, Level0SourceType);
    }

    /// <summary>
    /// MaxCompileTimeDepth of <c>0</c> disables the limit so a deep graph succeeds.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ZeroMaxCompileTimeDepthDisablesLimitForDeepGraph()
    {
        var sourceCode = BuildMapper(FiveLevelTypes, "Level0Target", "Level0Source", methodSettings: "MaxCompileTimeDepth = 0", classSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                Level0TargetType,
                NullableAnnotation.NotAnnotated,
                Level0SourceType,
                NullableAnnotation.NotAnnotated,
                AssertFiveLevelMap);
    }

    /// <summary>
    /// Sibling nested properties both succeed under a tight-but-sufficient limit (depth restore).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SiblingNestedPropertiesBothSucceedUnderTightLimit()
    {
        const string types = """
                             public class NestedSource
                             {
                                 public int Value { get; set; }
                             }

                             public class NestedTarget
                             {
                                 public int Value { get; set; }
                             }

                             public class Source
                             {
                                 public NestedSource Left { get; set; } = null!;
                                 public NestedSource Right { get; set; } = null!;
                             }

                             public class Target
                             {
                                 public NestedTarget Left { get; set; } = null!;
                                 public NestedTarget Right { get; set; } = null!;
                             }
                             """;

        // Root depth 0, Nested* depth 1, int→int depth 2; limit 2 is exact after disposable restore.
        var sourceCode = BuildMapper(types, "Target", "Source", methodSettings: "MaxCompileTimeDepth = 2", classSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertSiblingNestedMap);
    }

    /// <summary>
    /// Class-level MaxCompileTimeDepth trips the limit.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ClassMaxCompileTimeDepthTripsLimit()
    {
        var sourceCode = BuildMapper(ThreeLevelTypes, "Level0Target", "Level0Source", methodSettings: null, classSettings: "MaxCompileTimeDepth = 1");
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertDepthExceeded(generatedResults, Level2SourceType, Level2TargetType, expectedDepth: 1, Level0TargetType, Level0SourceType);
    }

    /// <summary>
    /// Editorconfig MaxCompileTimeDepth trips the limit.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EditorConfigMaxCompileTimeDepthTripsLimit()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.maxcompiletimedepth = 1
                                    """;

        var sourceCode = BuildMapper(ThreeLevelTypes, "Level0Target", "Level0Source", methodSettings: null, classSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        AssertDepthExceeded(generatedResults, Level2SourceType, Level2TargetType, expectedDepth: 1, Level0TargetType, Level0SourceType);
    }

    /// <summary>
    /// Method MaxCompileTimeDepth overrides a tighter class setting.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MethodMaxCompileTimeDepthOverridesClass()
    {
        var sourceCode = BuildMapper(
            ThreeLevelTypes,
            "Level0Target",
            "Level0Source",
            methodSettings: "MaxCompileTimeDepth = 10",
            classSettings: "MaxCompileTimeDepth = 1");
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertThreeLevelSuccess(generatedResults);
    }

    /// <summary>
    /// MaxCompileTimeDepth errors without ReferenceReusing, MaxRuntimeDepth, or MappaContext.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxCompileTimeDepthErrorIsIndependentOfRuntimeReferenceHandling()
    {
        // No MappaContext / ReferenceReusing / MaxRuntimeDepth — only MaxCompileTimeDepth.
        var sourceCode = """
                         #nullable enable
                         using Mappa.Attributes;

                         namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                         public class Level2Source { public int Value { get; set; } }
                         public class Level2Target { public int Value { get; set; } }
                         public class Level1Source { public Level2Source Child { get; set; } = null!; }
                         public class Level1Target { public Level2Target Child { get; set; } = null!; }
                         public class Level0Source { public Level1Source Child { get; set; } = null!; }
                         public class Level0Target { public Level1Target Child { get; set; } = null!; }

                         [Mappa]
                         public sealed partial class Mapper
                         {
                             [MappaSettings(MaxCompileTimeDepth = 1)]
                             public partial Level0Target Map(Level0Source input);
                         }
                         #nullable restore
                         """;
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MaxCompileTimeDepthReached,
                Level2SourceType,
                Level2TargetType,
                (short)1)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                Level0TargetType,
                NullableAnnotation.NotAnnotated,
                Level0SourceType,
                NullableAnnotation.NotAnnotated,
                block =>
                {
                    block
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement());
                });
    }

    /// <summary>
    /// Deep nesting via collection element mapping exceeds MaxCompileTimeDepth.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CollectionElementNestingExceedsMaxCompileTimeDepth()
    {
        const string types = """
                             using System.Collections.Generic;

                             public class LeafSource
                             {
                                 public int Value { get; set; }
                             }

                             public class LeafTarget
                             {
                                 public int Value { get; set; }
                             }

                             public class NodeSource
                             {
                                 public LeafSource Child { get; set; } = null!;
                             }

                             public class NodeTarget
                             {
                                 public LeafTarget Child { get; set; } = null!;
                             }

                             public class Source
                             {
                                 public List<NodeSource> Items { get; set; } = null!;
                             }

                             public class Target
                             {
                                 public List<NodeTarget> Items { get; set; } = null!;
                             }
                             """;

        // Root (0) → List→List (1) → Node (2) exceeds limit 1 at the node pair.
        var sourceCode = BuildMapper(types, "Target", "Source", methodSettings: "MaxCompileTimeDepth = 1", classSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertDepthExceeded(generatedResults, NodeSourceType, NodeTargetType, expectedDepth: 1, TargetType, SourceType);
    }

    /// <summary>
    /// Deep nesting via tuple element mapping exceeds MaxCompileTimeDepth.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TupleElementNestingExceedsMaxCompileTimeDepth()
    {
        const string types = """
                             public class Level2Source
                             {
                                 public int Value { get; set; }
                             }

                             public class Level2Target
                             {
                                 public int Value { get; set; }
                             }

                             public class Level1Source
                             {
                                 public Level2Source Child { get; set; } = null!;
                             }

                             public class Level1Target
                             {
                                 public Level2Target Child { get; set; } = null!;
                             }
                             """;

        // Tuple root (0) → Level1 (1) → Level2 (2) exceeds limit 1.
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{types}}

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings(MaxCompileTimeDepth = 1)]
                               public partial (Level1Target, int) Map((Level1Source, int) input);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertDepthExceeded(
            generatedResults,
            Level2SourceType,
            Level2TargetType,
            expectedDepth: 1,
            $"({Level1TargetType}, int)",
            $"({Level1SourceType}, int)");
    }

    /// <summary>
    /// Deep nesting via nullable element mapping exceeds MaxCompileTimeDepth.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NullableElementNestingExceedsMaxCompileTimeDepth()
    {
        const string types = """
                             public class Level2Source
                             {
                                 public int Value { get; set; }
                             }

                             public class Level2Target
                             {
                                 public int Value { get; set; }
                             }

                             public class Level1Source
                             {
                                 public Level2Source Child { get; set; } = null!;
                             }

                             public class Level1Target
                             {
                                 public Level2Target Child { get; set; } = null!;
                             }
                             """;

        // Nullable root (0) → Level1 (1) → Level2 (2) exceeds limit 1.
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{types}}

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings(MaxCompileTimeDepth = 1)]
                               public partial Level1Target? Map(Level1Source? input);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertDepthExceeded(
            generatedResults,
            Level2SourceType,
            Level2TargetType,
            expectedDepth: 1,
            Level1TargetType,
            Level1SourceType,
            returnNullableAnnotation: NullableAnnotation.Annotated,
            parameterNullableAnnotation: NullableAnnotation.Annotated);
    }

    private static string BuildMapper(
        string types,
        string targetTypeName,
        string sourceTypeName,
        string? methodSettings,
        string? classSettings)
    {
        var classAttribute = classSettings is null ? string.Empty : $"[MappaSettings({classSettings})]";
        var methodAttribute = methodSettings is null ? string.Empty : $"[MappaSettings({methodSettings})]";
        return $$"""
                 #nullable enable
                 using Mappa.Attributes;

                 namespace {{Ns}};

                 {{types}}

                 [Mappa]
                 {{classAttribute}}
                 public sealed partial class Mapper
                 {
                     {{methodAttribute}}
                     public partial {{targetTypeName}} Map({{sourceTypeName}} input);
                 }
                 #nullable restore
                 """;
    }

    private static void AssertThreeLevelSuccess(GeneratedResults generatedResults)
    {
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                Level0TargetType,
                NullableAnnotation.NotAnnotated,
                Level0SourceType,
                NullableAnnotation.NotAnnotated,
                AssertThreeLevelMap);
    }

    private static void AssertDepthExceeded(
        GeneratedResults generatedResults,
        string exceededSourceType,
        string exceededTargetType,
        short expectedDepth,
        string mapReturnType,
        string mapParameterType,
        NullableAnnotation returnNullableAnnotation = NullableAnnotation.NotAnnotated,
        NullableAnnotation parameterNullableAnnotation = NullableAnnotation.NotAnnotated)
    {
        generatedResults.Should()
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MaxCompileTimeDepthReached,
                exceededSourceType,
                exceededTargetType,
                expectedDepth)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                mapReturnType,
                returnNullableAnnotation,
                mapParameterType,
                parameterNullableAnnotation,
                block =>
                {
                    block
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement());
                });
    }

    private static void AssertThreeLevelMap(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(7)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                Level1SourceType,
                "__mappa_tmp_1",
                init => init.BeMemberAccessExpressionSyntax("input.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                Level2SourceType,
                "__mappa_tmp_2",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_3",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Value")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                Level2TargetType,
                "__mappa_tmp_4",
                init => init.BeObjectCreationExpressionSyntax(
                    Level2TargetType,
                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                Level1TargetType,
                "__mappa_tmp_5",
                init => init.BeObjectCreationExpressionSyntax(
                    Level1TargetType,
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_4")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                Level0TargetType,
                "__mappa_tmp_6",
                init => init.BeObjectCreationExpressionSyntax(
                    Level0TargetType,
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_5")))))
            .HasNextSyntaxNode(node => node.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_6")));
    }

    private static void AssertFiveLevelMap(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        const string level3SourceType = $"{Ns}.Level3Source";
        const string level3TargetType = $"{Ns}.Level3Target";
        const string level2SourceType = $"{Ns}.Level2Source";
        const string level2TargetType = $"{Ns}.Level2Target";

        blockSyntaxAssertions
            .HasSyntaxNodesCount(11)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                Level1SourceType,
                "__mappa_tmp_1",
                init => init.BeMemberAccessExpressionSyntax("input.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                level2SourceType,
                "__mappa_tmp_2",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                level3SourceType,
                "__mappa_tmp_3",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                Level4SourceType,
                "__mappa_tmp_4",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_5",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_4.Value")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                Level4TargetType,
                "__mappa_tmp_6",
                init => init.BeObjectCreationExpressionSyntax(
                    Level4TargetType,
                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_5")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                level3TargetType,
                "__mappa_tmp_7",
                init => init.BeObjectCreationExpressionSyntax(
                    level3TargetType,
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_6")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                level2TargetType,
                "__mappa_tmp_8",
                init => init.BeObjectCreationExpressionSyntax(
                    level2TargetType,
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_7")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                Level1TargetType,
                "__mappa_tmp_9",
                init => init.BeObjectCreationExpressionSyntax(
                    Level1TargetType,
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_8")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                Level0TargetType,
                "__mappa_tmp_10",
                init => init.BeObjectCreationExpressionSyntax(
                    Level0TargetType,
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_9")))))
            .HasNextSyntaxNode(node => node.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_10")));
    }

    private static void AssertSiblingNestedMap(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(8)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                NestedSourceType,
                "__mappa_tmp_1",
                init => init.BeMemberAccessExpressionSyntax("input.Left")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_2",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Value")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                NestedTargetType,
                "__mappa_tmp_3",
                init => init.BeObjectCreationExpressionSyntax(
                    NestedTargetType,
                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                NestedSourceType,
                "__mappa_tmp_4",
                init => init.BeMemberAccessExpressionSyntax("input.Right")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_5",
                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_4.Value")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                NestedTargetType,
                "__mappa_tmp_6",
                init => init.BeObjectCreationExpressionSyntax(
                    NestedTargetType,
                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_5")))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                TargetType,
                "__mappa_tmp_7",
                init => init.BeObjectCreationExpressionSyntax(
                    TargetType,
                    ("Left", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")),
                    ("Right", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_6")))))
            .HasNextSyntaxNode(node => node.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_7")));
    }
}