// <copyright file="IdentityMapStrategyDetectorTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa;
using Mappa.Generator.Algorithm.StrategyDetectors;
using Mappa.Generator.Diagnostics.Debug;
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
/// Unit tests for <see cref="IdentityMapStrategyDetector"/>.
/// </summary>
public sealed class IdentityMapStrategyDetectorTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test nullable value types ignore deep-copy settings and still use identity mapping.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectMapsNullableIntToNullableIntWithDeepCopySetting()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial int? Map(int? input);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        using (methodContext.MappaUserSettings.Apply(new MappaSettingsAttribute { IdentityMapDeepCopy = IdentityMapDeepCopySetting.DeepCopy }))
        {
            var detector = new IdentityMapStrategyDetector(methodContext, compilation, TestContext.Current.CancellationToken);

            var detected = detector.TryDetect(out var mapStrategy);

            detected.Should().BeTrue();
            mapStrategy.Should().BeOfType<IdentityMapStrategy>();
        }
    }

    /// <summary>
    /// Test nested deep copy succeeds for a type with no accessible instance fields.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectNestedDeepCopySucceedsForTypeWithoutInstanceFields()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Empty
                              {
                              }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Empty Map(Empty input);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        using (methodContext.MappaUserSettings.Apply(new MappaSettingsAttribute { IdentityMapDeepCopy = IdentityMapDeepCopySetting.NestedDeepCopy }))
        {
            var detector = new IdentityMapStrategyDetector(methodContext, compilation, TestContext.Current.CancellationToken);

            var detected = detector.TryDetect(out var mapStrategy);

            detected.Should().BeTrue();
            var identity = mapStrategy.Should().BeOfType<IdentityMapStrategy>().Subject;
            identity.NestedFieldStrategies.Should().BeEmpty();
        }
    }

    /// <summary>
    /// Test nested deep copy fails when a nested field type cannot be mapped without identity.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectNestedDeepCopyFailsWhenNestedFieldCannotBeMapped()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Nested
                              {
                                  public Nested Child;
                              }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Nested Map(Nested input);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        using (methodContext.MappaUserSettings.Apply(new MappaSettingsAttribute { IdentityMapDeepCopy = IdentityMapDeepCopySetting.NestedDeepCopy }))
        {
            var detector = new IdentityMapStrategyDetector(methodContext, compilation, TestContext.Current.CancellationToken);

            var detected = detector.TryDetect(out var mapStrategy);

            detected.Should().BeFalse();
            mapStrategy.Should().BeOfType<NoMapStrategy>();
        }
    }

    /// <summary>
    /// Test nested deep copy fails for structs when a nested field type cannot be mapped.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectNestedDeepCopyFailsForStructWhenNestedFieldCannotBeMapped()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public struct NestedStruct
                              {
                                  public NestedStruct Child;
                              }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial NestedStruct Map(NestedStruct input);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        using (methodContext.MappaUserSettings.Apply(new MappaSettingsAttribute { IdentityMapDeepCopy = IdentityMapDeepCopySetting.NestedDeepCopy }))
        {
            var detector = new IdentityMapStrategyDetector(methodContext, compilation, TestContext.Current.CancellationToken);

            var detected = detector.TryDetect(out var mapStrategy);

            detected.Should().BeFalse();
            mapStrategy.Should().BeOfType<NoMapStrategy>();
        }
    }

    /// <summary>
    /// Test an unexpected identity deep-copy setting falls back to a plain identity strategy.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectFallsBackToIdentityForUnexpectedDeepCopySetting()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Sample
                              {
                              }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Sample Map(Sample input);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        using (methodContext.MappaUserSettings.Apply(new MappaSettingsAttribute { IdentityMapDeepCopy = (IdentityMapDeepCopySetting)999 }))
        {
            var detector = new IdentityMapStrategyDetector(methodContext, compilation, TestContext.Current.CancellationToken);

            var detected = detector.TryDetect(out var mapStrategy);

            detected.Should().BeTrue();
            mapStrategy.Should().BeOfType<IdentityMapStrategy>();
        }
    }

    private static (MappaMethodGeneratorContext MethodContext, Compilation Compilation) CreateMethodContext(
        string source,
        string methodName)
    {
        var compilation = BuildCompilation(source);
        var syntaxTree = compilation.SyntaxTrees[0];
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