// <copyright file="MappaNullSubstituteAssignValueIntegrationTests.Variants.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// AssignValue variants for <c>[MappaNullSubstitute]</c>.
/// </summary>
public sealed partial class MappaNullSubstituteAssignValueIntegrationTests
{
    private const string SourceTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Source";
    private const string TargetTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";

    /// <summary>
    /// Test AssignValue with an explicit <c>SourcePropertyName</c> maps <c>string?</c> to <c>string</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSubstituteWithExplicitSourcePropertyName()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string? Other { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.Value), SourcePropertyName = nameof(Source.Other), AssignValue = "Other")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string?",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Other"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeBinaryExpressionSyntax(
                                    left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    SyntaxKind.CoalesceExpression,
                                    right => right.BeLiteralExpressionSyntax("Other")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test AssignValue under <c>#nullable disable</c> still emits a coalesce substitute.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSubstituteNullSourceStringWithAssignValueWhenNullableDisabled()
    {
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.Value), AssignValue = "Other")]
                                      public partial Target Map(Source input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetTypeName,
                NullableAnnotation.None,
                SourceTypeName,
                NullableAnnotation.None,
                NullableSetup.Disable,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions =>
                                {
                                    conditionAssertions.BeIsPatternExpressionSyntax(
                                        expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                        patternAssertions => patternAssertions.BeUnaryPatternSyntax(
                                            SyntaxKind.NotKeyword,
                                            argumentAssertions => argumentAssertions.BeConstantPatternSyntax(null)));
                                },
                                ifStatementAssertions =>
                                {
                                    ifStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(5)
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(
                                                SourceTypeName,
                                                "__mappa_tmp_2",
                                                initAssertions => initAssertions.BeIdentifierNameSyntax("input"));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_3",
                                                initAssertions => initAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Value"));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                initAssertions => initAssertions.BeBinaryExpressionSyntax(
                                                    left => left.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                                    SyntaxKind.CoalesceExpression,
                                                    right => right.BeLiteralExpressionSyntax("Other")));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeLocalDeclarationStatementSyntax(
                                                TargetTypeName,
                                                "__mappa_tmp_5",
                                                initAssertions => initAssertions.BeObjectCreationExpressionSyntax(
                                                    TargetTypeName,
                                                    ("Value", propertyInitAssertions => propertyInitAssertions.BeIdentifierNameSyntax("__mappa_tmp_4"))));
                                        })
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeAssignmentExpressionStatement(
                                                leftAssertions => leftAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightAssertions => rightAssertions.BeIdentifierNameSyntax("__mappa_tmp_5"));
                                        });
                                },
                                elseStatementAssertions =>
                                {
                                    elseStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeAssignmentExpressionStatement(
                                                leftAssertions => leftAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightAssertions => rightAssertions.BeCastExpressionSyntax(
                                                    TargetTypeName,
                                                    expressionAssertions => expressionAssertions.BeLiteralExpressionSyntax(null)));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test AssignValue maps <c>int?</c> to <c>int</c> with a numeric constant.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSubstituteNullableIntWithAssignValueToNonNullableInt()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int? Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.Value), AssignValue = 42)]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "int?",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeBinaryExpressionSyntax(
                                    left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    SyntaxKind.CoalesceExpression,
                                    right => right.BeLiteralExpressionSyntax(42)));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test AssignValue with a nested source property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSubstituteNullUsingNestedSourcePropertyPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Address
                                  {
                                      public string? City { get; set; }
                                  }

                                  public class Location
                                  {
                                      public Address? Address { get; set; }
                                  }

                                  public class Source
                                  {
                                      public Location? Location { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string City { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.City), SourcePropertyName = "Location.Address.City", AssignValue = "London")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string?",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeConditionalAccessExpressionSyntax(
                                    "input.Location?.Address?.City"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeBinaryExpressionSyntax(
                                    left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    SyntaxKind.CoalesceExpression,
                                    right => right.BeLiteralExpressionSyntax("London")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("City", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test AssignValue emits <c>??</c> inside an <see cref="System.Linq.IQueryable{T}"/> projection.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSubstituteNullWithAssignValueInQueryableProjection()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string? Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.Value), AssignValue = "Other")]
                                      public partial IQueryable<Target> Map(IQueryable<Source> query);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveQueryableProjectionMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "Map",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                "query",
                SourceTypeName,
                TargetTypeName,
                "__mappa_tmp_1",
                ElementExpressionAssertions);

        static void ElementExpressionAssertions(ExpressionSyntaxAssertions elementExpression)
        {
            elementExpression.BeObjectCreationExpressionSyntax(
                TargetTypeName,
                ("Value", ValuePropertyAssertions));
        }

        static void ValuePropertyAssertions(ExpressionSyntaxAssertions property)
        {
            property.BeBinaryExpressionSyntax(
                left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Value"),
                SyntaxKind.CoalesceExpression,
                right => right.BeLiteralExpressionSyntax("Other"));
        }
    }
}