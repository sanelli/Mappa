// <copyright file="OneOfWellKnownTypesStrategyBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for OneOf well-known strategy builders (edge / exception paths).
/// </summary>
public sealed class OneOfWellKnownTypesStrategyBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Emits a cast when construction kind is <see cref="OneOfBaseConstructionKind.ImplicitFromArm"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void OneOfBaseFromValueBuilderEmitsCastForImplicitFromArm()
    {
        var compilation = BuildCompilationWithOneOf();
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.StringOrInt")
            ?? throw new InvalidOperationException("StringOrInt was not found.");
        var strategy = new OneOfBaseFromValueMapStrategy(
            targetType,
            stringType,
            armIndex: 0,
            armType: stringType,
            OneOfBaseConstructionKind.ImplicitFromArm,
            optionStructType: null);
        var builder = new OneOfBaseFromValueMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = CreateGlobalOptions(compilation);

        var (_, code) = builder.BuildSource("input", builderContext, globalOptions);

        code.Should().Contain("= (global::Mappa.Generator.Tests.UnitTests.SourceCode.StringOrInt)input;");
    }

    /// <summary>
    /// Throws when construction kind is unexpected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void OneOfBaseFromValueBuilderThrowsForUnexpectedConstructionKind()
    {
        var compilation = BuildCompilationWithOneOf();
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.StringOrInt")
            ?? throw new InvalidOperationException("StringOrInt was not found.");
        var strategy = new OneOfBaseFromValueMapStrategy(
            targetType,
            stringType,
            armIndex: 0,
            armType: stringType,
            (OneOfBaseConstructionKind)999,
            optionStructType: null);
        var builder = new OneOfBaseFromValueMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = CreateGlobalOptions(compilation);

        var act = () => builder.BuildSource("input", builderContext, globalOptions);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Unexpected OneOfBase construction kind '999'.");
    }

    /// <summary>
    /// Throws when ConstructorFromOneOf is selected without an option struct type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void OneOfBaseFromValueBuilderThrowsWhenOptionStructTypeIsMissing()
    {
        var compilation = BuildCompilationWithOneOf();
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.StringOrInt")
            ?? throw new InvalidOperationException("StringOrInt was not found.");
        var strategy = new OneOfBaseFromValueMapStrategy(
            targetType,
            stringType,
            armIndex: 0,
            armType: stringType,
            OneOfBaseConstructionKind.ConstructorFromOneOf,
            optionStructType: null);
        var builder = new OneOfBaseFromValueMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = CreateGlobalOptions(compilation);

        var act = () => builder.BuildSource("input", builderContext, globalOptions);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("ConstructorFromOneOf construction requires an OptionStructType.");
    }

    /// <summary>
    /// Throws when wrap behavior is unexpected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void OneOfOptionFromValueBuilderThrowsForUnexpectedWrapBehavior()
    {
        var compilation = BuildCompilationWithOneOf();
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var optionType = GetOneOfStringNone(compilation);
        var strategy = new OneOfOptionFromValueMapStrategy(
            optionType,
            stringType,
            stringType,
            optionType,
            (OneOfOptionWrapBehavior)999,
            isOneOfBaseTarget: false);
        var builder = new OneOfOptionFromValueMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = CreateGlobalOptions(compilation);

        var act = () => builder.BuildSource("input", builderContext, globalOptions);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Unexpected OneOf option wrap behavior '999'.");
    }

    /// <summary>
    /// Throws when absent behavior is unexpected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void OneOfOptionToValueBuilderThrowsForUnexpectedAbsentBehavior()
    {
        var compilation = BuildCompilationWithOneOf();
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var optionType = GetOneOfStringNone(compilation);
        var strategy = new OneOfOptionToValueMapStrategy(
            stringType,
            optionType,
            stringType,
            (OneOfOptionAbsentBehavior)999);
        var builder = new OneOfOptionToValueMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = CreateGlobalOptions(compilation);

        var act = () => builder.BuildSource("input", builderContext, globalOptions);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Unexpected OneOf option absent behavior '999'.");
    }

    private static CSharpCompilation BuildCompilationWithOneOf()
    {
        const string source = """
                              using OneOf;
                              using OneOf.Types;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class StringOrInt : OneOfBase<string, int>
                              {
                                  public StringOrInt(OneOf<string, int> input)
                                      : base(input)
                                  {
                                  }

                                  public static implicit operator StringOrInt(string value)
                                      => new StringOrInt(value);
                              }
                              """;
        return BuildCompilation(source);
    }

    private static INamedTypeSymbol GetOneOfStringNone(Compilation compilation)
    {
        var unbound = compilation.GetTypeByMetadataName("OneOf.OneOf`2")
            ?? throw new InvalidOperationException("OneOf`2 was not found.");
        var none = compilation.GetTypeByMetadataName("OneOf.Types.None")
            ?? throw new InvalidOperationException("None was not found.");
        return unbound.Construct(compilation.GetSpecialType(SpecialType.System_String), none);
    }

    private static MappaGlobalOptions CreateGlobalOptions(Compilation compilation)
        => new(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees.First());
}