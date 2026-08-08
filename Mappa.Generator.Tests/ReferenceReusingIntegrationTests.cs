// <copyright file="ReferenceReusingIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Reflection;

using AwesomeAssertions;

using Mappa.Generator.Helpers;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Models;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <c>ReferenceReusing</c> with per-type map methods,
/// nullable cycle edges, shared-reference DAGs, and accessor scaffolding.
/// </summary>
public sealed class ReferenceReusingIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string Ns = "Mappa.Generator.Tests.UnitTests.SourceCode";
    private const string ASourceType = $"{Ns}.ASource";
    private const string ATargetType = $"{Ns}.ATarget";
    private const string BSourceType = $"{Ns}.BSource";
    private const string BTargetType = $"{Ns}.BTarget";
    private const string NodeSourceType = $"{Ns}.NodeSource";
    private const string NodeTargetType = $"{Ns}.NodeTarget";
    private const string RootSourceType = $"{Ns}.RootSource";
    private const string RootTargetType = $"{Ns}.RootTarget";
    private const string SourceType = $"{Ns}.Source";
    private const string TargetType = $"{Ns}.Target";

    private const string CycleTypes = """
                                      public class ASource
                                      {
                                          public int Id { get; set; }
                                          public BSource? Child { get; set; }
                                      }

                                      public class ATarget
                                      {
                                          public int Id { get; set; }
                                          public BTarget? Child { get; set; }
                                      }

                                      public class BSource
                                      {
                                          public int Id { get; set; }
                                          public ASource? Parent { get; set; }
                                      }

                                      public class BTarget
                                      {
                                          public int Id { get; set; }
                                          public ATarget? Parent { get; set; }
                                      }
                                      """;

    private const string DagTypes = """
                                    public class NodeSource
                                    {
                                        public int Id { get; set; }
                                    }

                                    public class NodeTarget
                                    {
                                        public int Id { get; set; }
                                    }

                                    public class RootSource
                                    {
                                        public NodeSource Left { get; set; } = null!;
                                        public NodeSource Right { get; set; } = null!;
                                    }

                                    public class RootTarget
                                    {
                                        public NodeTarget Left { get; set; } = null!;
                                        public NodeTarget Right { get; set; } = null!;
                                    }
                                    """;

    private static string ReferenceManager
        => $"{ReferenceHandlingCodeGenerator.AccessorTypeName}.{ReferenceHandlingCodeGenerator.AccessorMethodName}(context)";

    /// <summary>
    /// A↔B cycle with dedicated map methods emits TryGetReference / MapA / MapB
    /// and nullable short-circuit on cycle edges.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CycleWithPerTypeMapMethodsEmitsReuseAndNullableGuards()
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{CycleTypes}}

                           [Mappa]
                           [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                           public sealed partial class Mapper
                           {
                               public partial ATarget MapA(ASource input, MappaContext context);
                               public partial BTarget MapB(BSource input, MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveClass(ReferenceHandlingCodeGenerator.AccessorTypeName, _ => { /* presence only */ })
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
                "context",
                NullableAnnotation.NotAnnotated,
                RefKind.None,
                false,
                RefKind.None,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                AssertMapACycleSyntax)
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "MapB",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                BTargetType,
                NullableAnnotation.NotAnnotated,
                "input",
                BSourceType,
                "context",
                NullableAnnotation.NotAnnotated,
                RefKind.None,
                false,
                RefKind.None,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                AssertMapBCycleSyntax);
    }

    /// <summary>
    /// Closed A↔B cycle reuses the already-mapped A target for B.Parent.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CycleWithPerTypeMapMethodsReusesMappedReferencesAtRuntime()
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{CycleTypes}}

                           [Mappa]
                           [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                           public sealed partial class Mapper
                           {
                               public partial ATarget MapA(ASource input, MappaContext context);
                               public partial BTarget MapB(BSource input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors();

        var assembly = CompileToAssembly(generatedResults.OutputCompilation);
        var mapperType = assembly.GetType($"{Ns}.Mapper")
            ?? throw new InvalidOperationException("Mapper type was not found.");
        var mapper = Activator.CreateInstance(mapperType)
            ?? throw new InvalidOperationException("Mapper instance was not created.");
        var mapA = mapperType.GetMethod("MapA", BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException("MapA was not found.");

        var aSourceType = assembly.GetType(ASourceType)
            ?? throw new InvalidOperationException("ASource was not found.");
        var bSourceType = assembly.GetType(BSourceType)
            ?? throw new InvalidOperationException("BSource was not found.");
        var a = Activator.CreateInstance(aSourceType)
            ?? throw new InvalidOperationException("ASource instance was not created.");
        var b = Activator.CreateInstance(bSourceType)
            ?? throw new InvalidOperationException("BSource instance was not created.");
        aSourceType.GetProperty("Id")!.SetValue(a, 1);
        bSourceType.GetProperty("Id")!.SetValue(b, 2);
        aSourceType.GetProperty("Child")!.SetValue(a, b);
        bSourceType.GetProperty("Parent")!.SetValue(b, a);

        // Act
        var result = mapA.Invoke(mapper, [a, new MappaContext()]);

        // Assert
        result.Should().NotBeNull();
        var aTargetType = assembly.GetType(ATargetType)
            ?? throw new InvalidOperationException("ATarget was not found.");
        var bTargetType = assembly.GetType(BTargetType)
            ?? throw new InvalidOperationException("BTarget was not found.");
        aTargetType.GetProperty("Id")!.GetValue(result).Should().Be(1);
        var child = aTargetType.GetProperty("Child")!.GetValue(result);
        child.Should().NotBeNull().And.BeOfType(bTargetType);
        bTargetType.GetProperty("Id")!.GetValue(child).Should().Be(2);
        var parent = bTargetType.GetProperty("Parent")!.GetValue(child);
        parent.Should().BeSameAs(result);
    }

    /// <summary>
    /// Null cycle edge maps successfully without invoking the nested map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NullCycleEdgeMapsSuccessfullyWithoutRecursion()
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{CycleTypes}}

                           [Mappa]
                           [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                           public sealed partial class Mapper
                           {
                               public partial ATarget MapA(ASource input, MappaContext context);
                               public partial BTarget MapB(BSource input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors();

        var assembly = CompileToAssembly(generatedResults.OutputCompilation);
        var mapperType = assembly.GetType($"{Ns}.Mapper")
            ?? throw new InvalidOperationException("Mapper type was not found.");
        var mapper = Activator.CreateInstance(mapperType)
            ?? throw new InvalidOperationException("Mapper instance was not created.");
        var mapA = mapperType.GetMethod("MapA", BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException("MapA was not found.");

        var aSourceType = assembly.GetType(ASourceType)
            ?? throw new InvalidOperationException("ASource was not found.");
        var a = Activator.CreateInstance(aSourceType)
            ?? throw new InvalidOperationException("ASource instance was not created.");
        aSourceType.GetProperty("Id")!.SetValue(a, 11);
        aSourceType.GetProperty("Child")!.SetValue(a, null);

        // Act
        var result = mapA.Invoke(mapper, [a, new MappaContext()]);

        // Assert
        result.Should().NotBeNull();
        var aTargetType = assembly.GetType(ATargetType)
            ?? throw new InvalidOperationException("ATarget was not found.");
        aTargetType.GetProperty("Id")!.GetValue(result).Should().Be(11);
        aTargetType.GetProperty("Child")!.GetValue(result).Should().BeNull();
    }

    /// <summary>
    /// Shared source reference in a DAG maps to a single shared target instance.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SharedReferenceDagReusesMappedTargetAtRuntime()
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           {{DagTypes}}

                           [Mappa]
                           [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                           public sealed partial class Mapper
                           {
                               public partial RootTarget MapRoot(RootSource input, MappaContext context);
                               public partial NodeTarget MapNode(NodeSource input, MappaContext context);
                           }
                           #nullable restore
                           """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveClass(ReferenceHandlingCodeGenerator.AccessorTypeName, _ => { /* presence only */ })
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "MapRoot",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
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
                AssertMapRootSharedReferenceSyntax);

        var assembly = CompileToAssembly(generatedResults.OutputCompilation);
        var mapperType = assembly.GetType($"{Ns}.Mapper")
            ?? throw new InvalidOperationException("Mapper type was not found.");
        var mapper = Activator.CreateInstance(mapperType)
            ?? throw new InvalidOperationException("Mapper instance was not created.");
        var mapRoot = mapperType.GetMethod("MapRoot", BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException("MapRoot was not found.");

        var nodeSourceType = assembly.GetType(NodeSourceType)
            ?? throw new InvalidOperationException("NodeSource was not found.");
        var rootSourceType = assembly.GetType(RootSourceType)
            ?? throw new InvalidOperationException("RootSource was not found.");
        var node = Activator.CreateInstance(nodeSourceType)
            ?? throw new InvalidOperationException("NodeSource instance was not created.");
        nodeSourceType.GetProperty("Id")!.SetValue(node, 42);
        var root = Activator.CreateInstance(rootSourceType)
            ?? throw new InvalidOperationException("RootSource instance was not created.");
        rootSourceType.GetProperty("Left")!.SetValue(root, node);
        rootSourceType.GetProperty("Right")!.SetValue(root, node);

        // Act
        var result = mapRoot.Invoke(mapper, [root, new MappaContext()]);

        // Assert
        result.Should().NotBeNull();
        var rootTargetType = assembly.GetType(RootTargetType)
            ?? throw new InvalidOperationException("RootTarget was not found.");
        var left = rootTargetType.GetProperty("Left")!.GetValue(result);
        var right = rootTargetType.GetProperty("Right")!.GetValue(result);
        left.Should().NotBeNull();
        right.Should().BeSameAs(left);
        assembly.GetType(NodeTargetType)!.GetProperty("Id")!.GetValue(left).Should().Be(42);
    }

    /// <summary>
    /// Reference-handling accessor is emitted when ReferenceReusing is enabled
    /// and omitted when reference handling is not requested.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AccessorIsPresentWhenReferenceReusingEnabledAndAbsentWhenDisabled()
    {
        // Arrange / Act — enabled
        var enabledSource = $$"""
                              #nullable enable
                              using Mappa;
                              using Mappa.Attributes;

                              namespace {{Ns}};

                              public class Source { public int Value { get; set; } }
                              public class Target { public int Value { get; set; } }

                              [Mappa]
                              [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input, MappaContext context);
                              }
                              #nullable restore
                              """;
        var enabledResults = await RunMappaGeneratorAsync(enabledSource, CancellationToken.None).ConfigureAwait(true);

        // Assert — enabled
        enabledResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveClass(ReferenceHandlingCodeGenerator.AccessorTypeName, _ => { /* presence only */ })
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetType, "__mappa_tmp_3"))
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeIfStatementSyntax(
                                condition =>
                                {
                                    condition.BePrefixUnaryExpressionSyntax(
                                        SyntaxKind.ExclamationToken,
                                        operand => operand.BeInvocationExpressionSyntax(
                                            $"{ReferenceManager}.TryGetReference<{TargetType}>",
                                            arg => arg.BeIdentifierNameSyntax("input"),
                                            arg => arg.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                },
                                thenStatement =>
                                {
                                    thenStatement
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(6);
                                });
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                });

        // Arrange / Act — disabled (no ReferenceReusing / MaxRuntimeDepth)
        var disabledSource = $$"""
                               #nullable enable
                               using Mappa;
                               using Mappa.Attributes;

                               namespace {{Ns}};

                               public class Source { public int Value { get; set; } }
                               public class Target { public int Value { get; set; } }

                               [Mappa]
                               public sealed partial class Mapper
                               {
                                   public partial Target Map(Source input, MappaContext context);
                               }
                               #nullable restore
                               """;
        var disabledResults = await RunMappaGeneratorAsync(disabledSource, CancellationToken.None).ConfigureAwait(true);

        // Assert — disabled
        disabledResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors();

        AssertAccessorAbsence(disabledResults);
    }

    private static void AssertMapACycleSyntax(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(ATargetType, "__mappa_tmp_8"))
            .HasNextSyntaxNode(node =>
            {
                node.BeIfStatementSyntax(
                    condition =>
                    {
                        condition.BePrefixUnaryExpressionSyntax(
                            SyntaxKind.ExclamationToken,
                            operand => operand.BeInvocationExpressionSyntax(
                                $"{ReferenceManager}.TryGetReference<{ATargetType}>",
                                arg => arg.BeIdentifierNameSyntax("input"),
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_8")));
                    },
                    thenStatement =>
                    {
                        thenStatement
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(10)
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                ATargetType,
                                "__mappa_tmp_1",
                                init => init.BeObjectCreationExpressionSyntax(ATargetType)))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                arg => arg.BeIdentifierNameSyntax("input")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                init => init.BeMemberAccessExpressionSyntax("input.Id")))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Id"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_2")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                $"{BSourceType}?",
                                "__mappa_tmp_3",
                                init => init.BeMemberAccessExpressionSyntax("input.Child")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax($"{BTargetType}?", "__mappa_tmp_4"))
                            .HasNextSyntaxNode(n =>
                            {
                                n.BeIfStatementSyntax(
                                    condition =>
                                    {
                                        condition.BeIsPatternExpressionSyntax(
                                            expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                            pattern => pattern.BeUnaryPatternSyntax(
                                                SyntaxKind.NotKeyword,
                                                inner => inner.BeConstantPatternSyntax(null)));
                                    },
                                    nullableThen =>
                                    {
                                        nullableThen
                                            .BeBlockStatement()
                                            .AsBlock()
                                            .HasSyntaxNodesCount(4)
                                            .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                                                BSourceType,
                                                "__mappa_tmp_5",
                                                init => init.BeIdentifierNameSyntax("__mappa_tmp_3")))
                                            .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(BTargetType, "__mappa_tmp_7"))
                                            .HasNextSyntaxNode(inner =>
                                            {
                                                inner.BeIfStatementSyntax(
                                                    condition =>
                                                    {
                                                        condition.BePrefixUnaryExpressionSyntax(
                                                            SyntaxKind.ExclamationToken,
                                                            operand => operand.BeInvocationExpressionSyntax(
                                                                $"{ReferenceManager}.TryGetReference<{BTargetType}>",
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_5"),
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_7")));
                                                    },
                                                    mapThen =>
                                                    {
                                                        mapThen
                                                            .BeBlockStatement()
                                                            .AsBlock()
                                                            .HasSyntaxNodesCount(3)
                                                            .HasNextSyntaxNode(map => map.BeLocalDeclarationStatementSyntax(
                                                                BTargetType,
                                                                "__mappa_tmp_6",
                                                                init => init.BeInvocationExpressionSyntax(
                                                                    "this.MapB",
                                                                    arg => arg.BeIdentifierNameSyntax("__mappa_tmp_5"),
                                                                    arg => arg.BeIdentifierNameSyntax("context"))))
                                                            .HasNextSyntaxNode(map => map.BeAssignmentExpressionStatement(
                                                                "__mappa_tmp_7",
                                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_6")))
                                                            .HasNextSyntaxNode(map => map.BeInvocationExpressionSyntaxStatement(
                                                                $"{ReferenceManager}.AddReferencePair",
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_7"),
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_5")));
                                                    });
                                            })
                                            .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                                                "__mappa_tmp_4",
                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_7")));
                                    },
                                    nullableElse =>
                                    {
                                        nullableElse
                                            .BeBlockStatement()
                                            .AsBlock()
                                            .HasSyntaxNodesCount(1)
                                            .HasNextSyntaxNode(elseNode => elseNode.BeAssignmentExpressionStatement(
                                                left => left.BeIdentifierNameSyntax("__mappa_tmp_4"),
                                                right => right.BeCastExpressionSyntax(
                                                    $"{BTargetType}?",
                                                    expression => expression.BeLiteralExpressionSyntax(null))));
                                    });
                            })
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Child"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_4")))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                "__mappa_tmp_8",
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_1")))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_8"),
                                arg => arg.BeIdentifierNameSyntax("input")));
                    });
            })
            .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_8"));
    }

    private static void AssertMapBCycleSyntax(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(BTargetType, "__mappa_tmp_16"))
            .HasNextSyntaxNode(node =>
            {
                node.BeIfStatementSyntax(
                    condition =>
                    {
                        condition.BePrefixUnaryExpressionSyntax(
                            SyntaxKind.ExclamationToken,
                            operand => operand.BeInvocationExpressionSyntax(
                                $"{ReferenceManager}.TryGetReference<{BTargetType}>",
                                arg => arg.BeIdentifierNameSyntax("input"),
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_16")));
                    },
                    thenStatement =>
                    {
                        thenStatement
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(10)
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                BTargetType,
                                "__mappa_tmp_9",
                                init => init.BeObjectCreationExpressionSyntax(BTargetType)))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_9"),
                                arg => arg.BeIdentifierNameSyntax("input")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_10",
                                init => init.BeMemberAccessExpressionSyntax("input.Id")))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_9.Id"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_10")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                $"{ASourceType}?",
                                "__mappa_tmp_11",
                                init => init.BeMemberAccessExpressionSyntax("input.Parent")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax($"{ATargetType}?", "__mappa_tmp_12"))
                            .HasNextSyntaxNode(n =>
                            {
                                n.BeIfStatementSyntax(
                                    condition =>
                                    {
                                        condition.BeIsPatternExpressionSyntax(
                                            expression => expression.BeIdentifierNameSyntax("__mappa_tmp_11"),
                                            pattern => pattern.BeUnaryPatternSyntax(
                                                SyntaxKind.NotKeyword,
                                                inner => inner.BeConstantPatternSyntax(null)));
                                    },
                                    nullableThen =>
                                    {
                                        nullableThen
                                            .BeBlockStatement()
                                            .AsBlock()
                                            .HasSyntaxNodesCount(4)
                                            .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                                                ASourceType,
                                                "__mappa_tmp_13",
                                                init => init.BeIdentifierNameSyntax("__mappa_tmp_11")))
                                            .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(ATargetType, "__mappa_tmp_15"))
                                            .HasNextSyntaxNode(inner =>
                                            {
                                                inner.BeIfStatementSyntax(
                                                    condition =>
                                                    {
                                                        condition.BePrefixUnaryExpressionSyntax(
                                                            SyntaxKind.ExclamationToken,
                                                            operand => operand.BeInvocationExpressionSyntax(
                                                                $"{ReferenceManager}.TryGetReference<{ATargetType}>",
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_13"),
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_15")));
                                                    },
                                                    mapThen =>
                                                    {
                                                        mapThen
                                                            .BeBlockStatement()
                                                            .AsBlock()
                                                            .HasSyntaxNodesCount(3)
                                                            .HasNextSyntaxNode(map => map.BeLocalDeclarationStatementSyntax(
                                                                ATargetType,
                                                                "__mappa_tmp_14",
                                                                init => init.BeInvocationExpressionSyntax(
                                                                    "this.MapA",
                                                                    arg => arg.BeIdentifierNameSyntax("__mappa_tmp_13"),
                                                                    arg => arg.BeIdentifierNameSyntax("context"))))
                                                            .HasNextSyntaxNode(map => map.BeAssignmentExpressionStatement(
                                                                "__mappa_tmp_15",
                                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_14")))
                                                            .HasNextSyntaxNode(map => map.BeInvocationExpressionSyntaxStatement(
                                                                $"{ReferenceManager}.AddReferencePair",
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_15"),
                                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_13")));
                                                    });
                                            })
                                            .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                                                "__mappa_tmp_12",
                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_15")));
                                    },
                                    nullableElse =>
                                    {
                                        nullableElse
                                            .BeBlockStatement()
                                            .AsBlock()
                                            .HasSyntaxNodesCount(1)
                                            .HasNextSyntaxNode(elseNode => elseNode.BeAssignmentExpressionStatement(
                                                left => left.BeIdentifierNameSyntax("__mappa_tmp_12"),
                                                right => right.BeCastExpressionSyntax(
                                                    $"{ATargetType}?",
                                                    expression => expression.BeLiteralExpressionSyntax(null))));
                                    });
                            })
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_9.Parent"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_12")))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                "__mappa_tmp_16",
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_9")))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_16"),
                                arg => arg.BeIdentifierNameSyntax("input")));
                    });
            })
            .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_16"));
    }

    private static void AssertMapRootSharedReferenceSyntax(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(RootTargetType, "__mappa_tmp_8"))
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
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_8")));
                    },
                    thenStatement =>
                    {
                        thenStatement
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(12)
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                RootTargetType,
                                "__mappa_tmp_1",
                                init => init.BeObjectCreationExpressionSyntax(RootTargetType)))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                arg => arg.BeIdentifierNameSyntax("input")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                NodeSourceType,
                                "__mappa_tmp_2",
                                init => init.BeMemberAccessExpressionSyntax("input.Left")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(NodeTargetType, "__mappa_tmp_4"))
                            .HasNextSyntaxNode(n =>
                            {
                                n.BeIfStatementSyntax(
                                    condition =>
                                    {
                                        condition.BePrefixUnaryExpressionSyntax(
                                            SyntaxKind.ExclamationToken,
                                            operand => operand.BeInvocationExpressionSyntax(
                                                $"{ReferenceManager}.TryGetReference<{NodeTargetType}>",
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                    },
                                    leftThen =>
                                    {
                                        leftThen
                                            .BeBlockStatement()
                                            .AsBlock()
                                            .HasSyntaxNodesCount(3)
                                            .HasNextSyntaxNode(map => map.BeLocalDeclarationStatementSyntax(
                                                NodeTargetType,
                                                "__mappa_tmp_3",
                                                init => init.BeInvocationExpressionSyntax(
                                                    "this.MapNode",
                                                    arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    arg => arg.BeIdentifierNameSyntax("context"))))
                                            .HasNextSyntaxNode(map => map.BeAssignmentExpressionStatement(
                                                "__mappa_tmp_4",
                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_3")))
                                            .HasNextSyntaxNode(map => map.BeInvocationExpressionSyntaxStatement(
                                                $"{ReferenceManager}.AddReferencePair",
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_4"),
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                    });
                            })
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Left"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_4")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                NodeSourceType,
                                "__mappa_tmp_5",
                                init => init.BeMemberAccessExpressionSyntax("input.Right")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(NodeTargetType, "__mappa_tmp_7"))
                            .HasNextSyntaxNode(n =>
                            {
                                n.BeIfStatementSyntax(
                                    condition =>
                                    {
                                        condition.BePrefixUnaryExpressionSyntax(
                                            SyntaxKind.ExclamationToken,
                                            operand => operand.BeInvocationExpressionSyntax(
                                                $"{ReferenceManager}.TryGetReference<{NodeTargetType}>",
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_5"),
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_7")));
                                    },
                                    rightThen =>
                                    {
                                        rightThen
                                            .BeBlockStatement()
                                            .AsBlock()
                                            .HasSyntaxNodesCount(3)
                                            .HasNextSyntaxNode(map => map.BeLocalDeclarationStatementSyntax(
                                                NodeTargetType,
                                                "__mappa_tmp_6",
                                                init => init.BeInvocationExpressionSyntax(
                                                    "this.MapNode",
                                                    arg => arg.BeIdentifierNameSyntax("__mappa_tmp_5"),
                                                    arg => arg.BeIdentifierNameSyntax("context"))))
                                            .HasNextSyntaxNode(map => map.BeAssignmentExpressionStatement(
                                                "__mappa_tmp_7",
                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_6")))
                                            .HasNextSyntaxNode(map => map.BeInvocationExpressionSyntaxStatement(
                                                $"{ReferenceManager}.AddReferencePair",
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_7"),
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_5")));
                                    });
                            })
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Right"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_7")))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                "__mappa_tmp_8",
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_1")))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_8"),
                                arg => arg.BeIdentifierNameSyntax("input")));
                    });
            })
            .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_8"));
    }

    private static void AssertAccessorAbsence(GeneratedResults generatedResults)
    {
        var trees = generatedResults.Driver.GetRunResult().GeneratedTrees;
        var hasAccessor = trees.Any(tree =>
            tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Any(classDeclaration => classDeclaration.Identifier.Text.Equals(
                    ReferenceHandlingCodeGenerator.AccessorTypeName,
                    StringComparison.Ordinal)));
        hasAccessor.Should().BeFalse();
    }

    private static Assembly CompileToAssembly(Compilation compilation)
    {
        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        emitResult.Success.Should().BeTrue(string.Join(Environment.NewLine, emitResult.Diagnostics));
        return Assembly.Load(stream.ToArray());
    }
}