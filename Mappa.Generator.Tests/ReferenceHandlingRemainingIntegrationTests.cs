// <copyright file="ReferenceHandlingRemainingIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Reflection;

using AwesomeAssertions;

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Helpers;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Models;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Remaining reference-handling integration coverage: mixed context warnings,
/// projection rejection for MaxRuntimeDepth, and ReferenceReusing + MaxRuntimeDepth together.
/// </summary>
public sealed class ReferenceHandlingRemainingIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string Ns = "Mappa.Generator.Tests.UnitTests.SourceCode";
    private const string SourceType = $"{Ns}.Source";
    private const string TargetType = $"{Ns}.Target";
    private const string InnerSourceType = $"{Ns}.InnerSource";
    private const string InnerTargetType = $"{Ns}.InnerTarget";
    private const string Level1SourceType = $"{Ns}.Level1Source";
    private const string Level1TargetType = $"{Ns}.Level1Target";
    private const string Level0SourceType = $"{Ns}.Level0Source";
    private const string Level0TargetType = $"{Ns}.Level0Target";

    private static string ReferenceManager
        => $"{ReferenceHandlingCodeGenerator.AccessorTypeName}.{ReferenceHandlingCodeGenerator.AccessorMethodName}(context)";

    /// <summary>
    /// Same mapper reports both root-without-context (MP00074) and nested-map-without-context (MP00075).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MixedContextRootAndNestedWarningsInSameMapper()
    {
        // Arrange — distinct source/target pairs (MP00005 forbids duplicate type-pair maps).
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           public class FlatSource
                           {
                               public int Value { get; set; }
                           }

                           public class FlatTarget
                           {
                               public int Value { get; set; }
                           }

                           public class InnerSource
                           {
                               public int Value { get; set; }
                           }

                           public class InnerTarget
                           {
                               public int Value { get; set; }
                           }

                           public class Source
                           {
                               public InnerSource Child { get; set; } = null!;
                           }

                           public class Target
                           {
                               public InnerTarget Child { get; set; } = null!;
                           }

                           [Mappa]
                           [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                           public sealed partial class Mapper
                           {
                               public InnerTarget MapInner(InnerSource input)
                               {
                                   return new InnerTarget() { Value = input.Value };
                               }

                               public partial FlatTarget MapWithoutContext(FlatSource input);

                               public partial Target MapWithContext(Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ReferenceHandlingRootMapWithoutMappaContext, "MapWithoutContext")
            .HaveDiagnostic(MappaDiagnosticDescriptors.ReferenceHandlingNestedMapWithoutMappaContext, "MapInner")
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "MapWithoutContext",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                $"{Ns}.FlatTarget",
                NullableAnnotation.NotAnnotated,
                "input",
                $"{Ns}.FlatSource",
                null,
                NullableAnnotation.NotAnnotated,
                RefKind.None,
                false,
                RefKind.None,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                AssertFlatMapWithoutReferenceHandling)
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "MapWithContext",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                TargetType,
                NullableAnnotation.NotAnnotated,
                "input",
                SourceType,
                "context",
                NullableAnnotation.NotAnnotated,
                RefKind.None,
                false,
                RefKind.None,
                2,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                AssertMapWithContextCallsMapInner);
    }

    /// <summary>
    /// <c>MaxRuntimeDepth</c> on an <see cref="System.Linq.IQueryable{T}"/> projection is rejected.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxRuntimeDepthOnProjectionMethodIsRejected()
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using System.Linq;
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           public class Order
                           {
                               public int Id { get; set; }
                           }

                           public class OrderDto
                           {
                               public int Id { get; set; }
                           }

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings(MaxRuntimeDepth = 3)]
                               public partial IQueryable<OrderDto> ProjectToDto(IQueryable<Order> query);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMappingNotSupported, "ProjectToDto", "reference handling")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// ReferenceReusing and MaxRuntimeDepth together emit MaxDepth init, TryGetReference,
    /// and IncreaseDepth around nested map-method calls.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingAndMaxRuntimeDepthTogether()
    {
        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace {{Ns}};

                           public class Level1Source
                           {
                               public int Value { get; set; }
                           }

                           public class Level1Target
                           {
                               public int Value { get; set; }
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
                           [MappaSettings(ReferenceReusing = BooleanSetting.Enable, MaxRuntimeDepth = 2)]
                           public sealed partial class Mapper
                           {
                               public partial Level0Target Map(Level0Source input, MappaContext context);
                               public partial Level1Target MapLevel1(Level1Source input, MappaContext context);
                           }
                           #nullable restore
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert syntax
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveClass(ReferenceHandlingCodeGenerator.AccessorTypeName, _ => { /* presence */ })
            .HaveDefaultMapMethodWithContext(
                Level0TargetType,
                NullableAnnotation.NotAnnotated,
                Level0SourceType,
                NullableAnnotation.NotAnnotated,
                2,
                AssertMapWithReferenceReusingAndMaxRuntimeDepth);

        // Assert runtime succeeds within depth
        var assembly = CompileToAssembly(generatedResults.OutputCompilation);
        var mapperType = assembly.GetType($"{Ns}.Mapper")
            ?? throw new InvalidOperationException("Mapper type was not found.");
        var mapper = Activator.CreateInstance(mapperType)
            ?? throw new InvalidOperationException("Mapper instance was not created.");
        var map = mapperType.GetMethod("Map", BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException("Map was not found.");

        var level0SourceType = assembly.GetType(Level0SourceType)
            ?? throw new InvalidOperationException("Level0Source was not found.");
        var level1SourceType = assembly.GetType(Level1SourceType)
            ?? throw new InvalidOperationException("Level1Source was not found.");
        var level1 = Activator.CreateInstance(level1SourceType)
            ?? throw new InvalidOperationException("Level1Source instance was not created.");
        level1SourceType.GetProperty("Value")!.SetValue(level1, 99);
        var level0 = Activator.CreateInstance(level0SourceType)
            ?? throw new InvalidOperationException("Level0Source instance was not created.");
        level0SourceType.GetProperty("Child")!.SetValue(level0, level1);

        var result = map.Invoke(mapper, [level0, new MappaContext()]);
        result.Should().NotBeNull();
        var level0TargetType = assembly.GetType(Level0TargetType)
            ?? throw new InvalidOperationException("Level0Target was not found.");
        var child = level0TargetType.GetProperty("Child")!.GetValue(result);
        child.Should().NotBeNull();
        assembly.GetType(Level1TargetType)!.GetProperty("Value")!.GetValue(child).Should().Be(99);
    }

    private static void AssertFlatMapWithoutReferenceHandling(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                typeof(int).ToString(),
                "__mappa_tmp_1",
                init => init.BeMemberAccessExpressionSyntax("input.Value")))
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                $"{Ns}.FlatTarget",
                "__mappa_tmp_2",
                init => init.BeObjectCreationExpressionSyntax(
                    $"{Ns}.FlatTarget",
                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")))))
            .HasNextSyntaxNode(node => node.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2")));
    }

    private static void AssertMapWithContextCallsMapInner(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        // Temps continue after MapWithoutContext (shared counter): start at __mappa_tmp_3.
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetType, "__mappa_tmp_7"))
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
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_7")));
                    },
                    thenStatement =>
                    {
                        thenStatement
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(8)
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                TargetType,
                                "__mappa_tmp_3",
                                init => init.BeObjectCreationExpressionSyntax(TargetType)))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                arg => arg.BeIdentifierNameSyntax("input")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                InnerSourceType,
                                "__mappa_tmp_4",
                                init => init.BeMemberAccessExpressionSyntax("input.Child")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(InnerTargetType, "__mappa_tmp_6"))
                            .HasNextSyntaxNode(n =>
                            {
                                n.BeIfStatementSyntax(
                                    condition =>
                                    {
                                        condition.BePrefixUnaryExpressionSyntax(
                                            SyntaxKind.ExclamationToken,
                                            operand => operand.BeInvocationExpressionSyntax(
                                                $"{ReferenceManager}.TryGetReference<{InnerTargetType}>",
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_4"),
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_6")));
                                    },
                                    nestedThen =>
                                    {
                                        nestedThen
                                            .BeBlockStatement()
                                            .AsBlock()
                                            .HasSyntaxNodesCount(3)
                                            .HasNextSyntaxNode(inner => inner.BeLocalDeclarationStatementSyntax(
                                                InnerTargetType,
                                                "__mappa_tmp_5",
                                                init => init.BeInvocationExpressionSyntax(
                                                    "this.MapInner",
                                                    arg => arg.BeIdentifierNameSyntax("__mappa_tmp_4"))))
                                            .HasNextSyntaxNode(inner => inner.BeAssignmentExpressionStatement(
                                                "__mappa_tmp_6",
                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_5")))
                                            .HasNextSyntaxNode(inner => inner.BeInvocationExpressionSyntaxStatement(
                                                $"{ReferenceManager}.AddReferencePair",
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_6"),
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                    });
                            })
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Child"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_6")))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                "__mappa_tmp_7",
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_3")))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_7"),
                                arg => arg.BeIdentifierNameSyntax("input")));
                    });
            })
            .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_7"));
    }

    private static void AssertMapWithReferenceReusingAndMaxRuntimeDepth(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(4)
            .HasNextSyntaxNode(node =>
            {
                node.BeAssignmentExpressionStatement(
                    left => left.BeMemberAccessExpressionSyntax($"{ReferenceManager}.MaxDepth"),
                    right => right.BeLiteralExpressionSyntax(2));
            })
            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(Level0TargetType, "__mappa_tmp_5"))
            .HasNextSyntaxNode(node =>
            {
                node.BeIfStatementSyntax(
                    condition =>
                    {
                        condition.BePrefixUnaryExpressionSyntax(
                            SyntaxKind.ExclamationToken,
                            operand => operand.BeInvocationExpressionSyntax(
                                $"{ReferenceManager}.TryGetReference<{Level0TargetType}>",
                                arg => arg.BeIdentifierNameSyntax("input"),
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_5")));
                    },
                    thenStatement =>
                    {
                        thenStatement
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(8)
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                Level0TargetType,
                                "__mappa_tmp_1",
                                init => init.BeObjectCreationExpressionSyntax(Level0TargetType)))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                arg => arg.BeIdentifierNameSyntax("input")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(
                                Level1SourceType,
                                "__mappa_tmp_2",
                                init => init.BeMemberAccessExpressionSyntax("input.Child")))
                            .HasNextSyntaxNode(n => n.BeLocalDeclarationStatementSyntax(Level1TargetType, "__mappa_tmp_4"))
                            .HasNextSyntaxNode(n =>
                            {
                                n.BeIfStatementSyntax(
                                    condition =>
                                    {
                                        condition.BePrefixUnaryExpressionSyntax(
                                            SyntaxKind.ExclamationToken,
                                            operand => operand.BeInvocationExpressionSyntax(
                                                $"{ReferenceManager}.TryGetReference<{Level1TargetType}>",
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                    },
                                    nestedThen =>
                                    {
                                        nestedThen
                                            .BeBlockStatement()
                                            .AsBlock()
                                            .HasSyntaxNodesCount(2)
                                            .HasNextSyntaxNode(inner =>
                                            {
                                                inner.BeUsingStatementSyntax(
                                                    expression => expression.BeInvocationExpressionSyntax($"{ReferenceManager}.IncreaseDepth"),
                                                    usingBody =>
                                                    {
                                                        usingBody
                                                            .BeBlockStatement()
                                                            .AsBlock()
                                                            .HasSyntaxNodesCount(2)
                                                            .HasNextSyntaxNode(map => map.BeLocalDeclarationStatementSyntax(
                                                                Level1TargetType,
                                                                "__mappa_tmp_3",
                                                                init => init.BeInvocationExpressionSyntax(
                                                                    "this.MapLevel1",
                                                                    arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                                    arg => arg.BeIdentifierNameSyntax("context"))))
                                                            .HasNextSyntaxNode(map => map.BeAssignmentExpressionStatement(
                                                                "__mappa_tmp_4",
                                                                right => right.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                                    });
                                            })
                                            .HasNextSyntaxNode(inner => inner.BeInvocationExpressionSyntaxStatement(
                                                $"{ReferenceManager}.AddReferencePair",
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_4"),
                                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                    });
                            })
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Child"),
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_4")))
                            .HasNextSyntaxNode(n => n.BeAssignmentExpressionStatement(
                                "__mappa_tmp_5",
                                right => right.BeIdentifierNameSyntax("__mappa_tmp_1")))
                            .HasNextSyntaxNode(n => n.BeInvocationExpressionSyntaxStatement(
                                $"{ReferenceManager}.AddReferencePair",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_5"),
                                arg => arg.BeIdentifierNameSyntax("input")));
                    });
            })
            .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_5"));
    }

    private static Assembly CompileToAssembly(Compilation compilation)
    {
        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        emitResult.Success.Should().BeTrue(string.Join(Environment.NewLine, emitResult.Diagnostics));
        return Assembly.Load(stream.ToArray());
    }
}