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
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.IsLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            // TODO [#42] Add correct assertions.
                            syntaxNodeAssertions.IsSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.IsIdentifierName("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].IsBlockStatement();
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].IsBlockStatement();
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsDefault();
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].IsBlockStatement();
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.IsReturnStatement(assertion => assertion.IsIdentifierName("__mappa_tmp_1"));
                        });
                });
    }
}