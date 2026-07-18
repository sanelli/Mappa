// <copyright file="PropertyMapStrategyBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="PropertyMapStrategyBuilder"/>.
/// </summary>
public sealed class PropertyMapStrategyBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test chained source with empty remaining segments uses the receiver type from the path prefix.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceUsesReceiverTypeWhenChainedPathHasNoRemainingSegments()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Address
                              {
                                  public string City { get; set; }
                              }

                              public class Source
                              {
                                  public Address Address { get; set; }
                              }

                              public class Target
                              {
                                  public Address Address { get; set; }
                              }

                              public partial class Mapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var addressType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Address")!;
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")!;
        var targetAddress = targetType.GetMembers("Address").OfType<IPropertySymbol>().Single();
        var mapMethod = CreateMapMethod(compilation, "Map");

        var chained = new ChainedSourcePropertyPathInfo(
            "Address",
            [],
            sourceType,
            "input.Address");
        var identity = new IdentityMapStrategy(addressType, addressType);
        var strategy = new PropertyMapStrategy(targetAddress, null, identity, false, chained);
        var builder = new PropertyMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        using (builderContext.PushMapMethod(mapMethod))
        {
            var (temporary, code) = builder.BuildSource("ignored", builderContext, globalOptions);

            temporary.Should().NotBeNullOrWhiteSpace();
            code.Should().Contain("input.Address");
            code.Should().Contain(addressType.ToDisplayString());
        }
    }

    /// <summary>
    /// Test chained source falls back to the starting source type when the receiver prefix cannot be resolved.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceFallsBackToStartingSourceTypeWhenReceiverPrefixCannotBeResolved()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Address
                              {
                                  public string City { get; set; }
                              }

                              public class Source
                              {
                                  public Address Address { get; set; }
                              }

                              public class Target
                              {
                                  public Address Address { get; set; }
                              }

                              public partial class Mapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")!;
        var addressType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Address")!;
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")!;
        var targetAddress = targetType.GetMembers("Address").OfType<IPropertySymbol>().Single();
        var mapMethod = CreateMapMethod(compilation, "Map");

        var chained = new ChainedSourcePropertyPathInfo(
            "Address",
            [],
            sourceType,
            "other.Address");
        var identity = new IdentityMapStrategy(addressType, sourceType);
        var strategy = new PropertyMapStrategy(targetAddress, null, identity, false, chained);
        var builder = new PropertyMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        using (builderContext.PushMapMethod(mapMethod))
        {
            var (_, code) = builder.BuildSource("ignored", builderContext, globalOptions);

            code.Should().Contain(sourceType.ToDisplayString());
            code.Should().Contain(" = ignored;");
        }
    }

    private static MapMethod CreateMapMethod(CSharpCompilation compilation, string methodName)
    {
        var syntaxTree = compilation.SyntaxTrees.Single(tree =>
            tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Any(methodSyntax => methodSyntax.Identifier.Text == methodName));
        var methodDeclarationSyntax = syntaxTree.GetRoot()
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(methodSyntax => methodSyntax.Identifier.Text == methodName);
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        return new MapMethod(
            methodDeclarationSyntax,
            semanticModel,
            nullableEnabled: true,
            CancellationToken.None);
    }
}