// <copyright file="MappaInvokeMethodAttributeStrategyBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Builders.Strategies;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaInvokeMethodAttributeStrategyBuilder"/>.
/// </summary>
public sealed class MappaInvokeMethodAttributeStrategyBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttributeStrategyBuilder"/> throws when the invoked method
    /// requires <see cref="Mappa.MappaContext"/> but the root map method does not provide one.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenInvokedMethodRequiresMappaContextButRootMapMethodDoesNotProvideOne()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int PropertyA { get; set; }
                              }

                              public class Target
                              {
                                  public string PropertyA { get; set; }
                              }

                              public partial class Mapper
                              {
                                  public string CustomMapPropertyA(MappaContext context) => context.Keys.Count.ToString();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapper = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper");
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target");
        if (mapper is null || sourceType is null || targetType is null)
        {
            throw new InvalidOperationException("Expected mapper, source, and target types to be present in the compilation.");
        }

        var method = mapper.GetMembers("CustomMapPropertyA").OfType<IMethodSymbol>().Single();
        var propertyA = sourceType.GetMembers("PropertyA").OfType<IPropertySymbol>().Single();
        var strategy = new MappaInvokeMethodAttributeStrategy(
            targetType,
            sourceType,
            new MappaInvokeMethodAttribute("PropertyA", "CustomMapPropertyA"),
            fieldOrProperty: null,
            method,
            propertyA,
            isNullableEnabled: false,
            contextParameterName: null);
        var builder = new MappaInvokeMethodAttributeStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        using (builderContext.PushCurrentCompositeTypeSourceName("input"))
        {
            var act = () => builder.BuildSource("__mappa_tmp_1", builderContext, globalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("Invoked method requires MappaContext but the root map method does not provide one.");
        }
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttributeStrategyBuilder"/> throws when the invoked method
    /// parameter shape is not supported by <c>GetParameters</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenInvokedMethodHasUnexpectedParameterShape()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int PropertyA { get; set; }
                              }

                              public class Target
                              {
                                  public string PropertyA { get; set; }
                              }

                              public partial class Mapper
                              {
                                  public string CustomMapPropertyA(Source source, int propertyA, string extra, bool flag) => flag.ToString();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapper = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper");
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target");
        if (mapper is null || sourceType is null || targetType is null)
        {
            throw new InvalidOperationException("Expected mapper, source, and target types to be present in the compilation.");
        }

        var method = mapper.GetMembers("CustomMapPropertyA").OfType<IMethodSymbol>().Single();
        var propertyA = sourceType.GetMembers("PropertyA").OfType<IPropertySymbol>().Single();
        var strategy = new MappaInvokeMethodAttributeStrategy(
            targetType,
            sourceType,
            new MappaInvokeMethodAttribute("PropertyA", "CustomMapPropertyA"),
            fieldOrProperty: null,
            method,
            propertyA,
            isNullableEnabled: false,
            contextParameterName: null);
        var builder = new MappaInvokeMethodAttributeStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        using (builderContext.PushCurrentCompositeTypeSourceName("input"))
        {
            var act = () => builder.BuildSource("__mappa_tmp_1", builderContext, globalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("Unexpected parameter type");
        }
    }
}