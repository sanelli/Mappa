// <copyright file="InvokeToStringMapStrategyIntegrationTests.NumericSettings.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Numeric format and culture integration tests for <see cref="Mappa.Generator.Models.Strategies.InvokeToStringMapStrategy"/>.
/// </summary>
public sealed partial class InvokeToStringMapStrategyIntegrationTests
{
    /// <summary>
    /// Test a mapping can be created to <see cref="string"/> using numeric format defined on method.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapNumericToStringWithFormatDefinedOnMethod(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = editorConfigFormatKey;

        const string format = "N2";
        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{formatPropertyName}} = "{{format}}")]
                              public partial string Map({{aliasNumericType}} input);
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
                numericType,
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
                                expressionSyntaxAssertions =>
                                {
                                    expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                        "input.ToString",
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax(format));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created to <see cref="string"/> using numeric format and invariant culture defined on method.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapNumericToStringWithFormatAndInvariantCultureDefinedOnMethod(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = editorConfigFormatKey;

        const string format = "N2";
        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{formatPropertyName}} = "{{format}}", CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
                              public partial string Map({{aliasNumericType}} input);
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
                numericType,
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
                                expressionSyntaxAssertions =>
                                {
                                    expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                        "input.ToString",
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax(format),
                                        secondParameterAssertions => secondParameterAssertions.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.InvariantCulture"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test user defined culture without culture name falls back to format-only <c>ToString</c>.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapNumericToStringWithFormatAndUserDefinedCultureButCultureNameIsMissingDefinedOnMethod(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = editorConfigFormatKey;

        const string format = "N2";
        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{formatPropertyName}} = "{{format}}", CultureInfoSetting = CultureInfoSetting.UserDefined)]
                              public partial string Map({{aliasNumericType}} input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveOnlyWarnings("MP00012")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                numericType,
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
                                expressionSyntaxAssertions =>
                                {
                                    expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                        "input.ToString",
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax(format));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test method-level format and culture settings override class-level settings.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapNumericToStringWithFormatAndCultureDefinedOnMethodOverrideTheSetupOnClass(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = editorConfigFormatKey;

        const string format = "N2";
        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{formatPropertyName}} = "X", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "en-US")]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{formatPropertyName}} = "{{format}}", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                              public partial string Map({{aliasNumericType}} input);
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
                numericType,
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
                                expressionSyntaxAssertions =>
                                {
                                    expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                        "input.ToString",
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax(format),
                                        secondParameterAssertions => secondParameterAssertions.BeInvocationExpressionSyntax(
                                            "System.Globalization.CultureInfo.GetCultureInfo",
                                            getCultureParametersAssertions => getCultureParametersAssertions.BeLiteralExpressionSyntax("it-IT")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created to <see cref="string"/> using numeric format defined in editorconfig.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapNumericToStringWithFormatDefinedInEditorConfig(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        _ = formatPropertyName;

        const string format = "N2";
        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigFormatKey}} = {{format}}
                             """;

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public partial string Map({{aliasNumericType}} input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                numericType,
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
                                expressionSyntaxAssertions =>
                                {
                                    expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                        "input.ToString",
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax(format));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test class-level numeric format overrides editorconfig format.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapNumericToStringWithFormatInEditorConfigOverriddenByClassAttribute(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        const string format = "N2";
        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigFormatKey}} = bad
                             """;

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{formatPropertyName}} = "{{format}}")]
                          public sealed partial class Mapper
                          {
                              public partial string Map({{aliasNumericType}} input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                numericType,
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
                                expressionSyntaxAssertions =>
                                {
                                    expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                        "input.ToString",
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax(format));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test method-level format and culture settings override class-level and editorconfig settings.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="formatPropertyName">The format property name.</param>
    /// <param name="editorConfigFormatKey">The editorconfig format key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumericMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumericMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapNumericToStringWithMethodFormatAndCultureOverridesClassAndEditorConfig(
        string aliasNumericType,
        string numericType,
        string formatPropertyName,
        string editorConfigFormatKey)
    {
        const string format = "N2";
        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigFormatKey}} = bad
                             mappa.cultureinfosettings = UserDefined
                             mappa.culturename = de-DE
                             """;

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{formatPropertyName}} = "X", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "fr-FR")]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{formatPropertyName}} = "{{format}}", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                              public partial string Map({{aliasNumericType}} input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                numericType,
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
                                expressionSyntaxAssertions =>
                                {
                                    expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                        "input.ToString",
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax(format),
                                        secondParameterAssertions => secondParameterAssertions.BeInvocationExpressionSyntax(
                                            "System.Globalization.CultureInfo.GetCultureInfo",
                                            getCultureParametersAssertions => getCultureParametersAssertions.BeLiteralExpressionSyntax("it-IT")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }
}