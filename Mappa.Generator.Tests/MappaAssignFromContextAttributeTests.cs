// <copyright file="MappaAssignFromContextAttributeTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests for <see cref="MappaAssignFromContextAttribute"/>.
/// </summary>
public sealed class MappaAssignFromContextAttributeTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a property can be set using <see cref="MappaAssignFromContextAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInvokeOtherMapperRequiringContext()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source { }
                                  public class Target {
                                     public string Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignFromContext(nameof(Target.Property), "Value")]
                                      public partial Target Map(Source input, MappaContext context);
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
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeCastExpressionSyntax(
                                    typeof(string).ToString(),
                                    castExpression =>
                                    {
                                        castExpression.BeElementAccessExpressionSyntaxWithLiteralSyntax("context", "Value");
                                    }));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test a property can be set using <see cref="MappaAssignFromContextAttribute"/>
    /// and that even if the property exists a temporary will not be created
    /// as the source property will just be ignored anyway..
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInvokeOtherMapperRequiringContextAndDoesNotCreateNeedlessTemporaryForSourceProperty()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source {
                                    public DateTime Property {get; set;}
                                  }
                                  
                                  public class Target {
                                     public string Property { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignFromContext(nameof(Target.Property), "Value")]
                                      public partial Target Map(Source input, MappaContext context);
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
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeCastExpressionSyntax(
                                    typeof(string).ToString(),
                                    castExpression =>
                                    {
                                        castExpression.BeElementAccessExpressionSyntaxWithLiteralSyntax("context", "Value");
                                    }));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("Property", expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    /// <summary>
    /// Test a constructor parameter can be set using <see cref="MappaAssignFromContextAttribute"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInvokeOtherMapperRequiringContextWithNonEmptyConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source { }
                                  public class Target
                                  {
                                      public Target(string property)
                                      {
                                         this.Property = property;
                                      }
                                      
                                      public string Property { get; private set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignFromContext("property", "Value")]
                                      public partial Target Map(Source input, MappaContext context);
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
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeCastExpressionSyntax(
                                    typeof(string).ToString(),
                                    castExpression =>
                                    {
                                        castExpression.BeElementAccessExpressionSyntaxWithLiteralSyntax("context", "Value");
                                    }));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        firstParameterSyntaxAssertions => firstParameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }
}