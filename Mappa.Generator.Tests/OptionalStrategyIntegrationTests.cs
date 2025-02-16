// <copyright file="OptionalStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests for source nd target optionals.
/// </summary>
public sealed class OptionalStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created between two properties
    /// - when optional is present on the source but optional is disabled;
    /// - when the mapping happens from source to parameter;
    /// - target is constructor with parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapWithOptionalDisabledTargetingConstructorWithParameter()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public bool HasPropertyA { get; set; }
                                      public int PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      private int? propertyA;
                                      public Target(int? propertyA)
                                      {
                                         this.propertyA = propertyA;
                                      }
                                  }

                                  [Mappa]
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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
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
                                        parameterSyntaxAssertions => parameterSyntaxAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                        });
                });
    }

    // TODO [#48] Test with optional enabled on method targeting constructor parameter.
    // TODO [#48] Test with optional enabled on method overriding on class targeting constructor parameter.
    // TODO [#48] Test with optional disabled targeting constructor parameter with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on class targeting constructor parameter with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on method targeting constructor parameter with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on method overriding on class targeting constructor parameter with mapping user defined via attribute.
    // TODO [#48] Test with optional disabled targeting non-optional property.
    // TODO [#48] Test with optional enabled on method targeting non-optional property.
    // TODO [#48] Test with optional enabled on class targeting non-optional property.
    // TODO [#48] Test with optional enabled on method overriding on class targeting non-optional property.
    // TODO [#48] Test with optional disabled targeting non-optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on class targeting non-optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on method targeting non-optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on method overriding on class targeting non-optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional disabled targeting optional property.
    // TODO [#48] Test with optional enabled on class targeting optional property.
    // TODO [#48] Test with optional enabled on method targeting optional property.
    // TODO [#48] Test with optional enabled on method overriding on class targeting optional property.
    // TODO [#48] Test with optional disabled targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on class targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on method targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with optional enabled on method overriding on class targeting optional property with mapping user defined via attribute.
    // TODO [#48] Test with nested struct/classes.
}