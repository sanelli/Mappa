// <copyright file="NamespaceSupportIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Tests related to the identity strategy.
/// </summary>
public sealed class NamespaceSupportIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test a mapping can be added to a file with file-scoped namespaces.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanPerformMappingWithFileNamespace()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(string input);
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
                typeof(string).ToString(),
                NullableAnnotation.None,
                typeof(string).ToString(),
                NullableAnnotation.None,
                NullableSetup.Disable,
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

    /// <summary>
    /// Test a mapping can be added to a file without file-scoped namespaces.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanPerformMappingWithoutFileNamespace()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode
                                  {
                                      [Mappa]
                                      public sealed partial class Mapper
                                      {
                                          public partial string Map(string input);
                                      }
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
            .HaveCommentHeader()
            .HaveNamespaceDeclarationSyntax(namespaceDeclarationSyntaxAssertions =>
            {
                namespaceDeclarationSyntaxAssertions
                    .HaveClasses(1)
                    .HaveClass("Mapper", AssertIdentityMapOnClass);
            });
    }

    /// <summary>
    /// Test a mapping can be added to a file without namespace.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanPerformMappingWithoutAnyNamespace()
    {
        // Arrange
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;
                                  
                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(string input);
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
            .HaveCommentHeader()
            .HaveNoNamespaceDeclarationSyntax()
            .HaveClasses(1)
            .HaveClass("Mapper", AssertIdentityMapOnClass);
    }

    private static void AssertIdentityMapOnClass(ClassDeclarationSyntaxAssertions classDeclarationSyntaxAssertions)
    {
        classDeclarationSyntaxAssertions
            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
            .HaveMethods(1)
            .HaveMethod(
                typeof(string).ToString(),
                NullableAnnotation.None,
                "Map",
                false,
                [(typeof(string).ToString(), NullableAnnotation.None, "input", RefKind.None, false)],
                methodDeclarationSyntaxAssertions =>
                {
                    methodDeclarationSyntaxAssertions
                        .HaveNullabilityAnnotation(NullableSetup.Disable)
                        .HavePragmaWarningDisableAnnotation(PragmaWarning.NoBlock)
                        .HaveGeneratedCodeAttribute(attributeSyntaxAssertions => attributeSyntaxAssertions.BeMappaGeneratedCodeAttribute())
                        .HaveDebuggerNonUserCodeAttribute()
                        .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                        .HaveBody(blockSyntaxAssertions =>
                        {
                            blockSyntaxAssertions
                                .HasSyntaxNodesCount(1)
                                .HasNextSyntaxNode(nodeAssertions => nodeAssertions.BeReturnStatement(expressionSyntaxAssertions =>
                                {
                                    expressionSyntaxAssertions.BeIdentifierNameSyntax("input");
                                }));
                        });
                });
    }
}