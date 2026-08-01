// <copyright file="MappaDependencyInjectionIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <c>MappaDependencyInjectionAttribute</c> happy paths.
/// </summary>
public sealed partial class MappaDependencyInjectionIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string SourceNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";
    private const string ServiceCollectionTypeName = "Microsoft.Extensions.DependencyInjection.IServiceCollection";
    private const string MapperATypeName = $"{SourceNamespace}.MapperA";
    private const string MapperBTypeName = $"{SourceNamespace}.MapperB";
    private const string GlobalMapperA = $"global::{MapperATypeName}";
    private const string GlobalMapperB = $"global::{MapperBTypeName}";

    /// <summary>
    /// Default static registrar registers all same-assembly mappers as singletons via an extension method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DefaultStaticRegistrarRegistersMappersAsSingletons()
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
                                  public sealed partial class MapperB
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
                                            .HasSyntaxNodesCount(3)
                                            .HasNextSyntaxNode(nodeAssertions =>
                                                nodeAssertions.BeInvocationExpressionSyntaxStatement(
                                                    $"services.AddSingleton<{GlobalMapperA}>"))
                                            .HasNextSyntaxNode(nodeAssertions =>
                                                nodeAssertions.BeInvocationExpressionSyntaxStatement(
                                                    $"services.AddSingleton<{GlobalMapperB}>"))
                                            .HasNextSyntaxNode(nodeAssertions =>
                                                nodeAssertions.BeReturnStatement("services"));
                                    });
                            });
                });
            });
    }

    /// <summary>
    /// Constructor method name is used when <c>MethodName</c> is unset.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ConstructorMethodNameIsUsedWhenMethodNamePropertyIsUnset()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection("RegisterAllMappers")]
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
                        "RegisterAllMappers",
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
    /// <c>MethodName</c> property overrides the constructor method name.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MethodNamePropertyOverridesConstructorMethodName()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection("FromConstructor", MethodName = "FromProperty")]
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
                        "FromProperty",
                        true,
                        [
                            (ServiceCollectionTypeName, NullableAnnotation.None, "services", RefKind.None, false),
                        ],
                        _ => { /* method existence is enough */ });
                });
            });
    }

    /// <summary>
    /// Scoped lifetime emits <c>AddScoped</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ScopedLifetimeEmitsAddScoped()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection(ServiceLifetime = MappaDependencyInjectionServiceLifetime.Scoped)]
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
                                            $"services.AddScoped<{GlobalMapperA}>"))
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeReturnStatement("services"));
                            });
                        });
                });
            });
    }

    /// <summary>
    /// Transient lifetime emits <c>AddTransient</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TransientLifetimeEmitsAddTransient()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection(ServiceLifetime = MappaDependencyInjectionServiceLifetime.Transient)]
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
                                            $"services.AddTransient<{GlobalMapperA}>"))
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeReturnStatement("services"));
                            });
                        });
                });
            });
    }

    /// <summary>
    /// InterfaceOnly registers every eligible interface without the concrete class.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task InterfaceOnlyRegistersAllEligibleInterfaces()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IMapperA
                                  {
                                  }

                                  public interface IMapperB
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class MapperA : IMapperA, IMapperB
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
        const string globalIMapperA = $"global::{SourceNamespace}.IMapperA";
        const string globalIMapperB = $"global::{SourceNamespace}.IMapperB";
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
                                    .HasSyntaxNodesCount(3)
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeInvocationExpressionSyntaxStatement(
                                            $"services.AddSingleton<{globalIMapperA}, {GlobalMapperA}>"))
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeInvocationExpressionSyntaxStatement(
                                            $"services.AddSingleton<{globalIMapperB}, {GlobalMapperA}>"))
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeReturnStatement("services"));
                            });
                        });
                });
            });
    }

    /// <summary>
    /// InterfaceAndClass registers the concrete type and every eligible interface with a shared factory.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task InterfaceAndClassRegistersConcreteAndInterfaces()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IMapperA
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class MapperA : IMapperA
                                  {
                                  }

                                  [MappaDependencyInjection(InjectInterfaces = MappaDependencyInjectionInjectInterfaces.InterfaceAndClass)]
                                  public static partial class Registrar
                                  {
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        const string globalIMapperA = $"global::{SourceNamespace}.IMapperA";
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
                                    .HasSyntaxNodesCount(3)
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeInvocationExpressionSyntaxStatement(
                                            $"services.AddSingleton<{GlobalMapperA}>"))
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeInvocationExpressionSyntaxStatement(
                                            $"services.AddSingleton<{globalIMapperA}, {GlobalMapperA}>",
                                            argument => argument.BeSimpleLambdaExpressionSyntax(
                                                "serviceProvider",
                                                body => body.BeInvocationExpressionSyntax(
                                                    $"serviceProvider.GetRequiredService<{GlobalMapperA}>"))))
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeReturnStatement("services"));
                            });
                        });
                });
            });
    }

    /// <summary>
    /// <c>IgnoreType</c> excludes a concrete mapper from registration.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task IgnoreTypeExcludesConcreteMapper()
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
                                  public sealed partial class MapperB
                                  {
                                  }

                                  [MappaDependencyInjection(IgnoreType = new[] { typeof(MapperB) })]
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
    /// <c>IgnoreType</c> excludes an interface from interface registrations.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task IgnoreTypeExcludesInterfaceFromInterfaceRegistrations()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IMapperA
                                  {
                                  }

                                  public interface IMapperB
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class MapperA : IMapperA, IMapperB
                                  {
                                  }

                                  [MappaDependencyInjection(
                                      InjectInterfaces = MappaDependencyInjectionInjectInterfaces.InterfaceOnly,
                                      IgnoreType = new[] { typeof(IMapperB) })]
                                  public static partial class Registrar
                                  {
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        const string globalIMapperA = $"global::{SourceNamespace}.IMapperA";
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
                                            $"services.AddSingleton<{globalIMapperA}, {GlobalMapperA}>"))
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeReturnStatement("services"));
                            });
                        });
                });
            });
    }

    /// <summary>
    /// Static registrar with <c>ExtensionMethod = false</c> emits a non-extension static method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task StaticRegistrarWithExtensionMethodFalseIsNotExtension()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection(ExtensionMethod = false)]
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
                        false,
                        [
                            (ServiceCollectionTypeName, NullableAnnotation.None, "services", RefKind.None, false),
                        ],
                        methodAssertions =>
                        {
                            methodAssertions.HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword);
                        });
                });
            });
    }

    /// <summary>
    /// Non-static registrar emits an instance method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NonStaticRegistrarEmitsInstanceMethod()
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
                                  public sealed partial class Registrar
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
                        .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword)
                        .HaveMethod(
                            ServiceCollectionTypeName,
                            NullableAnnotation.None,
                            "RegisterRegistrar",
                            false,
                            [
                                (ServiceCollectionTypeName, NullableAnnotation.None, "services", RefKind.None, false),
                            ],
                            methodAssertions =>
                            {
                                methodAssertions.HaveModifiers(SyntaxKind.PublicKeyword);
                            });
                });
            });
    }

    /// <summary>
    /// Accessibility property controls the generated method modifiers.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task InternalAccessibilityEmitsInternalMethod()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection(Accessibility = MappaDependencyInjectionMethodAccessibility.Internal)]
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
                            methodAssertions.HaveModifiers(SyntaxKind.InternalKeyword, SyntaxKind.StaticKeyword);
                        });
                });
            });
    }

    /// <summary>
    /// Private accessibility emits a private method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PrivateAccessibilityEmitsPrivateMethod()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection(Accessibility = MappaDependencyInjectionMethodAccessibility.Private)]
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
                            methodAssertions.HaveModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword);
                        });
                });
            });
    }

    /// <summary>
    /// Protected accessibility on a non-static registrar emits a protected instance method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ProtectedAccessibilityEmitsProtectedMethod()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection(Accessibility = MappaDependencyInjectionMethodAccessibility.Protected)]
                                  public partial class Registrar
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
                        false,
                        [
                            (ServiceCollectionTypeName, NullableAnnotation.None, "services", RefKind.None, false),
                        ],
                        methodAssertions =>
                        {
                            methodAssertions.HaveModifiers(SyntaxKind.ProtectedKeyword);
                        });
                });
            });
    }

    /// <summary>
    /// ProtectedInternal accessibility emits a protected internal instance method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ProtectedInternalAccessibilityEmitsProtectedInternalMethod()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class MapperA
                                  {
                                  }

                                  [MappaDependencyInjection(Accessibility = MappaDependencyInjectionMethodAccessibility.ProtectedInternal)]
                                  public partial class Registrar
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
                        false,
                        [
                            (ServiceCollectionTypeName, NullableAnnotation.None, "services", RefKind.None, false),
                        ],
                        methodAssertions =>
                        {
                            methodAssertions.HaveModifiers(SyntaxKind.ProtectedKeyword, SyntaxKind.InternalKeyword);
                        });
                });
            });
    }

    /// <summary>
    /// Block-scoped namespace registrar emits a block namespace in generated source.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task BlockScopedNamespaceRegistrarEmitsBlockNamespace()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode
                                  {
                                      [Mappa]
                                      public sealed partial class MapperA
                                      {
                                      }

                                      [MappaDependencyInjection]
                                      public static partial class Registrar
                                      {
                                      }
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
            .HaveCommentHeader()
            .HaveNamespaceDeclarationSyntax(namespaceDeclaration =>
            {
                namespaceDeclaration
                    .HaveNamespaceIdentifier(SourceNamespace)
                    .HaveClass("Registrar", classAssertions =>
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
    /// Global-namespace registrar emits registration code with no namespace declaration.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task GlobalNamespaceRegistrarEmitsNoNamespace()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

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
            .HaveCommentHeader()
            .HaveNoNamespaceDeclarationSyntax()
            .HaveClasses(1)
            .HaveClass("Registrar", classAssertions =>
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
                                        "services.AddSingleton<global::MapperA>"))
                                .HasNextSyntaxNode(nodeAssertions =>
                                    nodeAssertions.BeReturnStatement("services"));
                        });
                    });
            });
    }

    /// <summary>
    /// Static <c>[Mappa]</c> mapper classes are skipped because they cannot be DI type arguments.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task StaticMappaMapperTypesAreSkipped()
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
    /// Nested <c>[Mappa]</c> mapper types in the same assembly are discovered and registered.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NestedMappaMapperTypeIsRegistered()
    {
        // Arrange
        const string sourceCode = """
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public partial class Outer
                                  {
                                      [Mappa]
                                      public sealed partial class NestedMapper
                                      {
                                      }
                                  }

                                  [MappaDependencyInjection]
                                  public static partial class Registrar
                                  {
                                  }
                                  """;

        // Act
        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        // Assert
        const string globalNestedMapper = $"global::{SourceNamespace}.Outer.NestedMapper";
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
                                            $"services.AddSingleton<{globalNestedMapper}>"))
                                    .HasNextSyntaxNode(nodeAssertions =>
                                        nodeAssertions.BeReturnStatement("services"));
                            });
                        });
                });
            });
    }
}