// <copyright file="MappaInterceptorIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for the <c>[MappaInterceptor]</c> happy paths.
/// </summary>
public sealed class MappaInterceptorIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string InterceptorsNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";

    /// <summary>
    /// Bare <c>MappaMap</c> without <c>With</c> emits stubs and an identity interceptor body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptBareMappaMapIdentity()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
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
                "InlineMapper",
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
    /// Context overload is intercepted with a matching interceptor signature.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptMappaMapWithContext()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public int Map(int value, MappaContext context)
                                      {
                                          return this.MappaMap<int>(value, context);
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
                "InlineMapper",
                "__MappaIntercept_0",
                "int",
                hasContext: true,
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
    /// Renamed interceptable method via <c>[MappaInterceptor("MapInline")]</c> is supported.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptRenamedMappaMapMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor("MapInline")]
                                  public partial class InlineMapper
                                  {
                                      public int Map(int value)
                                      {
                                          return this.MapInline<int>(value);
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
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                    .HaveMethods(3)
                    .HaveMethod(
                        "TTarget",
                        NullableAnnotation.NotAnnotated,
                        "MapInline",
                        false,
                        [("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false)],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveModifiers(SyntaxKind.PublicKeyword)
                                .HaveBody(_ => { /* stub body covered elsewhere */ });
                        })
                    .HaveMethod(
                        "int",
                        NullableAnnotation.NotAnnotated,
                        "__MappaIntercept_0",
                        false,
                        [("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false)],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveModifiers(SyntaxKind.PrivateKeyword)
                                .HaveInterceptsLocationAttribute()
                                .HaveBody(blockAssertions =>
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
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappaTypedSource"));
                                });
                        });
            });
    }

    /// <summary>
    /// Settings-only <c>With</c> still produces a successful interceptor mapping.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptMappaMapWithSettingsOnly()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public int Map(int value)
                                      {
                                          return this.MappaMap<int>(value)
                                              .With(new MappaSettingsAttribute { MapSourceFields = BooleanSetting.Enable });
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
                "InlineMapper",
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
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappaTypedSource"));
                });
    }

    /// <summary>
    /// Method-attributes-only <c>With</c> applies ignore-target configuration on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptMappaMapWithMethodAttributesOnly()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                      public int Ignored { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                      public int Ignored { get; set; }
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Target Map(Source value)
                                      {
                                          return this.MappaMap<Target>(value)
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new MappaIgnoreTargetPropertyAttribute("Ignored"),
                                                  });
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
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethods(3)
                    .HaveMethod(
                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                        NullableAnnotation.NotAnnotated,
                        "__MappaIntercept_0",
                        false,
                        [("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false)],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveInterceptsLocationAttribute()
                                .HaveBody(blockAssertions =>
                                {
                                    blockAssertions
                                        .HasSyntaxNodesCount(4)
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Source?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "int",
                                                "__mappa_tmp_1",
                                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Value"));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                "__mappa_tmp_2",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                                });
                        });
            });
    }

    /// <summary>
    /// Static-dependencies-only <c>With</c> resolves nested mapping via the declared helper.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptMappaMapWithStaticDependenciesOnly()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public long Value { get; set; }
                                  }

                                  public static class HelperMapper
                                  {
                                      public static long Map(int value) => value;
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Target Map(Source value)
                                      {
                                          return this.MappaMap<Target>(value)
                                              .With(
                                                  mappaStaticDependencies: new MappaStaticDependencyAttribute[]
                                                  {
                                                      new MappaStaticDependencyAttribute(typeof(HelperMapper)),
                                                  });
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
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethods(3)
                    .HaveMethod(
                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                        NullableAnnotation.NotAnnotated,
                        "__MappaIntercept_0",
                        false,
                        [("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false)],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveInterceptsLocationAttribute()
                                .HaveBody(blockAssertions =>
                                {
                                    blockAssertions
                                        .HasSyntaxNodesCount(5)
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Source?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "int",
                                                "__mappa_tmp_1",
                                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Value"));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "long",
                                                "__mappa_tmp_2",
                                                init => init.BeInvocationExpressionSyntax(
                                                    "global::Mappa.Generator.Tests.UnitTests.SourceCode.HelperMapper.Map",
                                                    arg => arg.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                "__mappa_tmp_3",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                                });
                        });
            });
    }

    /// <summary>
    /// Multiple contiguous <c>With</c> calls combine settings, static dependencies, and method attributes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInterceptMappaMapWithCombinedMultipleWiths()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Value { get; set; }
                                      public int Ignored { get; set; }
                                  }

                                  public class Target
                                  {
                                      public long Value { get; set; }
                                      public int Ignored { get; set; }
                                  }

                                  public static class HelperMapper
                                  {
                                      public static long Map(int value) => value;
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Target Map(Source value)
                                      {
                                          return this.MappaMap<Target>(value)
                                              .With(new MappaSettingsAttribute { MapSourceFields = BooleanSetting.Enable })
                                              .With(
                                                  mappaStaticDependencies: new MappaStaticDependencyAttribute[]
                                                  {
                                                      new MappaStaticDependencyAttribute(typeof(HelperMapper)),
                                                  })
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new MappaIgnoreTargetPropertyAttribute("Ignored"),
                                                  });
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
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethods(3)
                    .HaveMethod(
                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                        NullableAnnotation.NotAnnotated,
                        "__MappaIntercept_0",
                        false,
                        [("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false)],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveInterceptsLocationAttribute()
                                .HaveBody(blockAssertions =>
                                {
                                    blockAssertions
                                        .HasSyntaxNodesCount(5)
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Source?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "int",
                                                "__mappa_tmp_1",
                                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Value"));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "long",
                                                "__mappa_tmp_2",
                                                init => init.BeInvocationExpressionSyntax(
                                                    "global::Mappa.Generator.Tests.UnitTests.SourceCode.HelperMapper.Map",
                                                    arg => arg.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                "__mappa_tmp_3",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                                });
                        });
            });
    }

    /// <summary>
    /// A single generator pass with <c>InterceptorsNamespaces</c> emits both stubs and interceptors.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SinglePassEmitsStubsAndInterceptors()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
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
            InterceptorsNamespace,
            runTwice: false,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
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
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappaTypedSource"));
                });
    }
}