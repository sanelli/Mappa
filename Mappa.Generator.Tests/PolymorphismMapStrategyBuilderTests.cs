// <copyright file="PolymorphismMapStrategyBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa;
using Mappa.Attributes;
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
/// Unit tests for <see cref="PolymorphismMapStrategyBuilder"/> defensive default-branch paths.
/// </summary>
public sealed class PolymorphismMapStrategyBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test undefined default behavior throws while building the default branch.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsForUndefinedDefaultBehavior()
    {
        var setup = CreateSetup(new MappaTypeMappingDefaultAttribute(MappaTypeMappingDefaultBehavior.Undefined));

        using (setup.BuilderContext.PushMapMethod(setup.MapMethod))
        {
            var act = () => setup.Builder.BuildSource("input", setup.BuilderContext, setup.GlobalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("Unexpected undefined behavior while generating default branch for type mapping.");
        }
    }

    /// <summary>
    /// Test an unsupported default behavior value throws <see cref="ArgumentOutOfRangeException"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsForUnsupportedDefaultBehavior()
    {
        var setup = CreateSetup(new MappaTypeMappingDefaultAttribute((MappaTypeMappingDefaultBehavior)999));

        using (setup.BuilderContext.PushMapMethod(setup.MapMethod))
        {
            var act = () => setup.Builder.BuildSource("input", setup.BuilderContext, setup.GlobalOptions);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }

    /// <summary>
    /// Test throw behavior fails when the exception type has no suitable constructor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenExceptionTypeHasNoSuitableConstructor()
    {
        var setup = CreateSetup(
            new MappaTypeMappingDefaultAttribute(MappaTypeMappingDefaultBehavior.Throw, typeof(TwoArgException)),
            addTestAssemblyReference: true);

        using (setup.BuilderContext.PushMapMethod(setup.MapMethod))
        {
            var act = () => setup.Builder.BuildSource("input", setup.BuilderContext, setup.GlobalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("Cannot identify a suitable constructor to generate the exception");
        }
    }

    /// <summary>
    /// Test invoke-method behavior throws when the invoke method was not resolved.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenInvokeMethodIsMissing()
    {
        var setup = CreateSetup(new MappaTypeMappingDefaultAttribute("MissingMethod"));

        using (setup.BuilderContext.PushMapMethod(setup.MapMethod))
        {
            var act = () => setup.Builder.BuildSource("input", setup.BuilderContext, setup.GlobalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("Cannot identify the method to be invoked.");
        }
    }

    /// <summary>
    /// Test invoke-method behavior throws when the invoke type cannot be resolved in the compilation.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenInvokeTypeCannotBeResolved()
    {
        var setup = CreateSetup(
            new MappaTypeMappingDefaultAttribute(typeof(string[]), "Map"),
            useMapAsInvokeMethod: true);

        using (setup.BuilderContext.PushMapMethod(setup.MapMethod))
        {
            var act = () => setup.Builder.BuildSource("input", setup.BuilderContext, setup.GlobalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("Cannot identify the type on which the method is being invoked on.");
        }
    }

    /// <summary>
    /// Test invoke-method behavior throws when a two-parameter method is selected but context is missing.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenTwoParameterInvokeMethodHasNoContext()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input);

                                  public static Target DefaultMap(Source input, MappaContext context) => new Target();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = RequireType(compilation, "Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        var targetType = RequireType(compilation, "Mappa.Generator.Tests.UnitTests.SourceCode.Target");
        var mapperType = RequireType(compilation, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper");
        var invokeMethod = mapperType.GetMembers("DefaultMap").OfType<IMethodSymbol>().Single();
        var setup = CreateSetup(
            compilation,
            sourceType,
            targetType,
            new MappaTypeMappingDefaultAttribute("DefaultMap"),
            invokeMethod,
            contextParameterName: null);

        using (setup.BuilderContext.PushMapMethod(setup.MapMethod))
        {
            var act = () => setup.Builder.BuildSource("input", setup.BuilderContext, setup.GlobalOptions);

            act.Should()
                .Throw<MappaGeneratorException>()
                .WithMessage("Default mapping method requires to parameters but context on original mapping is not provided.");
        }
    }

    /// <summary>
    /// Test invoke-method behavior throws when the invoke method has an unsupported parameter count.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenInvokeMethodHasUnexpectedParameterCount()
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

                                  public static Target DefaultMap(Source input, int extra, string another) => new Target();
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = RequireType(compilation, "Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        var targetType = RequireType(compilation, "Mappa.Generator.Tests.UnitTests.SourceCode.Target");
        var mapperType = RequireType(compilation, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper");
        var invokeMethod = mapperType.GetMembers("DefaultMap").OfType<IMethodSymbol>().Single();
        var setup = CreateSetup(
            compilation,
            sourceType,
            targetType,
            new MappaTypeMappingDefaultAttribute("DefaultMap"),
            invokeMethod,
            contextParameterName: "context");

        using (setup.BuilderContext.PushMapMethod(setup.MapMethod))
        {
            var act = () => setup.Builder.BuildSource("input", setup.BuilderContext, setup.GlobalOptions);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }

    private static Setup CreateSetup(
        MappaTypeMappingDefaultAttribute attribute,
        bool useMapAsInvokeMethod = false,
        bool addTestAssemblyReference = false)
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
        if (addTestAssemblyReference)
        {
            compilation = compilation.AddReferences(
                MetadataReference.CreateFromFile(typeof(TwoArgException).Assembly.Location));
        }

        var sourceType = RequireType(compilation, "Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        var targetType = RequireType(compilation, "Mappa.Generator.Tests.UnitTests.SourceCode.Target");
        IMethodSymbol? invokeMethod = null;
        if (useMapAsInvokeMethod)
        {
            var mapperType = RequireType(compilation, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper");
            invokeMethod = mapperType.GetMembers("Map").OfType<IMethodSymbol>().Single();
        }

        return CreateSetup(compilation, sourceType, targetType, attribute, invokeMethod, contextParameterName: null);
    }

    private static Setup CreateSetup(
        CSharpCompilation compilation,
        ITypeSymbol sourceType,
        ITypeSymbol targetType,
        MappaTypeMappingDefaultAttribute attribute,
        IMethodSymbol? defaultInvokeMethod,
        string? contextParameterName)
    {
        var identity = new IdentityMapStrategy(targetType, sourceType);
        var strategy = new PolymorphismMapStrategy(
            targetType,
            sourceType,
            [],
            attribute,
            identity,
            nullableEnabled: true,
            mapMethodContextParameterName: contextParameterName,
            defaultInvokeMethod: defaultInvokeMethod);
        var builder = new PolymorphismMapStrategyBuilder(strategy);
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
        PolymorphismMapStrategyBuilder Builder,
        MappaBuilderContext BuilderContext,
        MappaGlobalOptions GlobalOptions,
        MapMethod MapMethod);
}