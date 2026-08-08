// <copyright file="BreakCompileTimeCyclesIntegrationTests.cs" company="Stefano Anelli">
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
/// Integration tests for enabled <see cref="Mappa.Attributes.MappaSettingsAttribute.BreakCompileTimeCycles"/>
/// (synthetic private map methods, ReferenceReusing combo, static mapper, name collisions).
/// </summary>
public sealed class BreakCompileTimeCyclesIntegrationTests
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
    private const string SyntheticMethodNameCollided = "Map__ASource__To__ATarget_1";

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

    private static readonly SyntaxKind[] SealedClassModifiers =
    [
        SyntaxKind.PublicKeyword,
        SyntaxKind.SealedKeyword,
        SyntaxKind.PartialKeyword,
    ];

    private static readonly SyntaxKind[] StaticClassModifiers =
    [
        SyntaxKind.PublicKeyword,
        SyntaxKind.StaticKeyword,
        SyntaxKind.PartialKeyword,
    ];

    private static readonly SyntaxKind[] PublicPartialMethodModifiers =
    [
        SyntaxKind.PublicKeyword,
        SyntaxKind.PartialKeyword,
    ];

    private static readonly SyntaxKind[] PublicStaticPartialMethodModifiers =
    [
        SyntaxKind.PublicKeyword,
        SyntaxKind.StaticKeyword,
        SyntaxKind.PartialKeyword,
    ];

    private static readonly SyntaxKind[] PrivateMethodModifiers =
    [
        SyntaxKind.PrivateKeyword,
    ];

    private static readonly SyntaxKind[] PrivateStaticMethodModifiers =
    [
        SyntaxKind.PrivateKeyword,
        SyntaxKind.StaticKeyword,
    ];

    private static string ReferenceManager
        => $"{ReferenceHandlingCodeGenerator.AccessorTypeName}.{ReferenceHandlingCodeGenerator.AccessorMethodName}(context)";

    /// <summary>
    /// Enabling BreakCompileTimeCycles on a mutual A↔B graph synthesizes a private map method (MP00078)
    /// and invokes it at the cycle edge.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EnabledMutualCycleSynthesizesPrivateMapMethodAndInvokesIt()
    {
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{MutualCycleTypes}}

                           [Mappa]
                           [MappaSettings(BreakCompileTimeCycles = BooleanSetting.Enable)]
                           public sealed partial class Mapper
                           {
                               public partial RootTarget Map(RootSource input);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertEnabledWithoutContext(
            generatedResults,
            SyntheticMethodName,
            SealedClassModifiers,
            PublicPartialMethodModifiers,
            PrivateMethodModifiers,
            invokePrefix: "this.");
    }

    /// <summary>
    /// ReferenceReusing with BreakCompileTimeCycles (class-level) succeeds at compile time and invokes
    /// the synthetic method inside TryGetReference wrapping (including on the synthetic body).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingWithBreakCompileTimeCyclesEnabledSucceeds()
    {
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{MutualCycleTypes}}

                           [Mappa]
                           [MappaSettings(
                               BreakCompileTimeCycles = BooleanSetting.Enable,
                               ReferenceReusing = BooleanSetting.Enable)]
                           public sealed partial class Mapper
                           {
                               public partial RootTarget Map(RootSource input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                SealedClassModifiers,
                "Map",
                PublicPartialMethodModifiers,
                false,
                RootTargetType,
                NullableAnnotation.NotAnnotated,
                "input",
                RootSourceType,
                "context",
                NullableAnnotation.NotAnnotated,
                RefKind.None,
                false,
                RefKind.None,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                AssertRootReferenceReusingInvokesSynthetic)
            .HaveMapMethod(
                "Mapper",
                SealedClassModifiers,
                SyntheticMethodName,
                PrivateMethodModifiers,
                false,
                ATargetType,
                NullableAnnotation.NotAnnotated,
                "source",
                ASourceType,
                "context",
                NullableAnnotation.NotAnnotated,
                RefKind.None,
                false,
                RefKind.None,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                AssertSyntheticReferenceReusingSelfInvokes);
    }

    /// <summary>
    /// A static mapper synthesizes a private static map method and invokes it without <c>this</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task StaticMapperSynthesizesPrivateStaticMapMethod()
    {
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{MutualCycleTypes}}

                           [Mappa]
                           public static partial class Mapper
                           {
                               [MappaSettings(BreakCompileTimeCycles = BooleanSetting.Enable)]
                               public static partial RootTarget Map(RootSource input);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertEnabledWithoutContext(
            generatedResults,
            SyntheticMethodName,
            StaticClassModifiers,
            PublicStaticPartialMethodModifiers,
            PrivateStaticMethodModifiers,
            invokePrefix: string.Empty);
    }

    /// <summary>
    /// When the default synthetic name is already a class member, allocation appends a numeric suffix.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SyntheticMethodNameCollisionAppendsNumericSuffix()
    {
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{MutualCycleTypes}}

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               private void Map__ASource__To__ATarget()
                               {
                               }

                               [MappaSettings(BreakCompileTimeCycles = BooleanSetting.Enable)]
                               public partial RootTarget Map(RootSource input);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertEnabledWithoutContext(
            generatedResults,
            SyntheticMethodNameCollided,
            SealedClassModifiers,
            PublicPartialMethodModifiers,
            PrivateMethodModifiers,
            invokePrefix: "this.");
    }

    /// <summary>
    /// When the root map is static and only an instance map exists for the cycling pair,
    /// BreakCompileTimeCycles cannot reuse or synthesize a usable method and reports MP00077.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task StaticRootWithInstanceOnlyCycleMapReportsMappingCycleWhenBreakEnabled()
    {
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{MutualCycleTypes}}

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               public partial ATarget MapA(ASource input);

                               [MappaSettings(BreakCompileTimeCycles = BooleanSetting.Enable)]
                               public static partial RootTarget Map(RootSource input);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappingCycleDetected, ASourceType, ATargetType)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, BTargetType, "Child")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                "Mapper",
                SealedClassModifiers,
                "Map",
                PublicStaticPartialMethodModifiers,
                false,
                RootTargetType,
                NullableAnnotation.NotAnnotated,
                "input",
                RootSourceType,
                NullableAnnotation.NotAnnotated,
                2,
                block =>
                {
                    // MapA is generated first (temps 1–7); static Map continues and omits the cyclic back-edge.
                    block
                        .HasSyntaxNodesCount(8)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            ASourceType,
                            "__mappa_tmp_8",
                            init => init.BeMemberAccessExpressionSyntax("input.Child")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_9",
                            init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_8.Id")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            BSourceType,
                            "__mappa_tmp_10",
                            init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_8.Child")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_11",
                            init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_10.Id")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            BTargetType,
                            "__mappa_tmp_12",
                            init => init.BeObjectCreationExpressionSyntax(
                                BTargetType,
                                ("Id", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_11")))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            ATargetType,
                            "__mappa_tmp_13",
                            init => init.BeObjectCreationExpressionSyntax(
                                ATargetType,
                                ("Id", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_9")),
                                ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_12")))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            RootTargetType,
                            "__mappa_tmp_14",
                            init => init.BeObjectCreationExpressionSyntax(
                                RootTargetType,
                                ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_13")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_14")));
                });
    }

    private static void AssertEnabledWithoutContext(
        GeneratedResults generatedResults,
        string syntheticMethodName,
        SyntaxKind[] classModifiers,
        SyntaxKind[] mapMethodModifiers,
        SyntaxKind[] syntheticMethodModifiers,
        string invokePrefix)
    {
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappingCycleAutoBroken,
                ASourceType,
                ATargetType,
                syntheticMethodName)
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                "Mapper",
                classModifiers,
                "Map",
                mapMethodModifiers,
                false,
                RootTargetType,
                NullableAnnotation.NotAnnotated,
                "input",
                RootSourceType,
                NullableAnnotation.NotAnnotated,
                2,
                block => AssertRootMapInvokesSynthetic(block, syntheticMethodName, invokePrefix))
            .HaveMapMethod(
                "Mapper",
                classModifiers,
                syntheticMethodName,
                syntheticMethodModifiers,
                false,
                ATargetType,
                NullableAnnotation.NotAnnotated,
                "source",
                ASourceType,
                NullableAnnotation.NotAnnotated,
                2,
                block => AssertSyntheticMapSelfInvokes(block, syntheticMethodName, invokePrefix));
    }

    private static void AssertRootMapInvokesSynthetic(
        BlockSyntaxAssertions blockSyntaxAssertions,
        string syntheticMethodName,
        string invokePrefix)
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
                init =>
                {
                    if (string.IsNullOrEmpty(invokePrefix))
                    {
                        init.BeInvocationExpressionUsingIdentifierNameSyntax(
                            syntheticMethodName,
                            argument => argument.BeIdentifierNameSyntax("__mappa_tmp_5"));
                    }
                    else
                    {
                        init.BeInvocationExpressionSyntax(
                            $"{invokePrefix}{syntheticMethodName}",
                            argument => argument.BeIdentifierNameSyntax("__mappa_tmp_5"));
                    }
                }))
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

    private static void AssertSyntheticMapSelfInvokes(
        BlockSyntaxAssertions blockSyntaxAssertions,
        string syntheticMethodName,
        string invokePrefix)
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
                init =>
                {
                    if (string.IsNullOrEmpty(invokePrefix))
                    {
                        init.BeInvocationExpressionUsingIdentifierNameSyntax(
                            syntheticMethodName,
                            argument => argument.BeIdentifierNameSyntax("__mappa_tmp_13"));
                    }
                    else
                    {
                        init.BeInvocationExpressionSyntax(
                            $"{invokePrefix}{syntheticMethodName}",
                            argument => argument.BeIdentifierNameSyntax("__mappa_tmp_13"));
                    }
                }))
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

    private static void AssertRootReferenceReusingInvokesSynthetic(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(RootTargetType, "__mappa_tmp_13"))
            .HasNextSyntaxNode(node =>
            {
                node.BeIfStatementSyntax(
                    condition =>
                    {
                        condition.BePrefixUnaryExpressionSyntax(
                            SyntaxKind.ExclamationToken,
                            operand => operand.BeInvocationExpressionSyntax(
                                $"{ReferenceManager}.TryGetReference<{RootTargetType}>",
                                arg => arg.BeIdentifierNameSyntax("input"),
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_13")));
                    },
                    thenStatement =>
                    {
                        thenStatement
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(8)
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                RootTargetType,
                                "__mappa_tmp_1",
                                init => init.BeObjectCreationExpressionSyntax(RootTargetType)))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                arg => arg.BeIdentifierNameSyntax("input")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                ASourceType,
                                "__mappa_tmp_2",
                                init => init.BeMemberAccessExpressionSyntax("input.Child")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(ATargetType, "__mappa_tmp_12"))
                            .HasNextSyntaxNode(n => AssertNestedATargetReferenceReuseInvokingSynthetic(n))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Child"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_12")))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                "__mappa_tmp_13",
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_1")))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_13"),
                                arg => arg.BeIdentifierNameSyntax("input")));
                    });
            })
            .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_13"));
    }

    private static void AssertNestedATargetReferenceReuseInvokingSynthetic(SyntaxNodeAssertions node)
    {
        node.BeIfStatementSyntax(
            condition =>
            {
                condition.BePrefixUnaryExpressionSyntax(
                    SyntaxKind.ExclamationToken,
                    operand => operand.BeInvocationExpressionSyntax(
                        $"{ReferenceManager}.TryGetReference<{ATargetType}>",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_12")));
            },
            thenStatement =>
            {
                thenStatement
                    .BeBlockStatement()
                    .AsBlock()
                    .HasSyntaxNodesCount(10)
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                        ATargetType,
                        "__mappa_tmp_3",
                        init => init.BeObjectCreationExpressionSyntax(ATargetType)))
                    .HasNextSyntaxNode(inner => inner.BeInvocationExpressionSyntaxStatement(
                        $"{ReferenceManager}.AddReferencePair",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_3"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2")))
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                        typeof(int).ToString(),
                        "__mappa_tmp_4",
                        init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Id")))
                    .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                        left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Id"),
                        right => right.BeIdentifierNameSyntax("__mappa_tmp_4")))
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                        BSourceType,
                        "__mappa_tmp_5",
                        init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Child")))
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(BTargetType, "__mappa_tmp_11"))
                    .HasNextSyntaxNode(AssertNestedBTargetReferenceReuseInvokingSynthetic)
                    .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                        left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Child"),
                        right => right.BeIdentifierNameSyntax("__mappa_tmp_11")))
                    .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                        "__mappa_tmp_12",
                        right => right.BeIdentifierNameSyntax("__mappa_tmp_3")))
                    .HasNextSyntaxNode(inner => inner.BeInvocationExpressionSyntaxStatement(
                        $"{ReferenceManager}.AddReferencePair",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_12"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2")));
            });
    }

    private static void AssertNestedBTargetReferenceReuseInvokingSynthetic(SyntaxNodeAssertions node)
    {
        node.BeIfStatementSyntax(
            condition =>
            {
                condition.BePrefixUnaryExpressionSyntax(
                    SyntaxKind.ExclamationToken,
                    operand => operand.BeInvocationExpressionSyntax(
                        $"{ReferenceManager}.TryGetReference<{BTargetType}>",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_5"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_11")));
            },
            thenStatement =>
            {
                thenStatement
                    .BeBlockStatement()
                    .AsBlock()
                    .HasSyntaxNodesCount(10)
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                        BTargetType,
                        "__mappa_tmp_6",
                        init => init.BeObjectCreationExpressionSyntax(BTargetType)))
                    .HasNextSyntaxNode(inner => inner.BeInvocationExpressionSyntaxStatement(
                        $"{ReferenceManager}.AddReferencePair",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_6"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_5")))
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                        typeof(int).ToString(),
                        "__mappa_tmp_7",
                        init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_5.Id")))
                    .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                        left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Id"),
                        right => right.BeIdentifierNameSyntax("__mappa_tmp_7")))
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                        ASourceType,
                        "__mappa_tmp_8",
                        init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_5.Child")))
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(ATargetType, "__mappa_tmp_10"))
                    .HasNextSyntaxNode(AssertInnermostATargetInvokesSynthetic)
                    .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                        left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Child"),
                        right => right.BeIdentifierNameSyntax("__mappa_tmp_10")))
                    .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                        "__mappa_tmp_11",
                        right => right.BeIdentifierNameSyntax("__mappa_tmp_6")))
                    .HasNextSyntaxNode(inner => inner.BeInvocationExpressionSyntaxStatement(
                        $"{ReferenceManager}.AddReferencePair",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_11"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_5")));
            });
    }

    private static void AssertInnermostATargetInvokesSynthetic(SyntaxNodeAssertions node)
    {
        node.BeIfStatementSyntax(
            condition =>
            {
                condition.BePrefixUnaryExpressionSyntax(
                    SyntaxKind.ExclamationToken,
                    operand => operand.BeInvocationExpressionSyntax(
                        $"{ReferenceManager}.TryGetReference<{ATargetType}>",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_8"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_10")));
            },
            thenStatement =>
            {
                thenStatement
                    .BeBlockStatement()
                    .AsBlock()
                    .HasSyntaxNodesCount(3)
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                        ATargetType,
                        "__mappa_tmp_9",
                        init => init.BeInvocationExpressionSyntax(
                            $"this.{SyntheticMethodName}",
                            arg => arg.BeIdentifierNameSyntax("__mappa_tmp_8"),
                            arg => arg.BeIdentifierNameSyntax("context"))))
                    .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                        "__mappa_tmp_10",
                        right => right.BeIdentifierNameSyntax("__mappa_tmp_9")))
                    .HasNextSyntaxNode(inner => inner.BeInvocationExpressionSyntaxStatement(
                        $"{ReferenceManager}.AddReferencePair",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_10"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_8")));
            });
    }

    private static void AssertSyntheticReferenceReusingSelfInvokes(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(ATargetType, "__mappa_tmp_23"))
            .HasNextSyntaxNode(node =>
            {
                node.BeIfStatementSyntax(
                    condition =>
                    {
                        condition.BePrefixUnaryExpressionSyntax(
                            SyntaxKind.ExclamationToken,
                            operand => operand.BeInvocationExpressionSyntax(
                                $"{ReferenceManager}.TryGetReference<{ATargetType}>",
                                arg => arg.BeIdentifierNameSyntax("source"),
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_23")));
                    },
                    thenStatement =>
                    {
                        thenStatement
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(10)
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                ATargetType,
                                "__mappa_tmp_14",
                                init => init.BeObjectCreationExpressionSyntax(ATargetType)))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_14"),
                                arg => arg.BeIdentifierNameSyntax("source")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_15",
                                init => init.BeMemberAccessExpressionSyntax("source.Id")))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_14.Id"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_15")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                BSourceType,
                                "__mappa_tmp_16",
                                init => init.BeMemberAccessExpressionSyntax("source.Child")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(BTargetType, "__mappa_tmp_22"))
                            .HasNextSyntaxNode(AssertSyntheticNestedBTargetSelfInvoke)
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_14.Child"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_22")))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                "__mappa_tmp_23",
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_14")))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_23"),
                                arg => arg.BeIdentifierNameSyntax("source")));
                    });
            })
            .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_23"));
    }

    private static void AssertSyntheticNestedBTargetSelfInvoke(SyntaxNodeAssertions node)
    {
        node.BeIfStatementSyntax(
            condition =>
            {
                condition.BePrefixUnaryExpressionSyntax(
                    SyntaxKind.ExclamationToken,
                    operand => operand.BeInvocationExpressionSyntax(
                        $"{ReferenceManager}.TryGetReference<{BTargetType}>",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_16"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_22")));
            },
            thenStatement =>
            {
                thenStatement
                    .BeBlockStatement()
                    .AsBlock()
                    .HasSyntaxNodesCount(10)
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                        BTargetType,
                        "__mappa_tmp_17",
                        init => init.BeObjectCreationExpressionSyntax(BTargetType)))
                    .HasNextSyntaxNode(inner => inner.BeInvocationExpressionSyntaxStatement(
                        $"{ReferenceManager}.AddReferencePair",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_17"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_16")))
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                        typeof(int).ToString(),
                        "__mappa_tmp_18",
                        init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_16.Id")))
                    .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                        left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_17.Id"),
                        right => right.BeIdentifierNameSyntax("__mappa_tmp_18")))
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                        ASourceType,
                        "__mappa_tmp_19",
                        init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_16.Child")))
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(ATargetType, "__mappa_tmp_21"))
                    .HasNextSyntaxNode(AssertSyntheticInnermostSelfInvoke)
                    .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                        left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_17.Child"),
                        right => right.BeIdentifierNameSyntax("__mappa_tmp_21")))
                    .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                        "__mappa_tmp_22",
                        right => right.BeIdentifierNameSyntax("__mappa_tmp_17")))
                    .HasNextSyntaxNode(inner => inner.BeInvocationExpressionSyntaxStatement(
                        $"{ReferenceManager}.AddReferencePair",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_22"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_16")));
            });
    }

    private static void AssertSyntheticInnermostSelfInvoke(SyntaxNodeAssertions node)
    {
        node.BeIfStatementSyntax(
            condition =>
            {
                condition.BePrefixUnaryExpressionSyntax(
                    SyntaxKind.ExclamationToken,
                    operand => operand.BeInvocationExpressionSyntax(
                        $"{ReferenceManager}.TryGetReference<{ATargetType}>",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_19"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_21")));
            },
            thenStatement =>
            {
                thenStatement
                    .BeBlockStatement()
                    .AsBlock()
                    .HasSyntaxNodesCount(3)
                    .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                        ATargetType,
                        "__mappa_tmp_20",
                        init => init.BeInvocationExpressionSyntax(
                            $"this.{SyntheticMethodName}",
                            arg => arg.BeIdentifierNameSyntax("__mappa_tmp_19"),
                            arg => arg.BeIdentifierNameSyntax("context"))))
                    .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                        "__mappa_tmp_21",
                        right => right.BeIdentifierNameSyntax("__mappa_tmp_20")))
                    .HasNextSyntaxNode(inner => inner.BeInvocationExpressionSyntaxStatement(
                        $"{ReferenceManager}.AddReferencePair",
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_21"),
                        arg => arg.BeIdentifierNameSyntax("__mappa_tmp_19")));
            });
    }
}