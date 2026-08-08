// <copyright file="ReferenceHandlingSettingsIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Helpers;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Models;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="Mappa.Attributes.MappaSettingsAttribute.ReferenceReusing"/>,
/// <see cref="Mappa.Attributes.MappaSettingsAttribute.MaxRuntimeDepth"/>, and
/// <see cref="Mappa.Attributes.MappaSettingsAttribute.MaxCompileTimeDepth"/> settings layering
/// (method vs class vs <c>.editorconfig</c>; negative depth = Undefined inherit).
/// </summary>
public sealed class ReferenceHandlingSettingsIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string SourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.Source";
    private const string TargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";
    private const string Level2SourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.Level2Source";
    private const string Level2TargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.Level2Target";
    private const string Level0SourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.Level0Source";
    private const string Level0TargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.Level0Target";

    private const string SimpleSourceTargetTypes = """
                                                   public class Source
                                                   {
                                                       public int Value { get; set; }
                                                   }

                                                   public class Target
                                                   {
                                                       public int Value { get; set; }
                                                   }
                                                   """;

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

    private static string ReferenceManager
        => $"{ReferenceHandlingCodeGenerator.AccessorTypeName}.{ReferenceHandlingCodeGenerator.AccessorMethodName}(context)";

    /// <summary>
    /// ReferenceReusing enabled via <c>.editorconfig</c> emits TryGetReference wrapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingEnabledInEditorConfigEmitsReuseWrapping()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.referencereusing = enable
                                    """;

        var sourceCode = BuildSimpleMapperWithContext(classSettings: null, methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertReferenceReusingSimpleIntMap);
    }

    /// <summary>
    /// ReferenceReusing enabled on the class emits TryGetReference wrapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingEnabledOnClassEmitsReuseWrapping()
    {
        var sourceCode = BuildSimpleMapperWithContext(
            classSettings: "ReferenceReusing = BooleanSetting.Enable",
            methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertReferenceReusingSimpleIntMap);
    }

    /// <summary>
    /// Method-level ReferenceReusing Disable overrides class Enable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingMethodDisableOverridesClassEnable()
    {
        var sourceCode = BuildSimpleMapperWithContext(
            classSettings: "ReferenceReusing = BooleanSetting.Enable",
            methodSettings: "ReferenceReusing = BooleanSetting.Disable");
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertSimpleIntValueMap);
    }

    /// <summary>
    /// Method-level ReferenceReusing Enable overrides class Disable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingMethodEnableOverridesClassDisable()
    {
        var sourceCode = BuildSimpleMapperWithContext(
            classSettings: "ReferenceReusing = BooleanSetting.Disable",
            methodSettings: "ReferenceReusing = BooleanSetting.Enable");
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertReferenceReusingSimpleIntMap);
    }

    /// <summary>
    /// Method-level ReferenceReusing Undefined inherits class Enable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingMethodUndefinedInheritsClassEnable()
    {
        var sourceCode = BuildSimpleMapperWithContext(
            classSettings: "ReferenceReusing = BooleanSetting.Enable",
            methodSettings: "ReferenceReusing = BooleanSetting.Undefined");
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertReferenceReusingSimpleIntMap);
    }

    /// <summary>
    /// Class-level ReferenceReusing Disable overrides <c>.editorconfig</c> enable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingClassDisableOverridesEditorConfigEnable()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.referencereusing = enable
                                    """;

        var sourceCode = BuildSimpleMapperWithContext(
            classSettings: "ReferenceReusing = BooleanSetting.Disable",
            methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertSimpleIntValueMap);
    }

    /// <summary>
    /// MaxRuntimeDepth from <c>.editorconfig</c> initializes MaxDepth.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxRuntimeDepthFromEditorConfigInitializesMaxDepth()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.maxruntimedepth = 7
                                    """;

        var sourceCode = BuildSimpleMapperWithContext(classSettings: null, methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                block => AssertMaxRuntimeDepthSimpleIntMap(block, 7));
    }

    /// <summary>
    /// MaxRuntimeDepth on the class initializes MaxDepth.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxRuntimeDepthOnClassInitializesMaxDepth()
    {
        var sourceCode = BuildSimpleMapperWithContext(
            classSettings: "MaxRuntimeDepth = 3",
            methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                block => AssertMaxRuntimeDepthSimpleIntMap(block, 3));
    }

    /// <summary>
    /// Method MaxRuntimeDepth overrides class MaxRuntimeDepth.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxRuntimeDepthMethodOverridesClass()
    {
        var sourceCode = BuildSimpleMapperWithContext(
            classSettings: "MaxRuntimeDepth = 3",
            methodSettings: "MaxRuntimeDepth = 10");
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                block => AssertMaxRuntimeDepthSimpleIntMap(block, 10));
    }

    /// <summary>
    /// Method MaxRuntimeDepth of <c>-1</c> inherits the class value.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxRuntimeDepthMethodNegativeInheritsClass()
    {
        var sourceCode = BuildSimpleMapperWithContext(
            classSettings: "MaxRuntimeDepth = 3",
            methodSettings: "MaxRuntimeDepth = -1");
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                block => AssertMaxRuntimeDepthSimpleIntMap(block, 3));
    }

    /// <summary>
    /// Negative MaxRuntimeDepth in <c>.editorconfig</c> is treated as unset (effective default <c>0</c>).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxRuntimeDepthNegativeInEditorConfigIsTreatedAsUnset()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.maxruntimedepth = -1
                                    """;

        var sourceCode = BuildSimpleMapperWithContext(classSettings: null, methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertSimpleIntValueMap);
    }

    /// <summary>
    /// Class MaxRuntimeDepth overrides <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxRuntimeDepthClassOverridesEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.maxruntimedepth = 7
                                    """;

        var sourceCode = BuildSimpleMapperWithContext(
            classSettings: "MaxRuntimeDepth = 4",
            methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                block => AssertMaxRuntimeDepthSimpleIntMap(block, 4));
    }

    /// <summary>
    /// MaxCompileTimeDepth from <c>.editorconfig</c> trips on a three-level graph.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxCompileTimeDepthFromEditorConfigTripsOnThreeLevelGraph()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.maxcompiletimedepth = 1
                                    """;

        var sourceCode = BuildThreeLevelMapper(classSettings: null, methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        AssertMaxCompileTimeDepthExceeded(generatedResults, expectedDepth: 1);
    }

    /// <summary>
    /// Method MaxCompileTimeDepth overrides a tighter class limit.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxCompileTimeDepthMethodOverridesClass()
    {
        var sourceCode = BuildThreeLevelMapper(
            classSettings: "MaxCompileTimeDepth = 1",
            methodSettings: "MaxCompileTimeDepth = 10");
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
                AssertThreeLevelMap);
    }

    /// <summary>
    /// Method MaxCompileTimeDepth of <c>-1</c> inherits the class limit.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxCompileTimeDepthMethodNegativeInheritsClass()
    {
        var sourceCode = BuildThreeLevelMapper(
            classSettings: "MaxCompileTimeDepth = 1",
            methodSettings: "MaxCompileTimeDepth = -1");
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertMaxCompileTimeDepthExceeded(generatedResults, expectedDepth: 1);
    }

    /// <summary>
    /// Negative MaxCompileTimeDepth in <c>.editorconfig</c> is treated as unset (effective default <c>50</c>).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxCompileTimeDepthNegativeInEditorConfigIsTreatedAsUnset()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.maxcompiletimedepth = -5
                                    """;

        var sourceCode = BuildThreeLevelMapper(classSettings: null, methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

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

    /// <summary>
    /// Class MaxCompileTimeDepth overrides a looser <c>.editorconfig</c> value.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxCompileTimeDepthClassOverridesEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.maxcompiletimedepth = 50
                                    """;

        var sourceCode = BuildThreeLevelMapper(
            classSettings: "MaxCompileTimeDepth = 1",
            methodSettings: null);
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        AssertMaxCompileTimeDepthExceeded(generatedResults, expectedDepth: 1);
    }

    private static string BuildSimpleMapperWithContext(string? classSettings, string? methodSettings)
    {
        var classAttribute = classSettings is null ? string.Empty : $"[MappaSettings({classSettings})]";
        var methodAttribute = methodSettings is null ? string.Empty : $"[MappaSettings({methodSettings})]";
        return $$"""
                 #nullable enable
                 using Mappa;
                 using Mappa.Attributes;

                 namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                 {{SimpleSourceTargetTypes}}

                 [Mappa]
                 {{classAttribute}}
                 public sealed partial class Mapper
                 {
                     {{methodAttribute}}
                     public partial Target Map(Source input, MappaContext context);
                 }
                 #nullable restore
                 """;
    }

    private static string BuildThreeLevelMapper(string? classSettings, string? methodSettings)
    {
        var classAttribute = classSettings is null ? string.Empty : $"[MappaSettings({classSettings})]";
        var methodAttribute = methodSettings is null ? string.Empty : $"[MappaSettings({methodSettings})]";
        return $$"""
                 #nullable enable
                 using Mappa.Attributes;

                 namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                 {{ThreeLevelTypes}}

                 [Mappa]
                 {{classAttribute}}
                 public sealed partial class Mapper
                 {
                     {{methodAttribute}}
                     public partial Level0Target Map(Level0Source input);
                 }
                 #nullable restore
                 """;
    }

    private static void AssertMaxCompileTimeDepthExceeded(GeneratedResults generatedResults, short expectedDepth)
    {
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MaxCompileTimeDepthReached,
                Level2SourceType,
                Level2TargetType,
                expectedDepth)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                Level0TargetType,
                NullableAnnotation.NotAnnotated,
                Level0SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    private static void AssertSimpleIntValueMap(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    typeof(int).ToString(),
                    "__mappa_tmp_1",
                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    TargetType,
                    "__mappa_tmp_2",
                    initializationAssertions =>
                    {
                        initializationAssertions.BeObjectCreationExpressionSyntax(
                            TargetType,
                            ("Value", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
            });
    }

    private static void AssertMaxRuntimeDepthSimpleIntMap(BlockSyntaxAssertions blockSyntaxAssertions, int maxDepth)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(4)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeAssignmentExpressionStatement(
                    leftExpressionAssertions => leftExpressionAssertions.BeMemberAccessExpressionSyntax($"{ReferenceManager}.MaxDepth"),
                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(maxDepth));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    typeof(int).ToString(),
                    "__mappa_tmp_1",
                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    TargetType,
                    "__mappa_tmp_2",
                    initializationAssertions =>
                    {
                        initializationAssertions.BeObjectCreationExpressionSyntax(
                            TargetType,
                            ("Value", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
            });
    }

    private static void AssertReferenceReusingSimpleIntMap(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(TargetType, "__mappa_tmp_3");
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeIfStatementSyntax(
                    conditionAssertions =>
                    {
                        conditionAssertions.BePrefixUnaryExpressionSyntax(
                            SyntaxKind.ExclamationToken,
                            operandAssertions => operandAssertions.BeInvocationExpressionSyntax(
                                $"{ReferenceManager}.TryGetReference<{TargetType}>",
                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input"),
                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                    },
                    thenStatementAssertions =>
                    {
                        thenStatementAssertions
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(6)
                            .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                            {
                                ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                    TargetType,
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(TargetType));
                            })
                            .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                            {
                                ifSyntaxNodeAssertions.BeInvocationExpressionSyntaxStatement(
                                    $"{ReferenceManager}.AddReferencePair",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input"));
                            })
                            .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                            {
                                ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                    typeof(int).ToString(),
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value"));
                            })
                            .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                            {
                                ifSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                    leftExpressionAssertions => leftExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Value"),
                                    rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                            })
                            .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                            {
                                ifSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                    "__mappa_tmp_3",
                                    rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                            })
                            .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                            {
                                ifSyntaxNodeAssertions.BeInvocationExpressionSyntaxStatement(
                                    $"{ReferenceManager}.AddReferencePair",
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input"));
                            });
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
            });
    }

    private static void AssertThreeLevelMap(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        const string level1SourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.Level1Source";
        const string level1TargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.Level1Target";

        blockSyntaxAssertions
            .HasSyntaxNodesCount(7)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    level1SourceType,
                    "__mappa_tmp_1",
                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Child"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    Level2SourceType,
                    "__mappa_tmp_2",
                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Child"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    typeof(int).ToString(),
                    "__mappa_tmp_3",
                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Value"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    Level2TargetType,
                    "__mappa_tmp_4",
                    initializationAssertions =>
                    {
                        initializationAssertions.BeObjectCreationExpressionSyntax(
                            Level2TargetType,
                            ("Value", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    level1TargetType,
                    "__mappa_tmp_5",
                    initializationAssertions =>
                    {
                        initializationAssertions.BeObjectCreationExpressionSyntax(
                            level1TargetType,
                            ("Child", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    Level0TargetType,
                    "__mappa_tmp_6",
                    initializationAssertions =>
                    {
                        initializationAssertions.BeObjectCreationExpressionSyntax(
                            Level0TargetType,
                            ("Child", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_5")));
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_6"));
            });
    }
}