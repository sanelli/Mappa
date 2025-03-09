// <copyright file="NullableToNullableMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="NullableToNullableMapStrategy"/>.
/// </summary>
public class NullableToNullableMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between two enums.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNullableToNullable()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestEnum
                                  {
                                    One,
                                  }
                                  
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestEnum? Map(int? input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum?",
                NullableAnnotation.Annotated,
                typeof(int?).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum?", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions => conditionAssertions.BeMemberAccessExpressionSyntax("input.HasValue"),
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(4)
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeLocalDeclarationStatementSyntax(
                                            typeof(int).ToString(),
                                            "__mappa_tmp_2",
                                            assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeLocalDeclarationStatementSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum",
                                            "__mappa_tmp_3"))
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeSwitchStatementSyntax(
                                            condition => condition.BeCastExpressionSyntax(
                                                typeof(int).ToString(),
                                                expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                            (labelAssertions, statementAssertions) =>
                                            {
                                                labelAssertions.Should().HaveCount(1);
                                                labelAssertions[0].IsCase().AsCase().HasValue(expression => expression.BeLiteralExpressionSyntax(0));
                                                statementAssertions.Should().HaveCount(1);
                                                statementAssertions[0].BeBlockStatement().AsBlock()
                                                    .HasSyntaxNodesCount(2)
                                                    .HasNextSyntaxNode(caseStatement => caseStatement.BeAssignmentExpressionStatement(
                                                        left => left.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                                        right => right.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.One")))
                                                    .HasNextSyntaxNode(caseSyntaxNode => caseSyntaxNode.BeBreakStatement());
                                            },
                                            (labelAssertions, statementAssertions) =>
                                            {
                                                labelAssertions.Should().HaveCount(1);
                                                labelAssertions[0].IsDefault();
                                                statementAssertions.Should().HaveCount(1);
                                                statementAssertions[0].BeBlockStatement().AsBlock()
                                                    .HasSyntaxNodesCount(1)
                                                    .HasNextSyntaxNode(defaultSyntaxNode => defaultSyntaxNode.BeThrowStatementSyntax<ArgumentOutOfRangeException>(expression => expression.BeLiteralExpressionSyntax("__mappa_tmp_2")));
                                            }))
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeAssignmentExpressionStatement(
                                            left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                            right => right.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                },
                                elseStatementAssertions =>
                                {
                                    elseStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeAssignmentExpressionStatement(
                                            left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                            right => right.BeCastExpressionSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum?",
                                                expression => expression.BeLiteralExpressionSyntax(null))));
                                }))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }
}