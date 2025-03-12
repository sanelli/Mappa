// <copyright file="ReadonlyDictionaryPropertyMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests for <see cref="ReadonlyDictionaryPropertyMapStrategy"/>.
/// </summary>
// TODO [#4] Add extra tests to make sure this works when setter exists but is not accessible.
public sealed class ReadonlyDictionaryPropertyMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test that a map from <see cref="Dictionary{TKey,TValue}"/> where key is <see cref="int"/> and value is <see cref="string"/>
    /// <see cref="Dictionary{TKey,TValue}"/> where key is <see cref="string"/> and value is <see cref="int"/>
    /// and the target property does not have a setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToDictionaryWhenSetterIsNotProvidedOnTargetProperty()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      Dictionary<int, string> PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      Dictionary<string, int> PropertyA {get;}
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(Dictionary<int, string>).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<int, string>).ToString(),
                                "__mappa_tmp_3",
                                expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                forBlockAssertions =>
                                {
                                    forBlockAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(5)
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Key"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_5",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_6",
                                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Value"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_7",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("int.Parse", parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_6")));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement
                                                .BeAssignmentExpressionStatement(
                                                    leftExpression => leftExpression.BeElementAccessExpressionSyntaxWithMemberAccessNameSyntax(
                                                        "__mappa_tmp_1.PropertyA",
                                                        "__mappa_tmp_5"),
                                                    rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_7"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test that a map from <see cref="IDictionary{TKey,TValue}"/> where key is <see cref="int"/> and value is <see cref="string"/>
    /// <see cref="IDictionary{TKey,TValue}"/> where key is <see cref="string"/> and value is <see cref="int"/>
    /// and the target property does not have a setter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIDictionaryToIDictionaryWhenSetterIsNotProvidedOnTargetProperty()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      IDictionary<int, string> PropertyA {get;}
                                  }

                                  public class Target
                                  {
                                      IDictionary<string, int> PropertyA {get;}
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(IDictionary<int, string>).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<int, string>).ToString(),
                                "__mappa_tmp_3",
                                expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                forBlockAssertions =>
                                {
                                    forBlockAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(5)
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Key"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_5",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_6",
                                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_3.Value"));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_7",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("int.Parse", parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_6")));
                                        })
                                        .HasNextSyntaxNode(forSyntaxStatement =>
                                        {
                                            forSyntaxStatement
                                                .BeAssignmentExpressionStatement(
                                                    leftExpression => leftExpression.BeElementAccessExpressionSyntaxWithMemberAccessNameSyntax(
                                                        "__mappa_tmp_1.PropertyA",
                                                        "__mappa_tmp_5"),
                                                    rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_7"));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }
}