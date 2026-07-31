// <copyright file="InvokeToStringMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="InvokeToStringMapStrategy"/>.
/// </summary>
public sealed partial class InvokeToStringMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString(string)</c> for
    /// <paramref name="sourceType"/>.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(DateOnly))]
    [InlineData(typeof(TimeOnly))]
    [InlineData(typeof(TimeSpan))]
    [IntegrationTest]
    public async Task CanMapToString(Type sourceType)
    {
        ArgumentNullException.ThrowIfNull(sourceType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
                                    expressionSyntaxAssertions.BeInvocationExpressionSyntax("input.ToString");
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString(string)</c> and format defined on method.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(Guid), "N")]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapToStringWithFormatDefinedOnMethod(Type sourceType, string format)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{sourceType.ToString().Split(".")[^1]}}Format = "{{format}}")]
                              public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString(string,IFormatProvider)</c> and format defined on method.
    /// The provided format is from <see cref="CultureInfo.InvariantCulture"/>.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(Guid), "N")]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapToStringWithFormatAndInvariantCultureDefinedOnMethod(Type sourceType, string format)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using Mappa;
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{sourceType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
                              public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString(string,IFormatProvider)</c> and format defined on method.
    /// The provided format is from <see cref="CultureInfo.CurrentCulture"/>.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(Guid), "N")]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapToStringWithFormatAndCurrentCultureDefinedOnMethod(Type sourceType, string format)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings({{sourceType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
                               public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
                                        secondParameterAssertions => secondParameterAssertions.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.CurrentCulture"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>TToString(string,IFormatProvider)</c> and format defined on method.
    /// The format provider is user defined.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(Guid), "N")]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapToStringWithFormatAndUserDefinedCultureDefinedOnMethod(Type sourceType, string format)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings({{sourceType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                               public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString(string,IFormatProvider)</c> and format defined on method.
    /// The provided format is user defined but the culture is missing.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(Guid), "N")]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapToStringWithFormatAndUserDefinedCultureButCultureNameIsMissingDefinedOnMethod(Type sourceType, string format)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings({{sourceType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.UserDefined)]
                               public partial string Map({{sourceType}} input);
                           }
                           """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.UserDefinedCultureIsMissingCultureName, "Map")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString()</c> when only culture info is defined.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(Guid))]
    [InlineData(typeof(TimeSpan))]
    [IntegrationTest]
    public async Task CanMapToStringWithoutFormatInvokesPlainToString(Type sourceType)
    {
        ArgumentNullException.ThrowIfNull(sourceType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
                               public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
                                        "input.ToString");
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString(IFormatProvider)</c> when only culture info is defined.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(DateTimeOffset))]
    [InlineData(typeof(DateOnly))]
    [InlineData(typeof(TimeOnly))]
    [InlineData(typeof(int))]
    [InlineData(typeof(decimal))]
    [IntegrationTest]
    public async Task CanMapToStringWithOnlyFormatProviderInvokesToStringWithFormatProviderOnly(Type sourceType)
    {
        ArgumentNullException.ThrowIfNull(sourceType);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa;
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
                               public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
                                        firstParameterAssertions => firstParameterAssertions.BeMemberAccessExpressionSyntax("System.Globalization.CultureInfo.InvariantCulture"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString(string)</c> and format defined on class.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(Guid), "N")]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapToStringWithFormatDefinedOnClass(Type sourceType, string format)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
        ArgumentNullException.ThrowIfNull(format);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                          #nullable enable
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          [MappaSettings({{sourceType.ToString().Split(".")[^1]}}Format = "{{format}}"]
                          public sealed partial class Mapper
                          {
                              public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString(string,IFormatProvider)</c> and format defined on method
    /// override the format and culture name defined on class.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(Guid), "N")]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanMapToStringWithFormatAndCultureNameDefinedOnMethodOverrideTheSetupOnClass(Type sourceType, string format)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
        ArgumentNullException.ThrowIfNull(format);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           [Mappa]
                           [MappaSettings({{sourceType.ToString().Split(".")[^1]}}Format = "X", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "en-US")]
                           public sealed partial class Mapper
                           {
                               [MappaSettings({{sourceType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                               public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString(string,IFormatProvider)</c> and culture into setting defined on method
    /// override the culture info setting name defined on class.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <param name="format">The format.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(Guid), "N")]
    [InlineData(typeof(DateTime), "d")]
    [InlineData(typeof(DateTimeOffset), "d")]
    [InlineData(typeof(DateOnly), "d")]
    [InlineData(typeof(TimeOnly), "t")]
    [InlineData(typeof(TimeSpan), "c")]
    [IntegrationTest]
    public async Task CanGuidToStringWithCultureInfoSettingDefinedOnMethodOverrideTheSetupOnClass(Type sourceType, string format)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
        ArgumentNullException.ThrowIfNull(format);

        const string identifierName = "__mappa_tmp_1";

        // Arrange
        var sourceCode = $$"""
                           #nullable enable
                           using Mappa.Attributes;

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           [Mappa]
                           [MappaSettings({{sourceType.ToString().Split(".")[^1]}}Format = "X", CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
                           public sealed partial class Mapper
                           {
                               [MappaSettings({{sourceType.ToString().Split(".")[^1]}}Format = "{{format}}", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                               public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString(string)</c> and format defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
    /// <param name="format">The format.</param>
    /// <param name="editorConfigFormatKey">The <c>.editorconfig</c> key suffix for the format setting.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(typeof(Guid), "N", "guidformat")]
    [InlineData(typeof(DateTime), "d", "datetimeformat")]
    [InlineData(typeof(DateTimeOffset), "d", "datetimeoffsetformat")]
    [InlineData(typeof(DateOnly), "d", "dateonlyformat")]
    [InlineData(typeof(TimeOnly), "t", "timeonlyformat")]
    [InlineData(typeof(TimeSpan), "c", "timespanformat")]
    [IntegrationTest]
    public async Task CanMapToStringWithFormatDefinedInEditorConfig(Type sourceType, string format, string editorConfigFormatKey)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
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
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
    /// Test class-level <see cref="MappaSettingsAttribute.GuidFormat"/> overrides
    /// the format defined in <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToStringWithGuidFormatInEditorConfigOverriddenByClassAttribute()
    {
        const string identifierName = "__mappa_tmp_1";

        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.guidformat = N
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(GuidFormat = "D")]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(System.Guid input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Guid).ToString(),
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
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax("D"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created to <see cref="string"/>
    /// by using the <c>T.ToString(string,IFormatProvider)</c> with format and culture
    /// defined in <c>.editorconfig</c>.
    /// </summary>
    /// <param name="sourceType">The type of the source.</param>
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
    public async Task CanMapToStringWithFormatAndInvariantCultureDefinedInEditorConfig(
        Type sourceType,
        string format,
        string editorConfigFormatKey)
    {
        ArgumentNullException.ThrowIfNull(sourceType);
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
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              public partial string Map({{sourceType}} input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                sourceType.ToString(),
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
    /// Test class-level culture settings override culture settings defined in <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapToStringWithCultureInEditorConfigOverriddenByClassAttribute()
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
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(System.DateTime input);
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
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(DateTime).ToString(),
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
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax("d"),
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