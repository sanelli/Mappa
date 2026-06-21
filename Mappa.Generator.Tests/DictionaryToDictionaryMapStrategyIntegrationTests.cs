// <copyright file="DictionaryToDictionaryMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
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
                                        .BeBlockStatement()
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
    /// to <see cref="SortedDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToSortedDictionary()
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
                                      public partial SortedDictionary<int, long> Map(Dictionary<short, int> input);
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
                typeof(SortedDictionary<int, long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(SortedDictionary<int, long>).ToString(),
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax(typeof(SortedDictionary<int, long>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
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
    /// Test a mapping can be created from <see cref="SortedDictionary{TKey,TValue}"/>
    /// to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapSortedDictionaryToDictionary()
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
                                      public partial Dictionary<int, long> Map(SortedDictionary<short, int> input);
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
                typeof(SortedDictionary<short, int>).ToString(),
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
                                        .BeBlockStatement()
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
                                typeof(IDictionary<int, long>).ToString(),
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
                                        .BeBlockStatement()
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
                                typeof(IDictionary<int, long>).ToString(),
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
                                        .BeBlockStatement()
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
                                        .BeBlockStatement()
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
    /// to a custom derived implementation of <see cref="IDictionary{TKey,TValue}"/>
    /// that exposes generic parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToCustomDerivedIDictionaryWithGenericParameters()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Target<T, K> : IDictionary<T, K>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target<int, long> Map(Dictionary<short, int> input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target<int, long>",
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target<int, long>",
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target<int, long>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
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
    /// to a custom derived implementation of <see cref="IDictionary{TKey,TValue}"/>
    /// that does not expose generic parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToCustomDerivedIDictionaryWithoutGenericParameters()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Target : IDictionary<int, long>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Dictionary<short, int> input);
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
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
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
    /// Test a mapping cannot be created from <see cref="Dictionary{TKey,TValue}"/>
    /// to a custom derived implementation of <see cref="IDictionary{TKey,TValue}"/>
    /// that does not expose generic parameters when target does not have an empty constructor.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapDictionaryToCustomDerivedIDictionaryWithoutGenericParametersWhenEmptyConstructorIsMissing()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Target : IDictionary<int, long>
                                  {
                                      public Target(int parameter){}
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Dictionary<short, int> input);
                                  }
                                  
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, "System.Collections.Generic.Dictionary<short, int>", "Mappa.Generator.Tests.UnitTests.SourceCode.Target");
    }

    /// <summary>
    /// Test a mapping cannot be created from <see cref="Dictionary{TKey,TValue}"/>
    /// to a custom derived implementation of <see cref="IDictionary{TKey,TValue}"/>
    /// that does not expose generic parameters when target has a private constructor
    /// with zero parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapDictionaryToCustomDerivedIDictionaryWithoutGenericParametersWhenEmptyConstructorIsPrivate()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public partial class Target : IDictionary<int, long>
                                  {
                                      private Target(){ }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Dictionary<short, int> input);
                                  }

                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, "System.Collections.Generic.Dictionary<short, int>", "Mappa.Generator.Tests.UnitTests.SourceCode.Target");
    }

    /// <summary>
    /// Test a mapping cannot be created from <see cref="Dictionary{TKey,TValue}"/>
    /// when the value type cannot be mapped to the target value type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapDictionaryWhenValueTypeCannotBeMapped()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class SourceValue
                                  {
                                      public int Id { get; set; }
                                  }

                                  public class TargetValue
                                  {
                                      public string Name { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Dictionary<int, TargetValue> Map(Dictionary<int, SourceValue> input);
                                  }

                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                "Mappa.Generator.Tests.UnitTests.SourceCode.SourceValue",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TargetValue");
    }

    /// <summary>
    /// Test a mapping can be created from  derived implementation of <see cref="IDictionary{TKey,TValue}"/>
    /// that exposes generic parameters to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapCustomDerivedIDictionaryWithGenericParametersToDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source<T, K> : IDictionary<T, K>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Dictionary<int, long> Map(Source<short, int> input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source<short, int>",
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
                                        .BeBlockStatement()
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
    /// Test a mapping can be created from  derived implementation of <see cref="IDictionary{TKey,TValue}"/>
    /// that does not expose generic parameters to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapCustomDerivedIDictionaryWithoutGenericParametersToDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source : IDictionary<short, int>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Dictionary<int, long> Map(Source input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
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
                                        .BeBlockStatement()
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
    /// Test a mapping can be created between two custom dictionaries deriving from <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapCustomDictionaryToCustomDictionaryWithGenerics()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class CustomDictionary<T, K> : Dictionary<T, K>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial CustomDictionary<int, long> Map(CustomDictionary<short, int> input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.CustomDictionary<int, long>",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.CustomDictionary<short, int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.CustomDictionary<int, long>",
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.CustomDictionary<int, long>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
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
    /// Test a mapping can be created between a <see cref="IEnumerable{T}"/>
    /// of <see cref="KeyValuePair{TKey,TValue}"/> and a <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumerableOfKeyValuePairsToDictionary()
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
                                      public partial Dictionary<int, long> Map(IEnumerable<KeyValuePair<short, int>> input);
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
                typeof(IEnumerable<KeyValuePair<short, int>>).ToString(),
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
                                        .BeBlockStatement()
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
    /// Test a mapping can be created between a type implementing <see cref="IEnumerable{T}"/>
    /// of <see cref="KeyValuePair{TKey,TValue}"/> and a <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTypeImplementingEnumerableOfKeyValuePairsToDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source :
                                    IEnumerable<KeyValuePair<short, int>>{ }
                                  
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Dictionary<int, long> Map(Source input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
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
                                        .BeBlockStatement()
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
    /// Test a mapping can be created between a type implementing <see cref="IEnumerable{T}"/>
    /// of <see cref="KeyValuePair{TKey,TValue}"/> with explicit generics
    /// and a <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTypeImplementingEnumerableOfKeyValuePairsWithExplicitGenericsToDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source<K, V> :
                                    IEnumerable<KeyValuePair<K, V>>{ }
                                  
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Dictionary<int, long> Map(Source<short, int> input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source<short, int>",
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
                                        .BeBlockStatement()
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
    /// Test a mapping can be created between a <see cref="IReadOnlyDictionary{TKey,TValue}"/>
    /// and a <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapIReadOnlyDictionaryToDictionary()
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
                                      public partial Dictionary<int, long> Map(IReadOnlyDictionary<short, int> input);
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
                typeof(IReadOnlyDictionary<short, int>).ToString(),
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
                                        .BeBlockStatement()
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
    /// Test a mapping can be created between a type implementing
    /// <see cref="IReadOnlyDictionary{TKey,TValue}"/> and a <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTypeImplementingIReadOnlyDictionaryToDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source :
                                    IReadOnlyDictionary<short, int> { }
                                  
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Dictionary<int, long> Map(Source input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
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
                                        .BeBlockStatement()
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
    /// Test a mapping can be created between a type implementing
    /// <see cref="IReadOnlyDictionary{TKey,TValue}"/> with explicit generics
    /// and a <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapTypeImplementingIReadOnlyDictionaryWithExplicitGenericsToDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source<K, V> :
                                    IReadOnlyDictionary<K, V>{ }
                                  
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Dictionary<int, long> Map(Source<short, int> input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source<short, int>",
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
                                        .BeBlockStatement()
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
    /// to <see cref="IEnumerable{T}"/> or <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToIEnumerableOfKeyValuePairs()
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
                                      public partial IEnumerable<KeyValuePair<int, long>> Map(Dictionary<short, int> input);
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
                typeof(IEnumerable<KeyValuePair<int, long>>).ToString(),
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
                                        .BeBlockStatement()
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
    /// to <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToIReadOnlyDictionary()
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
                                      public partial IReadOnlyDictionary<int, long> Map(Dictionary<short, int> input);
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
                typeof(IReadOnlyDictionary<int, long>).ToString(),
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
                                        .BeBlockStatement()
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
    /// to <see cref="System.Collections.ObjectModel.ReadOnlyDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToReadOnlyDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;
                                  using System.Collections.ObjectModel;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial ReadOnlyDictionary<int, long> Map(Dictionary<short, int> input);
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
                typeof(System.Collections.ObjectModel.ReadOnlyDictionary<int, long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
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
                                        .BeBlockStatement()
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
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(System.Collections.ObjectModel.ReadOnlyDictionary<int, long>).ToString(),
                                "__mappa_tmp_5",
                                assertions => assertions.BeObjectCreationExpressionSyntax(
                                    typeof(System.Collections.ObjectModel.ReadOnlyDictionary<int, long>).ToString(),
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_5")));
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="Dictionary{TKey,TValue}"/>
    /// to <see cref="System.Collections.Immutable.IImmutableDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToIImmutableDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;
                                  using System.Collections.Immutable;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IImmutableDictionary<int, long> Map(Dictionary<short, int> input);
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
                typeof(System.Collections.Immutable.IImmutableDictionary<int, long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
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
                                        .BeBlockStatement()
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
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(System.Collections.Immutable.IImmutableDictionary<int, long>).ToString(),
                                "__mappa_tmp_5",
                                assertions => assertions.BeInvocationExpressionSyntax(
                                    "System.Collections.Immutable.ImmutableDictionary.ToImmutableDictionary<int,long>",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_5")));
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="Dictionary{TKey,TValue}"/>
    /// to <see cref="System.Collections.Immutable.ImmutableDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToImmutableDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;
                                  using System.Collections.Immutable;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial ImmutableDictionary<int, long> Map(Dictionary<short, int> input);
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
                typeof(System.Collections.Immutable.ImmutableDictionary<int, long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
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
                                        .BeBlockStatement()
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
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(System.Collections.Immutable.ImmutableDictionary<int, long>).ToString(),
                                "__mappa_tmp_5",
                                assertions => assertions.BeInvocationExpressionSyntax(
                                    "System.Collections.Immutable.ImmutableDictionary.ToImmutableDictionary<int,long>",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_5")));
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="Dictionary{TKey,TValue}"/>
    /// to <see cref="System.Collections.Immutable.ImmutableSortedDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToImmutableSortedDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;
                                  using System.Collections.Immutable;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial ImmutableSortedDictionary<int, long> Map(Dictionary<short, int> input);
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
                typeof(System.Collections.Immutable.ImmutableSortedDictionary<int, long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
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
                                        .BeBlockStatement()
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
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(System.Collections.Immutable.ImmutableSortedDictionary<int, long>).ToString(),
                                "__mappa_tmp_5",
                                assertions => assertions.BeInvocationExpressionSyntax(
                                    "System.Collections.Immutable.ImmutableSortedDictionary.ToImmutableSortedDictionary<int,long>",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_5")));
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="Dictionary{TKey,TValue}"/>
    /// to <see cref="System.Collections.Frozen.FrozenDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToFrozenDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;
                                  using System.Collections.Frozen;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial FrozenDictionary<int, long> Map(Dictionary<short, int> input);
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
                typeof(System.Collections.Frozen.FrozenDictionary<int, long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
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
                                        .BeBlockStatement()
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
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(System.Collections.Frozen.FrozenDictionary<int, long>).ToString(),
                                "__mappa_tmp_5",
                                assertions => assertions.BeInvocationExpressionSyntax(
                                    "System.Collections.Frozen.FrozenDictionary.ToFrozenDictionary<int,long>",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_5")));
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="Dictionary{TKey,TValue}"/>
    /// to custom non-generic type implementing <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToNonGenericTypeImplementingIDictionaryWithDefinedIndexer()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Target : IDictionary<int, long>
                                  {
                                      public long this[int key]
                                      {
                                          get => { return 0; }
                                          set => { }
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Dictionary<short, int> input);
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
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
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
    /// to custom generic type implementing <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToGenericTypeImplementingIDictionaryWithDefinedIndexer()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Target<K, V> : IDictionary<K, V>
                                  {
                                      public V this[K key]
                                      {
                                          get => { return default; }
                                          set => { }
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target<int,long> Map(Dictionary<short, int> input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target<int,long>",
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target<int,long>",
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target<int,long>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
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
    /// to custom non-generic type implementing explictly <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToNonGenericTypeImplementingExplicitlyIDictionaryWithDefinedIndexer()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Target : IDictionary<int, long>
                                  {
                                      long IDictionary<int, long>.this[int key]
                                      {
                                          get => { return 0; }
                                          set => { }
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Dictionary<short, int> input);
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
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(4)
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
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                "System.Collections.Generic.IDictionary<int, long>",
                                                "__mappa_tmp_5",
                                                assertions => assertions.BeIdentifierNameSyntax($"__mappa_tmp_1")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_5", "__mappa_tmp_3"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="Dictionary{TKey,TValue}"/>
    /// to custom generic type implementing explicitly <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToGenericTypeImplementingExplicitlyIDictionaryWithDefinedIndexer()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Target<K, V> : IDictionary<K, V>
                                  {
                                      V IDictionary<K, V>.this[K key]
                                      {
                                          get => { return default; }
                                          set => { }
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target<int,long> Map(Dictionary<short, int> input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target<int,long>",
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target<int,long>",
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target<int,long>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(4)
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
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                "System.Collections.Generic.IDictionary<int, long>",
                                                "__mappa_tmp_5",
                                                assertions => assertions.BeIdentifierNameSyntax($"__mappa_tmp_1")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_5", "__mappa_tmp_3"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey,TValue}"/>
    /// to <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapConcurrentDictionaryToDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;
                                  using System.Collections.Concurrent;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Dictionary<int, long> Map(ConcurrentDictionary<short, int> input);
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
                typeof(System.Collections.Concurrent.ConcurrentDictionary<short, int>).ToString(),
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
                                        .BeBlockStatement()
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
    /// to <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToConcurrentDictionary()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;
                                  using System.Collections.Concurrent;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial ConcurrentDictionary<int, long> Map(Dictionary<short, int> input);
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
                typeof(System.Collections.Concurrent.ConcurrentDictionary<int, long>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<short, int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(System.Collections.Concurrent.ConcurrentDictionary<int, long>).ToString(),
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax(typeof(System.Collections.Concurrent.ConcurrentDictionary<int, long>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
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
    /// Test a mapping can be created between two custom dictionaries
    /// deriving from <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapCustomDictionaryToCustomConcurrentDictionaryWithGenerics()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;
                                  using System.Collections.Concurrent;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class CustomDictionary<T, K> : ConcurrentDictionary<T, K>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial CustomDictionary<int, long> Map(CustomDictionary<short, int> input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.CustomDictionary<int, long>",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.CustomDictionary<short, int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.CustomDictionary<int, long>",
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.CustomDictionary<int, long>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<short, int>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
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