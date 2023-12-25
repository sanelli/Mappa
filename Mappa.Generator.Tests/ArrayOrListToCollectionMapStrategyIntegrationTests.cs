// <copyright file="ArrayOrListToCollectionMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="ArrayOrListToCollectionMapStrategy"/>.
/// </summary>
public sealed class ArrayOrListToCollectionMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Gets the test values for <see cref="CanMapArrayOrListToCollection"/>.
    /// </summary>
    /// <returns>Test values for <see cref="CanMapArrayOrListToCollection"/>.</returns>
    public static IEnumerable<object[]> CanMapArrayOrListToCollectionTestValues()
    {
        yield return ["List<long>", typeof(List<long>).ToString(), "IList<int>", typeof(IList<int>).ToString(), false];
        yield return ["IList<long>", typeof(IList<long>).ToString(), "IList<int>", typeof(IList<int>).ToString(), false];
        yield return ["ICollection<long>", typeof(ICollection<long>).ToString(), "IList<int>", typeof(IList<int>).ToString(), false];
        yield return ["IReadOnlyCollection<long>", typeof(IReadOnlyCollection<long>).ToString(), "IList<int>", typeof(IList<int>).ToString(), false];
        yield return ["IEnumerable<long>", typeof(IEnumerable<long>).ToString(), "IList<int>", typeof(IList<int>).ToString(), false];

        yield return ["List<long>", typeof(List<long>).ToString(), "List<int>", typeof(List<int>).ToString(), false];
        yield return ["IList<long>", typeof(IList<long>).ToString(), "List<int>", typeof(List<int>).ToString(), false];
        yield return ["ICollection<long>", typeof(ICollection<long>).ToString(), "List<int>", typeof(List<int>).ToString(), false];
        yield return ["IReadOnlyCollection<long>", typeof(IReadOnlyCollection<long>).ToString(), "List<int>", typeof(List<int>).ToString(), false];
        yield return ["IEnumerable<long>", typeof(IEnumerable<long>).ToString(), "List<int>", typeof(List<int>).ToString(), false];

        yield return ["List<long>", typeof(List<long>).ToString(), "int[]", typeof(int[]).ToString(), true];
        yield return ["IList<long>", typeof(IList<long>).ToString(), "int[]", typeof(int[]).ToString(), true];
        yield return ["ICollection<long>", typeof(ICollection<long>).ToString(), "int[]", typeof(int[]).ToString(), true];
        yield return ["IReadOnlyCollection<long>", typeof(IReadOnlyCollection<long>).ToString(), "int[]", typeof(int[]).ToString(), true];
        yield return ["IEnumerable<long>", typeof(IEnumerable<long>).ToString(), "int[]", typeof(int[]).ToString(), true];
    }

    /// <summary>
    /// Test a mapping can be created between two <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="targetListRepresentation">Representation of the target type.</param>
    /// <param name="targetListType">The target type.</param>
    /// <param name="sourceListRepresentation">The representation of the source type.</param>
    /// <param name="sourceListType">The source type.</param>
    /// <param name="isSourceArray"><c>true</c> if the source type is an array.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(CanMapArrayOrListToCollectionTestValues))]
    [IntegrationTest]
    public async Task CanMapArrayOrListToCollection(
        string targetListRepresentation,
        string targetListType,
        string sourceListRepresentation,
        string sourceListType,
        bool isSourceArray)
    {
        // Arrange
        var sourceCode = """
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial %target-type% Map(%param-type% input);
                                  }
                                  """
                .Replace("%target-type%", targetListRepresentation, StringComparison.Ordinal)
                .Replace("%param-type%", sourceListRepresentation, StringComparison.Ordinal)
            ;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                targetListType,
                NullableAnnotation.None,
                sourceListType,
                NullableAnnotation.None,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_1", expressionSyntaxAssertions =>
                            {
                                expressionSyntaxAssertions.BeMemberAccessExpressionSyntax($"input.{(isSourceArray ? nameof(Array.Length) : nameof(List<int>.Count))}");
                            });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(List<long>).ToString(), "__mappa_tmp_2", expressionSyntaxAssertions =>
                            {
                                expressionSyntaxAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<long>).ToString(),
                                    firstArgumentAssertions => firstArgumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
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
                                            forStatement.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_2.{nameof(List<long>.Add)}",
                                                firstArgumentExpression => firstArgumentExpression.BeIdentifierNameSyntax("__mappa_tmp_4"));
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