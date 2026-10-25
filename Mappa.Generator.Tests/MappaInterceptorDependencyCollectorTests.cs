// <copyright file="MappaInterceptorDependencyCollectorTests.cs" company="Stefano Anelli">
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
/// Unit tests for <see cref="MappaInterceptorDependencyCollector"/>.
/// </summary>
public sealed class MappaInterceptorDependencyCollectorTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Interceptor-only classes pick up methods marked with
    /// <c>[MappaInterceptorMethodDependency]</c> as mapping dependencies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesInterceptorMethodDependencies()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  [MappaInterceptorMethodDependency]
                                  public long Helper(int value) => value;

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod =>
            mapMethod.MethodName == "Helper"
            && mapMethod.SourceType.SpecialType == SpecialType.System_Int32
            && mapMethod.TargetType.SpecialType == SpecialType.System_Int64);
    }

    /// <summary>
    /// Without <c>[Mappa]</c>, unmarked implemented methods are not interceptor dependencies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIgnoresUnmarkedMethodsWithoutMappaAttribute()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public long Helper(int value) => value;

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().NotContain(mapMethod => mapMethod.MethodName == "Helper");
    }

    /// <summary>
    /// With <c>[Mappa]</c>, implemented mapper methods become available as interceptor dependencies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesImplementedMethodsWhenMappaAttributePresent()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public long Helper(int value) => value;

                                  public partial string Map(int value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: true);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod => mapMethod.MethodName == "Helper");
    }

    /// <summary>
    /// <c>[MappaDependency]</c> properties contribute their map methods.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesMappaDependencyPropertyMethods()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Dependency
                              {
                                  public long Map(int value) => value;
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  [MappaDependency]
                                  private Dependency Helpers { get; } = new Dependency();

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod =>
            mapMethod.MethodName == "Map"
            && mapMethod.SourceType.SpecialType == SpecialType.System_Int32
            && mapMethod.TargetType.SpecialType == SpecialType.System_Int64);
    }

    /// <summary>
    /// <c>[MappaDependency]</c> fields contribute their map methods.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesMappaDependencyFieldMethods()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Dependency
                              {
                                  public long Map(int value) => value;
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  [MappaDependency]
                                  private readonly Dependency helpers = new Dependency();

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod =>
            mapMethod.MethodName == "Map"
            && mapMethod.SourceType.SpecialType == SpecialType.System_Int32
            && mapMethod.TargetType.SpecialType == SpecialType.System_Int64);
    }

    /// <summary>
    /// Static <c>[MappaDependency]</c> properties use a type-qualified accessor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesStaticMappaDependencyPropertyMethods()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Dependency
                              {
                                  public static long Map(int value) => value;
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  [MappaDependency]
                                  private static Dependency Helpers { get; } = new Dependency();

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod =>
            mapMethod.MethodName == "Map"
            && mapMethod.CanBeUsedByStaticMethod);
    }

    /// <summary>
    /// Static <c>[MappaDependency]</c> fields use a type-qualified accessor.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesStaticMappaDependencyFieldMethods()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Dependency
                              {
                                  public static long Map(int value) => value;
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  [MappaDependency]
                                  private static readonly Dependency helpers = new Dependency();

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod =>
            mapMethod.MethodName == "Map"
            && mapMethod.CanBeUsedByStaticMethod);
    }

    /// <summary>
    /// Inherited static <c>[MappaDependency]</c> properties from a base mapper are collected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesBaseTypeStaticMappaDependencyProperties()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Dependency
                              {
                                  public static long Map(int value) => value;
                              }

                              public class BaseMapper
                              {
                                  [MappaDependency]
                                  protected static Dependency BaseHelpers { get; } = new Dependency();
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper : BaseMapper
                              {
                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod =>
            mapMethod.MethodName == "Map"
            && mapMethod.CanBeUsedByStaticMethod);
    }

    /// <summary>
    /// Inherited static <c>[MappaDependency]</c> fields from a base mapper are collected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesBaseTypeStaticMappaDependencyFields()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Dependency
                              {
                                  public static long Map(int value) => value;
                              }

                              public class BaseMapper
                              {
                                  [MappaDependency]
                                  protected static readonly Dependency baseHelpers = new Dependency();
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper : BaseMapper
                              {
                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod =>
            mapMethod.MethodName == "Map"
            && mapMethod.CanBeUsedByStaticMethod);
    }

    /// <summary>
    /// Static dependency types without viable map methods report a diagnostic.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectReportsWhenStaticDependencyHasNoViableMethods()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public static class EmptyHelper
                              {
                                  public static void NotAMap() { }
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(
            source,
            hasMappaAttribute: false,
            staticDependencyMetadataNames: ["Mappa.Generator.Tests.UnitTests.SourceCode.EmptyHelper"]);

        classContext.MapMethods.Should().BeEmpty();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == Mappa.Generator.Diagnostics.MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod.Id);
    }

    /// <summary>
    /// Inherited <c>[MappaDependency]</c> members from a base mapper are collected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesBaseTypeMappaDependencyMembers()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Dependency
                              {
                                  public long Map(int value) => value;
                              }

                              public class BaseMapper
                              {
                                  [MappaDependency]
                                  protected Dependency BaseHelpers { get; } = new Dependency();
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper : BaseMapper
                              {
                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod =>
            mapMethod.MethodName == "Map"
            && mapMethod.SourceType.SpecialType == SpecialType.System_Int32);
    }

    /// <summary>
    /// Non-static methods on static dependency types are ignored.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIgnoresInstanceMethodsOnStaticDependencyTypes()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class HelperMapper
                              {
                                  public long Map(int value) => value;
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (_, diagnostics) = Collect(
            source,
            hasMappaAttribute: false,
            staticDependencyMetadataNames: ["Mappa.Generator.Tests.UnitTests.SourceCode.HelperMapper"]);

        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == Mappa.Generator.Diagnostics.MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod.Id);
    }

    /// <summary>
    /// Inherited <c>[MappaDependency]</c> fields from a base mapper are collected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesBaseTypeMappaDependencyFields()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Dependency
                              {
                                  public long Map(int value) => value;
                              }

                              public class BaseMapper
                              {
                                  [MappaDependency]
                                  protected readonly Dependency baseHelpers = new Dependency();
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper : BaseMapper
                              {
                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod =>
            mapMethod.MethodName == "Map"
            && mapMethod.SourceType.SpecialType == SpecialType.System_Int32);
    }

    /// <summary>
    /// Methods marked with <c>[MappaIgnore]</c> on a dependency type are skipped.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIgnoresMappaIgnoredDependencyMethods()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Dependency
                              {
                                  [MappaIgnore]
                                  public long Ignored(int value) => value;

                                  public string Keep(int value) => value.ToString();
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  [MappaDependency]
                                  private Dependency Helpers { get; } = new Dependency();

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().NotContain(mapMethod => mapMethod.MethodName == "Ignored");
        classContext.MapMethods.Should().Contain(mapMethod => mapMethod.MethodName == "Keep");
    }

    /// <summary>
    /// Static dependency methods that accept a <c>MappaContext</c> parameter are collected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesStaticDependencyMethodsWithContext()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public static class HelperMapper
                              {
                                  public static long Map(int value, MappaContext context) => value;
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(
            source,
            hasMappaAttribute: false,
            staticDependencyMetadataNames: ["Mappa.Generator.Tests.UnitTests.SourceCode.HelperMapper"]);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod =>
            mapMethod.MethodName == "Map"
            && mapMethod.CanBeUsedByStaticMethod);
    }

    /// <summary>
    /// <c>[MappaDependency]</c> members whose type exposes no viable map methods report a diagnostic.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectReportsWhenMappaDependencyHasNoViableMethods()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public interface IEmptyDependency
                              {
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  [MappaDependency]
                                  private IEmptyDependency Helpers { get; } = null!;

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        classContext.MapMethods.Should().BeEmpty();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == Mappa.Generator.Diagnostics.MappaDiagnosticDescriptors.DependencyDoesNotProvideAnyViableMethod.Id);
    }

    /// <summary>
    /// Dependency methods with invalid <c>ref</c> parameter modifiers are not collected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIgnoresDependencyMethodsWithRefParameters()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Dependency
                              {
                                  public long Map(ref int value) => value;

                                  public string Keep(int value) => value.ToString();
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  [MappaDependency]
                                  private Dependency Helpers { get; } = new Dependency();

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().NotContain(mapMethod => mapMethod.MethodName == "Map"
            && mapMethod.SourceType.SpecialType == SpecialType.System_Int32
            && mapMethod.TargetType.SpecialType == SpecialType.System_Int64);
        classContext.MapMethods.Should().Contain(mapMethod => mapMethod.MethodName == "Keep");
    }

    /// <summary>
    /// Void, <c>Task</c>, and inaccessible dependency methods are not collected.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIgnoresVoidTaskAndInaccessibleDependencyMethods()
    {
        const string source = """
                              using System.Threading.Tasks;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Dependency
                              {
                                  public void VoidMap(int value) { }

                                  public Task<long> TaskMap(int value) => Task.FromResult((long)value);

                                  private long PrivateMap(int value) => value;

                                  public string Keep(int value) => value.ToString();
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  [MappaDependency]
                                  private Dependency Helpers { get; } = new Dependency();

                                  public long Map(int value) => this.MappaMap<long>(value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: false);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().NotContain(mapMethod =>
            mapMethod.MethodName == "VoidMap"
            || mapMethod.MethodName == "TaskMap"
            || mapMethod.MethodName == "PrivateMap");
        classContext.MapMethods.Should().Contain(mapMethod => mapMethod.MethodName == "Keep");
    }

    /// <summary>
    /// With <c>[Mappa]</c>, base-type hierarchy methods become available as interceptor dependencies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void CollectIncludesBaseTypeHierarchyMethodsWhenMappaAttributePresent()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class BaseMapper
                              {
                                  public long Helper(int value) => value;
                              }

                              [Mappa]
                              [MappaInterceptor]
                              public partial class InlineMapper : BaseMapper
                              {
                                  public partial string Map(int value);
                              }
                              """;

        var (classContext, diagnostics) = Collect(source, hasMappaAttribute: true);

        diagnostics.Should().BeEmpty();
        classContext.MapMethods.Should().Contain(mapMethod => mapMethod.MethodName == "Helper");
    }

    private static (MappaClassGeneratorContext ClassContext, List<Diagnostic> Diagnostics) Collect(
        string source,
        bool hasMappaAttribute,
        string[]? staticDependencyMetadataNames = null)
    {
        var compilation = BuildCompilation(source);
        var classDeclaration = compilation.SyntaxTrees
            .SelectMany(syntaxTree => syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>())
            .Single(declaration => declaration.Identifier.Text == "InlineMapper");

        var options = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            classDeclaration.SyntaxTree);
        var diagnostics = new List<Diagnostic>();
        var mappaDebug = new MappaDebug(options, diagnostics.Add);
        var classContext = new MappaClassGeneratorContext(options, mappaDebug, compilation, classDeclaration);

        var staticDependencyTypes = new List<INamedTypeSymbol>();
        if (staticDependencyMetadataNames is not null)
        {
            foreach (var metadataName in staticDependencyMetadataNames)
            {
                var dependencyType = compilation.GetTypeByMetadataName(metadataName);
                dependencyType.Should().NotBeNull($"expected type '{metadataName}' in compilation");
                staticDependencyTypes.Add(dependencyType);
            }
        }

        MappaInterceptorDependencyCollector.Collect(
            classContext,
            compilation,
            hasMappaAttribute,
            staticDependencyTypes,
            diagnostics.Add,
            TestContext.Current.CancellationToken);

        return (classContext, diagnostics);
    }
}