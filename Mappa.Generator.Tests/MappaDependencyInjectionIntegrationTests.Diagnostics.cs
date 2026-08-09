// <copyright file="MappaDependencyInjectionIntegrationTests.Diagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Diagnostic tests for <c>MappaDependencyInjectionAttribute</c> generation.
/// </summary>
public sealed partial class MappaDependencyInjectionIntegrationTests
{
    /// <summary>
    /// Non-partial registrar is filtered by the DI syntax provider (<c>IsPartial</c>) and does not generate sources or MP00070.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NonPartialRegistrarIsIgnoredBySyntaxProvider()
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
                                  public static class Registrar
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
    /// Applying both <c>[Mappa]</c> and <c>[MappaDependencyInjection]</c> reports MP00071.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task BothMappaAndDependencyInjectionAttributesReportError()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaDependencyInjection]
                                  public static partial class Registrar
                                  {
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappaAndMappaDependencyInjectionBothApplied, "Registrar")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Interface-only injection with a mapper that has no eligible interfaces reports MP00072.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task InterfaceOnlyWithNoEligibleInterfacesReportsWarning()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperWithoutInterfaces
                                  {
                                  }

                                  [MappaDependencyInjection(InjectInterfaces = MappaDependencyInjectionInjectInterfaces.InterfaceOnly)]
                                  public static partial class Registrar
                                  {
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaDependencyInjectionMapperHasNoEligibleInterfaces,
                "Mappa.Generator.Tests.UnitTests.SourceCode.MapperWithoutInterfaces")
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
                            ServiceCollectionTypeName,
                            NullableAnnotation.None,
                            "RegisterRegistrar",
                            true,
                            [
                                (ServiceCollectionTypeName, NullableAnnotation.None, "services", RefKind.None, false),
                            ],
                            methodAssertions =>
                            {
                                methodAssertions
                                    .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword)
                                    .HaveBody(blockSyntaxAssertions =>
                                    {
                                        blockSyntaxAssertions
                                            .HasSyntaxNodesCount(1)
                                            .HasNextSyntaxNode(nodeAssertions =>
                                                nodeAssertions.BeReturnStatement("services"));
                                    });
                            });
                });
            });
    }

    /// <summary>
    /// A static <c>[Mappa]</c> mapper reports MP00073 and is omitted from registration.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task StaticMapperReportsWarningAndIsSkippedFromRegistration()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [Mappa]
                                  public static partial class StaticMapper
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
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaDependencyInjectionStaticMapperSkipped,
                $"{SourceNamespace}.StaticMapper")
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
                            ServiceCollectionTypeName,
                            NullableAnnotation.None,
                            "RegisterRegistrar",
                            true,
                            [
                                (ServiceCollectionTypeName, NullableAnnotation.None, "services", RefKind.None, false),
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
                                                    $"services.AddSingleton<{GlobalMapperA}>"))
                                            .HasNextSyntaxNode(nodeAssertions =>
                                                nodeAssertions.BeReturnStatement("services"));
                                    });
                            });
                });
            });
    }
}