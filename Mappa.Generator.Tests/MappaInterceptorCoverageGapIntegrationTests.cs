// <copyright file="MappaInterceptorCoverageGapIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests targeting remaining interceptor coverage gaps.
/// </summary>
public sealed class MappaInterceptorCoverageGapIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string InterceptorsNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";

    /// <summary>
    /// Global-namespace interceptor classes omit a namespace wrapper in generated output.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanEmitInterceptorInGlobalNamespace()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  [MappaInterceptor]
                                  public partial class GlobalNamespaceMapper
                                  {
                                      public int Map(int value)
                                      {
                                          return this.MappaMap<int>(value);
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(
            sourceCode,
            editorConfig: null,
            LanguageVersion.Latest,
            interceptorsNamespaces: null,
            runTwice: true,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "GlobalNamespaceMapper",
                "__MappaIntercept_0",
                "int",
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    "int",
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeReturnStatement("__mappaTypedSource");
                        });
                });
    }

    /// <summary>
    /// Block-scoped namespaces emit interceptor sources inside a namespace declaration.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanEmitInterceptorInsideBlockScopedNamespace()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode
                                  {
                                      [MappaInterceptor]
                                      public partial class BlockNamespaceMapper
                                      {
                                          public int Map(int value)
                                          {
                                              return this.MappaMap<int>(value);
                                          }
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "BlockNamespaceMapper",
                "__MappaIntercept_0",
                "int",
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    "int",
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeReturnStatement("__mappaTypedSource");
                        });
                });
    }

    /// <summary>
    /// Pre-existing members named like generated interceptors force a suffixed method name.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanAllocateSuffixedInterceptorMethodNameWhenBaseNameTaken()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class NameCollisionMapper
                                  {
                                      private int __MappaIntercept_0(int value) => value;

                                      public int Map(int value)
                                      {
                                          return this.MappaMap<int>(value);
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "NameCollisionMapper",
                "__MappaIntercept_0_1",
                "int",
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    "int",
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeReturnStatement("__mappaTypedSource");
                        });
                });
    }

    /// <summary>
    /// Duplicate class-level object factories stop interceptor strategy generation.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SkipsInterceptorEmissionWhenObjectFactoriesAreDuplicated()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [MappaInterceptor]
                                  [MappaObjectFactory(typeof(Target), nameof(CreateA))]
                                  [MappaObjectFactory(typeof(Target), nameof(CreateB))]
                                  public partial class DuplicateFactoryMapper
                                  {
                                      public Target CreateA() => new Target();

                                      public Target CreateB() => new Target();

                                      public Target Map(Source value)
                                      {
                                          return this.MappaMap<Target>(value);
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                Mappa.Generator.Diagnostics.MappaDiagnosticDescriptors.DuplicateObjectFactoryForTargetType,
                "__MappaIntercept_0",
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target")
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubMethodsOnly("DuplicateFactoryMapper");
    }

    /// <summary>
    /// Unmappable interceptor call sites skip emitting an interceptor method body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SkipsInterceptorEmissionWhenStrategyIsNoMap()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source { }

                                  public class Target
                                  {
                                      public Target(string required) { }
                                  }

                                  [MappaInterceptor]
                                  public partial class NoMapMapper
                                  {
                                      public Target Map(Source value)
                                      {
                                          return this.MappaMap<Target>(value);
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                Mappa.Generator.Diagnostics.MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target")
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubMethodsOnly("NoMapMapper");
    }
}