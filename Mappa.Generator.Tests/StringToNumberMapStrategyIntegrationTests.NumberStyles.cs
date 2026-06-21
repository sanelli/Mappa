// <copyright file="StringToNumberMapStrategyIntegrationTests.NumberStyles.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Number style integration tests for <see cref="Mappa.Generator.Models.Strategies.StringToNumberMapStrategy"/>.
/// </summary>
public sealed partial class StringToNumberMapStrategyIntegrationTests
{
    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a numeric type with style defined on method.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithNumberStyleDefinedOnMethod(
        string aliasNumericType,
        string numericType,
        string stylePropertyName,
        string editorConfigStyleKey)
    {
        _ = editorConfigStyleKey;

        ArgumentException.ThrowIfNullOrWhiteSpace(aliasNumericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(numericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(stylePropertyName);

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using System.Globalization;
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{stylePropertyName}} = NumberStyles.AllowThousands)]
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
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.AllowThousands")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a numeric type with style defined on class.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithNumberStyleDefinedOnClass(
        string aliasNumericType,
        string numericType,
        string stylePropertyName,
        string editorConfigStyleKey)
    {
        _ = editorConfigStyleKey;

        ArgumentException.ThrowIfNullOrWhiteSpace(aliasNumericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(numericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(stylePropertyName);

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using System.Globalization;
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{stylePropertyName}} = NumberStyles.AllowThousands)]
                          public sealed partial class Mapper
                          {
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
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.AllowThousands")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a numeric type with style defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithNumberStyleDefinedInEditorConfig(
        string aliasNumericType,
        string numericType,
        string stylePropertyName,
        string editorConfigStyleKey)
    {
        _ = stylePropertyName;

        ArgumentException.ThrowIfNullOrWhiteSpace(aliasNumericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(numericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigStyleKey);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigStyleKey}} = AllowThousands
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
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.AllowThousands")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test combined number style flags defined in <c>.editorconfig</c> are emitted as a bitwise OR expression.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithCombinedNumberStyleFlagsDefinedInEditorConfig(
        string aliasNumericType,
        string numericType,
        string stylePropertyName,
        string editorConfigStyleKey)
    {
        _ = stylePropertyName;

        ArgumentException.ThrowIfNullOrWhiteSpace(aliasNumericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(numericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigStyleKey);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigStyleKey}} = AllowThousands | AllowParentheses
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
                                    secondParameter => secondParameter.BeBinaryExpressionSyntax(
                                        left => left.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.AllowParentheses"),
                                        SyntaxKind.BarToken,
                                        right => right.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.AllowThousands"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test method-level style settings override class and <c>.editorconfig</c> style settings.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberAndMethodNumberStyleOverridesClassAndEditorConfig(
        string aliasNumericType,
        string numericType,
        string stylePropertyName,
        string editorConfigStyleKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(aliasNumericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(numericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigStyleKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(stylePropertyName);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigStyleKey}} = AllowLeadingWhite
                             """;

        var sourceCode = $$"""
                          #nullable enable
                          using System.Globalization;
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{stylePropertyName}} = NumberStyles.AllowThousands)]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{stylePropertyName}} = NumberStyles.AllowTrailingWhite)]
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
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.AllowTrailingWhite")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test class-level style settings override style settings defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberAndClassNumberStyleOverridesEditorConfig(
        string aliasNumericType,
        string numericType,
        string stylePropertyName,
        string editorConfigStyleKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(aliasNumericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(numericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigStyleKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(stylePropertyName);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigStyleKey}} = AllowLeadingWhite
                             """;

        var sourceCode = $$"""
                          #nullable enable
                          using System.Globalization;
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{stylePropertyName}} = NumberStyles.AllowThousands)]
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
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.AllowThousands")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test unset number style preserves the existing single-argument <c>Parse</c> overload.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithoutNumberStylePreservesExistingParseOverload(
        string aliasNumericType,
        string numericType,
        string stylePropertyName,
        string editorConfigStyleKey)
    {
        _ = stylePropertyName;
        _ = editorConfigStyleKey;

        ArgumentException.ThrowIfNullOrWhiteSpace(aliasNumericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(numericType);

        const string identifierName = "__mappa_tmp_1";

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
                                    syntaxAssertions => syntaxAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test invalid number style values in <c>.editorconfig</c> fall back to the existing parse overload.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithInvalidNumberStyleInEditorConfigFallsBackToStandardParse(
        string aliasNumericType,
        string numericType,
        string stylePropertyName,
        string editorConfigStyleKey)
    {
        _ = stylePropertyName;

        ArgumentException.ThrowIfNullOrWhiteSpace(aliasNumericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(numericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigStyleKey);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigStyleKey}} = NotAValidNumberStyle
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
                                    syntaxAssertions => syntaxAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a numeric type with style and invariant culture.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithNumberStyleAndInvariantCulture(
        string aliasNumericType,
        string numericType,
        string stylePropertyName,
        string editorConfigStyleKey)
    {
        _ = editorConfigStyleKey;

        ArgumentException.ThrowIfNullOrWhiteSpace(aliasNumericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(numericType);
        ArgumentException.ThrowIfNullOrWhiteSpace(stylePropertyName);

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using System.Globalization;
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{stylePropertyName}} = NumberStyles.AllowThousands, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
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
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.AllowThousands"),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.InvariantCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }
}