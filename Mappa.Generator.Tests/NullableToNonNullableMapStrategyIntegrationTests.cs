// <copyright file="NullableToNonNullableMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="NullableToNonNullableMapStrategy"/>.
/// </summary>
public class NullableToNonNullableMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between two enums.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNullableToNonNullable()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial long Map(int? input);
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
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int?).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(long).ToString(), "__mappa_tmp_1"))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions => conditionAssertions.BeMemberAccessExpressionSyntax("input.HasValue"),
                                thenStatementAssertions =>
                                {
                                    thenStatementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeLocalDeclarationStatementSyntax(
                                            typeof(int).ToString(),
                                            "__mappa_tmp_2",
                                            assertInitialization => assertInitialization.BeMemberAccessExpressionSyntax("input.Value")))
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeAssignmentExpressionStatement(
                                            left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                            right => right.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                },
                                elseStatementAssertions =>
                                {
                                    elseStatementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeThrowStatementSyntax<NullReferenceException>(argument => argument.BeLiteralExpressionSyntax("\"input\" is null.")));
                                }))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }
}