// <copyright file="StringToNumberMapStrategyIntegrationTests.GlobalNumberStyles.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Global number style integration tests for <see cref="Mappa.Generator.Models.Strategies.StringToNumberMapStrategy"/>.
/// </summary>
public sealed partial class StringToNumberMapStrategyIntegrationTests
{
    /// <summary>
    /// Test a mapping can be created when mapping a numeric type with global style defined on method.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithGlobalNumberStyleDefinedOnMethod(
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
                          using System.Globalization;
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(GlobalNumberStyle = NumberStyles.AllowThousands)]
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
    /// Test a mapping can be created when mapping a numeric type with global style defined on class.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithGlobalNumberStyleDefinedOnClass(
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
                          using System.Globalization;
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings(GlobalNumberStyle = NumberStyles.AllowThousands)]
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
    /// Test a mapping can be created when mapping a numeric type with global style defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithGlobalNumberStyleDefinedInEditorConfig(
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

        var editorConfig = """
                           root = true

                           [*.cs]
                           mappa.globalnumberstyle = AllowThousands
                           """;

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
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
    /// Test combined global number style flags defined in <c>.editorconfig</c> are emitted as a bitwise OR expression.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithCombinedGlobalNumberStyleFlagsDefinedInEditorConfig(
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

        var editorConfig = """
                           root = true

                           [*.cs]
                           mappa.globalnumberstyle = AllowThousands | AllowParentheses
                           """;

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
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
    /// Test type-specific number style overrides global style when both are set.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberAndTypeSpecificNumberStyleOverridesGlobal(
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
                              [MappaSettings(
                                  GlobalNumberStyle = NumberStyles.AllowThousands,
                                  {{stylePropertyName}} = NumberStyles.AllowParentheses)]
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
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.AllowParentheses")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test method-level type-specific style overrides global style from class and <c>.editorconfig</c>.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberAndMethodTypeSpecificNumberStyleOverridesGlobalAndEditorConfig(
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

        var editorConfig = """
                           root = true

                           [*.cs]
                           mappa.globalnumberstyle = AllowThousands
                           """;

        var sourceCode = $$"""
                          #nullable enable
                          using System.Globalization;
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings(GlobalNumberStyle = NumberStyles.AllowLeadingWhite)]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{stylePropertyName}} = NumberStyles.AllowParentheses)]
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
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.NumberStyles.AllowParentheses")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test invalid global number style values in <c>.editorconfig</c> fall back to the existing parse overload.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithInvalidGlobalNumberStyleInEditorConfigFallsBackToStandardParse(
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

        var editorConfig = """
                           root = true

                           [*.cs]
                           mappa.globalnumberstyle = NotAValidNumberStyle
                           """;

        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
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
    /// Test combined global number style flags defined on the method are emitted as a bitwise OR expression.
    /// </summary>
    /// <param name="aliasNumericType">The type alias.</param>
    /// <param name="numericType">The type full name.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(NumberStylesMappaSettingsTestHelper.NumericTypeTestData), MemberType = typeof(NumberStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToNumberWithCombinedGlobalNumberStyleFlagsDefinedOnMethod(
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
                          using System.Globalization;
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(GlobalNumberStyle = NumberStyles.AllowThousands | NumberStyles.AllowParentheses)]
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
}