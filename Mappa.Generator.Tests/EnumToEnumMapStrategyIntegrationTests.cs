// <copyright file="EnumToEnumMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;

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

        var compilationUnitSyntaxAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveCommentHeader()
            .HaveFileScopedNamespace(fileScopedNamespaceDeclarationSyntaxAssertions =>
            {
                fileScopedNamespaceDeclarationSyntaxAssertions
                    .HaveClasses(1)
                    .HaveClass(
                        "Mapper",
                        classDeclarationSyntaxAssertions =>
                        {
                            classDeclarationSyntaxAssertions
                                .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
                                .HaveMethods(1)
                                .HaveMethod(
                                    "Mappa.Generator.Tests.UnitTests.SourceCode.TestTargetEnum",
                                    NullableAnnotation.NotAnnotated,
                                    "Map",
                                    new[] { ("Mappa.Generator.Tests.UnitTests.SourceCode.TestSourceEnum", NullableAnnotation.NotAnnotated, "input") },
                                    methodDeclarationSyntaxAssertions =>
                                    {
                                        methodDeclarationSyntaxAssertions
                                            .HaveGeneratedCodeAttribute()
                                            .HaveModifiers(SyntaxKind.PartialKeyword, SyntaxKind.PublicKeyword)
                                            .HaveBody(blockSyntaxAssertions =>
                                            {
                                                blockSyntaxAssertions.HasNextSyntaxNode(syntaxNodeAssertions =>
                                                {
                                                    // TODO [#42] Add correct assertions.
                                                });
                                            });
                                    });
                        });
            });

        compilationUnitSyntaxAssertions.NotBeNull();
    }
}