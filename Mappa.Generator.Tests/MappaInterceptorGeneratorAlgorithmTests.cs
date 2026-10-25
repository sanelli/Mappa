// <copyright file="MappaInterceptorGeneratorAlgorithmTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaInterceptorGeneratorAlgorithm"/>.
/// </summary>
public sealed class MappaInterceptorGeneratorAlgorithmTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// When a candidate class has no <c>[MappaInterceptor]</c> attribute data,
    /// the algorithm returns without emitting sources.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteSkipsClassWithoutInterceptorAttributeData()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public partial class InlineMapper
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "InlineMapper");

        var runResult = RunAlgorithm(compilation, [classDeclaration]);

        runResult.Diagnostics.Should().BeEmpty();
        runResult.GeneratedTrees.Should().BeEmpty();
    }

    /// <summary>
    /// Null entries in the candidate array are skipped while valid interceptor classes still generate.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteSkipsNullClassDeclarations()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "InlineMapper");

        var runResult = RunAlgorithm(compilation, [null, classDeclaration]);

        runResult.Diagnostics.Should().BeEmpty();
        runResult.GeneratedTrees.Should().HaveCount(1);
    }

    /// <summary>
    /// When the declared class symbol cannot be resolved, the algorithm returns without emitting sources.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteSkipsClassWhenDeclaredSymbolCannotBeResolved()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "InlineMapper");

        var runResult = RunAlgorithm(
            compilation,
            [classDeclaration],
            static (_, _, _) => null);

        runResult.Diagnostics.Should().BeEmpty();
        runResult.GeneratedTrees.Should().BeEmpty();
    }

    /// <summary>
    /// Non-partial interceptor class reports a diagnostic and does not emit stubs.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteReportsNonPartialInterceptorDiagnostic()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public class InlineMapper
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "InlineMapper");

        var runResult = RunAlgorithm(compilation, [classDeclaration]);

        runResult.GeneratedTrees.Should().BeEmpty();
        runResult.Diagnostics.Should().ContainSingle(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorClassIsNotPartial.Id
            && diagnostic.GetMessage(null).Contains("InlineMapper", StringComparison.Ordinal));
    }

    /// <summary>
    /// Static interceptor class reports an error and does not emit stubs.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteReportsStaticInterceptorDiagnostic()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public static partial class InlineMapper
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "InlineMapper");

        var runResult = RunAlgorithm(compilation, [classDeclaration]);

        runResult.GeneratedTrees.Should().BeEmpty();
        runResult.Diagnostics.Should().ContainSingle(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorClassIsStatic.Id
            && diagnostic.GetMessage(null).Contains("InlineMapper", StringComparison.Ordinal));
    }

    /// <summary>
    /// Partial interceptor class emits default <c>MappaMap</c> stubs with and without context.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteEmitsDefaultMappaMapStubs()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "InlineMapper");

        var runResult = RunAlgorithm(compilation, [classDeclaration]);

        runResult.Diagnostics.Should().BeEmpty();
        runResult.GeneratedTrees.Should().ContainSingle();
        var generated = runResult.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generated.Should().Contain("TTarget MappaMap<TTarget>(object source)");
        generated.Should().Contain("TTarget MappaMap<TTarget>(object source, global::Mappa.MappaContext context)");
        generated.Should().Contain("InvalidOperationException");
    }

    /// <summary>
    /// Custom interceptor method name is used for emitted stubs.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteEmitsRenamedStubs()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor("MapInline")]
                              public partial class InlineMapper
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "InlineMapper");

        var runResult = RunAlgorithm(compilation, [classDeclaration]);

        runResult.Diagnostics.Should().BeEmpty();
        runResult.GeneratedTrees.Should().ContainSingle();
        var generated = runResult.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generated.Should().Contain("TTarget MapInline<TTarget>(object source)");
        generated.Should().Contain("TTarget MapInline<TTarget>(object source, global::Mappa.MappaContext context)");
        generated.Should().NotContain("TTarget MappaMap<TTarget>");
    }

    /// <summary>
    /// Call-site collation diagnostics from <c>With</c> are reported while stubs are still emitted.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteReportsCallSiteCollationDiagnostics()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      var settings = new MappaSettingsAttribute();
                                      return this.MappaMap<string>(value).With(settings);
                                  }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "InlineMapper");

        var runResult = RunAlgorithm(compilation, [classDeclaration]);

        runResult.GeneratedTrees.Should().ContainSingle();
        runResult.Diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorWithArgumentMustBeObjectCreation.Id);
    }

    /// <summary>
    /// After stubs exist in the compilation, a second generator pass emits interceptor methods
    /// with <c>[InterceptsLocation]</c> for identity-mappable call sites.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteEmitsInterceptorMethodsOnSecondPass()
    {
        const string source = """
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
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "InlineMapper");

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new InterceptorAlgorithmHarness([classDeclaration]).AsSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var updatedCompilation,
            out _,
            TestContext.Current.CancellationToken);
        driver = driver.RunGeneratorsAndUpdateCompilation(
            updatedCompilation,
            out _,
            out _,
            TestContext.Current.CancellationToken);
        var runResult = driver.GetRunResult();

        runResult.GeneratedTrees.Should().ContainSingle();
        var generated = runResult.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generated.Should().Contain("InterceptsLocationAttribute");
        generated.Should().Contain("__MappaIntercept_");
        generated.Should().Contain("__mappaTypedSource");
    }

    /// <summary>
    /// Classes with both <c>[Mappa]</c> and <c>[MappaInterceptor]</c> still emit interceptor stubs.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ExecuteAllowsCoexistenceWithMappaAttribute()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public partial long Map(int value);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var classDeclaration = GetClassDeclaration(compilation, "InlineMapper");

        var runResult = RunAlgorithm(compilation, [classDeclaration]);

        runResult.GeneratedTrees.Should().ContainSingle();
        var generated = runResult.GeneratedTrees[0].GetText(TestContext.Current.CancellationToken).ToString();
        generated.Should().Contain("TTarget MappaMap<TTarget>(object source)");
    }

    private static ClassDeclarationSyntax GetClassDeclaration(Compilation compilation, string className)
        => compilation.SyntaxTrees
            .SelectMany(syntaxTree => syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>())
            .Single(declaration => declaration.Identifier.Text == className);

    private static GeneratorDriverRunResult RunAlgorithm(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax?> classDeclarations,
        Func<Compilation, ClassDeclarationSyntax, CancellationToken, INamedTypeSymbol?>? classSymbolResolver = null)
    {
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new InterceptorAlgorithmHarness(classDeclarations, classSymbolResolver).AsSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out _,
            out _,
            TestContext.Current.CancellationToken);
        return driver.GetRunResult();
    }

    /// <summary>
    /// Harness generator that runs <see cref="MappaInterceptorGeneratorAlgorithm"/>
    /// against a fixed set of class declarations.
    /// </summary>
    private sealed class InterceptorAlgorithmHarness
        : IIncrementalGenerator
    {
        private readonly ImmutableArray<ClassDeclarationSyntax?> classDeclarations;
        private readonly Func<Compilation, ClassDeclarationSyntax, CancellationToken, INamedTypeSymbol?>? classSymbolResolver;

        public InterceptorAlgorithmHarness(
            ImmutableArray<ClassDeclarationSyntax?> classDeclarations,
            Func<Compilation, ClassDeclarationSyntax, CancellationToken, INamedTypeSymbol?>? classSymbolResolver = null)
        {
            this.classDeclarations = classDeclarations;
            this.classSymbolResolver = classSymbolResolver;
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterSourceOutput(
                context.AnalyzerConfigOptionsProvider.Combine(context.CompilationProvider),
                (productionContext, pair) =>
                {
                    new MappaInterceptorGeneratorAlgorithm(
                            productionContext,
                            pair.Left,
                            pair.Right,
                            this.classDeclarations,
                            this.classSymbolResolver)
                        .Execute();
                });
        }
    }
}