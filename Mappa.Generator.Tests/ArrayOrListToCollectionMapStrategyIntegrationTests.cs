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
// TODO [#42] Add tests for all other combinations of input/output types.
public sealed class ArrayOrListToCollectionMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between two <see cref="IList{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIListToIList()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IList<long> Map(IList<int> input);
                                  }
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
                typeof(IList<long>).ToString(),
                NullableAnnotation.None,
                typeof(IList<int>).ToString(),
                NullableAnnotation.None,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodes(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_1", expressionSyntaxAssertions =>
                            {
                                expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("input.Count");
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
                                        .HasSyntaxNodes(2)
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

    /// <summary>
    /// Test a mapping can be created between two <see cref="List{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapListToList()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial List<long> Map(List<int> input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO [#42] Add correct assertions.
        compilationUnitSyntaxAssertions.NotBeNull();
    }

    /// <summary>
    /// Test a mapping can be created from array to enumerable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapArrayToEnumerable()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IEnumerable<long> Map(int[] input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO [#42] Add correct assertions.
        compilationUnitSyntaxAssertions.NotBeNull();
    }
}