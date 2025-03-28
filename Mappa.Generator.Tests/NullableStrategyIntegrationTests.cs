// <copyright file="NullableStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="NullableStrategy"/>.
/// </summary>
public class NullableStrategyIntegrationTests
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
                                statementAssertions[0].BeBlockStatement().AsBlock()
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
                                statementAssertions[0].BeBlockStatement().AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(defaultSyntaxNode => defaultSyntaxNode.BeThrowStatementSyntax<ArgumentOutOfRangeException>(expression => expression.BeLiteralExpressionSyntax("input")));
                            }))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

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
                                        .BeBlockStatement()
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
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(syntaxNode => syntaxNode.BeThrowStatementSyntax<NullReferenceException>(argument => argument.BeLiteralExpressionSyntax("\"input\" is null.")));
                                }))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

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