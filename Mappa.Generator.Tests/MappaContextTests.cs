// <copyright file="MappaContextTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

// TODO [#55] Add test to check can use method with MappaContext from a dependency.

/// <summary>
/// Tests for maps using <see cref="MappaContext"/>.
/// </summary>
public sealed class MappaContextTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created when the second
    /// parameter is a <see cref="MappaContext"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapAMethodWithMappaContext()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  internal sealed partial class Mapper
                                  {
                                      internal partial long Map(int input, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveCommentHeader()
            .HaveDefaultMapMethodWithContext(
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                        }));
                });
    }

    /// <summary>
    /// Test a mapping can use an existing method
    /// defined by the user with a <see cref="MappaContext"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInvokeOtherMapperRequiringContext()
    {
        // TODO [#55] Fix failing tests.

        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int B { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public InnerTarget CustomMapInner(InnerSource input, MappaContext context)
                                      {
                                          return new InnerTarget() { B = input.A };
                                      }
                                      
                                      public partial Target Map(Source input, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions =>
                                {
                                    conditionAssertions.BeIsPatternExpressionSyntax(
                                        identifierAssertions => identifierAssertions.BeIdentifierNameSyntax("input"),
                                        patternAssertions => patternAssertions.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, argumentAssertions => argumentAssertions.BeConstantPatternSyntax(null)));
                                },
                                ifStatementAssertions =>
                                {
                                    ifStatementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(5)
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                                "__mappa_tmp_2",
                                                initializationAssertions => initializationAssertions.BeIdentifierNameSyntax("input"));
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                                "__mappa_tmp_3",
                                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Property"));
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                                "__mappa_tmp_4",
                                                initializationAssertions =>
                                                {
                                                    initializationAssertions.BeInvocationExpressionSyntax(
                                                        "this.Map",
                                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                                                });
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                "__mappa_tmp_5",
                                                initializationAssertions =>
                                                {
                                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                                });
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_5"));
                                        });
                                },
                                elseStatementAssertions =>
                                {
                                    elseStatementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(elseSyntaxNodeAssertions =>
                                        {
                                            elseSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeCastExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target", assertions => assertions.BeLiteralExpressionSyntax(null)));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }
}