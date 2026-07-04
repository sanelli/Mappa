// <copyright file="EnumToEnumMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <see cref="EnumToEnumMapStrategy"/>.
/// </summary>
public sealed class EnumToEnumMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between two enums.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnum()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      One,
                                      Two,
                                      Three,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      Two,
                                      Three,
                                      Four,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.NotAllSourceEnumMembersCanBeMapped,
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "'One'")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Three"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Three")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Two"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Two")))
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
    /// Test no warning is emitted when all source enum members have a matching target member name.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotEmitWarningWhenAllSourceMembersAreMapped()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      One,
                                      Two,
                                      Three,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      One,
                                      Two,
                                      Three,
                                      Four,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.One"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.One")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Three"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Three")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Two"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Two")))
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
    /// Test a warning is emitted when no source enum member names match the target enum.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsWarningWhenNoMemberNamesMatch()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      Alpha,
                                      Beta,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      One,
                                      Two,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.NotAllSourceEnumMembersCanBeMapped,
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "'Alpha', 'Beta'")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
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
    /// Test enum-to-enum mapping by numeric value with <see cref="MappaSettingsAttribute.EnumToEnumMapSetting"/>
    /// enabled on the mapper class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumByNumericValueOnClass()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      Alpha,
                                      Beta,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      One,
                                      Two,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.NumericValue)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                AssertNumericValueEnumToEnumSwitch);
    }

    /// <summary>
    /// Test enum-to-enum mapping by numeric value with <see cref="MappaSettingsAttribute.EnumToEnumMapSetting"/>
    /// enabled on the map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumByNumericValueOnMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      Alpha,
                                      Beta,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      One,
                                      Two,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.NumericValue)]
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                AssertNumericValueEnumToEnumSwitch);
    }

    /// <summary>
    /// Test enum-to-enum mapping by numeric value configured via <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumByNumericValueInEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.enumtoenummapsetting = NumericValue
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      Alpha,
                                      Beta,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      One,
                                      Two,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                AssertNumericValueEnumToEnumSwitch);
    }

    /// <summary>
    /// Test no warning is emitted when all source enum values have a matching target value in numeric mode.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotEmitWarningWhenAllSourceValuesAreMappedByNumericValue()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      One,
                                      Two,
                                      Three,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      Two,
                                      Three,
                                      Four,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.NumericValue)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.One"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Two")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Three"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Four")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Two"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Three")))
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
    /// Test a warning is emitted when a source enum value has no matching target value in numeric mode.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsWarningWhenSourceValueHasNoTargetMatchInNumericMode()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      Alpha,
                                      Beta = 99,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      One,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.NumericValue)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.NotAllSourceEnumMembersCanBeMapped,
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "'Beta'")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Alpha"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.One")))
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
    /// Test enum-to-enum mapping remains name-based when
    /// <see cref="MappaSettingsAttribute.EnumToEnumMapSetting"/> is <see cref="EnumToEnumMapSetting.MemberName"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task GeneratesNameBasedSwitchWhenEnumToEnumMapSettingIsMemberName()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      Alpha,
                                      Beta,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      One,
                                      Two,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.MemberName)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.NotAllSourceEnumMembersCanBeMapped,
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "'Alpha', 'Beta'")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
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
    /// Test enum-to-enum mapping by Description with <see cref="MappaSettingsAttribute.EnumToEnumMapSetting"/>
    /// enabled on the mapper class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumByDescriptionOnClass()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.ComponentModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      [Description("Alpha")]
                                      Alpha,
                                      [Description("Beta")]
                                      Beta,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      [Description("Alpha")]
                                      First,
                                      [Description("Beta")]
                                      Second,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.Description)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Alpha"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.First")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Beta"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Second")))
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
    /// Test enum-to-enum mapping with case-insensitive member name matching enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumWhenCaseInsensitiveEnumMapIsEnabledOnClass()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      ONe,
                                      Two,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      one,
                                      Two,
                                  }

                                  [Mappa]
                                  [MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.ONe"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.one")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Two"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Two")))
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
    /// Test MP00041 is emitted when case-insensitive enum-to-enum mapping is ambiguous.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsErrorWhenEnumToEnumCaseInsensitiveMapIsAmbiguous()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      ONe,
                                      One,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      one,
                                  }

                                  [Mappa]
                                  [MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Enable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.AmbiguousEnumMap,
                "Target enum member 'one' is matched by multiple source members: 'ONe', 'One'.");
    }

    /// <summary>
    /// Test MP00040 is emitted when an enum member lacks a Description attribute in enum-to-enum Description mode.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsErrorWhenEnumMemberMissingDescriptionForEnumToEnum()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.ComponentModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      Alpha,
                                      [Description("Beta")]
                                      Beta,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      [Description("Alpha")]
                                      First,
                                      [Description("Beta")]
                                      Second,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.Description)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.EnumMemberMissingDescription,
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                "'Alpha'");
    }

    /// <summary>
    /// Test MP00039 is emitted when a source Description has no matching target Description.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsWarningWhenSourceDescriptionHasNoTargetMatchInDescriptionMode()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.ComponentModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      [Description("Alpha")]
                                      Alpha,
                                      [Description("Beta")]
                                      Beta,
                                      [Description("Gamma")]
                                      Gamma,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      [Description("Alpha")]
                                      First,
                                      [Description("Beta")]
                                      Second,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.Description)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.NotAllSourceEnumMembersCanBeMapped,
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "'Gamma'")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Alpha"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.First")))
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                                },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Beta"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Second")))
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
    /// Test enum-to-enum mapping by Description with <see cref="MappaSettingsAttribute.EnumToEnumMapSetting"/>
    /// enabled on the map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumByDescriptionOnMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.ComponentModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      [Description("Alpha")]
                                      Alpha,
                                      [Description("Beta")]
                                      Beta,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      [Description("Alpha")]
                                      First,
                                      [Description("Beta")]
                                      Second,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.Description)]
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                AssertDescriptionEnumToEnumSwitch);
    }

    /// <summary>
    /// Test enum-to-enum mapping by Description configured via <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumByDescriptionInEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.enumtoenummapsetting = Description
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using System.ComponentModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      [Description("Alpha")]
                                      Alpha,
                                      [Description("Beta")]
                                      Beta,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      [Description("Alpha")]
                                      First,
                                      [Description("Beta")]
                                      Second,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                AssertDescriptionEnumToEnumSwitch);
    }

    /// <summary>
    /// Test class-level <see cref="MappaSettingsAttribute.EnumToEnumMapSetting"/> overrides
    /// <c>.editorconfig</c> for enum-to-enum mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumWhenClassEnumToEnumMapSettingOverridesEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.enumtoenummapsetting = Description
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using System.ComponentModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      [Description("Alpha")]
                                      Alpha,
                                      [Description("Beta")]
                                      Beta,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      [Description("Different")]
                                      Alpha,
                                      [Description("AlsoDifferent")]
                                      Beta,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.MemberName)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                AssertMemberNameEnumToEnumSwitch);
    }

    /// <summary>
    /// Test method-level <see cref="MappaSettingsAttribute.EnumToEnumMapSetting"/> overrides
    /// class-level and <c>.editorconfig</c> settings for enum-to-enum mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumWhenMethodEnumToEnumMapSettingOverridesClassAndEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.enumtoenummapsetting = NumericValue
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using System.ComponentModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      [Description("Alpha")]
                                      Alpha,
                                      [Description("Beta")]
                                      Beta,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      [Description("Alpha")]
                                      First,
                                      [Description("Beta")]
                                      Second,
                                  }

                                  [Mappa]
                                  [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.MemberName)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(EnumToEnumMapSetting = EnumToEnumMapSetting.Description)]
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                AssertDescriptionEnumToEnumSwitch);
    }

    /// <summary>
    /// Test enum-to-enum mapping with case-insensitive member name matching enabled on the map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumWhenCaseInsensitiveEnumMapIsEnabledOnMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      ONe,
                                      Two,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      one,
                                      Two,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Enable)]
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                AssertCaseInsensitiveEnumToEnumSwitch);
    }

    /// <summary>
    /// Test enum-to-enum mapping with case-insensitive member name matching configured via <c>.editorconfig</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumWhenCaseInsensitiveEnumMapIsEnabledInEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.caseinsensitiveenummap = enable
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      ONe,
                                      Two,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      one,
                                      Two,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                AssertCaseInsensitiveEnumToEnumSwitch);
    }

    /// <summary>
    /// Test class-level <see cref="MappaSettingsAttribute.CaseInsensitiveEnumMap"/> overrides
    /// <c>.editorconfig</c> for enum-to-enum mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumWhenClassCaseInsensitiveEnumMapOverridesEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.caseinsensitiveenummap = enable
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      ONe,
                                      Two,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      one,
                                      Two,
                                  }

                                  [Mappa]
                                  [MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.NotAllSourceEnumMembersCanBeMapped,
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "'ONe'")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                AssertPartialMemberNameEnumToEnumSwitch);
    }

    /// <summary>
    /// Test method-level <see cref="MappaSettingsAttribute.CaseInsensitiveEnumMap"/> overrides
    /// class-level and <c>.editorconfig</c> settings for enum-to-enum mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToEnumWhenMethodCaseInsensitiveEnumMapOverridesClassAndEditorConfig()
    {
        const string editorConfig = """
                                    root = true

                                    [*.cs]
                                    mappa.caseinsensitiveenummap = disable
                                    """;

        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum TestSourceEnum
                                  {
                                      ONe,
                                      Two,
                                  }

                                  public enum TestTargetEnum
                                  {
                                      one,
                                      Two,
                                  }

                                  [Mappa]
                                  [MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Disable)]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(CaseInsensitiveEnumMap = BooleanSetting.Enable)]
                                      public partial TestTargetEnum Map(TestSourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, editorConfig, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                "Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum",
                AssertCaseInsensitiveEnumToEnumSwitch);
    }

    private static void AssertDescriptionEnumToEnumSwitch(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeSwitchStatementSyntax(
                    switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Alpha"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.First")))
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                    },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Beta"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Second")))
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

    private static void AssertMemberNameEnumToEnumSwitch(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeSwitchStatementSyntax(
                    switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Alpha"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Alpha")))
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                    },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Beta"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Beta")))
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

    private static void AssertCaseInsensitiveEnumToEnumSwitch(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeSwitchStatementSyntax(
                    switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.ONe"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.one")))
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                    },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Two"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Two")))
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

    private static void AssertPartialMemberNameEnumToEnumSwitch(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeSwitchStatementSyntax(
                    switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Two"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Two")))
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

    private static void AssertNumericValueEnumToEnumSwitch(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum", "__mappa_tmp_1");
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeSwitchStatementSyntax(
                    switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Alpha"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.One")))
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeBreakStatement());
                    },
                    (labelsAssertions, statementAssertions) =>
                    {
                        labelsAssertions.Should().HaveCount(1);
                        labelsAssertions[0].IsCase();
                        labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum.Beta"));
                        statementAssertions.Should().HaveCount(1);
                        statementAssertions[0].BeBlockStatement();
                        statementAssertions[0].AsBlock()
                            .HasSyntaxNodesCount(2)
                            .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum.Two")))
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