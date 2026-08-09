// <copyright file="QueryableProjectionMapStrategyDetectorTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Algorithm.StrategyDetectors;
using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="QueryableProjectionMapStrategyDetector"/>.
/// </summary>
public sealed class QueryableProjectionMapStrategyDetectorTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test the detector rejects queryable pairs with the same element type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectReturnsFalseWhenQueryableElementTypesAreEqual()
    {
        const string source = """
                              using System.Linq;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Order
                              {
                                  public int Id { get; set; }
                              }

                              [Mappa]
                              public static partial class Mapper
                              {
                                  public static partial IQueryable<Order> Project(this IQueryable<Order> query);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Project");
        var detector = new QueryableProjectionMapStrategyDetector(
            methodContext,
            compilation,
            TestContext.Current.CancellationToken);

        var detected = detector.TryDetect(out var mapStrategy);

        detected.Should().BeFalse();
        mapStrategy.Should().BeOfType<NoMapStrategy>();
    }

    /// <summary>
    /// Test the detector rejects element mappings that are not projection-capable.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectReturnsFalseWhenElementProjectionIsNotSupported()
    {
        const string source = """
                              using System.Collections.Generic;
                              using System.Linq;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Line
                              {
                                  public int Id { get; set; }
                              }

                              public class LineDto
                              {
                                  public int Id { get; set; }
                              }

                              public class Order
                              {
                                  public int Id { get; set; }
                                  public List<Line> Lines { get; set; } = null!;
                              }

                              public class OrderDto
                              {
                                  public int Id { get; set; }
                                  public List<LineDto> Lines { get; set; } = null!;
                              }

                              [Mappa]
                              public static partial class Mapper
                              {
                                  public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "ProjectToDto");
        var detector = new QueryableProjectionMapStrategyDetector(
            methodContext,
            compilation,
            TestContext.Current.CancellationToken);

        var detected = detector.TryDetect(out var mapStrategy);

        detected.Should().BeFalse();
        mapStrategy.Should().BeOfType<NoMapStrategy>();
    }

    /// <summary>
    /// Test the detector returns false when the algorithm context has no map method.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectReturnsFalseWhenMapMethodIsNull()
    {
        const string source = """
                              using System.Linq;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Order
                              {
                                  public int Id { get; set; }
                              }

                              public class OrderDto
                              {
                                  public int Id { get; set; }
                              }

                              [Mappa]
                              public static partial class Mapper
                              {
                                  public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "ProjectToDto");
        var derivedContext = new DerivedMappaMapAlgorithmContext(
            methodContext,
            methodContext.TargetType,
            methodContext.SourceType);
        var detector = new QueryableProjectionMapStrategyDetector(
            derivedContext,
            compilation,
            TestContext.Current.CancellationToken);

        var detected = detector.TryDetect(out var mapStrategy);

        detected.Should().BeFalse();
        mapStrategy.Should().BeOfType<NoMapStrategy>();
    }

    /// <summary>
    /// Test the detector returns false when source or target is not queryable.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectReturnsFalseWhenTypesAreNotQueryable()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Order
                              {
                                  public int Id { get; set; }
                              }

                              public class OrderDto
                              {
                                  public int Id { get; set; }
                              }

                              [Mappa]
                              public static partial class Mapper
                              {
                                  public static partial OrderDto Map(Order order);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "Map");
        var detector = new QueryableProjectionMapStrategyDetector(
            methodContext,
            compilation,
            TestContext.Current.CancellationToken);

        var detected = detector.TryDetect(out var mapStrategy);

        detected.Should().BeFalse();
        mapStrategy.Should().BeOfType<NoMapStrategy>();
    }

    /// <summary>
    /// Test the detector accepts a projection-compatible queryable pair.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectReturnsTrueForCompatibleQueryableProjection()
    {
        const string source = """
                              using System.Linq;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Order
                              {
                                  public int Id { get; set; }
                                  public string Name { get; set; } = null!;
                              }

                              public class OrderDto
                              {
                                  public int Id { get; set; }
                                  public string Name { get; set; } = null!;
                              }

                              [Mappa]
                              public static partial class Mapper
                              {
                                  public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "ProjectToDto");
        var detector = new QueryableProjectionMapStrategyDetector(
            methodContext,
            compilation,
            TestContext.Current.CancellationToken);

        var detected = detector.TryDetect(out var mapStrategy);

        detected.Should().BeTrue();
        mapStrategy.Should().BeOfType<QueryableProjectionMapStrategy>();
    }

    /// <summary>
    /// Test the detector rejects queryable projections when reference reusing is enabled.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryDetectReturnsFalseWhenReferenceReusingIsEnabled()
    {
        const string source = """
                              using System.Linq;
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Order
                              {
                                  public int Id { get; set; }
                                  public string Name { get; set; } = null!;
                              }

                              public class OrderDto
                              {
                                  public int Id { get; set; }
                                  public string Name { get; set; } = null!;
                              }

                              [Mappa]
                              public static partial class Mapper
                              {
                                  [MappaSettings(ReferenceReusing = BooleanSetting.Enable)]
                                  public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                              }
                              """;

        var (methodContext, compilation) = CreateMethodContext(source, "ProjectToDto");
        var settingsAttribute = methodContext.MapMethod?.GetAttribute<MappaSettingsAttribute>();
        using (methodContext.MappaUserSettings.Apply(settingsAttribute))
        {
            var detector = new QueryableProjectionMapStrategyDetector(
                methodContext,
                compilation,
                TestContext.Current.CancellationToken);

            var detected = detector.TryDetect(out var mapStrategy);

            detected.Should().BeFalse();
            mapStrategy.Should().BeOfType<NoMapStrategy>();
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