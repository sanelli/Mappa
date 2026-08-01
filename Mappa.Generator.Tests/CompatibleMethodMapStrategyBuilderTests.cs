// <copyright file="CompatibleMethodMapStrategyBuilderTests.cs" company="Stefano Anelli">
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
/// Unit tests for <see cref="CompatibleMethodMapStrategyBuilder"/>.
/// </summary>
public sealed class CompatibleMethodMapStrategyBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="CompatibleMethodMapStrategyBuilder"/> types the temporary as the required target type
    /// and invokes the compatible map method without a forced cast.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceUsesRequiredTargetTypeWithoutCast()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class BaseSource { }

                              public class DerivedSource : BaseSource { }

                              public class BaseTarget { }

                              public class DerivedTarget : BaseTarget { }

                              public sealed class Dependency
                              {
                                  public DerivedTarget Map(BaseSource input) => new DerivedTarget();
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
        var requiredTarget = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget")!;
        var requiredSource = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.DerivedSource")!;
        var strategy = new CompatibleMethodMapStrategy(requiredTarget, requiredSource, mapMethod, contextParameterName: null);
        var builder = new CompatibleMethodMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        var (_, code) = builder.BuildSource("input", builderContext, globalOptions);

        code.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget __mappa_tmp_1 = this.DependencyProperty.Map(input);");
    }
}