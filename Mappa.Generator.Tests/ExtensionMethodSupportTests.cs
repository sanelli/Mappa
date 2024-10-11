// <copyright file="ExtensionMethodSupportTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests to check that extension methods are supported.
/// </summary>
public sealed class ExtensionMethodSupportTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be created for extension methods.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSupportExtensionMethods()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial long Map(this int input);
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
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                "Map",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                true,
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                "input",
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                1,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                        {
                            expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                        }));
                });
    }
}