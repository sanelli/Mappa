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
// TODO [#105] IList<int> -> string[].
// TODO [#105] non-generic impl IList<int> -> string[].
// TODO [#105] generic impl IList<int> -> string[].
// TODO [#105] Span<int> -> string[].
// TODO [#105] ReadOnlySpan<int> -> string[].
// TODO [#105] Memory<int> -> string[].
// TODO [#105] ReadOnlyMemory<int> -> string[].
// TODO [#105] int[] -> Span<string>.
// TODO [#105] int[] -> ReadOnlySpan<string>.
// TODO [#105] int[] -> Memory<string>.
// TODO [#105] int[] -> ReadOnlyMemory<string>.
// TODO [#105] ICollection<int> -> Span<string>.
// TODO [#105] ICollection<int> -> ReadOnlySpan<string>.
// TODO [#105] ICollection<int> -> Memory<string>.
// TODO [#105] ICollection<int> -> ReadOnlyMemory<string>.
// TODO [#105] IEnumerable<int> -> Span<long>.
// TODO [#105] IEnumerable<int> -> ReadOnlySpan<long>.
// TODO [#105] IEnumerable<int> -> Memory<long>.
// TODO [#105] IEnumerable<int> -> ReadOnlyMemory<long>.
// TODO [#105] ERROR WHEN FROM IEnumerable<int> -> Custom<string> without empty param constructor.
public class CollectionToCollectionMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToIEnumerable()
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
                                      public partial IEnumerable<string> Map(IEnumerable<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from a non-generic type implementing <see cref="IEnumerable{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromNonGenericTypeImplementingIEnumerableToIEnumerable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source : IEnumerable<int> { }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IEnumerable<string> Map(Source input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from a generic type implementing <see cref="IEnumerable{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromGenericTypeImplementingIEnumerableToIEnumerable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source<T> : IEnumerable<T> { }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IEnumerable<string> Map(Source<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source<int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Array"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromArrayToIEnumerable()
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
                                      public partial IEnumerable<string> Map(int[] input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int[]).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Length"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Length")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Span{T}"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromSpanToIEnumerable()
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
                                      public partial IEnumerable<string> Map(System.Span<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Span<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Length"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Length")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="ReadOnlySpan{T}"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromReadOnlySpanToIEnumerable()
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
                                      public partial IEnumerable<string> Map(System.ReadOnlySpan<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ReadOnlySpan<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Length"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Length")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Memory{T}"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromMemoryToIEnumerable()
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
                                      public partial IEnumerable<string> Map(System.Memory<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Memory<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Length"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(Span<int>).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Span")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_3", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Length")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_5",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_5")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="ReadOnlyMemory{T}"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromReadOnlyMemoryToIEnumerable()
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
                                      public partial IEnumerable<string> Map(System.ReadOnlyMemory<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ReadOnlyMemory<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Length"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(ReadOnlySpan<int>).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Span")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_3", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Length")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_5",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_5")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IList{T}"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIListToIEnumerable()
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
                                      public partial IEnumerable<string> Map(IList<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IList<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Count")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from non-generic type implementing <see cref="IList{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromNonGenericTypeImplementingIListToIEnumerable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source : IList<int> { }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IEnumerable<string> Map(Source input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Count")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from generic type implementing <see cref="IList{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromGenericTypeImplementingIListToIEnumerable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source<T> : IList<T> { }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IEnumerable<string> Map(Source<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source<int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Count")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="ICollection{T}"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromICollectionToIEnumerable()
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
                                      public partial IEnumerable<string> Map(ICollection<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from non-generic type implementing <see cref="ICollection{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromNonGenericTypeImplementingICollectionToIEnumerable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source : ICollection<int> { }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IEnumerable<string> Map(Source input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from generic type implementing <see cref="ICollection{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromGenericTypeImplementingICollectionToIEnumerable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source<T> : ICollection<T> { }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IEnumerable<string> Map(Source<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source<int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IReadOnlyCollection{T}"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIReadOnlyCollectionToIEnumerable()
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
                                      public partial IEnumerable<string> Map(IReadOnlyCollection<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IReadOnlyCollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from non-generic type implementing <see cref="IReadOnlyCollection{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromNonGenericTypeImplementingIReadOnlyCollectionToIEnumerable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source : IReadOnlyCollection<int> { }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IEnumerable<string> Map(Source input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from generic type implementing <see cref="IReadOnlyCollection{T}"/>
    /// to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromGenericTypeImplementingIReadOnlyCollectionToIEnumerable()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public class Source<T> : IReadOnlyCollection<T> { }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IEnumerable<string> Map(Source<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source<int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="List{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToList()
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
                                      public partial List<string> Map(IEnumerable<int> input);
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
                typeof(List<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Array"/> to <see cref="List{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromArrayToList()
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
                                      public partial List<string> Map(int[] input);
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
                typeof(List<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int[]).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Length"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Length")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="IList{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToIList()
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
                                      public partial IList<string> Map(IEnumerable<int> input);
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
                typeof(IList<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Array"/> to <see cref="IList{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromArrayToIList()
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
                                      public partial IList<string> Map(int[] input);
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
                typeof(IList<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int[]).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Length"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Length")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToICollection()
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
                                      public partial ICollection<string> Map(IEnumerable<int> input);
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
                typeof(ICollection<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Array"/> to <see cref="ICollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromArrayToICollection()
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
                                      public partial ICollection<string> Map(int[] input);
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
                typeof(ICollection<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int[]).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Length"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Length")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToIReadOnlyCollection()
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
                                      public partial IReadOnlyCollection<string> Map(IEnumerable<int> input);
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
                typeof(IReadOnlyCollection<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString())))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Array"/> to <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromArrayToIReadOnlyCollection()
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
                                      public partial IReadOnlyCollection<string> Map(int[] input);
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
                typeof(IReadOnlyCollection<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int[]).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("input.Length"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Length")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Stack{T}"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromStackToIEnumerable()
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
                                      public partial IEnumerable<string> Map(Stack<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Stack<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Queue{T}"/> to <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromQueueToIEnumerable()
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
                                      public partial IEnumerable<string> Map(Queue<int> input);
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
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Queue<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                    typeof(List<string>).ToString(),
                                    parameterAssertions => parameterAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Array"/> to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromArrayToArray()
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
                                      public partial string[] Map(int[] input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int[]).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertion => sizeAssertion.BeMemberAccessExpressionSyntax("input.Length"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForStatementSyntax(
                                declarationAssertions => declarationAssertions.BeAssignmentFromConstant(typeof(int).ToString(), "__mappa_tmp_2", 0),
                                conditionAssertions => conditionAssertions.BeBinaryExpressionSyntax(
                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                    SyntaxKind.LessThanToken,
                                    rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("input.Length")),
                                incrementorAssertions => incrementorAssertions.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, operandAssertions => operandAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("input", "__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftExpression => leftExpression.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                                rightExpression => rightExpression.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IEnumerable{T}"/> to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIEnumerableToArray()
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
                                      public partial string[] Map(IEnumerable<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertions => sizeAssertions.BeInvocationExpressionSyntax(
                                        "global::System.Linq.Enumerable.Count<int>",
                                        parameter => parameter.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                               leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                               rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    SyntaxKind.PlusToken,
                                                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from non-generic type implementing <see cref="IEnumerable{T}"/>
    /// to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromNonGenericTypeImplementingIEnumerableToArray()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source : IEnumerable<int>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string[] Map(Source input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertions => sizeAssertions.BeInvocationExpressionSyntax(
                                        "global::System.Linq.Enumerable.Count<int>",
                                        parameter => parameter.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                               leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                               rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    SyntaxKind.PlusToken,
                                                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from generic type implementing <see cref="IEnumerable{T}"/>
    /// to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromGenericTypeImplementingIEnumerableToArray()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source<T> : IEnumerable<T>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string[] Map(Source<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source<int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertions => sizeAssertions.BeInvocationExpressionSyntax(
                                        "global::System.Linq.Enumerable.Count<int>",
                                        parameter => parameter.BeIdentifierNameSyntax("input")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                               leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                               rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    SyntaxKind.PlusToken,
                                                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="ICollection{T}"/> to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromICollectionToArray()
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
                                      public partial string[] Map(ICollection<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(ICollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertions => sizeAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                               leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                               rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    SyntaxKind.PlusToken,
                                                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from non-generic type implementing <see cref="ICollection{T}"/>
    /// to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromNonGenericTypeImplementingICollectionToArray()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source : ICollection<int>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string[] Map(Source input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertions => sizeAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                               leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                               rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    SyntaxKind.PlusToken,
                                                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from generic type implementing <see cref="ICollection{T}"/>
    /// to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromGenericTypeImplementingICollectionToArray()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source<T> : ICollection<T>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string[] Map(Source<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source<int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertions => sizeAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                               leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                               rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    SyntaxKind.PlusToken,
                                                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="IReadOnlyCollection{T}"/> to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromIReadOnlyCollectionToArray()
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
                                      public partial string[] Map(IReadOnlyCollection<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IReadOnlyCollection<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertions => sizeAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                               leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                               rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    SyntaxKind.PlusToken,
                                                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from non-generic type implementing <see cref="IReadOnlyCollection{T}"/>
    /// to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromNonGenericTypeImplementingIReadOnlyCollectionToArray()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source : IReadOnlyCollection<int>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string[] Map(Source input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertions => sizeAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                               leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                               rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    SyntaxKind.PlusToken,
                                                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from generic type implementing <see cref="IReadOnlyCollection{T}"/>
    /// to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromGenericTypeImplementingIReadOnlyCollectionToArray()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;
                                  
                                  public partial class Source<T> : IReadOnlyCollection<T>
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string[] Map(Source<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source<int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertions => sizeAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                               leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                               rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    SyntaxKind.PlusToken,
                                                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Stack{T}"/> to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromStackToArray()
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
                                      public partial string[] Map(Stack<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Stack<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertions => sizeAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                               leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                               rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    SyntaxKind.PlusToken,
                                                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Test map from <see cref="Queue{T}"/> to <see cref="Array"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapFromQueueToArray()
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
                                      public partial string[] Map(Queue<int> input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Queue<int>).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string[]).ToString(),
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeArrayCreationExpressionSyntax(
                                    typeof(string).ToString(),
                                    sizeAssertions => sizeAssertions.BeMemberAccessExpressionSyntax("input.Count"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeLiteralExpressionSyntax(0)))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                               leftAssignmentAssertions => leftAssignmentAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_2"),
                                               rightAssignmentAssertions => rightAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftAssignmentAssertions => leftAssignmentAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                rightAssignmentAssertions => rightAssignmentAssertions.BeBinaryExpressionSyntax(
                                                    leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                                    SyntaxKind.PlusToken,
                                                    rightExpressionAssertions => rightExpressionAssertions.BeLiteralExpressionSyntax(1))))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }
}