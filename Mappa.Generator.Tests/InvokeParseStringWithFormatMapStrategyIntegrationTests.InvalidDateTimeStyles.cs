// <copyright file="InvokeParseStringWithFormatMapStrategyIntegrationTests.InvalidDateTimeStyles.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Invalid date/time style integration tests for string parse strategies.
/// </summary>
public sealed partial class InvokeParseStringWithFormatMapStrategyIntegrationTests
{
    /// <summary>
    /// Test MP00038 is emitted and code generation continues when an invalid integer
    /// <see cref="System.Globalization.DateTimeStyles"/> value is set on the method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsWarningAndGeneratesCodeWhenInvalidDateTimeStyleIsDefinedOnMethod()
    {
        const string identifierName = "__mappa_tmp_1";

        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using System.Globalization;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(DateTimeStyle = (DateTimeStyles)256)]
                                      public partial DateTime Map(string input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.InvalidMappaSettingsStyleValue,
                "DateTimeStyle",
                256,
                "DateTimeStyles")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(DateTime).ToString(),
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
                                typeof(DateTime).ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{typeof(DateTime).FullName}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(null),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.DateTimeStyles.None")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test MP00038 is emitted when an invalid global date/time style is defined on the class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsWarningWhenInvalidGlobalDateTimeStyleIsDefinedOnClass()
    {
        const string identifierName = "__mappa_tmp_1";

        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using System.Globalization;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaSettings(GlobalDateTimeStyle = (DateTimeStyles)256)]
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial DateTime Map(string input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.InvalidMappaSettingsStyleValue,
                "GlobalDateTimeStyle",
                256,
                "DateTimeStyles")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(DateTime).ToString(),
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
                                typeof(DateTime).ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{typeof(DateTime).FullName}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(null),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.DateTimeStyles.None")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }
}