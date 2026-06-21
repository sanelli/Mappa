// <copyright file="MethodMapStrategyBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MethodMapStrategyBuilder"/>.
/// </summary>
public sealed class MethodMapStrategyBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MethodMapStrategyBuilder"/> omits the context argument when the dependency method requires
    /// <see cref="Mappa.MappaContext"/> but the root map method does not provide one.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceOmitsContextArgumentWhenRootMapMethodDoesNotProvideContext()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              public sealed class Dependency
                              {
                                  public Target Map(Source input, MappaContext context) => new Target();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var dependencyType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Dependency");
        if (dependencyType is null)
        {
            throw new InvalidOperationException("Expected dependency type to be present in the compilation.");
        }

        var methodSymbol = dependencyType.GetMembers("Map").OfType<IMethodSymbol>().Single();
        var mapMethod = new MapMethod(
            methodSymbol,
            "this.DependencyProperty",
            nullableEnabled: false,
            canBeUsedByStaticMethod: false,
            attributes: []);
        var strategy = new MethodMapStrategy(mapMethod, contextParameterName: null);
        var builder = new MethodMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        var (_, code) = builder.BuildSource("input", builderContext, globalOptions);

        code.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.Target __mappa_tmp_1 = this.DependencyProperty.Map(input);");
    }

    /// <summary>
    /// Test <see cref="MethodMapStrategyBuilder"/> uses <see cref="MapMethod.AccessFieldName"/> when invoking a dependency method.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceUsesAccessFieldNameForDependencyInvocation()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              public sealed class Dependency
                              {
                                  public Target Map(Source input) => new Target();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var dependencyType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Dependency");
        if (dependencyType is null)
        {
            throw new InvalidOperationException("Expected dependency type to be present in the compilation.");
        }

        var methodSymbol = dependencyType.GetMembers("Map").OfType<IMethodSymbol>().Single();
        var mapMethod = new MapMethod(
            methodSymbol,
            "this.DependencyProperty",
            nullableEnabled: false,
            canBeUsedByStaticMethod: false,
            attributes: []);
        var strategy = new MethodMapStrategy(mapMethod, contextParameterName: null);
        var builder = new MethodMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        var (_, code) = builder.BuildSource("mappedSource", builderContext, globalOptions);

        code.Should().Contain("this.DependencyProperty.Map(mappedSource)");
    }
}