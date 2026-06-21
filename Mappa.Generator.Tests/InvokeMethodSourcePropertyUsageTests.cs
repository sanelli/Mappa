// <copyright file="InvokeMethodSourcePropertyUsageTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="InvokeMethodSourcePropertyUsage"/>.
/// </summary>
public sealed class InvokeMethodSourcePropertyUsageTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="InvokeMethodSourcePropertyUsage.UsesSourceProperty"/> returns <c>false</c>
    /// when the method has four or more parameters.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsesSourcePropertyReturnsFalseWhenMethodHasFourOrMoreParameters()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int Foo { get; set; }
                              }

                              public class Mapper
                              {
                                  public string GetValue(Source source, int foo, string extra, bool flag) => foo.ToString();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapper = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper");
        mapper.Should().NotBeNull();
        var method = mapper!.GetMembers("GetValue").OfType<IMethodSymbol>().Single();
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        sourceType.Should().NotBeNull();
        var fooProperty = sourceType!.GetMembers("Foo").OfType<IPropertySymbol>().Single();

        var result = method.UsesSourceProperty(compilation, fooProperty, sourceType, false);

        result.Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="InvokeMethodSourcePropertyUsage.UsesSourceProperty"/> returns <c>true</c>
    /// when the method has two parameters and the second is not <see cref="Mappa.MappaContext"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsesSourcePropertyReturnsTrueWhenMethodHasTwoParametersWithoutMappaContextAsSecondParameter()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int Foo { get; set; }
                              }

                              public class Mapper
                              {
                                  public string GetValue(int foo, string extra) => foo.ToString();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapper = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper");
        mapper.Should().NotBeNull();
        var method = mapper!.GetMembers("GetValue").OfType<IMethodSymbol>().Single();
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        sourceType.Should().NotBeNull();
        var fooProperty = sourceType!.GetMembers("Foo").OfType<IPropertySymbol>().Single();

        var result = method.UsesSourceProperty(compilation, fooProperty, sourceType, false);

        result.Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="InvokeMethodSourcePropertyUsage.UsesSourceProperty"/> returns <c>false</c>
    /// when the source property is <c>null</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void UsesSourcePropertyReturnsFalseWhenSourcePropertyIsNull()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int Foo { get; set; }
                              }

                              public class Mapper
                              {
                                  public string GetValue(int foo) => foo.ToString();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapper = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper");
        mapper.Should().NotBeNull();
        var method = mapper!.GetMembers("GetValue").OfType<IMethodSymbol>().Single();
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        sourceType.Should().NotBeNull();

        var result = method.UsesSourceProperty(compilation, null, sourceType!, false);

        result.Should().BeFalse();
    }
}