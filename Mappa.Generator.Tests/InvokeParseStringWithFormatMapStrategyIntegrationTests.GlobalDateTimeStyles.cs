// <copyright file="InvokeParseStringWithFormatMapStrategyIntegrationTests.GlobalDateTimeStyles.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Global date/time style integration tests for string parse strategies.
/// </summary>
public sealed partial class InvokeParseStringWithFormatMapStrategyIntegrationTests
{
    /// <summary>
    /// Test a mapping can be created when mapping a date/time type with global style defined on method.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithGlobalDateTimeStyleDefinedOnMethod(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = stylePropertyName;
        _ = editorConfigStyleKey;
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using System.Globalization;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(GlobalDateTimeStyle = DateTimeStyles.AllowWhiteSpaces)]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                targetType.ToString(),
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
                                targetType.ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{targetType.FullName}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(null),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.DateTimeStyles.AllowWhiteSpaces")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a date/time type with global style defined on class.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithGlobalDateTimeStyleDefinedOnClass(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = stylePropertyName;
        _ = editorConfigStyleKey;
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using System.Globalization;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings(GlobalDateTimeStyle = DateTimeStyles.AllowWhiteSpaces)]
                          public sealed partial class Mapper
                          {
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                targetType.ToString(),
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
                                targetType.ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{targetType.FullName}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(null),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.DateTimeStyles.AllowWhiteSpaces")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a date/time type with global style defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithGlobalDateTimeStyleDefinedInEditorConfig(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = stylePropertyName;
        _ = editorConfigStyleKey;
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = """
                           root = true

                           [*.cs]
                           mappa.globaldatetimestyle = AllowWhiteSpaces
                           """;

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                targetType.ToString(),
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
                                targetType.ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{targetType.FullName}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(null),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.DateTimeStyles.AllowWhiteSpaces")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test combined global date/time style flags defined in <c>.editorconfig</c> are emitted as a bitwise OR expression.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithCombinedGlobalDateTimeStyleFlagsDefinedInEditorConfig(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = stylePropertyName;
        _ = editorConfigStyleKey;
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = """
                           root = true

                           [*.cs]
                           mappa.globaldatetimestyle = AllowWhiteSpaces | AssumeUniversal
                           """;

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                targetType.ToString(),
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
                                targetType.ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{targetType.FullName}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(null),
                                    thirdParameter => thirdParameter.BeBinaryExpressionSyntax(
                                        left => left.BeMemberAccessExpressionSyntax("System.Globalization.DateTimeStyles.AllowWhiteSpaces"),
                                        SyntaxKind.BarToken,
                                        right => right.BeMemberAccessExpressionSyntax("System.Globalization.DateTimeStyles.AssumeUniversal"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test type-specific date/time style overrides global style when both are set.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetAndTypeSpecificDateTimeStyleOverridesGlobal(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = editorConfigStyleKey;
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentException.ThrowIfNullOrWhiteSpace(stylePropertyName);

        const string identifierName = "__mappa_tmp_1";

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using System.Globalization;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(
                                  GlobalDateTimeStyle = DateTimeStyles.AllowWhiteSpaces,
                                  {{stylePropertyName}} = DateTimeStyles.AssumeUniversal)]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                targetType.ToString(),
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
                                targetType.ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{targetType.FullName}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(null),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.DateTimeStyles.AssumeUniversal")));
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
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetAndMethodTypeSpecificDateTimeStyleOverridesGlobalAndEditorConfig(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = editorConfigStyleKey;
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentException.ThrowIfNullOrWhiteSpace(stylePropertyName);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = """
                           root = true

                           [*.cs]
                           mappa.globaldatetimestyle = AssumeUniversal
                           """;

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using System.Globalization;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings(GlobalDateTimeStyle = DateTimeStyles.AllowWhiteSpaces)]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{stylePropertyName}} = DateTimeStyles.AdjustToUniversal)]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                targetType.ToString(),
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
                                targetType.ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{targetType.FullName}.Parse",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(null),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.DateTimeStyles.AdjustToUniversal")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test invalid global date/time style values in <c>.editorconfig</c> fall back to the existing parse overload.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The type-specific style property name.</param>
    /// <param name="editorConfigStyleKey">The type-specific editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithInvalidGlobalDateTimeStyleInEditorConfigFallsBackToStandardParse(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = stylePropertyName;
        _ = editorConfigStyleKey;
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = """
                           root = true

                           [*.cs]
                           mappa.globaldatetimestyle = NotAValidDateTimeStyle
                           """;

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                targetType.ToString(),
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
                                targetType.ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{targetType.FullName}.Parse",
                                    syntaxAssertions => syntaxAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }
}