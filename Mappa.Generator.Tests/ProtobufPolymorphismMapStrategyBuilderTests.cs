// <copyright file="ProtobufPolymorphismMapStrategyBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="ProtobufPolymorphismMapStrategyBuilder"/> defensive paths.
/// </summary>
public sealed class ProtobufPolymorphismMapStrategyBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test undefined default behavior throws while building the default branch.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsForUndefinedDefaultBehavior()
    {
        var setup = CreateSetup(
            ProtobufPolymorphismScenario.PocoToProto,
            new MappaProtobufTypeMappingDefaultAttribute(MappaProtobufTypeMappingDefaultBehavior.Undefined));

        using (setup.BuilderContext.PushMapMethod(setup.MapMethod))
        {
            var act = () => setup.Builder.BuildSource("input", setup.BuilderContext, setup.GlobalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("Unexpected undefined behavior while generating default branch for protobuf type mapping.");
        }
    }

    /// <summary>
    /// Test MapTo default behavior throws when the MapTo arm has not been identified.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenMapToArmIsMissing()
    {
        var setup = CreateSetup(
            ProtobufPolymorphismScenario.PocoToProto,
            new MappaProtobufTypeMappingDefaultAttribute(MappaProtobufTypeMappingDefaultBehavior.MapTo, "Fallback"));

        using (setup.BuilderContext.PushMapMethod(setup.MapMethod))
        {
            var act = () => setup.Builder.BuildSource("input", setup.BuilderContext, setup.GlobalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("The MapTo arm has not been identified.");
        }
    }

    /// <summary>
    /// Test switching on the source oneof case throws when the case property is missing.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenSourceCasePropertyIsMissing()
    {
        var setup = CreateSetup(
            ProtobufPolymorphismScenario.ProtoToPoco,
            new MappaProtobufTypeMappingDefaultAttribute(MappaProtobufTypeMappingDefaultBehavior.Throw));

        using (setup.BuilderContext.PushMapMethod(setup.MapMethod))
        {
            var act = () => setup.Builder.BuildSource("input", setup.BuilderContext, setup.GlobalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("The source oneof case property is missing.");
        }
    }

    /// <summary>
    /// Test building a case arm throws when the source case enum is missing.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenSourceCaseEnumIsMissing()
    {
        var setup = CreateSetup(
            ProtobufPolymorphismScenario.ProtoToPoco,
            new MappaProtobufTypeMappingDefaultAttribute(MappaProtobufTypeMappingDefaultBehavior.Throw),
            includeCaseProperty: true,
            includeArm: true);

        using (setup.BuilderContext.PushMapMethod(setup.MapMethod))
        {
            var act = () => setup.Builder.BuildSource("input", setup.BuilderContext, setup.GlobalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("The source oneof case enum is missing.");
        }
    }

    private static Setup CreateSetup(
        ProtobufPolymorphismScenario scenario,
        MappaProtobufTypeMappingDefaultAttribute defaultAttribute,
        bool includeCaseProperty = false,
        bool includeArm = false)
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int Case { get; set; }
                              }

                              public class Target { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = RequireType(compilation, "Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        var targetType = RequireType(compilation, "Mappa.Generator.Tests.UnitTests.SourceCode.Target");
        var caseProperty = includeCaseProperty
            ? sourceType.GetMembers("Case").OfType<IPropertySymbol>().Single()
            : null;
        ProtobufPolymorphismArm[] arms = includeArm
            ? [new ProtobufPolymorphismArm(new IdentityMapStrategy(targetType, sourceType), "Case", "Case", null)]
            : [];

        var strategy = new ProtobufPolymorphismMapStrategy(
            targetType,
            sourceType,
            scenario,
            caseProperty,
            sourceCaseEnumType: null,
            arms,
            defaultAttribute,
            mapToArm: null);
        var builder = new ProtobufPolymorphismMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);
        var mapMethod = CreateMapMethod(compilation, "Map");
        return new Setup(builder, builderContext, globalOptions, mapMethod);
    }

    private static MapMethod CreateMapMethod(Compilation compilation, string methodName)
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

    private static INamedTypeSymbol RequireType(Compilation compilation, string metadataName)
        => compilation.GetTypeByMetadataName(metadataName)
           ?? throw new InvalidOperationException($"Type '{metadataName}' was not found.");

    private sealed record Setup(
        ProtobufPolymorphismMapStrategyBuilder Builder,
        MappaBuilderContext BuilderContext,
        MappaGlobalOptions GlobalOptions,
        MapMethod MapMethod);
}