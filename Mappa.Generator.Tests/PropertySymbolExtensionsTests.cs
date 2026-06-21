// <copyright file="PropertySymbolExtensionsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="PropertySymbolExtensions"/>.
/// </summary>
public sealed class PropertySymbolExtensionsTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <c>PropertySymbolExtensions.IsSetterAccessible</c> and
    /// <c>PropertySymbolExtensions.IsGetterAccessible</c> for common accessor combinations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void PropertyAccessorAccessibilityDetectsGetOnlyPrivateSetAndPublicProperties()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Target
                              {
                                  public int PublicSet { get; set; }

                                  public int PrivateSet { get; private set; }

                                  public int GetOnly { get; }
                              }

                              public sealed partial class Mapper
                              {
                                  public Target Map(Source input) => throw new System.NotImplementedException();
                              }

                              public sealed class Source
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var target = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target");
        var mapper = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper");
        var mapMethod = mapper!.GetMembers("Map").OfType<IMethodSymbol>().Single();
        var publicSet = target!.GetMembers("PublicSet").OfType<IPropertySymbol>().Single();
        var privateSet = target.GetMembers("PrivateSet").OfType<IPropertySymbol>().Single();
        var getOnly = target.GetMembers("GetOnly").OfType<IPropertySymbol>().Single();

        publicSet.IsSetterAccessible(compilation, mapMethod).Should().BeTrue();
        publicSet.IsGetterAccessible(compilation, mapMethod).Should().BeTrue();

        privateSet.IsSetterAccessible(compilation, mapMethod).Should().BeFalse();
        privateSet.IsGetterAccessible(compilation, mapMethod).Should().BeTrue();

        getOnly.IsSetterAccessible(compilation, mapMethod).Should().BeFalse();
        getOnly.IsGetterAccessible(compilation, mapMethod).Should().BeTrue();
    }

    /// <summary>
    /// Test <c>PropertySymbolExtensions.IsGetterAccessible</c> returns <c>false</c> when the getter is inaccessible.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsGetterAccessibleReturnsFalseWhenGetterIsInaccessible()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Target
                              {
                                  public int Hidden { private get; set; }
                              }

                              public sealed partial class Mapper
                              {
                                  public Target Map(Source input) => throw new System.NotImplementedException();
                              }

                              public sealed class Source
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var target = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target");
        var mapper = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Mapper");
        var mapMethod = mapper!.GetMembers("Map").OfType<IMethodSymbol>().Single();
        var hidden = target!.GetMembers("Hidden").OfType<IPropertySymbol>().Single();

        hidden.IsGetterAccessible(compilation, mapMethod).Should().BeFalse();
        hidden.IsSetterAccessible(compilation, mapMethod).Should().BeTrue();
    }
}