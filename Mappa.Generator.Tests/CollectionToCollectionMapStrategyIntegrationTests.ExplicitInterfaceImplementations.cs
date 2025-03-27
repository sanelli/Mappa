// <copyright file="CollectionToCollectionMapStrategyIntegrationTests.ExplicitInterfaceImplementations.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="CollectionToCollectionMapStrategy"/>.
/// </summary>
public sealed partial class CollectionToCollectionMapStrategyIntegrationTests
{
    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to non-generic generic
    /// type implementing explicitly <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToNonGenericTypeImplementingICollectionExplicitly()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Target : ICollection<string>
                                  {
                                        void ICollection<string>.Add(string item) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(IEnumerable<int> input);
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
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(ICollection<string>).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_4.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to generic type
    /// implementing explicitly <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToGenericTypeImplementingICollectionExplicitly()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Target<T> : ICollection<T>
                                  {
                                        void ICollection<T>.Add(T item) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target<string> Map(IEnumerable<int> input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target<string>",
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target<string>",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target<string>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(ICollection<string>).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_4.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }
}