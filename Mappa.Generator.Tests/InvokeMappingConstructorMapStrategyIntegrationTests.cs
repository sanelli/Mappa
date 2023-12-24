// <copyright file="InvokeMappingConstructorMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="InvokeMappingConstructorMapStrategy"/> strategy.
/// </summary>
public sealed class InvokeMappingConstructorMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping from two classes using the only
    /// existing mapping constructor on the target.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingSingleMappingConstructor()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public Source(int sourceProperty)
                                      {
                                          this.SourceProperty = sourceProperty;
                                      }
                                  
                                      public int SourceProperty { get; };
                                  }

                                  public class Target
                                  {
                                      public Target(Source source)
                                      {
                                          this.TargetProperty = source.SourceProperty;
                                      }
                                  
                                      public int TargetProperty { get; };
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
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
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .IsBlockStatement()
                        .AsBlock()
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                "__mappa_tmp_1",
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("input")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }

    /// <summary>
    /// Test a mapping from enum to class accepting
    /// as mapping constructor a different but compatible
    /// type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingSingleMappingConstructorWithMappableParameter()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum Source
                                  {
                                      One,
                                      Two,
                                      Three,
                                  }

                                  public class Target
                                  {
                                      public Target(int source)
                                      {
                                          this.TargetProperty = (Source)source;
                                      }
                                  
                                      public Source TargetProperty { get; };
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        // TODO [#42] Add correct assertions.
        compilationUnitSyntaxAssertions.NotBeNull();
    }

    /// <summary>
    /// Test a mapping from enum to class accepting
    /// as mapping constructor a different but compatible
    /// type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMultipleMappingConstructorButOnlyOneMatchExactly()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum Source
                                  {
                                      One,
                                      Two,
                                      Three,
                                  }

                                  public class Target
                                  {
                                      public Target(int source)
                                      {
                                          this.TargetProperty = (Source)source;
                                      }
                                  
                                      public Target(Source source)
                                      {
                                          this.TargetProperty = source;
                                      }
                                  
                                      public Source TargetProperty { get; };
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
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
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .IsBlockStatement()
                        .AsBlock()
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                "__mappa_tmp_1",
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeIdentifierNameSyntax("input")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_2",
                                expressionSyntaxAssertions => expressionSyntaxAssertions.BeObjectCreationExpressionSyntax(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2")));
                });
    }
}