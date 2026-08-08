// <copyright file="ReferenceHandlingDiagnosticsIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Helpers;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Diagnostic assertion coverage for reference-handling MP00074–MP00076
/// (mapping-cycle MP00077 is covered primarily in dedicated cycle tests).
/// </summary>
public sealed class ReferenceHandlingDiagnosticsIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string SourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.Source";
    private const string TargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";
    private const string InnerSourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource";
    private const string InnerTargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget";
    private const string Level2SourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.Level2Source";
    private const string Level2TargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.Level2Target";
    private const string Level0SourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.Level0Source";
    private const string Level0TargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.Level0Target";

    /// <summary>
    /// MP00074 when <c>ReferenceReusing</c> is enabled on a root map without <see cref="MappaContext"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingWithoutMappaContextReportsRootWarning()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ReferenceHandlingRootMapWithoutMappaContext, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertSimpleIntValueMapWithoutReferenceHandling);
    }

    /// <summary>
    /// MP00074 when <c>MaxRuntimeDepth</c> is set on a root map without <see cref="MappaContext"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxRuntimeDepthWithoutMappaContextReportsRootWarning()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(MaxRuntimeDepth = 3)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ReferenceHandlingRootMapWithoutMappaContext, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertSimpleIntValueMapWithoutReferenceHandling);
    }

    /// <summary>
    /// MP00075 when an invoked map method lacks <see cref="MappaContext"/> while reference handling is requested.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NestedMapWithoutMappaContextReportsWarning()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

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
                                  public sealed partial class Mapper
                                  {
                                      public InnerTarget MapInner(InnerSource input)
                                      {
                                          return new InnerTarget() { Value = input.Value };
                                      }

                                      [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                                      public partial Target Map(Source input, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        var referenceManager = $"{ReferenceHandlingCodeGenerator.AccessorTypeName}.{ReferenceHandlingCodeGenerator.AccessorMethodName}(context)";
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ReferenceHandlingNestedMapWithoutMappaContext, "MapInner")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(TargetType, "__mappa_tmp_5");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions =>
                                {
                                    conditionAssertions.BePrefixUnaryExpressionSyntax(
                                        SyntaxKind.ExclamationToken,
                                        operandAssertions => operandAssertions.BeInvocationExpressionSyntax(
                                            $"{referenceManager}.TryGetReference<{TargetType}>",
                                            argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input"),
                                            argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_5")));
                                },
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(8)
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
                                                $"{referenceManager}.AddReferencePair",
                                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input"));
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                InnerSourceType,
                                                "__mappa_tmp_2",
                                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Child"));
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(InnerTargetType, "__mappa_tmp_4");
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeIfStatementSyntax(
                                                conditionAssertions =>
                                                {
                                                    conditionAssertions.BePrefixUnaryExpressionSyntax(
                                                        SyntaxKind.ExclamationToken,
                                                        operandAssertions => operandAssertions.BeInvocationExpressionSyntax(
                                                            $"{referenceManager}.TryGetReference<{InnerTargetType}>",
                                                            argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                            argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                                },
                                                nestedThenAssertions =>
                                                {
                                                    nestedThenAssertions
                                                        .BeBlockStatement()
                                                        .AsBlock()
                                                        .HasSyntaxNodesCount(3)
                                                        .HasNextSyntaxNode(nestedSyntaxNodeAssertions =>
                                                        {
                                                            nestedSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                                InnerTargetType,
                                                                "__mappa_tmp_3",
                                                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                                                    "this.MapInner",
                                                                    argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                                        })
                                                        .HasNextSyntaxNode(nestedSyntaxNodeAssertions =>
                                                        {
                                                            nestedSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                                                "__mappa_tmp_4",
                                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                                                        })
                                                        .HasNextSyntaxNode(nestedSyntaxNodeAssertions =>
                                                        {
                                                            nestedSyntaxNodeAssertions.BeInvocationExpressionSyntaxStatement(
                                                                $"{referenceManager}.AddReferencePair",
                                                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"),
                                                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                                        });
                                                });
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Child"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"));
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                                "__mappa_tmp_5",
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"{referenceManager}.AddReferencePair",
                                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_5"),
                                                argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("input"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_5");
                        });
                });
    }

    /// <summary>
    /// MP00076 when strategy discovery exceeds <c>MaxCompileTimeDepth</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MaxCompileTimeDepthExceededReportsError()
    {
        // Arrange
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
                                      [MappaSettings(MaxCompileTimeDepth = 1)]
                                      public partial Level0Target Map(Level0Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement());
                });
    }

    /// <summary>
    /// Nested map without <see cref="MappaContext"/> does not emit MP00075 when the root
    /// also lacks context (early return in <c>MaybeReportNestedMapWithoutMappaContext</c>).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NestedMapWithoutContextDoesNotWarnWhenRootAlsoLacksContext()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

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
                                  public sealed partial class Mapper
                                  {
                                      public InnerTarget MapInner(InnerSource input)
                                      {
                                          return new InnerTarget() { Value = input.Value };
                                      }

                                      [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert — only root-without-context (MP00074); nested-map warning (MP00075) is skipped.
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ReferenceHandlingRootMapWithoutMappaContext, "Map")
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                InnerSourceType,
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Child"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                InnerTargetType,
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeInvocationExpressionSyntax(
                                        "this.MapInner",
                                        parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetType,
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetType,
                                        ("Child", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Reference handling on C# 11 reports UnsafeAccessorNotSupported and does not apply reuse.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceReusingOnCSharp11ReportsUnsafeAccessorNotSupported()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                                      public partial Target Map(Source input, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, LanguageVersion.CSharp11, CancellationToken.None)
            .ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.UnsafeAccessorNotSupported, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                AssertSimpleIntValueMapWithoutReferenceHandling);
    }

    /// <summary>
    /// Reference handling is rejected on <see cref="System.Linq.IQueryable{T}"/> projection methods.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReferenceHandlingOnProjectionMethodIsRejected()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

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
                                      [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
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

    private static void AssertSimpleIntValueMapWithoutReferenceHandling(BlockSyntaxAssertions blockSyntaxAssertions)
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
}