// <copyright file="StringToGuidMapStrategyBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa;
using Mappa.Generator.Builders.Strategies;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="StringToGuidMapStrategyBuilder"/>.
/// </summary>
public sealed class StringToGuidMapStrategyBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test building with a user-defined culture and culture name emits <c>GetCultureInfo</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceEmitsGetCultureInfoForUserDefinedCulture()
    {
        var (builder, builderContext, globalOptions) = CreateBuilder(
            CultureInfoSetting.UserDefined,
            cultureName: "it-IT",
            format: null);

        var (_, code) = builder.BuildSource("input", builderContext, globalOptions);

        code.Should().Contain("Guid.Parse(input, System.Globalization.CultureInfo.GetCultureInfo(\"it-IT\"))");
    }

    /// <summary>
    /// Test building with a user-defined culture but missing culture name throws.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsWhenUserDefinedCultureIsMissingCultureName()
    {
        var (builder, builderContext, globalOptions) = CreateBuilder(
            CultureInfoSetting.UserDefined,
            cultureName: null,
            format: null);

        var act = () => builder.BuildSource("input", builderContext, globalOptions);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Reached the scenario where we are trying to build using user defined custom culture without culture name.");
    }

    /// <summary>
    /// Test building with an unexpected culture setting throws.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceThrowsForUnexpectedCultureSetting()
    {
        var (builder, builderContext, globalOptions) = CreateBuilder(
            (CultureInfoSetting)999,
            cultureName: null,
            format: null);

        var act = () => builder.BuildSource("input", builderContext, globalOptions);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Unexpected culture info setting '999'.");
    }

    private static (StringToGuidMapStrategyBuilder Builder, MappaBuilderContext BuilderContext, MappaGlobalOptions GlobalOptions) CreateBuilder(
        CultureInfoSetting cultureInfoSetting,
        string? cultureName,
        string? format)
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var guidType = compilation.GetTypeByMetadataName("System.Guid")
            ?? throw new InvalidOperationException("Guid type was not found.");
        var strategy = new StringToGuidMapStrategy(guidType, stringType, format, cultureInfoSetting, cultureName);
        var builder = new StringToGuidMapStrategyBuilder(strategy);
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);
        return (builder, builderContext, globalOptions);
    }
}