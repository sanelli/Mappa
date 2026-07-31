// <copyright file="StringToNumberMapStrategyIntegrationTests.InvalidNumberStyles.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Invalid number style integration tests for <see cref="Mappa.Generator.Models.Strategies.StringToNumberMapStrategy"/>.
/// </summary>
public sealed partial class StringToNumberMapStrategyIntegrationTests
{
    /// <summary>
    /// Test MP00038 is emitted and code generation continues when an invalid integer
    /// <see cref="System.Globalization.NumberStyles"/> value is set on the method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsWarningAndGeneratesCodeWhenInvalidNumberStyleIsDefinedOnMethod()
    {
        const string identifierName = "__mappa_tmp_1";

        const string sourceCode = """
                                  #nullable enable
                                  using System.Globalization;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(IntStyle = (NumberStyles)1048576)]
                                      public partial int Map(string input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveOnlyWarnings("MP00038")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    "int.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.None")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test MP00038 is emitted when an invalid global number style is defined on the class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsWarningWhenInvalidGlobalNumberStyleIsDefinedOnClass()
    {
        const string identifierName = "__mappa_tmp_1";

        const string sourceCode = """
                                  #nullable enable
                                  using System.Globalization;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaSettings(GlobalNumberStyle = (NumberStyles)1048576)]
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial int Map(string input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveOnlyWarnings("MP00038")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    "int.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.None")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }
}