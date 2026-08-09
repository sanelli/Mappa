// <copyright file="ForAttributeWithMetadataNameDiscoveryIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests documenting attribute-driven discovery via <c>ForAttributeWithMetadataName</c>
/// (partial-class predicates for both <c>[Mappa]</c> and <c>[MappaDependencyInjection]</c>).
/// </summary>
public sealed class ForAttributeWithMetadataNameDiscoveryIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// A partial class with unrelated attributes (no <c>[Mappa]</c>) does not enter the mapper pipeline.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialClassWithUnrelatedAttributesDoesNotGenerateMapperSources()
    {
        // Arrange
        const string sourceCode = """
                                  using System;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Obsolete("not a mapper")]
                                  public sealed partial class NotAMapper
                                  {
                                      public partial int Map(int input);
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// A non-partial class with <c>[MappaDependencyInjection]</c> is filtered by <c>cds.IsPartial()</c>
    /// and does not enter the DI pipeline (no sources, no MP00070).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NonPartialDependencyInjectionClassDoesNotEnterDiPipeline()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection]
                                  public static class NonPartialRegistrar
                                  {
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// A partial class with <c>[MappaDependencyInjection]</c> still enters the DI pipeline and generates registration.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PartialDependencyInjectionClassStillGeneratesRegistration()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection]
                                  public static partial class Registrar
                                  {
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
            .HaveFileScopedNamespace(fileScopedNamespace =>
            {
                fileScopedNamespace.HaveClass("Registrar", classAssertions =>
                {
                    classAssertions
                        .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
                        .HaveMethods(1)
                        .HaveMethod(
                            "Microsoft.Extensions.DependencyInjection.IServiceCollection",
                            NullableAnnotation.None,
                            "RegisterRegistrar",
                            true,
                            [
                                ("Microsoft.Extensions.DependencyInjection.IServiceCollection", NullableAnnotation.None, "services", RefKind.None, false),
                            ],
                            methodAssertions =>
                            {
                                methodAssertions
                                    .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)
                                    .HaveBody(blockSyntaxAssertions =>
                                    {
                                        blockSyntaxAssertions
                                            .HasSyntaxNodesCount(2)
                                            .HasNextSyntaxNode(nodeAssertions =>
                                                nodeAssertions.BeInvocationExpressionSyntaxStatement(
                                                    "services.AddSingleton<global::Mappa.Generator.Tests.UnitTests.SourceCode.MapperA>"))
                                            .HasNextSyntaxNode(nodeAssertions =>
                                                nodeAssertions.BeReturnStatement("services"));
                                    });
                            });
                });
            });
    }
}