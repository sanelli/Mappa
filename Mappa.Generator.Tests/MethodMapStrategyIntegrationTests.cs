// <copyright file="MethodMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
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
                NullableSetup.Disable,
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
                                        .BeBlockStatement()
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
                                        .BeBlockStatement()
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
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_1");
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
                NullableSetup.Disable,
                PragmaWarning.NoBlock,
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
                                        .BeBlockStatement()
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
                                        .BeBlockStatement()
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
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_1");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created using a method from
    /// a dependency class via non-static property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticPropertyDependency()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public sealed class Dependency
                                  {
                                      public InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private Dependency DependencyProperty { get; } = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "this.DependencyProperty.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test a warning is returning when a property does not provide
    /// any dependency. Also test private methods are ignored.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PrivateMethodsDependenciesOnPropertyAreIgnoreAndWarningIsDiagnostics()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public sealed class Dependency
                                  {
                                      private InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private Dependency DependencyProperty { get; } = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod, "DependencyProperty")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.A"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                        ("A", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_4",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_3"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created using a method from
    /// a dependency class via non-static property and the method
    /// that can be used for mapping is static.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticPropertyDependencyButMethodIsStatic()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public sealed class Dependency
                                  {
                                      public static InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private Dependency DependencyProperty { get; } = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "global::Mappa.Generator.Tests.UnitTests.SourceCode.Dependency.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created using a method from
    /// a dependency class via static property.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticPropertyDependency()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public sealed class Dependency
                                  {
                                      public InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      static private Dependency DependencyProperty { get; } = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "DependencyProperty.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created using a method from
    /// a dependency class via static property and the method
    /// to be invoked is static.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticPropertyDependencyAndStaticMethod()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public sealed class Dependency
                                  {
                                      public static InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      static private Dependency DependencyProperty { get; } = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "global::Mappa.Generator.Tests.UnitTests.SourceCode.Dependency.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created using a method from
    /// a dependency class via non-static field.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticFieldDependency()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public sealed class Dependency
                                  {
                                      public InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private Dependency dependencyField = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "this.dependencyField.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created using a method from
    /// a dependency class via non-static field, but the method is static.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticFieldDependencyButMethodIsStatic()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public sealed class Dependency
                                  {
                                      public static InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private Dependency dependencyField = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "global::Mappa.Generator.Tests.UnitTests.SourceCode.Dependency.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test a warning is returning when a field does not provide
    /// any dependency. Also test private methods are ignored.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PrivateMethodsDependenciesOnFieldAreIgnoreAndWarningIsDiagnostics()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public sealed class Dependency
                                  {
                                      private InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private readonly Dependency dependencyField = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod, "dependencyField")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.A"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                        ("A", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_4",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_3"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created using a method from
    /// a dependency class via static field.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticFieldDependency()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public sealed class Dependency
                                  {
                                      public InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      static private Dependency dependencyField = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "dependencyField.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created using a method from
    /// a dependency class via static field, and the method is static as well.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticFieldDependencyAndMethodIsStatic()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public sealed class Dependency
                                  {
                                      public static InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      static private Dependency dependencyField = new Dependency();
                                  
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "global::Mappa.Generator.Tests.UnitTests.SourceCode.Dependency.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created using a static method from
    /// a static class defined on a <see cref="MappaStaticDependencyAttribute"/> attribute.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticMethodOnStaticClassDependencyForStaticDependencyAttribute()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public static class Dependency
                                  {
                                      public static InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  [MappaStaticDependency(typeof(Dependency))]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "global::Mappa.Generator.Tests.UnitTests.SourceCode.Dependency.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can be created using a non-static method from
    /// a static class defined on a <see cref="MappaStaticDependencyAttribute"/> attribute.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticMethodOnNonStaticClassDependencyForStaticDependencyAttribute()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public class Dependency
                                  {
                                      public static InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  [MappaStaticDependency(typeof(Dependency))]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "global::Mappa.Generator.Tests.UnitTests.SourceCode.Dependency.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test a warning is returning when a static dependency attribute
    /// does not have any method that can be used for mapping because
    /// the method is private.
    /// This tests that private methods are ignored.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PrivateMethodsAreIgnoredOnDependenciesDefinedByStaticDependencyAttribute()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public class Dependency
                                  {
                                      private static InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  [MappaStaticDependency(typeof(Dependency)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod, "Mappa.Generator.Tests.UnitTests.SourceCode.Dependency")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.A"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                        ("A", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_4",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_3"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test a warning is returning when a static dependency attribute
    /// does not have any method that can be used for mapping because
    /// the method is non-static.
    /// This tests that non-static methods are ignored.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NonStaticMethodsAreIgnoredOnDependenciesDefinedByStaticDependencyAttribute()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public class Dependency
                                  {
                                      public InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  [Mappa]
                                  [MappaStaticDependency(typeof(Dependency)]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod, "Mappa.Generator.Tests.UnitTests.SourceCode.Dependency")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.A"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                        ("A", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_4",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_3"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test a mapping does not use a non-static method if the
    /// method being mapped is static.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [Bug("#185")]
    [IntegrationTest]
    public async Task CanMapWithoutPickingUpANonStaticMethodWhenTheMethodBeingMappedIsStatic()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget() { A = input.A };
                                      }
                                      public static partial Target Map(Source input);
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
            .HaveDefaultStaticMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                1,
                false,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                            "__mappa_tmp_1",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_2",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.A")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                            "__mappa_tmp_3",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                ("A", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_4",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Test a mapping does not use a non-static method from a non-static dependency property if the
    /// method being mapped is static.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [Bug("#185")]
    [IntegrationTest]
    public async Task CanMapWithoutPickingUpANonStaticMethodFromNonStaticDependencyPropertyWhenTheMethodBeingMappedIsStatic()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }
                                  
                                  public sealed class Dependency
                                  {
                                     public InnerTarget Map(InnerSource input)
                                     {
                                         return new InnerTarget() { A = input.A };
                                     }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private Dependency DependencyProperty { get; } = new Dependency();
                                     
                                      public static partial Target Map(Source input);
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
            .HaveDefaultStaticMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                1,
                false,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                            "__mappa_tmp_1",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_2",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.A")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                            "__mappa_tmp_3",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                ("A", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_4",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Test a mapping does not use a non-static method from a non-static dependency field if the
    /// method being mapped is static.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [Bug("#185")]
    [IntegrationTest]
    public async Task CanMapWithoutPickingUpANonStaticMethodFromNonStaticDependencyFieldWhenTheMethodBeingMappedIsStatic()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }
                                  
                                  public sealed class Dependency
                                  {
                                     public InnerTarget Map(InnerSource input)
                                     {
                                         return new InnerTarget() { A = input.A };
                                     }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private Dependency dependencyField = new Dependency();
                                     
                                      public static partial Target Map(Source input);
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
            .HaveDefaultStaticMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                1,
                false,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                            "__mappa_tmp_1",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("input.Property")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_2",
                            initializerAssertions => initializerAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_1.A")))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                            "__mappa_tmp_3",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                ("A", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                            "__mappa_tmp_4",
                            initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Test a mapping can use a method defined on a base class of the mapper.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMethodFromMapperBaseClass()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int B { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public class MapperBase
                                  {
                                      public InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget() { B = input.A };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper : MapperBase
                                  {
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
                NullableSetup.Disable,
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
                                        .BeBlockStatement()
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
                                        .BeBlockStatement()
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
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_1");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can use a method defined on a base class of a dependency property type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMethodFromDependencyPropertyBaseClass()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public class DependencyBase
                                  {
                                      public InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  public sealed class DerivedDependency : DependencyBase
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private DerivedDependency DependencyProperty { get; } = new DerivedDependency();

                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "this.DependencyProperty.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }

    /// <summary>
    /// Test a mapping can use a method defined on a base class of a dependency field type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMethodFromDependencyFieldBaseClass()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }

                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public class DependencyBase
                                  {
                                      public InnerTarget Map(InnerSource input)
                                      {
                                          return new InnerTarget{ A = input.A };
                                      }
                                  }

                                  public sealed class DerivedDependency : DependencyBase
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaDependency]
                                      private DerivedDependency dependencyField = new DerivedDependency();

                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
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
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource",
                                    "__mappa_tmp_1",
                                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget",
                                    "__mappa_tmp_2",
                                    initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                        "this.dependencyField.Map",
                                        argumentAssertions => argumentAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions
                                .BeLocalDeclarationStatementSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    "__mappa_tmp_3",
                                    initializationAssertions => initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_3");
                        });
                });
    }
}