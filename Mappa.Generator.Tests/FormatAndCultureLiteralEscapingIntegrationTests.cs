// <copyright file="FormatAndCultureLiteralEscapingIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Text;

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for escaped format and culture literals in generated mapping code.
/// </summary>
public sealed class FormatAndCultureLiteralEscapingIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Gets test data for format strings containing special characters.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, string> SpecialCharacterFormatTestData()
    {
        var data = new TheoryData<string, string>();
        data.Add("embeddedDoubleQuote", @"yyyy""MM""dd");
        data.Add("backslash", @"yyyy\\MM");
        data.Add("newline", "yyyy\nMM".Replace("\\n", "\n", StringComparison.Ordinal));
        return data;
    }

    /// <summary>
    /// Gets test data for editorconfig format values that fit on a single line.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, string> SpecialCharacterEditorConfigFormatTestData()
    {
        var data = new TheoryData<string, string>();
        data.Add("embeddedDoubleQuote", @"yyyy""MM""dd");
        data.Add("backslash", @"yyyy\\MM");
        return data;
    }

    /// <summary>
    /// Gets test data for culture names containing special characters.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, string> SpecialCharacterCultureNameTestData()
    {
        var data = new TheoryData<string, string>();
        data.Add("embeddedDoubleQuote", @"it""IT");
        data.Add("backslash", @"it\\IT");
        data.Add("newline", "it\nIT".Replace("\\n", "\n", StringComparison.Ordinal));
        return data;
    }

    /// <summary>
    /// Gets test data for editorconfig culture names that fit on a single line.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<string, string> SpecialCharacterEditorConfigCultureNameTestData()
    {
        var data = new TheoryData<string, string>();
        data.Add("embeddedDoubleQuote", @"it""IT");
        data.Add("backslash", @"it\\IT");
        return data;
    }

    /// <summary>
    /// Test string to <see cref="DateTime"/> mapping escapes special characters in a format defined on the method.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="format">The format string.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterFormatTestData))]
    [IntegrationTest]
    public async Task CanMapStringToDateTimeWithSpecialCharacterFormatDefinedInAttribute(string scenario, string format)
    {
        ArgumentNullException.ThrowIfNull(format);
        _ = scenario;

        const string identifierName = "__mappa_tmp_1";
        var formatInAttribute = EscapeForCSharpStringAttribute(format);

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(DateTimeFormat = "{{formatInAttribute}}", CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
                              public partial DateTime Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                                    "System.DateTime.ParseExact",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(format),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.InvariantCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test <see cref="int"/> to <see cref="string"/> mapping escapes special characters in a format defined on the method.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="format">The format string.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterFormatTestData))]
    [IntegrationTest]
    public async Task CanMapIntToStringWithSpecialCharacterFormatDefinedInAttribute(string scenario, string format)
    {
        ArgumentNullException.ThrowIfNull(format);
        _ = scenario;

        const string identifierName = "__mappa_tmp_1";
        var formatInAttribute = EscapeForCSharpStringAttribute(format);

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(IntFormat = "{{formatInAttribute}}")]
                              public partial string Map(int input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    "input.ToString",
                                    firstParameter => firstParameter.BeLiteralExpressionSyntax(format)));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test string to <see cref="int"/> mapping escapes special characters in a user-defined culture name on the method.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterCultureNameTestData))]
    [IntegrationTest]
    public async Task CanMapStringToIntWithSpecialCharacterCultureNameDefinedInAttribute(string scenario, string cultureName)
    {
        ArgumentNullException.ThrowIfNull(cultureName);
        _ = scenario;

        const string identifierName = "__mappa_tmp_1";
        var cultureNameInAttribute = EscapeForCSharpStringAttribute(cultureName);

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "{{cultureNameInAttribute}}")]
                              public partial int Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                                    secondParameter => secondParameter.BeInvocationExpressionSyntax(
                                        "System.Globalization.CultureInfo.GetCultureInfo",
                                        getCultureInfoParameter => getCultureInfoParameter.BeLiteralExpressionSyntax(cultureName))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test string to <see cref="Guid"/> mapping escapes special characters in a format defined on the method.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="format">The format string.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterFormatTestData))]
    [IntegrationTest]
    public async Task CanMapStringToGuidWithSpecialCharacterFormatDefinedInAttribute(string scenario, string format)
    {
        ArgumentNullException.ThrowIfNull(format);
        _ = scenario;

        const string identifierName = "__mappa_tmp_1";
        var formatInAttribute = EscapeForCSharpStringAttribute(format);

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(GuidFormat = "{{formatInAttribute}}")]
                              public partial Guid Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(Guid).ToString(),
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
                                typeof(Guid).ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    "System.Guid.ParseExact",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(format)));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test string to <see cref="DateTime"/> mapping escapes special characters in a format defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="format">The format string.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterEditorConfigFormatTestData))]
    [IntegrationTest]
    public async Task CanMapStringToDateTimeWithSpecialCharacterFormatDefinedInEditorConfig(string scenario, string format)
    {
        ArgumentNullException.ThrowIfNull(format);
        _ = scenario;

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.datetimeformat = {{format}}
                             mappa.cultureinfosettings = InvariantCulture
                             """;

        var sourceCode = """
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public partial DateTime Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                                    "System.DateTime.ParseExact",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(format),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.InvariantCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test string to <see cref="int"/> mapping escapes special characters in a culture name defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="cultureName">The culture name.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(SpecialCharacterEditorConfigCultureNameTestData))]
    [IntegrationTest]
    public async Task CanMapStringToIntWithSpecialCharacterCultureNameDefinedInEditorConfig(string scenario, string cultureName)
    {
        ArgumentNullException.ThrowIfNull(cultureName);
        _ = scenario;

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.cultureinfosettings = UserDefined
                             mappa.culturename = {{cultureName}}
                             """;

        var sourceCode = """
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public partial int Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                                    secondParameter => secondParameter.BeInvocationExpressionSyntax(
                                        "System.Globalization.CultureInfo.GetCultureInfo",
                                        getCultureInfoParameter => getCultureInfoParameter.BeLiteralExpressionSyntax(cultureName))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    private static string EscapeForCSharpStringAttribute(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value)
        {
            switch (character)
            {
                case '\\':
                    builder.Append("\\\\");
                    break;
                case '"':
                    builder.Append("\\\"");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                default:
                    builder.Append(character);
                    break;
            }
        }

        return builder.ToString();
    }
}