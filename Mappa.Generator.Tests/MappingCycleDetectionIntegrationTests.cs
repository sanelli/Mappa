// <copyright file="MappingCycleDetectionIntegrationTests.cs" company="Stefano Anelli">
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
/// Integration tests for compile-time mapping-cycle detection on <c>GetStrategy</c>.
/// Covers default / <c>BreakCompileTimeCycles</c> Disable regressions (MP00077) as well as
/// sibling and distinct type-pair non-cycle cases.
/// </summary>
public sealed class MappingCycleDetectionIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string Ns = "Mappa.Generator.Tests.UnitTests.SourceCode";
    private const string RootSourceType = $"{Ns}.RootSource";
    private const string RootTargetType = $"{Ns}.RootTarget";
    private const string ASourceType = $"{Ns}.ASource";
    private const string ATargetType = $"{Ns}.ATarget";
    private const string BSourceType = $"{Ns}.BSource";
    private const string BTargetType = $"{Ns}.BTarget";
    private const string NestedSourceType = $"{Ns}.NestedSource";
    private const string NestedTargetType = $"{Ns}.NestedTarget";
    private const string SourceType = $"{Ns}.Source";
    private const string TargetType = $"{Ns}.Target";
    private const string Level0SourceType = $"{Ns}.Level0Source";
    private const string Level0TargetType = $"{Ns}.Level0Target";
    private const string Level1SourceType = $"{Ns}.Level1Source";
    private const string Level1TargetType = $"{Ns}.Level1Target";
    private const string Level2SourceType = $"{Ns}.Level2Source";
    private const string Level2TargetType = $"{Ns}.Level2Target";

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

    private const string ManagerLocal = "__mappa_tmp_1";

    private static string AccessorGetReferenceManager
        => $"{ReferenceHandlingCodeGenerator.AccessorTypeName}.{ReferenceHandlingCodeGenerator.AccessorMethodName}";

    /// <summary>
    /// Mutual A↔B nesting without a map method for the cycling pair reports MP00077
    /// when <c>BreakCompileTimeCycles</c> is unset (effective off).
    /// Nested discovery uses <c>TypeMapIdentifierWithMapMethodAlgorithm</c> under the root map.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MutualNestingWithoutMapMethodReportsMappingCycle()
    {
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{MutualCycleTypes}}

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               public partial RootTarget Map(RootSource input);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertMappingCycle(generatedResults, ASourceType, ATargetType, RootTargetType, RootSourceType);
    }

    /// <summary>
    /// Explicit <c>BreakCompileTimeCycles = Disable</c> keeps reporting MP00077 and does not synthesize a map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MutualNestingWithBreakCompileTimeCyclesDisableReportsMappingCycle()
    {
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{MutualCycleTypes}}

                           [Mappa]
                           [MappaSettings(BreakCompileTimeCycles = BooleanSetting.Disable)]
                           public sealed partial class Mapper
                           {
                               public partial RootTarget Map(RootSource input);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertMappingCycle(generatedResults, ASourceType, ATargetType, RootTargetType, RootSourceType);
    }

    /// <summary>
    /// An explicit map method for the cycling pair breaks the compile-time cycle.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ExplicitMapMethodBreaksMappingCycle()
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

                               public partial BTarget MapB(BSource input);

                               public partial RootTarget Map(RootSource input);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "MapA",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                ATargetType,
                NullableAnnotation.NotAnnotated,
                "input",
                ASourceType,
                NullableAnnotation.NotAnnotated,
                3,
                AssertMapAInvokesMapB);
    }

    /// <summary>
    /// Sibling properties of the same nested type do not report a false-positive cycle.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SiblingNestedPropertiesDoNotReportMappingCycle()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

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

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

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
    /// Deep non-repeating type pairs succeed (cycle is re-entry, not mere depth).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DistinctTypePairsWithoutReentrySucceed()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

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

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Level0Target Map(Level0Source input);
                                  }
                                  #nullable restore
                                  """;

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
    /// ReferenceReusing + MappaContext does not suppress compile-time cycle detection when
    /// <c>BreakCompileTimeCycles</c> is unset (effective off) and the generator would still
    /// recurse forever building an inline strategy.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingWithoutExplicitMapMethodStillReportsCompileTimeCycle()
    {
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{MutualCycleTypes}}

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                               public partial RootTarget Map(RootSource input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertReferenceReusingMappingCycle(generatedResults);
    }

    /// <summary>
    /// ReferenceReusing with explicit <c>BreakCompileTimeCycles = Disable</c> still reports MP00077.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingWithBreakCompileTimeCyclesDisableStillReportsCompileTimeCycle()
    {
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{MutualCycleTypes}}

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings(
                                   ReferenceReusing = BooleanSetting.Enable,
                                   BreakCompileTimeCycles = BooleanSetting.Disable)]
                               public partial RootTarget Map(RootSource input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        AssertReferenceReusingMappingCycle(generatedResults);
    }

    private static void AssertMappingCycle(
        GeneratedResults generatedResults,
        string cycleSourceType,
        string cycleTargetType,
        string mapReturnType,
        string mapParameterType)
    {
        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappingCycleDetected, cycleSourceType, cycleTargetType)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, BTargetType, "Child")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                mapReturnType,
                NullableAnnotation.NotAnnotated,
                mapParameterType,
                NullableAnnotation.NotAnnotated,
                AssertRootMapOmittingCyclicBackEdge);
    }

    private static void AssertReferenceReusingMappingCycle(GeneratedResults generatedResults)
    {
        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappingCycleDetected, ASourceType, ATargetType)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, BTargetType, "Child")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                RootTargetType,
                NullableAnnotation.NotAnnotated,
                RootSourceType,
                NullableAnnotation.NotAnnotated,
                AssertRootReferenceReusingOmittingCyclicBackEdge);
    }

    private static void AssertMapAInvokesMapB(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(5)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_1",
                init => init.BeMemberAccessExpressionSyntax("input.Id")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                BSourceType,
                "__mappa_tmp_2",
                init => init.BeMemberAccessExpressionSyntax("input.Child")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                BTargetType,
                "__mappa_tmp_3",
                init => init.BeInvocationExpressionSyntax(
                    "this.MapB",
                    argument => argument.BeIdentifierNameSyntax("__mappa_tmp_2"))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                ATargetType,
                "__mappa_tmp_4",
                init => init.BeObjectCreationExpressionSyntax(
                    ATargetType,
                    ("Id", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")),
                    ("Child", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")))))
            .HasNextSyntaxNode(node => node.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_4")));
    }

    private static void AssertRootMapOmittingCyclicBackEdge(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        // B.Child (A→A) hits the cycle and is omitted; A.Child (B) is still mapped.
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

    private static void AssertRootReferenceReusingOmittingCyclicBackEdge(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(4)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                "global::Mappa.MappaReferenceManager",
                ManagerLocal,
                init => init.BeInvocationExpressionSyntax(
                    AccessorGetReferenceManager,
                    arg => arg.BeIdentifierNameSyntax("context"))))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(RootTargetType, "__mappa_tmp_11"))
            .HasNextSyntaxNode(node =>
            {
                node.BeIfStatementSyntax(
                    condition =>
                    {
                        condition.BePrefixUnaryExpressionSyntax(
                            SyntaxKind.ExclamationToken,
                            operand => operand.BeInvocationExpressionSyntax(
                                $"{ManagerLocal}.TryGetReference<{RootTargetType}>",
                                arg => arg.BeIdentifierNameSyntax("input"),
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_11")));
                    },
                    thenStatement =>
                    {
                        thenStatement
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(7)
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                RootTargetType,
                                "__mappa_tmp_2",
                                init => init.BeObjectCreationExpressionSyntax(RootTargetType)))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ManagerLocal}.AddReferencePair<{RootTargetType}, {RootSourceType}>",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                arg => arg.BeIdentifierNameSyntax("input")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                ASourceType,
                                "__mappa_tmp_3",
                                init => init.BeMemberAccessExpressionSyntax("input.Child")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(ATargetType, "__mappa_tmp_10"))
                            .HasNextSyntaxNode(n =>
                            {
                                n.BeIfStatementSyntax(
                                    condition =>
                                    {
                                        condition.BePrefixUnaryExpressionSyntax(
                                            SyntaxKind.ExclamationToken,
                                            operand => operand.BeInvocationExpressionSyntax(
                                                $"{ManagerLocal}.TryGetReference<{ATargetType}>",
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_10")));
                                    },
                                    nestedThen =>
                                    {
                                        nestedThen
                                            .BeBlockStatement()
                                            .AsBlock()
                                            .HasSyntaxNodesCount(9)
                                            .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                                                ATargetType,
                                                "__mappa_tmp_4",
                                                init => init.BeObjectCreationExpressionSyntax(ATargetType)))
                                            .HasNextSyntaxNode(inner => inner.BeInvocationExpressionSyntaxStatement(
                                                $"{ManagerLocal}.AddReferencePair<{ATargetType}, {ASourceType}>",
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_4"),
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_3")))
                                            .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_5",
                                                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Id")))
                                            .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_4.Id"),
                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_5")))
                                            .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                                                BSourceType,
                                                "__mappa_tmp_6",
                                                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Child")))
                                            .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(BTargetType, "__mappa_tmp_9"))
                                            .HasNextSyntaxNode(inner =>
                                            {
                                                inner.BeIfStatementSyntax(
                                                    condition =>
                                                    {
                                                        condition.BePrefixUnaryExpressionSyntax(
                                                            SyntaxKind.ExclamationToken,
                                                            operand => operand.BeInvocationExpressionSyntax(
                                                                $"{ManagerLocal}.TryGetReference<{BTargetType}>",
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_6"),
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_9")));
                                                    },
                                                    bThen =>
                                                    {
                                                        // Cycle omits B.Child (A→A); only Id is mapped.
                                                        bThen
                                                            .BeBlockStatement()
                                                            .AsBlock()
                                                            .HasSyntaxNodesCount(5)
                                                            .HasNextSyntaxNode(b => b.BeLocalDeclarationStatementSyntax(
                                                                BTargetType,
                                                                "__mappa_tmp_7",
                                                                init => init.BeObjectCreationExpressionSyntax(BTargetType)))
                                                            .HasNextSyntaxNode(b => b.BeInvocationExpressionSyntaxStatement(
                                                                $"{ManagerLocal}.AddReferencePair<{BTargetType}, {BSourceType}>",
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_7"),
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_6")))
                                                            .HasNextSyntaxNode(b => b.BeLocalDeclarationStatementSyntax(
                                                                typeof(int).ToString(),
                                                                "__mappa_tmp_8",
                                                                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_6.Id")))
                                                            .HasNextSyntaxNode(b => b.BeAssignmentExpressionStatement(
                                                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_7.Id"),
                                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_8")))
                                                            .HasNextSyntaxNode(b => b.BeAssignmentExpressionStatement(
                                                                "__mappa_tmp_9",
                                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_7")));
                                                    });
                                            })
                                            .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_4.Child"),
                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_9")))
                                            .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                                                "__mappa_tmp_10",
                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                    });
                            })
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Child"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_10")))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                "__mappa_tmp_11",
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_2")));
                    });
            })
            .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_11"));
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
}