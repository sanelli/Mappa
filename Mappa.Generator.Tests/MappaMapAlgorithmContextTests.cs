// <copyright file="MappaMapAlgorithmContextTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaMapAlgorithmContext"/> defensive and dispose paths.
/// </summary>
public sealed class MappaMapAlgorithmContextTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MappaMapAlgorithmContext.GetMapMethod"/> throws when the map method is not defined.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMapMethodThrowsWhenMapMethodIsNotDefined()
    {
        var (context, _) = CreateRecordingContext();

        var act = () => context.GetMapMethod();

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Map method is not defined.");
    }

    /// <summary>
    /// Test <see cref="MappaMapAlgorithmContext.TryGetClassGeneratorContext"/> returns <c>false</c>
    /// when the root context is not a method generator context.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetClassGeneratorContextReturnsFalseWhenRootIsNotMethodContext()
    {
        var (context, _) = CreateRecordingContext();

        var found = context.TryGetClassGeneratorContext(out var classContext);

        found.Should().BeFalse();
        classContext.Should().BeNull();
    }

    /// <summary>
    /// Test <see cref="MappaMapAlgorithmContext.TryGetClassGeneratorContext"/> returns the class context
    /// when the root is a <see cref="MappaMethodGeneratorContext"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetClassGeneratorContextReturnsTrueForMethodContext()
    {
        var (methodContext, _) = CreateMethodContext();

        var found = methodContext.TryGetClassGeneratorContext(out var classContext);

        found.Should().BeTrue();
        classContext.Should().NotBeNull();
    }

    /// <summary>
    /// Test compile-time depth dispose is idempotent.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IncreaseCompileTimeDepthDisposeIsIdempotent()
    {
        var (methodContext, _) = CreateMethodContext();
        methodContext.CurrentDepth.Should().Be(-1);

        var scope = methodContext.IncreaseCompileTimeDepth();
        methodContext.CurrentDepth.Should().Be(0);
        scope.Dispose();
        methodContext.CurrentDepth.Should().Be(-1);
        scope.Dispose();
        methodContext.CurrentDepth.Should().Be(-1);
    }

    /// <summary>
    /// Test mapping-type-pair dispose is idempotent.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryPushMappingTypePairDisposeIsIdempotent()
    {
        var (methodContext, sourceType) = CreateMethodContext();
        var scope = methodContext.TryPushMappingTypePair(sourceType, sourceType);
        scope.Should().NotBeNull();

        scope.Dispose();
        scope.Dispose();

        var second = methodContext.TryPushMappingTypePair(sourceType, sourceType);
        second.Should().NotBeNull();
        second.Dispose();
    }

    private static (RecordingMapAlgorithmContext Context, ITypeSymbol SourceType) CreateRecordingContext()
    {
        var compilation = BuildCompilation("""
                                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                           public sealed class Source { }
                                           """);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")
            ?? throw new InvalidOperationException("Source type was not found.");
        var context = new RecordingMapAlgorithmContext(sourceType, sourceType, compilation.SyntaxTrees[0]);
        return (context, sourceType);
    }

    private static (MappaMethodGeneratorContext MethodContext, ITypeSymbol SourceType) CreateMethodContext()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Source { }

                              public sealed class Target { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target Map(Source input);
                              }
                              """;

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
            .Single(methodSyntax => methodSyntax.Identifier.Text == "Map");
        var mapMethod = new MapMethod(
            methodDeclarationSyntax,
            compilation.GetSemanticModel(syntaxTree),
            nullableEnabled: true,
            TestContext.Current.CancellationToken);
        var methodContext = new MappaMethodGeneratorContext(classContext, new MappaUserSettings(globalOptions), mapMethod);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source")
            ?? throw new InvalidOperationException("Source type was not found.");
        return (methodContext, sourceType);
    }

    private sealed class RecordingMapAlgorithmContext : MappaMapAlgorithmContext
    {
        public RecordingMapAlgorithmContext(ITypeSymbol sourceType, ITypeSymbol targetType, SyntaxTree syntaxTree)
        {
            this.SourceType = sourceType;
            this.TargetType = targetType;
            this.ParentSymbol = targetType;
            this.AlgorithmSettings = new MappaMapAlgorithmContextSettings();
            var globalOptions = new MappaGlobalOptions(
                TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
                syntaxTree);
            this.MappaUserSettings = new MappaUserSettings(globalOptions);
        }

        internal override ISymbol ParentSymbol { get; }

        internal override ITypeSymbol SourceType { get; }

        internal override ITypeSymbol TargetType { get; }

        internal override MapMethod? MapMethod => null;

        internal override MappaMapAlgorithmContextSettings AlgorithmSettings { get; }

        internal override MappaUserSettings MappaUserSettings { get; }

        internal override bool HasErrorDiagnostics => false;

        internal override bool IsNullableEnabled() => true;

        internal override bool TryGetMethod(ITypeSymbol targetType, ITypeSymbol sourceType, out MapMethod mapMethod)
        {
            mapMethod = null!;
            return false;
        }

        internal override bool TryGetPolymorphicMethod(ITypeSymbol targetType, ITypeSymbol sourceType, IMappaUserSettings mappaUserSettings, out MapMethod mapMethod)
        {
            mapMethod = null!;
            return false;
        }

        internal override bool TryGetCompatibleMethod(ITypeSymbol targetType, ITypeSymbol sourceType, Compilation compilation, out MapMethod? mapMethod)
        {
            mapMethod = null;
            return false;
        }

        internal override void ReportDiagnostic(Diagnostic diagnostic)
        {
        }

        internal override Location? GetLocation() => null;
    }
}