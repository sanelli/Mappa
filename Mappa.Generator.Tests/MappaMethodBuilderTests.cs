// <copyright file="MappaMethodBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaMethodBuilder"/>.
/// </summary>
public sealed class MappaMethodBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MappaMethodBuilder"/> emits a private non-partial signature for synthetic methods.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceEmitsPrivateNonPartialSignatureForSyntheticMethod()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")!;
        var mapperType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper")!;
        var mapMethod = MapMethod.CreateSynthetic(
            "Map__Source__To__Target",
            sourceType,
            targetType,
            mapperType,
            nullableEnabled: true,
            isStatic: false,
            sourceParameterName: "source",
            mappaContextParameterName: "context",
            location: null);
        mapMethod.SetStrategy(new MethodParameterMapStrategy(new IdentityMapStrategy(targetType, sourceType)));

        var builder = new MappaMethodBuilder(mapMethod);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        string generated;
        using (builderContext.PushMapMethod(mapMethod))
        {
            generated = builder.BuildSource(builderContext, globalOptions);
        }

        var signatureLine = generated
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(line => line.Trim())
            .Single(line => line.StartsWith("private ", StringComparison.Ordinal));

        signatureLine.Should().Be(
            "private Mappa.Generator.Tests.UnitTests.SourceCode.Target Map__Source__To__Target(Mappa.Generator.Tests.UnitTests.SourceCode.Source source, Mappa.MappaContext context)");
        signatureLine.Should().NotContain("partial");
    }
}