// <copyright file="OneOfWellKnownTypesMapStrategyDetectorTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Algorithm.StrategyDetectors;
using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="OneOfWellKnownTypesMapStrategyDetector"/> edge paths.
/// </summary>
public sealed class OneOfWellKnownTypesMapStrategyDetectorTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Refuses Option unwrap when the nullable target element type does not match T0.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectReturnsFalseForMismatchedNullableOptionUnwrap()
    {
        const string source = """
                              #nullable enable
                              using Mappa.Attributes;
                              using OneOf;
                              using OneOf.Types;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial int? Map(OneOf<string, None> input);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        using (methodContext.MappaUserSettings.Apply(new MappaSettingsAttribute { OneOfWellKnownTypes = BooleanSetting.Enable }))
        {
            var detector = new OneOfWellKnownTypesMapStrategyDetector(methodContext, compilation);

            var detected = detector.TryDetect(out var mapStrategy);

            detected.Should().BeFalse();
            mapStrategy.Should().BeOfType<NoMapStrategy>();
        }
    }

    /// <summary>
    /// Detects Option unwrap to a matching nullable value type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectMapsOptionToMatchingNullableInt()
    {
        const string source = """
                              #nullable enable
                              using Mappa.Attributes;
                              using OneOf;
                              using OneOf.Types;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial int? Map(OneOf<int, None> input);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        using (methodContext.MappaUserSettings.Apply(new MappaSettingsAttribute { OneOfWellKnownTypes = BooleanSetting.Enable }))
        {
            var detector = new OneOfWellKnownTypesMapStrategyDetector(methodContext, compilation);

            var detected = detector.TryDetect(out var mapStrategy);

            detected.Should().BeTrue();
            var optionStrategy = mapStrategy.Should().BeOfType<OneOfOptionToValueMapStrategy>().Subject;
            optionStrategy.AbsentBehavior.Should().Be(OneOfOptionAbsentBehavior.ReturnNull);
        }
    }

    /// <summary>
    /// Detects Option wrap from a matching nullable value type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectMapsNullableIntToOption()
    {
        const string source = """
                              #nullable enable
                              using Mappa.Attributes;
                              using OneOf;
                              using OneOf.Types;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial OneOf<int, None> Map(int? input);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        using (methodContext.MappaUserSettings.Apply(new MappaSettingsAttribute { OneOfWellKnownTypes = BooleanSetting.Enable }))
        {
            var detector = new OneOfWellKnownTypesMapStrategyDetector(methodContext, compilation);

            var detected = detector.TryDetect(out var mapStrategy);

            detected.Should().BeTrue();
            var optionStrategy = mapStrategy.Should().BeOfType<OneOfOptionFromValueMapStrategy>().Subject;
            optionStrategy.WrapBehavior.Should().Be(OneOfOptionWrapBehavior.SomeWhenHasValue);
        }
    }

    /// <summary>
    /// Refuses Option wrap when the source type does not match T0.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectReturnsFalseForMismatchedOptionWrap()
    {
        const string source = """
                              #nullable enable
                              using Mappa.Attributes;
                              using OneOf;
                              using OneOf.Types;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial OneOf<string, None> Map(int input);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        using (methodContext.MappaUserSettings.Apply(new MappaSettingsAttribute { OneOfWellKnownTypes = BooleanSetting.Enable }))
        {
            var detector = new OneOfWellKnownTypesMapStrategyDetector(methodContext, compilation);

            var detected = detector.TryDetect(out var mapStrategy);

            detected.Should().BeFalse();
            mapStrategy.Should().BeOfType<NoMapStrategy>();
        }
    }

    /// <summary>
    /// Refuses Option-shaped <c>OneOfBase</c> wrap when only an implicit conversion exists (no OneOf ctor).
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectReturnsFalseForOneOfBaseOptionWithOnlyImplicitConversion()
    {
        const string source = """
                              #nullable enable
                              using Mappa.Attributes;
                              using OneOf;
                              using OneOf.Types;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class StringOption : OneOfBase<string, None>
                              {
                                  private StringOption(OneOf<string, None> input)
                                      : base(input)
                                  {
                                  }

                                  public static implicit operator StringOption(string value)
                                      => new StringOption(value);
                              }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial StringOption Map(string input);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        using (methodContext.MappaUserSettings.Apply(new MappaSettingsAttribute { OneOfWellKnownTypes = BooleanSetting.Enable }))
        {
            var detector = new OneOfWellKnownTypesMapStrategyDetector(methodContext, compilation);

            var detected = detector.TryDetect(out var mapStrategy);

            detected.Should().BeFalse();
            mapStrategy.Should().BeOfType<NoMapStrategy>();
        }
    }

    /// <summary>
    /// Returns false when OneOf well-known strategies are not enabled.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectReturnsFalseWhenSettingIsUndefined()
    {
        const string source = """
                              #nullable enable
                              using Mappa.Attributes;
                              using OneOf;
                              using OneOf.Types;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial string Map(OneOf<string, None> input);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        var detector = new OneOfWellKnownTypesMapStrategyDetector(methodContext, compilation);

        var detected = detector.TryDetect(out var mapStrategy);

        detected.Should().BeFalse();
        mapStrategy.Should().BeOfType<NoMapStrategy>();
    }

    private static (MappaMethodGeneratorContext MethodContext, Compilation Compilation) CreateMethodContext(
        string source,
        string methodName)
    {
        var compilation = BuildCompilation(source);
        var syntaxTree = compilation.SyntaxTrees.First();
        var classDeclarationSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single(classSyntax => classSyntax.Identifier.Text == "Mapper");
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            syntaxTree);
        var classContext = new MappaClassGeneratorContext(
            globalOptions,
            new MappaDebug(globalOptions, _ => { }),
            compilation,
            classDeclarationSyntax);
        var methodDeclarationSyntax = classDeclarationSyntax.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(methodSyntax => methodSyntax.Identifier.Text == methodName);
        var mapMethod = new MapMethod(
            methodDeclarationSyntax,
            compilation.GetSemanticModel(syntaxTree),
            nullableEnabled: true,
            TestContext.Current.CancellationToken);
        var methodContext = new MappaMethodGeneratorContext(classContext, new MappaUserSettings(globalOptions), mapMethod);
        return (methodContext, compilation);
    }
}