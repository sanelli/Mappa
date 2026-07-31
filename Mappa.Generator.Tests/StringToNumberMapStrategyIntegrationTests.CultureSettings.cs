// <copyright file="StringToNumberMapStrategyIntegrationTests.CultureSettings.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Culture and override integration tests for <see cref="Mappa.Generator.Models.Strategies.StringToNumberMapStrategy"/>.
/// </summary>
public sealed partial class StringToNumberMapStrategyIntegrationTests
{
    /// <summary>
    /// Test a mapping can be created from a string to a number using invariant culture.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithInvariantCulture(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = formatPropertyName;
        _ = editorConfigFormatKey;

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
                              public partial {{aliasNumericType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                numericType,
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
                                numericType,
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{aliasNumericType}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.InvariantCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from a string to a number using current culture.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithCurrentCulture(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = formatPropertyName;
        _ = editorConfigFormatKey;

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
                              public partial {{aliasNumericType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                numericType,
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
                                numericType,
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{aliasNumericType}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.CurrentCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from a string to a number using current culture defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithCurrentCultureDefinedInEditorConfig(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = formatPropertyName;
        _ = editorConfigFormatKey;

        const string identifierName = "__mappa_tmp_1";

        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.cultureinfosettings = CurrentCulture
                                    """;

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public partial {{aliasNumericType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                numericType,
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
                                numericType,
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{aliasNumericType}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.CurrentCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from a string to a number using user defined culture.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithUserDefinedCulture(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = formatPropertyName;
        _ = editorConfigFormatKey;

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                              public partial {{aliasNumericType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                numericType,
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
                                numericType,
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{aliasNumericType}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeInvocationExpressionSyntax(
                                        "System.Globalization.CultureInfo.GetCultureInfo",
                                        getCultureInfoParameter => getCultureInfoParameter.BeLiteralExpressionSyntax("it-IT"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test format settings do not affect numeric parsing and <c>Parse</c> is still used with culture.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithFormatAndCultureUsesParseNotParseExact(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = editorConfigFormatKey;

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{formatPropertyName}} = "N2", CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
                              public partial {{aliasNumericType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                numericType,
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
                                numericType,
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{aliasNumericType}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.InvariantCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test user defined culture without culture name falls back to plain parse.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithUserDefinedCultureWithoutCultureName(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = formatPropertyName;
        _ = editorConfigFormatKey;

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined)]
                              public partial {{aliasNumericType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.UserDefinedCultureIsMissingCultureName, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                numericType,
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
                                numericType,
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{aliasNumericType}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test method-level culture settings override class-level culture settings.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberAndMethodCultureOverridesClassCulture(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = formatPropertyName;
        _ = editorConfigFormatKey;

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                              public partial {{aliasNumericType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                numericType,
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
                                numericType,
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{aliasNumericType}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeInvocationExpressionSyntax(
                                        "System.Globalization.CultureInfo.GetCultureInfo",
                                        getCultureInfoParameter => getCultureInfoParameter.BeLiteralExpressionSyntax("it-IT"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test class-level culture settings override editorconfig culture settings.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberAndClassCultureOverridesEditorConfigCulture(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = formatPropertyName;
        _ = editorConfigFormatKey;

        const string identifierName = "__mappa_tmp_1";

        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.cultureinfosettings = InvariantCulture
                                    """;

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
                          public sealed partial class Mapper
                          {
                              public partial {{aliasNumericType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                numericType,
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
                                numericType,
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{aliasNumericType}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.CurrentCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test method-level culture settings override class-level and editorconfig culture settings.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberAndMethodCultureOverridesClassAndEditorConfigCulture(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = formatPropertyName;
        _ = editorConfigFormatKey;

        const string identifierName = "__mappa_tmp_1";

        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.cultureinfosettings = UserDefined
                                    mappa.culturename = de-DE
                                    """;

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "fr-FR")]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                              public partial {{aliasNumericType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                numericType,
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
                                numericType,
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{aliasNumericType}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeInvocationExpressionSyntax(
                                        "System.Globalization.CultureInfo.GetCultureInfo",
                                        getCultureInfoParameter => getCultureInfoParameter.BeLiteralExpressionSyntax("it-IT"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }
}