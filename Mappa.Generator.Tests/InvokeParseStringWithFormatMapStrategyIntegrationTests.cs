// <copyright file="InvokeParseStringWithFormatMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="InvokeParseStringWithFormatMapStrategy"/>.
/// </summary>
public sealed partial class InvokeParseStringWithFormatMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/>.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(DateOnly))]
    [InlineData(typeof(TimeOnly))]
    [InlineData(typeof(TimeSpan))]
    [IntegrationTest]
    public async Task CanMapStringToTargetTypeWithoutSettings(Type targetType)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
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

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
    /// to a <paramref name="targetType"/> with only culture settings.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(DateOnly))]
    [InlineData(typeof(TimeOnly))]
    [InlineData(typeof(TimeSpan))]
    [IntegrationTest]
    public async Task CanMapStringToTargetTypeWithCultureSettings(Type targetType)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
                                    firstArgument => firstArgument.BeIdentifierNameSyntax("input"),
                                    secondArgument => secondArgument.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.CurrentCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with only format.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <param name="parseExact"><c>true</c> if the <paramref name="targetType"/> support <c>ParseExact(string,string)</c>.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d", false)]
    [InlineData(typeof(DateTimeOffset), "d", false)]
    [InlineData(typeof(DateOnly), "d", true)]
    [InlineData(typeof(TimeOnly), "t", true)]
    [InlineData(typeof(TimeSpan), "c", false)]
    [IntegrationTest]
    public async Task CanMapStringToTargetUsingStandardParseWhenOnlyFormatIsProvided(
        Type targetType,
        string format,
        bool parseExact)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "{{format}}")]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var warnings = parseExact ? Array.Empty<string>() : new[] { "MP00013" };

        // Assert
        generatedResults.Should()
            .HaveOnlyWarnings(warnings)
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
                                expressionSyntaxAssertions =>
                                {
                                    if (parseExact)
                                    {
                                        expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                            $"{targetType.FullName}.ParseExact",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                            secondParameter => secondParameter.BeLiteralExpressionSyntax(format));
                                    }
                                    else
                                    {
                                        expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                            $"{targetType.FullName}.Parse",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("input"));
                                    }
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with only format provided
    /// on class attribute.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <param name="parseExact"><c>true</c> if the <paramref name="targetType"/> support <c>ParseExact(string,string)</c>.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d", false)]
    [InlineData(typeof(DateTimeOffset), "d", false)]
    [InlineData(typeof(DateOnly), "d", true)]
    [InlineData(typeof(TimeOnly), "t", true)]
    [InlineData(typeof(TimeSpan), "c", false)]
    [IntegrationTest]
    public async Task CanMapStringToTargetUsingStandardParseWhenOnlyFormatIsProvidedOnClass(
        Type targetType,
        string format,
        bool parseExact)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "{{format}}")]
                          public sealed partial class Mapper
                          {
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var warnings = parseExact ? Array.Empty<string>() : new[] { "MP00013" };

        // Assert
        generatedResults.Should()
            .HaveOnlyWarnings(warnings)
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
                                expressionSyntaxAssertions =>
                                {
                                    if (parseExact)
                                    {
                                        expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                            $"{targetType.FullName}.ParseExact",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                            secondParameter => secondParameter.BeLiteralExpressionSyntax(format));
                                    }
                                    else
                                    {
                                        expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                            $"{targetType.FullName}.Parse",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("input"));
                                    }
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with format on method that replace
    /// format on class.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapStringToTargetAndFormatOnMethodReplaceFormatOnClass(Type targetType, string format)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "bad")]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.CurrentCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with format on method that replace
    /// format on class. Format on method is empty forcing to use
    /// <c>Parse(string,IFormatProvider)</c>.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapStringToTargetAndEmptyFormatOnMethodReplaceFormatOnClassAndForceUsageOfParse(Type targetType, string format)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "{{format}}")]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "", CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
                                    secondParameter => secondParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.CurrentCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with format and current culture.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapStringToTargetUsingParseExactAndCurrentCulture(Type targetType, string format)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.CurrentCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with format and invariant culture.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapStringToTargetUsingParseExactAndInvariantCulture(Type targetType, string format)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.InvariantCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with format and user defined culture.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapStringToTargetUsingParseExactAndUserDefinedCulture(Type targetType, string format)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
                                    thirdParameter => thirdParameter.BeInvocationExpressionSyntax(
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
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with format and user defined culture
    /// but without culture name, resulting in current culture being used.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <param name="parseExact"><c>true</c> if the <paramref name="targetType"/> support <c>ParseExact(string,string)</c>.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d", false)]
    [InlineData(typeof(DateTimeOffset), "d", false)]
    [InlineData(typeof(DateOnly), "d", true)]
    [InlineData(typeof(TimeOnly), "t", true)]
    [InlineData(typeof(TimeSpan), "c", false)]
    [IntegrationTest]
    public async Task CanMapStringToTargetUsingParseExactAndUserDefinedCultureWithoutCultureName(
        Type targetType,
        string format,
        bool parseExact)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.UserDefined)]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var warnings = parseExact ? new[] { "MP00012" } : new[] { "MP00012", "MP00013" };

        // Assert
        generatedResults.Should()
            .HaveOnlyWarnings(warnings)
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
                                expressionSyntaxAssertions =>
                                {
                                    if (parseExact)
                                    {
                                        expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                            $"{targetType.FullName}.ParseExact",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                            secondParameter => secondParameter.BeLiteralExpressionSyntax(format));
                                    }
                                    else
                                    {
                                        expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                            $"{targetType.FullName}.Parse",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("input"));
                                    }
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with format and user defined culture.
    /// Culture info settings on method override culture info settings on class.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapStringToTargetAndMethodClassInfoOverridesClassClassInfo(Type targetType, string format)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
                                    thirdParameter => thirdParameter.BeInvocationExpressionSyntax(
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
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with format and user defined culture.
    /// Culture name on method overrides culture name on class.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapStringToTargetAndMethodCultureNameOverridesClassCultureName(Type targetType, string format)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using System;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{targetType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "de-DE")]
                          public sealed partial class Mapper
                          {
                              [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                              public partial {{targetType}} Map(string input);
                          }
                          """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
                                    thirdParameter => thirdParameter.BeInvocationExpressionSyntax(
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
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with only format defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <param name="editorConfigFormatKey">The <c>.editorconfig</c> key suffix for the format setting.</param>
    /// <param name="parseExact"><c>true</c> if the <paramref name="targetType"/> support <c>ParseExact(string,string)</c>.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d", "datetimeformat", false)]
    [InlineData(typeof(DateTimeOffset), "d", "datetimeoffsetformat", false)]
    [InlineData(typeof(DateOnly), "d", "dateonlyformat", true)]
    [InlineData(typeof(TimeOnly), "t", "timeonlyformat", true)]
    [InlineData(typeof(TimeSpan), "c", "timespanformat", false)]
    [IntegrationTest]
    public async Task CanMapStringToTargetUsingStandardParseWhenOnlyFormatIsProvidedInEditorConfig(
        Type targetType,
        string format,
        string editorConfigFormatKey,
        bool parseExact)
    {
        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigFormatKey);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigFormatKey}} = {{format}}
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

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        var warnings = parseExact ? Array.Empty<string>() : new[] { "MP00013" };

        // Assert
        generatedResults.Should()
            .HaveOnlyWarnings(warnings)
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
                                expressionSyntaxAssertions =>
                                {
                                    if (parseExact)
                                    {
                                        expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                            $"{targetType.FullName}.ParseExact",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                            secondParameter => secondParameter.BeLiteralExpressionSyntax(format));
                                    }
                                    else
                                    {
                                        expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                            $"{targetType.FullName}.Parse",
                                            firstParameter => firstParameter.BeIdentifierNameSyntax("input"));
                                    }
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test class-level format settings override format settings defined in <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToDateOnlyAndFormatInEditorConfigIsOverriddenByClassAttribute()
    {
        const string identifierName = "__mappa_tmp_1";

        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.dateonlyformat = bad
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(DateOnlyFormat = "d")]
                                  public sealed partial class Mapper
                                  {
                                      public partial DateOnly Map(string input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(DateOnly).ToString(),
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
                                typeof(DateOnly).ToString(),
                                identifierName,
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeInvocationExpressionSyntax(
                                    $"{typeof(DateOnly).FullName}.ParseExact",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax("d")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created when mapping a <see cref="string"/>
    /// to a <paramref name="targetType"/> with format and invariant culture defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="targetType">The target of the mapping.</param>
    /// <param name="format">The format.</param>
    /// <param name="editorConfigFormatKey">The <c>.editorconfig</c> key suffix for the format setting.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime), "d", "datetimeformat")]
    [InlineData(typeof(DateTimeOffset), "d", "datetimeoffsetformat")]
    [InlineData(typeof(DateOnly), "d", "dateonlyformat")]
    [InlineData(typeof(TimeOnly), "t", "timeonlyformat")]
    [InlineData(typeof(TimeSpan), "c", "timespanformat")]
    [IntegrationTest]
    public async Task CanMapStringToTargetUsingParseExactAndInvariantCultureDefinedInEditorConfig(
        Type targetType,
        string format,
        string editorConfigFormatKey)
    {
        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);
        ArgumentException.ThrowIfNullOrWhiteSpace(editorConfigFormatKey);

        const string identifierName = "__mappa_tmp_1";

        var editorConfig = $$"""
                             root = true

                             [*.cs]
                             mappa.{{editorConfigFormatKey}} = {{format}}
                             mappa.cultureinfosettings = InvariantCulture
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

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
                                    thirdParameter => thirdParameter.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.InvariantCulture")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test class-level culture settings override culture settings defined in <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToDateTimeAndCultureInEditorConfigIsOverriddenByClassAttribute()
    {
        const string identifierName = "__mappa_tmp_1";

        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.datetimeformat = d
                                    mappa.cultureinfosettings = InvariantCulture
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                                  public sealed partial class Mapper
                                  {
                                      public partial DateTime Map(string input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        // Assert
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
                                    $"{typeof(DateTime).FullName}.ParseExact",
                                    firstParameter => firstParameter.BeIdentifierNameSyntax("input"),
                                    secondParameter => secondParameter.BeLiteralExpressionSyntax("d"),
                                    thirdParameter => thirdParameter.BeInvocationExpressionSyntax(
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