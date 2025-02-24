// <copyright file="NonNullableToNullableMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="NonNullableToNullableMapStrategy"/>.
/// </summary>
public class NonNullableToNullableMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between two enums.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNonNullableToNullable()
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
                                      public partial int? Map(TestEnum input);
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
                typeof(int?).ToString(),
                NullableAnnotation.Annotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_1"))
                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeSwitchStatementSyntax(
                            condition => condition.BeIdentifierNameSyntax("input"),
                            (labelAssertions, statementAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsCase().AsCase().HasValue(expression => expression.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.One"));
                                statementAssertions.Should().HaveCount(1);
                                statementAssertions[0].IsBlockStatement().AsBlock()
                                    .HasSyntaxNodesCount(2)
                                    .HasNextSyntaxNode(caseStatement => caseStatement.BeAssignmentExpressionStatement(
                                        left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                        right => right.BeLiteralExpressionSyntax(0)))
                                    .HasNextSyntaxNode(caseSyntaxNode => caseSyntaxNode.BeBreakStatement());
                            },
                            (labelAssertions, statementAssertions) =>
                            {
                                labelAssertions.Should().HaveCount(1);
                                labelAssertions[0].IsDefault();
                                statementAssertions.Should().HaveCount(1);
                                statementAssertions[0].IsBlockStatement().AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(defaultSyntaxNode => defaultSyntaxNode.BeThrowStatementSyntax<ArgumentOutOfRangeException>(expression => expression.BeLiteralExpressionSyntax("input")));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }
}