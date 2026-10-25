// <copyright file="MappaInterceptorDependencyCollectorTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaInterceptorDependencyCollector"/>.
/// </summary>
public sealed class MappaInterceptorDependencyCollectorTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Interceptor-only classes pick up methods marked with
    /// <c>[MappaInterceptorMethodDependency]</c> as mapping dependencies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesInterceptorMethodDependencies()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  [MappaInterceptorMethodDependency]
                                  public long Helper(int value) => value;

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod =>
            mapMethod.MethodName == "Helper"
            && mapMethod.SourceType.SpecialType == SpecialType.System_Int32
            && mapMethod.TargetType.SpecialType == SpecialType.System_Int64);
    }

    /// <summary>
    /// Without <c>[Mappa]</c>, unmarked implemented methods are not interceptor dependencies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIgnoresUnmarkedMethodsWithoutMappaAttribute()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public long Helper(int value) => value;

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().NotContain(mapMethod => mapMethod.MethodName == "Helper");
    }

    /// <summary>
    /// With <c>[Mappa]</c>, implemented mapper methods become available as interceptor dependencies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesImplementedMethodsWhenMappaAttributePresent()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public long Helper(int value) => value;

                                  public partial string Map(int value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: true);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod => mapMethod.MethodName == "Helper");
    }

    private static (MappaClassGeneratorContext ClassContext, List<Diagnostic> Diagnostics) Collect(
        string source,
        bool hasMappaAttribute)
    {
        var compilation = BuildCompilation(source);
        var classDeclaration = compilation.SyntaxTrees
            .SelectMany(syntaxTree => syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>())
            .Single(declaration => declaration.Identifier.Text == "InlineMapper");

        var options = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            classDeclaration.SyntaxTree);
        var diagnostics = new List<Diagnostic>();
        var mappaDebug = new MappaDebug(options, diagnostics.Add);
        var classContext = new MappaClassGeneratorContext(options, mappaDebug, compilation, classDeclaration);

        MappaInterceptorDependencyCollector.Collect(
            classContext,
            compilation,
            hasMappaAttribute,
            staticDependencyTypes: [],
            diagnostics.Add,
            TestContext.Current.CancellationToken);

        return (classContext, diagnostics);
    }
}