// <copyright file="CollectionToCollectionMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="CollectionToCollectionMapStrategy"/>.
/// </summary>
public class CollectionToCollectionMapStrategyIntegrationTests
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
                                        .BeBlockStatement()
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
                                        .BeBlockStatement()
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
                                        .BeBlockStatement()
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
        var sourceCode = $$"""
                           #nullable enable

                           using Mappa.Attributes;
                           using System.Collections.Generic;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               public partial {{targetListRepresentation}} Map({{sourceListRepresentation}} input);
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
                targetListType,
                NullableAnnotation.NotAnnotated,
                sourceListType,
                NullableAnnotation.NotAnnotated,
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
                                        .BeBlockStatement()
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

    /// <summary>
    /// Test a mapping can be created from <see cref="ICollection{T}"/>
    /// to array.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapICollectionToArray()
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
                                      public partial long[] Map(ICollection<int> input);
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
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long[]).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeInvocationExpressionSyntax($"__mappa_tmp_1.{nameof(List<long>.ToArray)}");
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IReadOnlyCollection{T}"/>
    /// to array.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIReadOnlyCollectionToArray()
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
                                      public partial long[] Map(IReadOnlyCollection<int> input);
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
                typeof(IReadOnlyCollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long[]).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeInvocationExpressionSyntax($"__mappa_tmp_1.{nameof(List<long>.ToArray)}");
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IEnumerable{T}"/>
    /// to array.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIEnumerableToArray()
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
                                      public partial long[] Map(IEnumerable<int> input);
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
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(long[]).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeInvocationExpressionSyntax($"__mappa_tmp_1.{nameof(List<long>.ToArray)}");
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created between <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapICollectionToICollection()
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
                                      public partial ICollection<long> Map(ICollection<int> input);
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
                typeof(ICollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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
    /// Test a mapping can be created between two <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIEnumerableToIEnumerable()
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
                                      public partial IEnumerable<long> Map(IEnumerable<int> input);
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
                typeof(IEnumerable<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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
    /// Test a mapping can be created between <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIReadOnlyCollectionToIReadOnlyCollection()
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
                                      public partial IReadOnlyCollection<long> Map(IReadOnlyCollection<int> input);
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
                typeof(IReadOnlyCollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IReadOnlyCollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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
    /// Test a mapping can be created from <see cref="ICollection{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapICollectionToIEnumerable()
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
                                      public partial IEnumerable<long> Map(ICollection<int> input);
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
                typeof(IEnumerable<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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
    /// Test a mapping can be created from <see cref="ICollection{T}"/>
    /// to <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapICollectionToIReadOnlyCollection()
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
                                      public partial IReadOnlyCollection<long> Map(ICollection<int> input);
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
                typeof(IReadOnlyCollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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
    /// Test a mapping can be created from <see cref="IEnumerable{T}"/>
    /// to <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIEnumerableToICollection()
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
                                      public partial ICollection<long> Map(IEnumerable<int> input);
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
                typeof(ICollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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
    /// Test a mapping can be created from <see cref="IEnumerable{T}"/>
    /// to <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIEnumerableToIReadOnlyCollection()
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
                                      public partial IReadOnlyCollection<long> Map(IEnumerable<int> input);
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
                typeof(IReadOnlyCollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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
    /// Test a mapping can be created from <see cref="IReadOnlyCollection{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIReadOnlyCollectionToIEnumerable()
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
                                      public partial IEnumerable<long> Map(IReadOnlyCollection<int> input);
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
                typeof(IEnumerable<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IReadOnlyCollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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
    /// Test a mapping can be created from <see cref="IReadOnlyCollection{T}"/>
    /// to <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIReadOnlyCollectionTICollection()
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
                                      public partial ICollection<long> Map(IReadOnlyCollection<int> input);
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
                typeof(ICollection<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IReadOnlyCollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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
    /// Test a mapping can be created from <see cref="ICollection{T}"/>
    /// to <see cref="IList{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapICollectionToIList()
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
                                      public partial IList<long> Map(ICollection<int> input);
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
                typeof(IList<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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
    /// Test a mapping can be created from <see cref="IEnumerable{T}"/>
    /// to <see cref="IList{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIEnumerableToIList()
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
                                      public partial IList<long> Map(IEnumerable<int> input);
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
                typeof(IList<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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
    /// Test a mapping can be created from <see cref="IReadOnlyCollection{T}"/>
    /// to <see cref="IList{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIReadOnlyCollectionToIList()
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
                                      public partial IList<long> Map(IReadOnlyCollection<int> input);
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
                typeof(IList<long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IReadOnlyCollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<long>).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(typeof(List<long>).ToString()));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions =>
                                {
                                    expressionAssertions.BeIdentifierNameSyntax("input");
                                },
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(forStatementAssertions =>
                                        {
                                            forStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                $"__mappa_tmp_1.{nameof(List<long>.Add)}",
                                                parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"));
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