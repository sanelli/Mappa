// <copyright file="StringToEnumMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
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
    /// Test case-insensitive string-to-enum mapping with <see cref="MappaSettingsAttribute.CaseInsensitiveEnumMap"/>
    /// enabled on the mapper class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToEnumWhenCaseInsensitiveEnumMapIsEnabledOnClass()
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
                                  [MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Enable)]
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
                AssertCaseInsensitiveEnumSwitch);
    }

    /// <summary>
    /// Test case-insensitive string-to-enum mapping with <see cref="MappaSettingsAttribute.CaseInsensitiveEnumMap"/>
    /// enabled on the map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToEnumWhenCaseInsensitiveEnumMapIsEnabledOnMethod()
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
                                      [MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Enable)]
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
                AssertCaseInsensitiveEnumSwitch);
    }

    /// <summary>
    /// Test case-insensitive string-to-enum mapping configured via <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToEnumWhenCaseInsensitiveEnumMapIsEnabledInEditorConfig()
    {
        // Arrange
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.caseinsensitiveenummap = enable
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
                AssertCaseInsensitiveEnumSwitch);
    }

    /// <summary>
    /// Test string-to-enum mapping remains case-sensitive when
    /// <see cref="MappaSettingsAttribute.CaseInsensitiveEnumMap"/> is disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task GeneratesCaseSensitiveStringToEnumSwitchWhenCaseInsensitiveEnumMapIsDisabled()
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
                                  [MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Disable)]
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

    /// <summary>
    /// Test string-to-enum mapping by Description with <see cref="MappaSettingsAttribute.EnumStringMapSetting"/>
    /// enabled on the mapper class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToEnumByDescriptionOnClass()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.ComponentModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestEnum
                                  {
                                      [Description("First")]
                                      One,
                                      [Description("Second")]
                                      Two,
                                      [Description("Third")]
                                      Three,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumStringMapSetting = EnumStringMapSetting.Description)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestEnum Map(string input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                AssertDescriptionStringToEnumSwitch);
    }

    /// <summary>
    /// Test string-to-enum Description mapping with case-insensitive matching enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapStringToEnumByDescriptionWhenCaseInsensitiveEnumMapIsEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.ComponentModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestEnum
                                  {
                                      [Description("First")]
                                      One,
                                      [Description("Second")]
                                      Two,
                                  }

                                  [Mappa]
                                  [MappaSettings(
                                      EnumStringMapSetting = EnumStringMapSetting.Description,
                                      CaseInsensitiveEnumMap = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestEnum Map(string input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                                switchExpressionAssertions => { switchExpressionAssertions.BeInvocationExpressionSyntax("input.ToUpperInvariant"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeLiteralExpressionSyntax("FIRST"));
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
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeLiteralExpressionSyntax("SECOND"));
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
    /// Test MP00040 is emitted when an enum member lacks a Description attribute in string-to-enum Description mode.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsErrorWhenEnumMemberMissingDescriptionForStringToEnum()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.ComponentModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestEnum
                                  {
                                      One,
                                      [Description("Second")]
                                      Two,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumStringMapSetting = EnumStringMapSetting.Description)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestEnum Map(string input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.EnumMemberMissingDescription,
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum",
                "'One'");
    }

    /// <summary>
    /// Test MP00041 is emitted when duplicate Description values exist in string-to-enum Description mode.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsErrorWhenEnumHasDuplicateDescriptionsForStringToEnum()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.ComponentModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestEnum
                                  {
                                      [Description("Same")]
                                      One,
                                      [Description("Same")]
                                      Two,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumStringMapSetting = EnumStringMapSetting.Description)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestEnum Map(string input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.AmbiguousEnumMap,
                "Enum 'Mappa.Generator.Tests.UnitTests.SourceCode.TestEnum' has duplicate Description values for members: 'One', 'Two'.");
    }

    private static void AssertDescriptionStringToEnumSwitch(BlockSyntaxAssertions blockSyntaxAssertions)
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
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeLiteralExpressionSyntax("First"));
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
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeLiteralExpressionSyntax("Third"));
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
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeLiteralExpressionSyntax("Second"));
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

    private static void AssertCaseInsensitiveEnumSwitch(BlockSyntaxAssertions blockSyntaxAssertions)
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