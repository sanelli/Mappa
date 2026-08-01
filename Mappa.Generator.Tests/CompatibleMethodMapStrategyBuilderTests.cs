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
        var mapMethod = CreateDependencyMapMethod(compilation, "Map", "this.DependencyProperty");
        var (_, code) = BuildCompatibleSource(compilation, mapMethod, contextParameterName: null, sourceVariable: "input");

        code.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget __mappa_tmp_1 = this.DependencyProperty.Map(input);");
    }

    /// <summary>
    /// Test <see cref="CompatibleMethodMapStrategyBuilder"/> omits the access-field prefix when
    /// <see cref="MapMethod.AccessFieldName"/> is empty.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceOmitsAccessFieldPrefixWhenAccessFieldNameIsEmpty()
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
                                  public static DerivedTarget Map(BaseSource input) => new DerivedTarget();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapMethod = CreateDependencyMapMethod(compilation, "Map", accessFieldName: string.Empty);
        var (_, code) = BuildCompatibleSource(compilation, mapMethod, contextParameterName: null, sourceVariable: "input");

        code.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget __mappa_tmp_1 = Map(input);");
    }

    /// <summary>
    /// Test <see cref="CompatibleMethodMapStrategyBuilder"/> omits the context argument when the method requires
    /// <see cref="Mappa.MappaContext"/> but the strategy does not provide a context parameter name.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceOmitsContextArgumentWhenContextParameterNameIsNull()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class BaseSource { }

                              public class DerivedSource : BaseSource { }

                              public class BaseTarget { }

                              public class DerivedTarget : BaseTarget { }

                              public sealed class Dependency
                              {
                                  public DerivedTarget Map(BaseSource input, MappaContext context) => new DerivedTarget();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapMethod = CreateDependencyMapMethod(compilation, "Map", "this.DependencyProperty");
        var (_, code) = BuildCompatibleSource(compilation, mapMethod, contextParameterName: null, sourceVariable: "input");

        code.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget __mappa_tmp_1 = this.DependencyProperty.Map(input);");
    }

    /// <summary>
    /// Test <see cref="CompatibleMethodMapStrategyBuilder"/> passes the context argument when the method requires
    /// <see cref="Mappa.MappaContext"/> and the strategy provides a context parameter name.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceIncludesContextArgumentWhenContextParameterNameIsProvided()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class BaseSource { }

                              public class DerivedSource : BaseSource { }

                              public class BaseTarget { }

                              public class DerivedTarget : BaseTarget { }

                              public sealed class Dependency
                              {
                                  public DerivedTarget Map(BaseSource input, MappaContext context) => new DerivedTarget();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapMethod = CreateDependencyMapMethod(compilation, "Map", "this.DependencyProperty");
        var (_, code) = BuildCompatibleSource(compilation, mapMethod, contextParameterName: "context", sourceVariable: "input");

        code.Should().Be("Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget __mappa_tmp_1 = this.DependencyProperty.Map(input, context);");
    }

    private static MapMethod CreateDependencyMapMethod(CSharpCompilation compilation, string methodName, string accessFieldName)
    {
        var dependencyType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Dependency");
        if (dependencyType is null)
        {
            throw new InvalidOperationException("Expected dependency type to be present in the compilation.");
        }

        var methodSymbol = dependencyType.GetMembers(methodName).OfType<IMethodSymbol>().Single();
        return new MapMethod(
            methodSymbol,
            accessFieldName,
            nullableEnabled: false,
            canBeUsedByStaticMethod: false,
            attributes: []);
    }

    private static (string VariableName, string Code) BuildCompatibleSource(
        CSharpCompilation compilation,
        MapMethod mapMethod,
        string? contextParameterName,
        string sourceVariable)
    {
        var requiredTarget = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.BaseTarget")!;
        var requiredSource = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.DerivedSource")!;
        var strategy = new CompatibleMethodMapStrategy(requiredTarget, requiredSource, mapMethod, contextParameterName);
        var builder = new CompatibleMethodMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        return builder.BuildSource(sourceVariable, builderContext, globalOptions);
    }
}