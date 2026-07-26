// <copyright file="QueryableProjectionMapStrategyBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;
using Mappa.Generator.Exceptions;
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
/// Unit tests for <see cref="QueryableProjectionMapStrategyBuilder"/>.
/// </summary>
public sealed class QueryableProjectionMapStrategyBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test the builder throws when the element strategy cannot be expressed as a projection.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenElementStrategyIsNotSupported()
    {
        const string source = """
                              using System.Linq;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Order
                              {
                                  public int Id { get; set; }
                              }

                              public class OrderDto
                              {
                                  public int Id { get; set; }
                              }

                              public static partial class Mapper
                              {
                                  public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var orderType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Order")
                        ?? throw new InvalidOperationException("Order type was not found.");
        var orderDtoType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.OrderDto")
                           ?? throw new InvalidOperationException("OrderDto type was not found.");
        var mapMethod = CreateMapMethod(compilation, "ProjectToDto");
        var unsupportedElementStrategy = new CollectionToCollectionMapStrategy(
            orderDtoType,
            orderType,
            new IdentityMapStrategy(
                orderDtoType,
                orderType,
                IdentityMapDeepCopySetting.ShallowCopy,
                requiresMemberwiseClone: false,
                nestedFieldStrategies: []),
            methodSymbol: null,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            EnumerableConcreteTypeSetting.Undefined);
        var strategy = new QueryableProjectionMapStrategy(
            mapMethod.TargetType,
            mapMethod.SourceType,
            unsupportedElementStrategy,
            orderType,
            orderDtoType,
            mapMethod.MethodSymbol);
        var builder = new QueryableProjectionMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        using (builderContext.PushMapMethod(mapMethod))
        {
            var act = () => builder.BuildSource("query", builderContext, globalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("Queryable projection element strategy is not supported.");
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