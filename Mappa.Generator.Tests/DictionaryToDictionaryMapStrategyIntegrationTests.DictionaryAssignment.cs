// <copyright file="DictionaryToDictionaryMapStrategyIntegrationTests.DictionaryAssignment.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="MappaSettingsAttribute.DictionaryAssignment"/>.
/// </summary>
public sealed partial class DictionaryToDictionaryMapStrategyIntegrationTests
{
    /// <summary>
    /// Test a mapping from <see cref="Dictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/>
    /// with <see cref="MappaSettingsAttribute.DictionaryAssignment"/> set to <see cref="DictionaryAssignmentSetting.Add"/> on method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToDictionaryWhenDictionaryAssignmentIsAddOnMethod()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Add)]
                                      public partial Dictionary<int, long> Map(Dictionary<short, int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                AssertDictionaryToDictionaryUsesAddInForEachBody);
    }

    /// <summary>
    /// Test a mapping from <see cref="Dictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/>
    /// with <see cref="MappaSettingsAttribute.DictionaryAssignment"/> set to <see cref="DictionaryAssignmentSetting.Add"/> on class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToDictionaryWhenDictionaryAssignmentIsAddOnClass()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Add)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Dictionary<int, long> Map(Dictionary<short, int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                AssertDictionaryToDictionaryUsesAddInForEachBody);
    }

    /// <summary>
    /// Test a mapping from <see cref="Dictionary{TKey,TValue}"/>
    /// to custom generic type implementing explicitly <see cref="IDictionary{TKey,TValue}.Add"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToGenericTypeImplementingExplicitlyIDictionaryWithDefinedAdd()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public partial class Target<K, V> : IDictionary<K, V>
                                  {
                                      void IDictionary<K, V>.Add(K key, V value) { }
                                  }

                                  [Mappa]
                                  [MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Add)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target<int, long> Map(Dictionary<short, int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                AssertDictionaryToExplicitIDictionaryUsesAddInForEachBody);
    }

    /// <summary>
    /// Test a mapping from <see cref="Dictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/>
    /// with <see cref="MappaSettingsAttribute.DictionaryAssignment"/> set to <see cref="DictionaryAssignmentSetting.Add"/> in <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToDictionaryWhenDictionaryAssignmentIsSetInEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.dictionaryassignment = Add
                                    """;

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

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

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
                AssertDictionaryToDictionaryUsesAddInForEachBody);
    }

    /// <summary>
    /// Test class-level <see cref="MappaSettingsAttribute.DictionaryAssignment"/> overrides
    /// <c>.editorconfig</c> for dictionary-to-dictionary mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToDictionaryWhenClassDictionaryAssignmentOverridesEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.dictionaryassignment = Add
                                    """;

        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Indexer)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Dictionary<int, long> Map(Dictionary<short, int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

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
                AssertDictionaryToDictionaryUsesIndexerInForEachBody);
    }

    /// <summary>
    /// Test method-level <see cref="MappaSettingsAttribute.DictionaryAssignment"/> overrides
    /// class-level and <c>.editorconfig</c> settings for dictionary-to-dictionary mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToDictionaryWhenMethodDictionaryAssignmentOverridesClassAndEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.dictionaryassignment = Add
                                    """;

        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Indexer)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Add)]
                                      public partial Dictionary<int, long> Map(Dictionary<short, int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

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
                AssertDictionaryToDictionaryUsesAddInForEachBody);
    }

    /// <summary>
    /// Test a mapping from <see cref="Dictionary{TKey,TValue}"/> to <see cref="Dictionary{TKey,TValue}"/>
    /// when <c>mappa.dictionaryassignment</c> is <see cref="DictionaryAssignmentSetting.Undefined"/> in <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToDictionaryWhenDictionaryAssignmentIsUndefinedInEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.dictionaryassignment = Undefined
                                    """;

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

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

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
                AssertDictionaryToDictionaryUsesIndexerInForEachBody);
    }

    /// <summary>
    /// Test method-level <see cref="MappaSettingsAttribute.DictionaryAssignment"/> overrides
    /// class-level setting for dictionary-to-dictionary mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToDictionaryWhenMethodDictionaryAssignmentOverridesClass()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Add)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Indexer)]
                                      public partial Dictionary<int, long> Map(Dictionary<short, int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                AssertDictionaryToDictionaryUsesIndexerInForEachBody);
    }

    /// <summary>
    /// Test a mapping from <see cref="Dictionary{TKey,TValue}"/>
    /// to <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey,TValue}"/>
    /// with <see cref="MappaSettingsAttribute.DictionaryAssignment"/> set to <see cref="DictionaryAssignmentSetting.Add"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapDictionaryToConcurrentDictionaryWhenDictionaryAssignmentIsAdd()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa;
                                  using Mappa.Attributes;
                                  using System.Collections.Concurrent;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(DictionaryAssignment = DictionaryAssignmentSetting.Add)]
                                  public sealed partial class Mapper
                                  {
                                      public partial ConcurrentDictionary<int, long> Map(Dictionary<short, int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                                                assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_5.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                                secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    private static void AssertDictionaryToDictionaryUsesAddInForEachBody(BlockSyntaxAssertions blockSyntaxAssertions)
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
                                forEachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                    "__mappa_tmp_1.Add",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
    }

    private static void AssertDictionaryToDictionaryUsesIndexerInForEachBody(BlockSyntaxAssertions blockSyntaxAssertions)
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
    }

    private static void AssertDictionaryToExplicitIDictionaryUsesAddInForEachBody(BlockSyntaxAssertions blockSyntaxAssertions)
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
                                    assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")))
                            .HasNextSyntaxNode(forEachStatementAssertions =>
                                forEachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                    "__mappa_tmp_5.Add",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
    }
}