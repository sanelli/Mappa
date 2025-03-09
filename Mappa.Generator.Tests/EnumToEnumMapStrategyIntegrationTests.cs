// <copyright file="EnumToEnumMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="EnumToEnumMapStrategy"/>.
/// </summary>
public sealed class EnumToEnumMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between two enums.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnum()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      One,
                                      Two,
                                      Three,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      Two,
                                      Three,
                                      Four,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Three"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Three")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Two"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Two")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsDefault();
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeThrowStatementSyntax<ArgumentOutOfRangeException>(
                                            assertion => assertion.BeLiteralExpressionSyntax("input")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }
}