// <copyright file="DictionaryToDictionaryMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="DictionaryToDictionaryMapStrategy"/> strategy.
/// </summary>
public sealed class DictionaryToDictionaryMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created from <see cref="Dictionary{TKey,TValue}"/>
    /// to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToDictionary()
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
                                      public partial Dictionary<int, long> Map(Dictionary<short, int> input);
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
                typeof(Dictionary<int, long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(Dictionary<int, long>).ToString(),
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax(typeof(Dictionary<int, long>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(short).ToString(),
                                                "__mappa_tmp_3",
                                                assertions => assertions.BeMemberAccessExpressionSyntax($"__mappa_tmp_2.{nameof(KeyValuePair<short, int>.Key)}")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                assertions => assertions.BeMemberAccessExpressionSyntax($"__mappa_tmp_2.{nameof(KeyValuePair<short, int>.Value)}")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_3"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="Dictionary{TKey,TValue}"/>
    /// to <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToIDictionary()
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
                                      public partial IDictionary<int, long> Map(Dictionary<short, int> input);
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
                typeof(IDictionary<int, long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(Dictionary<int, long>).ToString(),
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax(typeof(Dictionary<int, long>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(short).ToString(),
                                                "__mappa_tmp_3",
                                                assertions => assertions.BeMemberAccessExpressionSyntax($"__mappa_tmp_2.{nameof(KeyValuePair<short, int>.Key)}")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                assertions => assertions.BeMemberAccessExpressionSyntax($"__mappa_tmp_2.{nameof(KeyValuePair<short, int>.Value)}")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_3"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IDictionary{TKey,TValue}"/>
    /// to <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIDictionaryToIDictionary()
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
                                      public partial IDictionary<int, long> Map(IDictionary<short, int> input);
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
                typeof(IDictionary<int, long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IDictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(Dictionary<int, long>).ToString(),
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax(typeof(Dictionary<int, long>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(short).ToString(),
                                                "__mappa_tmp_3",
                                                assertions => assertions.BeMemberAccessExpressionSyntax($"__mappa_tmp_2.{nameof(KeyValuePair<short, int>.Key)}")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                assertions => assertions.BeMemberAccessExpressionSyntax($"__mappa_tmp_2.{nameof(KeyValuePair<short, int>.Value)}")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_3"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="IDictionary{TKey,TValue}"/>
    /// to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIDictionaryToDictionary()
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
                                      public partial Dictionary<int, long> Map(IDictionary<short, int> input);
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
                typeof(Dictionary<int, long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IDictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(Dictionary<int, long>).ToString(),
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax(typeof(Dictionary<int, long>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(short).ToString(),
                                                "__mappa_tmp_3",
                                                assertions => assertions.BeMemberAccessExpressionSyntax($"__mappa_tmp_2.{nameof(KeyValuePair<short, int>.Key)}")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                assertions => assertions.BeMemberAccessExpressionSyntax($"__mappa_tmp_2.{nameof(KeyValuePair<short, int>.Value)}")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_3"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }
}