// <copyright file="MappaDependencyInjectionIntegrationTests.InjectFromAssemblies.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <c>InjectFromAssemblies</c> on <c>MappaDependencyInjectionAttribute</c>.
/// </summary>
public sealed partial class MappaDependencyInjectionIntegrationTests
{
    private const string GlobalExternalMapperAlpha = "global::Mappa.Ext.Alpha.ExternalMapperAlpha";
    private const string GlobalExternalMapperBeta = "global::Mappa.Ext.Beta.ExternalMapperBeta";

    /// <summary>
    /// When <c>InjectFromAssemblies</c> is unset, only same-assembly mappers are registered.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmptyInjectFromAssembliesRegistersOnlySameAssemblyMappers()
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
                    classAssertions.HaveMethod(
                        ServiceCollectionTypeName,
                        NullableAnnotation.None,
                        "RegisterRegistrar",
                        true,
                        [
                            (ServiceCollectionTypeName, NullableAnnotation.None, "services", RefKind.None, false),
                        ],
                        methodAssertions =>
                        {
                            methodAssertions.HaveBody(blockSyntaxAssertions =>
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

    /// <summary>
    /// Providing an external assembly marker registers that assembly's <c>[Mappa]</c> mappers
    /// in addition to same-assembly mappers.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task InjectFromAssembliesRegistersExternalAssemblyMappers()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;
                                  using Mappa.Ext.Alpha;
                                  using Mappa.Ext.Beta;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection(InjectFromAssemblies = new[]
                                  {
                                      typeof(ExternalMapperAlpha),
                                      typeof(ExternalMapperBeta),
                                  })]
                                  public static partial class Registrar
                                  {
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert — assemblies ordered by Identity.GetDisplayName(): Alpha, Beta, then test assembly.
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveFileScopedNamespace(fileScopedNamespace =>
            {
                fileScopedNamespace.HaveClass("Registrar", classAssertions =>
                {
                    classAssertions.HaveMethod(
                        ServiceCollectionTypeName,
                        NullableAnnotation.None,
                        "RegisterRegistrar",
                        true,
                        [
                            (ServiceCollectionTypeName, NullableAnnotation.None, "services", RefKind.None, false),
                        ],
                        methodAssertions =>
                        {
                            methodAssertions.HaveBody(blockSyntaxAssertions =>
                            {
                                blockSyntaxAssertions
                                    .HasSyntaxNodesCount(4)
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeInvocationExpressionSyntaxStatement(
                                            $"services.AddSingleton<{GlobalExternalMapperAlpha}>"))
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeInvocationExpressionSyntaxStatement(
                                            $"services.AddSingleton<{GlobalExternalMapperBeta}>"))
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

    /// <summary>
    /// <c>IgnoreType</c> excludes an external mapper discovered via <c>InjectFromAssemblies</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task IgnoreTypeExcludesExternalMapperFromInjectFromAssemblies()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;
                                  using Mappa.Ext.Alpha;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection(
                                      InjectFromAssemblies = new[] { typeof(ExternalMapperAlpha) },
                                      IgnoreType = new[] { typeof(ExternalMapperAlpha) })]
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
                    classAssertions.HaveMethod(
                        ServiceCollectionTypeName,
                        NullableAnnotation.None,
                        "RegisterRegistrar",
                        true,
                        [
                            (ServiceCollectionTypeName, NullableAnnotation.None, "services", RefKind.None, false),
                        ],
                        methodAssertions =>
                        {
                            methodAssertions.HaveBody(blockSyntaxAssertions =>
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