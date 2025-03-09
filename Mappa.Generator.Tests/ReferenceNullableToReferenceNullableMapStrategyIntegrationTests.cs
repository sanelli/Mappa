// <copyright file="ReferenceNullableToReferenceNullableMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="ReferenceNullableToReferenceNullableMapStrategy"/>.
/// </summary>
public class ReferenceNullableToReferenceNullableMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between two nullable
    /// reference types.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceNullableToReferenceNullable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target? Map(Source? input);
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
                NullableAnnotation.Annotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target?",
                                "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions =>
                                {
                                    conditionAssertions.BeIsPatternExpressionSyntax(
                                        expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                        patternAssertions => patternAssertions.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, argumentAssertions => argumentAssertions.BeConstantPatternSyntax(null)));
                                },
                                ifStatementAssertions =>
                                {
                                    ifStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(4)
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                                "__mappa_tmp_2",
                                                initAssertions => initAssertions.BeIdentifierNameSyntax("input"));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initAssertions => initAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.PropertyA"));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target?",
                                                "__mappa_tmp_4",
                                                initAssertions => initAssertions.BeObjectCreationExpressionSyntax(
                                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                    ("PropertyA", propertyInitAssertions => propertyInitAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"))));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeAssignmentExpressionStatement(
                                                leftAssertions => leftAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightAssertions =>
                                                    rightAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"));
                                        });
                                },
                                elseStatementAssertions =>
                                {
                                    elseStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeAssignmentExpressionStatement(
                                                leftAssertions => leftAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightAssertions =>
                                                    rightAssertions.BeCastExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target?", expressionAssertions => expressionAssertions.BeLiteralExpressionSyntax(null)));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created between two reference types when
    /// nullable is disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReferenceToReferenceWhenNullableIsDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int PropertyA { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
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
                NullableAnnotation.None,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.None,
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
                                        expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                        patternAssertions => patternAssertions.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, argumentAssertions => argumentAssertions.BeConstantPatternSyntax(null)));
                                },
                                ifStatementAssertions =>
                                {
                                    ifStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(4)
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                                "__mappa_tmp_2",
                                                initAssertions => initAssertions.BeIdentifierNameSyntax("input"));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initAssertions => initAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.PropertyA"));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                "__mappa_tmp_4",
                                                initAssertions => initAssertions.BeObjectCreationExpressionSyntax(
                                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                    ("PropertyA", propertyInitAssertions => propertyInitAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"))));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeAssignmentExpressionStatement(
                                                leftAssertions => leftAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightAssertions =>
                                                    rightAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"));
                                        });
                                },
                                elseStatementAssertions =>
                                {
                                    elseStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeAssignmentExpressionStatement(
                                                leftAssertions => leftAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightAssertions =>
                                                    rightAssertions.BeCastExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target", expressionAssertions => expressionAssertions.BeLiteralExpressionSyntax(null)));
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