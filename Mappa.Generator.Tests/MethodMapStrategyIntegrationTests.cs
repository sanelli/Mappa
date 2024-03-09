// <copyright file="MethodMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="MethodMapStrategy"/>.
/// </summary>
public sealed class MethodMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can use an existing method
    /// defined by the user.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingCustomUserMethod()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int B { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget() { B = input.A };
                                      }
                                      public partial Target Map(Source input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.None,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.None,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions =>
                                {
                                    conditionAssertions.BeIsPatternExpressionSyntax(
                                        identifierAssertions => identifierAssertions.BeIdentifierNameSyntax("input"),
                                        patternAssertions => patternAssertions.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, argumentAssertions => argumentAssertions.BeConstantPatternSyntax(null)));
                                },
                                ifStatementAssertions =>
                                {
                                    ifStatementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(5)
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                                "__mappa_tmp_2",
                                                initializationAssertions => initializationAssertions.BeIdentifierNameSyntax("input"));
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                                "__mappa_tmp_3",
                                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Property"));
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                                "__mappa_tmp_4",
                                                initializationAssertions =>
                                                {
                                                    initializationAssertions.BeInvocationExpressionSyntax(
                                                        "this.Map",
                                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                                                });
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                "__mappa_tmp_5",
                                                initializationAssertions =>
                                                {
                                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                                });
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_5"));
                                        });
                                },
                                elseStatementAssertions =>
                                {
                                    elseStatementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(elseSyntaxNodeAssertions =>
                                        {
                                            elseSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeCastExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target", assertions => assertions.BeLiteralExpressionSyntax(null)));
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
    /// Test a mapping can use an existing method
    /// generated by Mappa.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanUsingExistingMappedMethod()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                    public partial Target Map(Source input);
                                    public partial InnerTarget Map(InnerSource input);
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
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.None,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.None,
                2,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions =>
                                {
                                    conditionAssertions.BeIsPatternExpressionSyntax(
                                        identifierAssertions => identifierAssertions.BeIdentifierNameSyntax("input"),
                                        patternAssertions => patternAssertions.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, argumentAssertions => argumentAssertions.BeConstantPatternSyntax(null)));
                                },
                                ifStatementAssertions =>
                                {
                                    ifStatementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(5)
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                                "__mappa_tmp_2",
                                                initializationAssertions => initializationAssertions.BeIdentifierNameSyntax("input"));
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                                "__mappa_tmp_3",
                                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Property"));
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                                "__mappa_tmp_4",
                                                initializationAssertions =>
                                                {
                                                    initializationAssertions.BeInvocationExpressionSyntax(
                                                        "this.Map",
                                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"));
                                                });
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                "__mappa_tmp_5",
                                                initializationAssertions =>
                                                {
                                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                                });
                                        })
                                        .HasNextSyntaxNode(ifSyntaxNodeAssertions =>
                                        {
                                            ifSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_5"));
                                        });
                                },
                                elseStatementAssertions =>
                                {
                                    elseStatementAssertions
                                        .IsBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(elseSyntaxNodeAssertions =>
                                        {
                                            elseSyntaxNodeAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeCastExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Target", assertions => assertions.BeLiteralExpressionSyntax(null)));
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
    /// Test a mapping can be created using a method from
    /// a dependency class via property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingPropertyDependency()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  [Mappa]
                                  public sealed partial class Dependency
                                  {
                                      public partial InnerTarget Map(InnerSource input);
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private Dependency DependencyProperty { get; } = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode(howMany: 2)
            .WithCompilationUnits(2);

        // TODO [#42] Add correct assertions.
        foreach (var compilationUnitAssertion in compilationUnitSyntaxAssertions)
        {
            compilationUnitAssertion.NotBeNull();
        }
    }

    /// <summary>
    /// Test a mapping can be created using a method from
    /// a dependency class via a field.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingFieldDependency()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  [Mappa]
                                  public sealed partial class Dependency
                                  {
                                      public partial InnerTarget Map(InnerSource input);
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private Dependency dependencyField = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode(howMany: 2)
            .WithCompilationUnits(2);

        // TODO [#42] Add correct assertions.
        foreach (var compilationUnitAssertion in compilationUnitSyntaxAssertions)
        {
            compilationUnitAssertion.NotBeNull();
        }
    }
}