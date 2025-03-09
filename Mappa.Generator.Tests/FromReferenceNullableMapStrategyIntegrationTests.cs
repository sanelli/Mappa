// <copyright file="FromReferenceNullableMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="FromReferenceNullableMapStrategy"/>.
/// </summary>
public class FromReferenceNullableMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created from a
    /// nullable reference type to a non-nullable
    /// reference type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromReferenceNullable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                    public int Property { get; set; }
                                  }

                                  public class Target
                                  {
                                     public int Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source? input);
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
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions =>
                                {
                                    conditionAssertions.BeIsPatternExpressionSyntax(
                                        expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                        patternAssertions =>
                                        {
                                            patternAssertions.BeUnaryPatternSyntax(
                                                SyntaxKind.NotKeyword,
                                                unaryPatternSyntax => unaryPatternSyntax.BeConstantPatternSyntax(null));
                                        });
                                },
                                thenBranchAssertions =>
                                {
                                    thenBranchAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(4)
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Source", "__mappa_tmp_2", initExpressionAssertions => initExpressionAssertions.BeIdentifierNameSyntax("input"));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_3", initExpressionAssertions => initExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Property"));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                "__mappa_tmp_4",
                                                initExpressionAssertions => initExpressionAssertions.BeObjectCreationExpressionSyntax(
                                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                    ("Property", parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"))));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"));
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
                                            statementAssertions.BeThrowStatementSyntax<NullReferenceException>(expressionAssertions => expressionAssertions.BeLiteralExpressionSyntax("\"input\" is null."));
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