// <copyright file="InvokeToStringMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Globalization;

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

// TODO [#56] Add tests for all types but Guid/TimeSpan for testing the support to ToString(IFormatProvider).

/// <summary>
/// Integration tests for the <see cref="InvokeToStringMapStrategy"/>.
/// </summary>
public sealed class InvokeToStringMapStrategyIntegrationTests
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
                NullableAnnotation.None,
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
                          using Mappa.Attributes;

                          namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                          [Mappa]
                          public sealed partial class Mapper
                          {
                              [MappaSettings({{sourceType.ToString().Split(".")[^1]}}Format = "{{format}}"]
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
                NullableAnnotation.None,
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
                NullableAnnotation.None,
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
                NullableAnnotation.None,
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
                NullableAnnotation.None,
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

    // TODO [#56] Make subsequent tests Theory so they can re-use the same tests for all the different supported types.

    /// <summary>
    /// Test a mapping can be created from <see cref="Guid"/> to <see cref="string"/>
    /// by using the <see cref="Guid.ToString(string,IFormatProvider)"/> and format defined on method.
    /// The provided format is from <see cref="CultureInfo.CurrentCulture"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapGuidToStringWithFormatAndUserDefinedCultureButCultureNameIsMissingDefinedOnMethod()
    {
        const string identifierName = "__mappa_tmp_1";

        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(GuidFormat = "N", CultureInfoSetting = CultureInfoSetting.UserDefined)]
                                      public partial string Map(System.Guid input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveOneWarning("MP00012")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.None,
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
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax("N"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="Guid"/> to <see cref="string"/>
    /// by using the <see cref="Guid.ToString()"/> when only culture info is setup.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapGuidToStringWithoutFormatInvokesPlainToString()
    {
        const string identifierName = "__mappa_tmp_1";

        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(CultureInfoSetting = CultureInfoSetting.InvariantCulture)]
                                      public partial string Map(System.Guid input);
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
                NullableAnnotation.None,
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
    /// Test a mapping can be created from <see cref="Guid"/> to <see cref="string"/>
    /// by using the <see cref="Guid.ToString(string)"/> and format defined on class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapGuidToStringWithFormatDefinedOnClass()
    {
        const string identifierName = "__mappa_tmp_1";

        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(GuidFormat = "N"]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(System.Guid input);
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
                NullableAnnotation.None,
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
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax("N"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax(identifierName));
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created from <see cref="Guid"/> to <see cref="string"/>
    /// by using the <see cref="Guid.ToString(string,IFormatProvider)"/> and format defined on method
    /// override the format and culture name defined on class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapGuidToStringWithFormatAndCultureNameDefinedOnMethodOverrideTheSetupOnClass()
    {
        const string identifierName = "__mappa_tmp_1";

        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(GuidFormat = "D", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "en-US")]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(GuidFormat = "N", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                                      public partial string Map(System.Guid input);
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
                NullableAnnotation.None,
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
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax("N"),
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
    /// Test a mapping can be created from <see cref="Guid"/> to <see cref="string"/>
    /// by using the <see cref="Guid.ToString(string,IFormatProvider)"/> and culture into setting defined on method
    /// override the culture info setting name defined on class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapGuidToStringWithCultureInfoSettingDefinedOnMethodOverrideTheSetupOnClass()
    {
        const string identifierName = "__mappa_tmp_1";

        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaSettings(GuidFormat = "D", CultureInfoSetting = CultureInfoSetting.CurrentCulture)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(GuidFormat = "N", CultureInfoSetting = CultureInfoSetting.UserDefined, CultureName = "it-IT")]
                                      public partial string Map(System.Guid input);
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
                NullableAnnotation.None,
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
                                        firstParameterAssertions => firstParameterAssertions.BeLiteralExpressionSyntax("N"),
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