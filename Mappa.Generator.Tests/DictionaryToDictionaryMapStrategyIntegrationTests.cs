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
// TODO [#105] Add tests where source is IReadOnlyDictionary.
// TODO [#105] Add tests where source implements IReadOnlyDictionary.
// TODO [#105] Add tests where target is IReadOnlyDictionary.
// TODO [#105] Add tests where target is ReadOnlyDictionary.
// TODO [#105] Add tests where target is ImmutableDictionary.
// TODO [#105] Add tests where target is FrozenDictionary.
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
            .ContainDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, typeof(Dictionary<short, int>).ToString(), "Mappa.Generator.Tests.UnitTests.SourceCode.Target");
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
}