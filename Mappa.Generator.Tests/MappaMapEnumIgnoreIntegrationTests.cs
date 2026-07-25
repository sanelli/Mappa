// <copyright file="MappaMapEnumIgnoreIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="Mappa.Attributes.MappaMapEnumIgnoreAttribute{TEnum}"/>.
/// </summary>
public sealed class MappaMapEnumIgnoreIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string Ns = "Mappa.Generator.Tests.UnitTests.SourceCode";

    /// <summary>
    /// Test enum-to-integral mapping ignores a member and only emits the remaining case plus default throw.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToIntegralIgnoringMember()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum StatusEnum
                                  {
                                      Active,
                                      Inactive,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaMapEnumIgnore<StatusEnum>(StatusEnum.Inactive)]
                                      public partial int Map(StatusEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                $"{Ns}.StatusEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax($"{Ns}.StatusEnum.Active"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeLiteralExpressionSyntax(0)))
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
    /// Test MP00039 is not emitted when an ignored source member is unmapped by name.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotEmitMp00039WhenIgnoredMemberIsUnmappedByName()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum SourceEnum
                                  {
                                      One,
                                      Beta,
                                  }

                                  public enum TargetEnum
                                  {
                                      One,
                                      Two,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaMapEnumIgnore<SourceEnum>(SourceEnum.Beta)]
                                      public partial TargetEnum Map(SourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                $"{Ns}.TargetEnum",
                $"{Ns}.SourceEnum",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax($"{Ns}.TargetEnum", "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("input"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax($"{Ns}.SourceEnum.One"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_1", assert => assert.BeMemberAccessExpressionSyntax($"{Ns}.TargetEnum.One")))
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
    /// Test enum-to-integral ignore on a nested class property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapEnumToIntegralIgnoringMemberOnNestedClassProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum StatusEnum
                                  {
                                      Active,
                                      Inactive,
                                  }

                                  public class Source
                                  {
                                      public StatusEnum Status { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Status { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaMapEnumIgnore<StatusEnum>(StatusEnum.Inactive)]
                                      public partial Target Map(Source input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                $"{Ns}.Target",
                NullableAnnotation.NotAnnotated,
                $"{Ns}.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                $"{Ns}.StatusEnum",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Status")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_2"))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeSwitchStatementSyntax(
                                switchExpressionAssertions => { switchExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"); },
                                (labelsAssertions, statementAssertions) =>
                                {
                                    labelsAssertions.Should().HaveCount(1);
                                    labelsAssertions[0].IsCase();
                                    labelsAssertions[0].AsCase().HasValue(expressionSyntaxAssertions => expressionSyntaxAssertions.BeMemberAccessExpressionSyntax($"{Ns}.StatusEnum.Active"));
                                    statementAssertions.Should().HaveCount(1);
                                    statementAssertions[0].BeBlockStatement();
                                    statementAssertions[0].AsBlock()
                                        .HasSyntaxNodesCount(2)
                                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeAssignmentExpressionStatement("__mappa_tmp_2", assert => assert.BeLiteralExpressionSyntax(0)))
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
                                            assertion => assertion.BeLiteralExpressionSyntax("__mappa_tmp_1")));
                                }))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                $"{Ns}.Target",
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        $"{Ns}.Target",
                                        ("Status", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3")));
                });
    }

    /// <summary>
    /// Test an error is emitted when ignore conflicts with member mapping on a class map.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsErrorWhenIgnoreConflictsWithMemberMapping()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum StatusEnum
                                  {
                                      Active,
                                      Inactive,
                                  }

                                  public class Source
                                  {
                                      public StatusEnum Status { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Status { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaMapEnumIgnore<StatusEnum>(StatusEnum.Inactive)]
                                      [MappaMapEnumMember<StatusEnum>(StatusEnum.Inactive, 99)]
                                      public partial Target Map(Source input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.EnumMapIgnoreConflictsWithMemberMapping,
                "Map",
                $"{Ns}.StatusEnum",
                "Inactive");
    }

    /// <summary>
    /// Test an error is emitted when ignore conflicts with member mapping on a direct enum map.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsErrorWhenIgnoreConflictsWithMemberMappingOnDirectEnumMap()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum StatusEnum
                                  {
                                      Active,
                                      Inactive,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaMapEnumIgnore<StatusEnum>(StatusEnum.Inactive)]
                                      [MappaMapEnumMember<StatusEnum>(StatusEnum.Inactive, 99)]
                                      public partial int Map(StatusEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.EnumMapIgnoreConflictsWithMemberMapping,
                "Map",
                $"{Ns}.StatusEnum",
                "Inactive");
    }

    /// <summary>
    /// Test an error is emitted when ignore conflicts with an enum-to-enum source member mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsErrorWhenIgnoreConflictsWithEnumToEnumSourceMemberMapping()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum SourceEnum
                                  {
                                      Alpha,
                                      Beta,
                                  }

                                  public enum TargetEnum
                                  {
                                      One,
                                      Two,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaMapEnumIgnore<SourceEnum>(SourceEnum.Beta)]
                                      [MappaMapEnumMember<SourceEnum, TargetEnum>(SourceEnum.Beta, TargetEnum.Two)]
                                      public partial TargetEnum Map(SourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.EnumMapIgnoreConflictsWithMemberMapping,
                "Map",
                $"{Ns}.SourceEnum",
                "Beta");
    }

    /// <summary>
    /// Test an error is emitted when ignore conflicts with an enum-to-enum target member mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsErrorWhenIgnoreConflictsWithEnumToEnumTargetMemberMapping()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum SourceEnum
                                  {
                                      Alpha,
                                      Beta,
                                  }

                                  public enum TargetEnum
                                  {
                                      One,
                                      Two,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaMapEnumIgnore<TargetEnum>(TargetEnum.Two)]
                                      [MappaMapEnumMember<SourceEnum, TargetEnum>(SourceEnum.Beta, TargetEnum.Two)]
                                      public partial TargetEnum Map(SourceEnum input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.EnumMapIgnoreConflictsWithMemberMapping,
                "Map",
                $"{Ns}.TargetEnum",
                "Two");
    }

    /// <summary>
    /// Test an error is emitted when ignore conflicts with a string-to-enum member mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmitsErrorWhenIgnoreConflictsWithStringToEnumMemberMapping()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum StatusEnum
                                  {
                                      Active,
                                      Inactive,
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaMapEnumIgnore<StatusEnum>(StatusEnum.Inactive)]
                                      [MappaMapEnumMember<StatusEnum>(StatusEnum.Inactive, "disabled")]
                                      public partial StatusEnum Map(string input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.EnumMapIgnoreConflictsWithMemberMapping,
                "Map",
                $"{Ns}.StatusEnum",
                "Inactive");
    }
}