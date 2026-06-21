// <copyright file="MappaBuilderContextTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaBuilderContext"/>.
/// </summary>
public sealed class MappaBuilderContextTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MappaBuilderContext.GetMapMethod"/> throws when no map method has been pushed.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetMapMethodThrowsWhenNoMethodHasBeenPushed()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var context = new MappaBuilderContext(compilation);

        var act = () => context.GetMapMethod();

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Cannot obtain the map method.");
    }

    /// <summary>
    /// Test <see cref="MappaBuilderContext.PushMapMethod"/> scopes the current map method until disposed.
    /// </summary>
    [Fact]
    [UnitTest]
    public void PushMapMethodScopesCurrentMethodUntilDisposed()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source { }

                              public class Target { }

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial Target MapOne(Source input);

                                  public partial Target MapTwo(Source input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var mapMethodOne = CreateMapMethod(compilation, "MapOne");
        var mapMethodTwo = CreateMapMethod(compilation, "MapTwo");
        var context = new MappaBuilderContext(compilation);

        using (context.PushMapMethod(mapMethodOne))
        {
            context.GetMapMethod().Should().BeSameAs(mapMethodOne);

            using (context.PushMapMethod(mapMethodTwo))
            {
                context.GetMapMethod().Should().BeSameAs(mapMethodTwo);
            }

            context.GetMapMethod().Should().BeSameAs(mapMethodOne);
        }

        var act = () => context.GetMapMethod();

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Cannot obtain the map method.");
    }

    /// <summary>
    /// Test composite type source and target names are scoped by their push operations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void PushCompositeTypeNamesAreScopedUntilDisposed()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var context = new MappaBuilderContext(compilation);

        context.GetCompositeTypeSourceName().Should().BeEmpty();
        context.GetCompositeTypeTargetName().Should().BeEmpty();

        using (context.PushCurrentCompositeTypeSourceName("sourceName"))
        {
            using (context.PushCurrentCompositeTypeTargetName("targetName"))
            {
                context.GetCompositeTypeSourceName().Should().Be("sourceName");
                context.GetCompositeTypeTargetName().Should().Be("targetName");
            }

            context.GetCompositeTypeTargetName().Should().BeEmpty();
        }

        context.GetCompositeTypeSourceName().Should().BeEmpty();
    }

    /// <summary>
    /// Test <see cref="MappaBuilderContext.ReportDiagnostic"/> adds diagnostics to the context.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ReportDiagnosticAddsDiagnosticToContext()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var context = new MappaBuilderContext(compilation);
        var diagnostic = Diagnostic.Create(
            MappaDiagnosticDescriptors.CannotIdentifyStrategy,
            Location.None,
            "System.String",
            "System.Int32");

        context.ReportDiagnostic(diagnostic);

        context.Diagnostics.Should().ContainSingle().Which.Should().Be(diagnostic);
    }

    /// <summary>
    /// Test <see cref="MappaBuilderContext.NextTemporary"/> returns unique temporary names.
    /// </summary>
    [Fact]
    [UnitTest]
    public void NextTemporaryReturnsUniqueNames()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var context = new MappaBuilderContext(compilation);

        context.NextTemporary().Should().Be("__mappa_tmp_1");
        context.NextTemporary().Should().Be("__mappa_tmp_2");
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
            nullableEnabled: false,
            CancellationToken.None);
    }
}