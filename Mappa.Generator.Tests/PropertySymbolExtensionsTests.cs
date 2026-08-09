// <copyright file="PropertySymbolExtensionsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
    /// <c>PropertySymbolExtensions.IsGetterAccessible</c> for public set, private set, and get-only properties.
    /// </summary>
    [Fact]
    [UnitTest]
    public void PropertyAccessorAccessibilityDetectsGetOnlyPrivateSetAndPublicProperties()
    {
        const string source = """
                              namespace Coverage;

                              public sealed class Target
                              {
                                  public int PublicSet { get; set; }

                                  public int PrivateSet { get; private set; }

                                  public int GetOnly { get; }
                              }

                              public sealed class Source
                              {
                              }

                              public sealed class Mapper
                              {
                                  public Target Map(Source input) => new Target();
                              }
                              """;

        var (compilation, mapMethod, targetType) = CreateMapMethodAndTarget(source);
        var publicSet = targetType.GetMembers("PublicSet").OfType<IPropertySymbol>().Single();
        var privateSet = targetType.GetMembers("PrivateSet").OfType<IPropertySymbol>().Single();
        var getOnly = targetType.GetMembers("GetOnly").OfType<IPropertySymbol>().Single();

        publicSet.IsSetterAccessible(compilation, mapMethod).Should().BeTrue();
        publicSet.IsGetterAccessible(compilation, mapMethod).Should().BeTrue();

        privateSet.IsSetterAccessible(compilation, mapMethod).Should().BeFalse();
        privateSet.IsGetterAccessible(compilation, mapMethod).Should().BeTrue();

        getOnly.SetMethod.Should().BeNull();
        getOnly.IsSetterAccessible(compilation, mapMethod).Should().BeFalse();
        getOnly.IsGetterAccessible(compilation, mapMethod).Should().BeTrue();
    }

    /// <summary>
    /// Test getter accessibility for private-get and set-only properties.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsGetterAccessibleReturnsFalseWhenGetterIsMissingOrInaccessible()
    {
        const string source = """
                              namespace Coverage;

                              public sealed class Target
                              {
                                  public int Hidden { private get; set; }

                                  public int SetOnly
                                  {
                                      set { }
                                  }
                              }

                              public sealed class Source
                              {
                              }

                              public sealed class Mapper
                              {
                                  public Target Map(Source input) => new Target();
                              }
                              """;

        var (compilation, mapMethod, targetType) = CreateMapMethodAndTarget(source);
        var hidden = targetType.GetMembers("Hidden").OfType<IPropertySymbol>().Single();
        var setOnly = targetType.GetMembers("SetOnly").OfType<IPropertySymbol>().Single();

        hidden.IsGetterAccessible(compilation, mapMethod).Should().BeFalse();
        hidden.IsSetterAccessible(compilation, mapMethod).Should().BeTrue();

        setOnly.GetMethod.Should().BeNull();
        setOnly.IsGetterAccessible(compilation, mapMethod).Should().BeFalse();
        setOnly.IsSetterAccessible(compilation, mapMethod).Should().BeTrue();
    }

    private static (Compilation Compilation, MapMethod MapMethod, INamedTypeSymbol TargetType) CreateMapMethodAndTarget(
        string source)
    {
        var compilation = BuildCompilation(source);
        var tree = compilation.SyntaxTrees.Single();
        var model = compilation.GetSemanticModel(tree);
        var mapMethodSyntax = tree.GetRoot(TestContext.Current.CancellationToken)
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(method => method.Identifier.Text == "Map");
        var mapMethod = new MapMethod(
            mapMethodSyntax,
            model,
            nullableEnabled: true,
            TestContext.Current.CancellationToken);

        var targetType = compilation.GetTypeByMetadataName("Coverage.Target");
        if (targetType is null)
        {
            throw new InvalidOperationException("Expected Coverage.Target to be present in the compilation.");
        }

        return (compilation, mapMethod, targetType);
    }
}