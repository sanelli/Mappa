// <copyright file="StringToEnumMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="StringToEnumMapStrategy"/>.
/// </summary>
public sealed class StringToEnumMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created from a string to
    /// an enum.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToEnum()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestEnum
                                  {
                                      One,
                                      Two,
                                      Three,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestEnum Map(string input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum",
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeNameOf(paramAssertions => paramAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.One")));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.One")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeNameOf(paramAssertions => paramAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.Two")));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.Two")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeNameOf(paramAssertions => paramAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.Three")));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.Three")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsDefault();
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeThrowStatementSyntax<ArgumentOutOfRangeException>(
                                            assertion => assertion.BeLiteralExpressionSyntax("input")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test case-insensitive string-to-enum mapping with <see cref="MappaSettingsAttribute.CaseInsensitiveStringToEnumMap"/>
    /// enabled on the mapper class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToEnumWhenCaseInsensitiveStringToEnumMapIsEnabledOnClass()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestEnum
                                  {
                                      One,
                                      Two,
                                      Three,
                                  }

                                  [Mappa]
                                  [MappaSettings(CaseInsensitiveStringToEnumMap = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestEnum Map(string input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum",
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertCaseInsensitiveStringToEnumSwitch);
    }

    /// <summary>
    /// Test case-insensitive string-to-enum mapping with <see cref="MappaSettingsAttribute.CaseInsensitiveStringToEnumMap"/>
    /// enabled on the map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToEnumWhenCaseInsensitiveStringToEnumMapIsEnabledOnMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestEnum
                                  {
                                      One,
                                      Two,
                                      Three,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(CaseInsensitiveStringToEnumMap = BooleanSetting.Enable)]
                                      public partial TestEnum Map(string input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum",
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertCaseInsensitiveStringToEnumSwitch);
    }

    /// <summary>
    /// Test case-insensitive string-to-enum mapping configured via <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToEnumWhenCaseInsensitiveStringToEnumMapIsEnabledInEditorConfig()
    {
        // Arrange
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.caseinsensitivestringtoenummap = enable
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestEnum
                                  {
                                      One,
                                      Two,
                                      Three,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestEnum Map(string input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum",
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertCaseInsensitiveStringToEnumSwitch);
    }

    /// <summary>
    /// Test string-to-enum mapping remains case-sensitive when
    /// <see cref="MappaSettingsAttribute.CaseInsensitiveStringToEnumMap"/> is disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task GeneratesCaseSensitiveStringToEnumSwitchWhenCaseInsensitiveStringToEnumMapIsDisabled()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestEnum
                                  {
                                      One,
                                      Two,
                                      Three,
                                  }

                                  [Mappa]
                                  [MappaSettings(CaseInsensitiveStringToEnumMap = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestEnum Map(string input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum",
                NullableAnnotation.NotAnnotated,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                AssertCaseSensitiveStringToEnumSwitch);
    }

    private static void AssertCaseInsensitiveStringToEnumSwitch(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum", "__mappa_tmp_1");
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeSwitchStatementSyntax(
                    switchExpressionAssertions => { switchExpressionAssertions.BeInvocationExpressionSyntax("input.ToUpperInvariant"); },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeLiteralExpressionSyntax("ONE"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.One")))
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                    },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeLiteralExpressionSyntax("TWO"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.Two")))
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                    },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeLiteralExpressionSyntax("THREE"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.Three")))
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                    },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsDefault();
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(1)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeThrowStatementSyntax<ArgumentOutOfRangeException>(
                                assertion => assertion.BeLiteralExpressionSyntax("input")));
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
            });
    }

    private static void AssertCaseSensitiveStringToEnumSwitch(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum", "__mappa_tmp_1");
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeSwitchStatementSyntax(
                    switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeNameOf(paramAssertions => paramAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.One")));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.One")))
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                    },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeNameOf(paramAssertions => paramAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.Two")));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.Two")))
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                    },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeNameOf(paramAssertions => paramAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.Three")));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum.Three")))
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                    },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsDefault();
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(1)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeThrowStatementSyntax<ArgumentOutOfRangeException>(
                                assertion => assertion.BeLiteralExpressionSyntax("input")));
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
            });
    }
}