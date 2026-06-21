// <copyright file="MappaAssignFromConstantAttributeStrategyBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaAssignFromConstantAttributeStrategyBuilder"/>.
/// </summary>
public sealed class MappaAssignFromConstantAttributeStrategyBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <c>ValueToCode</c> throws when the attribute constant type is unsupported.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenConstantTypeIsUnsupported()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target
                              {
                                  public decimal Property { get; set; }
                              }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  [MappaAssignFromConstant(nameof(Target.Property), 17.5m)]
                                  public partial Target Map(Source input, MappaContext context);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target");
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper",
            "Map");
        var assignFromConstantAttribute = attributes.GetMappaAssignFromConstantAttributes(compilation).Single();
        if (targetType is null)
        {
            throw new InvalidOperationException("Expected target type to be present in the compilation.");
        }

        var strategy = new MappaAssignFromConstantAttributeStrategy(targetType, assignFromConstantAttribute);
        var builder = new MappaAssignFromConstantAttributeStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        var act = () => builder.BuildSource("__mappa_tmp_1", builderContext, globalOptions);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Unexpected MappaAssignFromConstant attribute value.");
    }

    /// <summary>
    /// Test <c>ValueToCode</c> throws when the attribute enum constant does not match a declared enum member.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenEnumConstantIsUndefined()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public enum MyEnum
                              {
                                  One,
                                  Two,
                              }

                              public class Source { }

                              public class Target
                              {
                                  public MyEnum Property { get; set; }
                              }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  [MappaAssignFromConstant(nameof(Target.Property), (MyEnum)99)]
                                  public partial Target Map(Source input, MappaContext context);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target");
        var attributes = AttributeDataExtensionsTestHelper.GetMethodAttributes(
            compilation,
            "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper",
            "Map");
        var assignFromConstantAttribute = attributes.GetMappaAssignFromConstantAttributes(compilation).Single();
        if (targetType is null)
        {
            throw new InvalidOperationException("Expected target type to be present in the compilation.");
        }

        var strategy = new MappaAssignFromConstantAttributeStrategy(targetType, assignFromConstantAttribute);
        var builder = new MappaAssignFromConstantAttributeStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        var act = () => builder.BuildSource("__mappa_tmp_1", builderContext, globalOptions);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Unexpected enumeration value");
    }
}