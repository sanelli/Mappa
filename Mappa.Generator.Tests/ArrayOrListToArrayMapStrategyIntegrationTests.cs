// <copyright file="ArrayOrListToArrayMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="ArrayOrListToArrayMapStrategy"/>.
/// </summary>
public class ArrayOrListToArrayMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between two arrays.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapArrayToArray()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial long[] Map(int[] input);
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
                typeof(long[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int[]).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_1", expressionSyntaxAssertions =>
                            {
                                expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("input.Length");
                            });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(long[]).ToString(), "__mappa_tmp_2", expressionSyntaxAssertions =>
                            {
                                expressionSyntaxAssertions.BeArrayCreationExpressionSyntax(typeof(long).ToString(), sizeAssertion =>
                                {
                                    sizeAssertion.BeIdentifierNameSyntax("__mappa_tmp_1");
                                });
                            });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions =>
                                {
                                    declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_3", 0);
                                },
                                conditionAssertion =>
                                {
                                    conditionAssertion.BeBinaryExpressionSyntax(
                                        leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                        SyntaxKind.LessThanToken,
                                        rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                },
                                incrementorAssertions =>
                                {
                                    incrementorAssertions.BePrefixUnaryExpressionSyntax(
                                        SyntaxKind.PlusPlusToken,
                                        operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                                },
                                statementSyntaxBaseAssertions =>
                                {
                                    statementSyntaxBaseAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(forStatement =>
                                        {
                                            forStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                expressionAssertions =>
                                                {
                                                    expressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_3");
                                                });
                                        })
                                        .HasNextSyntaxNode(forStatement =>
                                        {
                                            forStatement.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions =>
                                                {
                                                    leftExpressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3");
                                                },
                                                rightExpressionAssertions =>
                                                {
                                                    rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4");
                                                });
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created between an
    /// <see cref="IList{T}"/> and an array.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIListToArray()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial long[] Map(IList<int> input);
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
                typeof(long[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IList<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_1", expressionSyntaxAssertions =>
                            {
                                expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("input.Count");
                            });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(long[]).ToString(), "__mappa_tmp_2", expressionSyntaxAssertions =>
                            {
                                expressionSyntaxAssertions.BeArrayCreationExpressionSyntax(typeof(long).ToString(), sizeAssertion =>
                                {
                                    sizeAssertion.BeIdentifierNameSyntax("__mappa_tmp_1");
                                });
                            });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions =>
                                {
                                    declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_3", 0);
                                },
                                conditionAssertion =>
                                {
                                    conditionAssertion.BeBinaryExpressionSyntax(
                                        leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                        SyntaxKind.LessThanToken,
                                        rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                },
                                incrementorAssertions =>
                                {
                                    incrementorAssertions.BePrefixUnaryExpressionSyntax(
                                        SyntaxKind.PlusPlusToken,
                                        operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                                },
                                statementSyntaxBaseAssertions =>
                                {
                                    statementSyntaxBaseAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(forStatement =>
                                        {
                                            forStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                expressionAssertions =>
                                                {
                                                    expressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_3");
                                                });
                                        })
                                        .HasNextSyntaxNode(forStatement =>
                                        {
                                            forStatement.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions =>
                                                {
                                                    leftExpressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3");
                                                },
                                                rightExpressionAssertions =>
                                                {
                                                    rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4");
                                                });
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created between an
    /// <see cref="List{T}"/> and an array.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapListToArray()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial long[] Map(List<int> input);
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
                typeof(long[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(List<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_1", expressionSyntaxAssertions =>
                            {
                                expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("input.Count");
                            });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(long[]).ToString(), "__mappa_tmp_2", expressionSyntaxAssertions =>
                            {
                                expressionSyntaxAssertions.BeArrayCreationExpressionSyntax(typeof(long).ToString(), sizeAssertion =>
                                {
                                    sizeAssertion.BeIdentifierNameSyntax("__mappa_tmp_1");
                                });
                            });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions =>
                                {
                                    declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_3", 0);
                                },
                                conditionAssertion =>
                                {
                                    conditionAssertion.BeBinaryExpressionSyntax(
                                        leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                        SyntaxKind.LessThanToken,
                                        rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                },
                                incrementorAssertions =>
                                {
                                    incrementorAssertions.BePrefixUnaryExpressionSyntax(
                                        SyntaxKind.PlusPlusToken,
                                        operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                                },
                                statementSyntaxBaseAssertions =>
                                {
                                    statementSyntaxBaseAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(forStatement =>
                                        {
                                            forStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                expressionAssertions =>
                                                {
                                                    expressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_3");
                                                });
                                        })
                                        .HasNextSyntaxNode(forStatement =>
                                        {
                                            forStatement.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions =>
                                                {
                                                    leftExpressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3");
                                                },
                                                rightExpressionAssertions =>
                                                {
                                                    rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4");
                                                });
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }
}