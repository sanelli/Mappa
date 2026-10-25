// <copyright file="MappaInterceptorDependencyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Models;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for interceptor-route dependency eligibility.
/// </summary>
public sealed class MappaInterceptorDependencyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string InterceptorsNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";
    private const string SourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.Source";
    private const string TargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";
    private const string InnerSourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.InnerSource";
    private const string InnerTargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.InnerTarget";

    /// <summary>
    /// <c>[MappaDependency]</c> properties are usable from interceptor maps.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanUseMappaDependencyFromInterceptorMap()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }
                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  public class Dependency
                                  {
                                      public InnerTarget Map(InnerSource input) => new InnerTarget { A = input.A };
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      [MappaDependency]
                                      private Dependency DependencyProperty { get; } = new Dependency();

                                      public Target Map(Source value) => this.MappaMap<Target>(value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        AssertUsesDependencyInvocation(
            generatedResults,
            "this.DependencyProperty.Map");
    }

    /// <summary>
    /// Unmarked helper methods on an interceptor-only class are not used as mapping dependencies.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task UnmarkedHelperIsNotUsedAsInterceptorDependency()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }
                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public InnerTarget Helper(InnerSource input) => new InnerTarget { A = input.A + 1 };

                                      public Target Map(Source value) => this.MappaMap<Target>(value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        AssertUsesInlineConstructorMapping(generatedResults);
    }

    /// <summary>
    /// Dual <c>[Mappa]</c> + <c>[MappaInterceptor]</c> makes implemented helpers available to interceptor maps.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DualMappaAttributeMakesImplementedHelpersAvailable()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }
                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  [Mappa]
                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public InnerTarget Helper(InnerSource input) => new InnerTarget { A = input.A };

                                      public Target Map(Source value) => this.MappaMap<Target>(value);

                                      public partial string Map(int value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        AssertUsesDependencyInvocation(
            generatedResults,
            "this.Helper",
            expectedGeneratedSources: 2);
    }

    /// <summary>
    /// <c>[MappaInterceptorMethodDependency]</c> helpers are usable on the interceptor path.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task InterceptorMethodDependencyIsUsableOnInterceptorPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }
                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      [MappaInterceptorMethodDependency]
                                      public InnerTarget Helper(InnerSource input) => new InnerTarget { A = input.A };

                                      public Target Map(Source value) => this.MappaMap<Target>(value);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        AssertUsesDependencyInvocation(
            generatedResults,
            "this.Helper");
    }

    /// <summary>
    /// <c>[MappaInterceptorMethodDependency]</c> helpers are ignored on the normal <c>[Mappa]</c> path.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task InterceptorMethodDependencyIsIgnoredOnMappaPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class InnerSource { public int A { get; set; } }
                                  public class InnerTarget { public int A { get; set; } }
                                  public class Source { public InnerSource Property { get; set; } }
                                  public class Target { public InnerTarget Property { get; set; } }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInterceptorMethodDependency]
                                      public InnerTarget Helper(InnerSource input) => new InnerTarget { A = input.A + 99 };

                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(
            sourceCode,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                InnerSourceType,
                                "__mappa_tmp_1",
                                init => init.BeMemberAccessExpressionSyntax("input.Property"));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappa_tmp_2",
                                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.A"));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                InnerTargetType,
                                "__mappa_tmp_3",
                                init => init.BeObjectCreationExpressionSyntax(
                                    InnerTargetType,
                                    ("A", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                TargetType,
                                "__mappa_tmp_4",
                                init => init.BeObjectCreationExpressionSyntax(
                                    TargetType,
                                    ("Property", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3"))));
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    private static void AssertUsesDependencyInvocation(
        GeneratedResults generatedResults,
        string invocationName,
        int expectedGeneratedSources = 1)
    {
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode(expectedGeneratedSources)
            .WithGeneratedCompilationUnit(hintNameContains: "Interceptor")
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethod(
                        TargetType,
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
                                                SourceType + "?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    SourceType,
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                InnerSourceType,
                                                "__mappa_tmp_1",
                                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Property"));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                InnerTargetType,
                                                "__mappa_tmp_2",
                                                init => init.BeInvocationExpressionSyntax(
                                                    invocationName,
                                                    arg => arg.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                TargetType,
                                                "__mappa_tmp_3",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    TargetType,
                                                    ("Property", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                                });
                        });
            });
    }

    private static void AssertUsesInlineConstructorMapping(GeneratedResults generatedResults)
    {
        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethod(
                        TargetType,
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
                                        .HasSyntaxNodesCount(6)
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                SourceType + "?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    SourceType,
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                InnerSourceType,
                                                "__mappa_tmp_1",
                                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Property"));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "int",
                                                "__mappa_tmp_2",
                                                init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.A"));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                InnerTargetType,
                                                "__mappa_tmp_3",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    InnerTargetType,
                                                    ("A", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                TargetType,
                                                "__mappa_tmp_4",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    TargetType,
                                                    ("Property", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                                });
                        });
            });
    }
}