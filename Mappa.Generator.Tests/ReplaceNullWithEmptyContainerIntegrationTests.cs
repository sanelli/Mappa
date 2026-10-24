// <copyright file="ReplaceNullWithEmptyContainerIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <c>MappaSettings.ReplaceNullWithEmptyContainer</c>.
/// </summary>
public sealed partial class ReplaceNullWithEmptyContainerIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a null <see cref="List{T}"/> source maps to <c>new List&lt;T&gt;()</c> when the setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullListWithEmptyListWhenSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial List<string> Map(List<int>? input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(List<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(List<int>).ToString(),
                NullableAnnotation.Annotated,
                AssertNullThenEmptyListThenMapped);
    }

    /// <summary>
    /// Test a null array source maps to <c>[]</c> when the setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullArrayWithEmptyCollectionExpressionWhenSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial string[] Map(int[]? input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int[]).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(string[]).ToString(),
                        rightAssertions => rightAssertions.BeCollectionExpressionSyntax());
                });
    }

    /// <summary>
    /// Test a null <see cref="IEnumerable{T}"/> maps to <c>[]</c> when the setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullIEnumerableWithEmptyCollectionExpressionWhenSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial IEnumerable<string> Map(IEnumerable<int>? input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(IEnumerable<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(IEnumerable<string>).ToString(),
                        rightAssertions => rightAssertions.BeCollectionExpressionSyntax());
                });
    }

    private static void AssertNullThenEmptyListThenMapped(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        AssertNullThenEmptyThenMapped(
            blockSyntaxAssertions,
            typeof(List<string>).ToString(),
            rightAssertions => rightAssertions.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString()));
    }

    private static void AssertNullThenEmptyThenMapped(
        BlockSyntaxAssertions blockSyntaxAssertions,
        string targetType,
        Action<ExpressionSyntaxAssertions> emptyExpressionAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(targetType, "__mappa_tmp_1");
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeIfStatementSyntax(
                    conditionAssertions =>
                    {
                        conditionAssertions.BeIsPatternExpressionSyntax(
                            expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                            patternAssertions => patternAssertions.BeConstantPatternSyntax(null));
                    },
                    ifStatementAssertions =>
                    {
                        ifStatementAssertions
                            .BeBlockStatement()
                            .AsBlock()
                            .HasSyntaxNodesCount(1)
                            .HasNextSyntaxNode(statementAssertions =>
                            {
                                statementAssertions.BeAssignmentExpressionStatement(
                                    leftAssertions => leftAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    emptyExpressionAssertions);
                            });
                    },
                    elseStatementAssertions =>
                    {
                        elseStatementAssertions.BeBlockStatement();
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
            });
    }
}