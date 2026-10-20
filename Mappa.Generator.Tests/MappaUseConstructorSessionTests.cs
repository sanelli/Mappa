// <copyright file="MappaUseConstructorSessionTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaUseConstructorSession"/>.
/// </summary>
public sealed class MappaUseConstructorSessionTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// ValidateAttributes is idempotent when called more than once.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ValidateAttributesIsIdempotent()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int PropertyA { get; set; }
                              }

                              public class Target
                              {
                                  public string PropertyA { get; set; }
                              }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaUseConstructor(ConstructorType.NoParameters)]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var (session, methodContext, mapMethod) = CreateSession(compilation);

        session.ValidateAttributes(methodContext, mapMethod.MethodName);
        var diagnosticsAfterFirst = methodContext.ClassContext.Diagnostics.Count;
        session.ValidateAttributes(methodContext, mapMethod.MethodName);

        methodContext.ClassContext.Diagnostics.Count.Should().Be(diagnosticsAfterFirst);
    }

    /// <summary>
    /// Attributes whose effective target type is null are excluded from validation and unused warnings.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NullEffectiveTargetTypeIsExcludedFromValidationAndUnusedWarnings()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { public int PropertyA { get; set; } }
                              public class Target { public string PropertyA { get; set; } }

                              [Mappa]
                              public sealed partial class TestMapper
                              {
                                  [MappaUseConstructor(ConstructorType.NoParameters)]
                                  public partial Target Map(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var (session, methodContext, mapMethod) = CreateSession(compilation);

        var attributesField = typeof(MappaUseConstructorSession)
            .GetField("attributes", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var attributes = (MappaUseConstructorAttributeData[])attributesField.GetValue(session)!;
        attributes[0].EffectiveTargetType = null;

        session.ValidateAttributes(methodContext, mapMethod.MethodName);
        session.TryGetForTargetType(mapMethod.TargetType, out _).Should().BeFalse();
        session.ReportUnusedWarnings(methodContext, mapMethod.MethodName);

        methodContext.ClassContext.Diagnostics.Should().BeEmpty();
    }

    private static (MappaUseConstructorSession Session, MappaMethodGeneratorContext MethodContext, MapMethod MapMethod) CreateSession(
        CSharpCompilation compilation)
    {
        var syntaxTree = compilation.SyntaxTrees[0];
        var classDeclarationSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single(classSyntax => classSyntax.Identifier.Text == "TestMapper");
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
            .Single(methodSyntax => methodSyntax.Identifier.Text == "Map");
        var mapMethod = new MapMethod(
            methodDeclarationSyntax,
            compilation.GetSemanticModel(syntaxTree),
            nullableEnabled: true,
            TestContext.Current.CancellationToken);
        var methodContext = new MappaMethodGeneratorContext(classContext, new MappaUserSettings(globalOptions), mapMethod);

        var session = new MappaUseConstructorSession();
        session.Initialize(
            mapMethod.MethodSymbol!.GetAttributes(),
            compilation,
            mapMethod.TargetType);

        return (session, methodContext, mapMethod);
    }
}