// <copyright file="ReplaceNullWithEmptyContainerIntegrationTests.SettingOff.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests that <c>ReplaceNullWithEmptyContainer</c> leaves default null-throw behaviour when disabled.
/// </summary>
public sealed partial class ReplaceNullWithEmptyContainerIntegrationTests
{
    /// <summary>
    /// Test a null list source still throws when the setting is disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotReplaceNullListWhenSettingDisabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Disable)]
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(List<string>).ToString(),
                                "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions =>
                                {
                                    conditionAssertions.BeIsPatternExpressionSyntax(
                                        expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                        patternAssertions =>
                                        {
                                            patternAssertions.BeUnaryPatternSyntax(
                                                SyntaxKind.NotKeyword,
                                                unaryPatternSyntax => unaryPatternSyntax.BeConstantPatternSyntax(null));
                                        });
                                },
                                thenBranchAssertions =>
                                {
                                    thenBranchAssertions.BeBlockStatement();
                                },
                                elseStatementAssertions =>
                                {
                                    elseStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeThrowStatementSyntax<NullReferenceException>(
                                                expressionAssertions => expressionAssertions.BeLiteralExpressionSyntax("\"input\" is null."));
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