// <copyright file="InvokeParseStringWithFormatMapStrategyIntegrationTests.DateTimeStyles.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Date/time style integration tests for string parse strategies.
/// </summary>
public sealed partial class InvokeParseStringWithFormatMapStrategyIntegrationTests
{
    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a date/time type with style defined on method.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithDateTimeStyleDefinedOnMethod(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
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
                              [MappaSettings({{stylePropertyName}} = DateTimeStyles.AllowWhiteSpaces)]
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
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a date/time type with style defined on class.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithDateTimeStyleDefinedOnClass(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
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
                          [MappaSettings({{stylePropertyName}} = DateTimeStyles.AllowWhiteSpaces)]
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
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a date/time type with style defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithDateTimeStyleDefinedInEditorConfig(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = stylePropertyName;
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigStyleKey);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigStyleKey}} = AllowWhiteSpaces
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
    /// Test combined date/time style flags defined in <c>.editorconfig</c> are emitted as a bitwise OR expression.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithCombinedDateTimeStyleFlagsDefinedInEditorConfig(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = stylePropertyName;
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigStyleKey);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigStyleKey}} = AllowWhiteSpaces | AssumeUniversal
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
    /// Test method-level style settings override class and <c>.editorconfig</c> style settings.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetAndMethodDateTimeStyleOverridesClassAndEditorConfig(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigStyleKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(stylePropertyName);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigStyleKey}} = AssumeUniversal
                             """;

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using System.Globalization;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{stylePropertyName}} = DateTimeStyles.AllowWhiteSpaces)]
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
    /// Test class-level style settings override style settings defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetAndClassDateTimeStyleOverridesEditorConfig(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigStyleKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(stylePropertyName);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigStyleKey}} = AssumeUniversal
                             """;

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using System.Globalization;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{stylePropertyName}} = DateTimeStyles.AllowWhiteSpaces)]
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
    /// Test unset date/time style preserves the existing single-argument <c>Parse</c> overload.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithoutDateTimeStylePreservesExistingParseOverload(
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
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
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
                                    syntaxAssertions => syntaxAssertions.BeIdentifierNameSyntax("input")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test invalid date/time style values in <c>.editorconfig</c> fall back to the existing parse overload.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithInvalidDateTimeStyleInEditorConfigFallsBackToStandardParse(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = stylePropertyName;
        _ = format;

        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigStyleKey);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigStyleKey}} = NotAValidDateTimeStyle
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

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a date/time type with style and current culture.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithDateTimeStyleAndCurrentCulture(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
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
                              [MappaSettings({{stylePropertyName}} = DateTimeStyles.AllowWhiteSpaces, CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
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
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.CurrentCulture"),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.DateTimeStyles.AllowWhiteSpaces")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a date/time type with style, format, and invariant culture.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="stylePropertyName">The style property name.</param>
    /// <param name="editorConfigStyleKey">The editorconfig style key.</param>
    /// <param name="format">The default format for combined settings tests.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [MemberData(nameof(DateTimeStylesMappaSettingsTestHelper.DateTimeTypeTestData), MemberType = typeof(DateTimeStylesMappaSettingsTestHelper))]
    [IntegrationTest]
    public async Task CanMapStringToTargetWithDateTimeStyleFormatAndInvariantCulture(
        Type targetType,
        string stylePropertyName,
        string editorConfigStyleKey,
        string format)
    {
        _ = editorConfigStyleKey;

        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentException.ThrowIfNullOrWhiteSpace(stylePropertyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        const string identifierName = "__mappa_tmp_1";
        var formatPropertyName = targetType.ToString().Split(".")[^1] + "Format";

        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using System.Globalization;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{formatPropertyName}} = "{{format}}", {{stylePropertyName}} = DateTimeStyles.AllowWhiteSpaces, CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
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
                                    $"{targetType.FullName}.ParseExact",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax(format),
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.InvariantCulture"),
                                    fourthParameter => fourthParameter.BeMemberAccessExpressionSyntax("System.Globalization.DateTimeStyles.AllowWhiteSpaces")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }
}