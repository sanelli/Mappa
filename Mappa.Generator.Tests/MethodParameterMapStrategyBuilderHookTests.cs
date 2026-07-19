// <copyright file="MethodParameterMapStrategyBuilderHookTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for hook generation in <see cref="MethodParameterMapStrategyBuilder"/>.
/// </summary>
public sealed class MethodParameterMapStrategyBuilderHookTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test hooks wrap an identity strategy and materialize the after-hook target.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceWrapsIdentityStrategyWithHooks()
    {
        var (compilation, mapMethod, beforeMethod, afterMethod) = BuildHookSymbols(
            """
            public partial int Map(int input, MappaContext context);
            private void Before(ref int input, MappaContext context) { }
            private static void After() { }
            """);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var strategy = new MethodParameterMapStrategy(
            new IdentityMapStrategy(intType, intType),
            [new MapHook(beforeMethod, null, null, null)],
            [new MapHook(afterMethod, null, null, null)]);
        var builder = new MethodParameterMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        using var mapMethodScope = builderContext.PushMapMethod(mapMethod);
        var (variableName, code) = builder.BuildSource("input", builderContext, globalOptions);

        variableName.Should().Be("return __mappa_tmp_1;");
        code.Should().Be(
            """
            this.Before(ref input, context);
            int __mappa_tmp_1 = input;
            After();
            """);
    }

    /// <summary>
    /// Test a context-only hook does not copy an <c>in</c> source parameter.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceDoesNotCopyInParameterForContextOnlyHook()
    {
        var (compilation, mapMethod, beforeMethod, _) = BuildHookSymbols(
            """
            public partial int Map(in int input, MappaContext context);
            private void Before(MappaContext context) { }
            private static void After() { }
            """);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var strategy = new MethodParameterMapStrategy(
            new IdentityMapStrategy(intType, intType),
            [new MapHook(beforeMethod, null, null, null)]);
        var builder = new MethodParameterMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        using var mapMethodScope = builderContext.PushMapMethod(mapMethod);
        var (variableName, code) = builder.BuildSource("input", builderContext, globalOptions);

        variableName.Should().Be("return input;");
        code.Should().Be("this.Before(context);");
    }

    private static (CSharpCompilation Compilation, MapMethod MapMethod, IMethodSymbol BeforeMethod, IMethodSymbol AfterMethod) BuildHookSymbols(
        string mapperMembers)
    {
        var source = $$"""
                       using Mappa;

                       namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                       public sealed partial class Mapper
                       {
                           {{mapperMembers}}
                       }
                       """;
        var compilation = BuildCompilation(source);
        var mapper = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper")
                     ?? throw new InvalidOperationException("Expected mapper type to be present in the compilation.");
        var mapMethodSymbol = mapper.GetMembers("Map").OfType<IMethodSymbol>().Single();
        var beforeMethod = mapper.GetMembers("Before").OfType<IMethodSymbol>().Single();
        var afterMethod = mapper.GetMembers("After").OfType<IMethodSymbol>().Single();
        var mapMethod = new MapMethod(
            mapMethodSymbol,
            "this",
            nullableEnabled: true,
            canBeUsedByStaticMethod: false,
            attributes: []);
        return (compilation, mapMethod, beforeMethod, afterMethod);
    }
}