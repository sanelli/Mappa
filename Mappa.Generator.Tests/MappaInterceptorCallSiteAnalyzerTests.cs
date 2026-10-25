// <copyright file="MappaInterceptorCallSiteAnalyzerTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MappaInterceptorCallSiteAnalyzer"/>.
/// </summary>
public sealed class MappaInterceptorCallSiteAnalyzerTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Discovers a bare <c>MappaMap</c> call site with source and target types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeDiscoversBareMapCallSite()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value);
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        var callSite = callSites[0];
        callSite.HasErrors.Should().BeFalse();
        callSite.HasContext.Should().BeFalse();
        callSite.TargetType.ToDisplayString().Should().Be("string");
        callSite.SourceType.ToDisplayString().Should().Be("int");
        callSite.Settings.Should().BeEmpty();
        callSite.MethodAttributes.Should().BeEmpty();
    }

    /// <summary>
    /// Discovers a context-bearing call and materializes settings / method attributes from <c>With</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeMaterializesWithSettingsAndMethodAttributes()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value, MappaContext context)
                                  {
                                      return this.MappaMap<string>(value, context)
                                          .With(
                                              new MappaSettingsAttribute { MapSourceFields = BooleanSetting.Enable },
                                              null,
                                              new IMappaMethodAttribute[]
                                              {
                                                  new MappaIgnoreTargetPropertyAttribute("Ignored"),
                                                  new MappaUsePropertyAttribute("Target", "Source"),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        var callSite = callSites[0];
        callSite.HasContext.Should().BeTrue();
        callSite.HasErrors.Should().BeFalse();
        callSite.Settings.Should().ContainSingle()
            .Which.MapSourceFields.Should().Be(BooleanSetting.Enable);
        callSite.MethodAttributes.Should().HaveCount(2);
        callSite.MethodAttributes[0].Should().BeOfType<MappaIgnoreTargetPropertyAttribute>()
            .Which.TargetPropertyName.Should().Be("Ignored");
        callSite.MethodAttributes[1].Should().BeOfType<MappaUsePropertyAttribute>()
            .Which.TargetPropertyName.Should().Be("Target");
    }

    /// <summary>
    /// Walks multiple contiguous <c>With</c> calls and keeps settings in source order.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeCollatesMultipleWithCallsInSourceOrder()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(new MappaSettingsAttribute { DateTimeFormat = "first" })
                                          .With(new MappaSettingsAttribute { DateTimeFormat = "second" });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle();
        callSites[0].Settings.Should().HaveCount(2);
        callSites[0].Settings[0].DateTimeFormat.Should().Be("first");
        callSites[0].Settings[1].DateTimeFormat.Should().Be("second");
        diagnostics.Should().ContainSingle(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorMultipleSettings.Id);
        callSites[0].HasErrors.Should().BeFalse();
    }

    /// <summary>
    /// Rejects non-<c>new</c> <c>With</c> arguments with a dedicated diagnostic.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeReportsNonObjectCreationWithArgument()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      var settings = new MappaSettingsAttribute();
                                      return this.MappaMap<string>(value).With(settings);
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle();
        callSites[0].HasErrors.Should().BeTrue();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorWithArgumentMustBeObjectCreation.Id);
    }

    /// <summary>
    /// Unions class and <c>With</c> static dependencies and warns on duplicate types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeDedupesStaticDependenciesAndWarnsOnDuplicates()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public static class HelperMapper
                              {
                              }

                              [MappaInterceptor]
                              [MappaStaticDependency(typeof(HelperMapper))]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaStaticDependencies: new MappaStaticDependencyAttribute[]
                                              {
                                                  new MappaStaticDependencyAttribute(typeof(HelperMapper)),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle();
        callSites[0].StaticDependencies.Should().ContainSingle()
            .Which.Name.Should().Be("HelperMapper");
        callSites[0].HasErrors.Should().BeFalse();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorDuplicateStaticDependencyType.Id);
    }

    /// <summary>
    /// Errors when a non-<c>AllowMultiple</c> attribute appears more than once.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeErrorsOnDuplicateNonMultipleMethodAttributes()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaMustMapTargetPropertyAttribute("A"),
                                                  new MappaMustMapTargetPropertyAttribute("B"),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle();
        callSites[0].HasErrors.Should().BeTrue();
        diagnostics.Should().HaveCount(2);
        diagnostics.Should().OnlyContain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorDuplicateNonMultipleAttribute.Id);
    }

    /// <summary>
    /// Errors when a non-marker type is supplied in the method-attributes array.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeErrorsOnInvalidMethodAttributeType()
    {
        const string source = """
                              using System;
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new ObsoleteAttribute(),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle();
        callSites[0].HasErrors.Should().BeTrue();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorInvalidMethodAttributeType.Id);
    }

    /// <summary>
    /// Errors when projection attributes are used on the interceptor route.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeErrorsOnUnsupportedProjectionAttribute()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaProjectionAttribute { Enabled = BooleanSetting.Enable },
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle();
        callSites[0].HasErrors.Should().BeTrue();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorUnsupportedFeature.Id);
    }

    /// <summary>
    /// Errors when the mapping involves <c>IQueryable</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeErrorsOnQueryableCallSite()
    {
        const string source = """
                              using System.Linq;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public IQueryable<string> Map(IQueryable<int> value)
                                  {
                                      return this.MappaMap<IQueryable<string>>(value);
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle();
        callSites[0].HasErrors.Should().BeTrue();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorQueryableNotSupported.Id);
    }

    /// <summary>
    /// Honors a renamed interceptable method from <c>[MappaInterceptor]</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeDiscoversRenamedMapMethod()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor("MapInline")]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MapInline<string>(value);
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source, "MapInline");

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].TargetType.ToDisplayString().Should().Be("string");
    }

    /// <summary>
    /// Accepts explicit array creation for method attributes.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeAcceptsExplicitArrayCreationForMethodAttributes()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaMethodAttributes: new IMappaMethodAttribute[]
                                              {
                                                  new MappaIgnoreTargetPropertyAttribute("Ignored"),
                                              });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].MethodAttributes.Should().ContainSingle()
            .Which.Should().BeOfType<MappaIgnoreTargetPropertyAttribute>();
    }

    /// <summary>
    /// Accepts collection expressions for static dependencies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeAcceptsCollectionExpressionForStaticDependencies()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public static class HelperMapper
                              {
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              mappaStaticDependencies:
                                              [
                                                  new MappaStaticDependencyAttribute(typeof(HelperMapper)),
                                              ]);
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].StaticDependencies.Should().ContainSingle()
            .Which.Name.Should().Be("HelperMapper");
    }

    /// <summary>
    /// Reports when an array initializer contains a non-creation expression.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeReportsNonCreationInsideArrayInitializer()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      var ignore = new MappaIgnoreTargetPropertyAttribute("Ignored");
                                      return this.MappaMap<string>(value)
                                          .With(mappaMethodAttributes: new IMappaMethodAttribute[] { ignore });
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle().Which.HasErrors.Should().BeTrue();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorWithArgumentMustBeObjectCreation.Id);
    }

    /// <summary>
    /// Reports when method-attribute arguments are not inline creations or arrays of creations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeReportsMethodAttributesVariableArgument()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      IMappaMethodAttribute[] attrs =
                                      {
                                          new MappaIgnoreTargetPropertyAttribute("Ignored"),
                                      };
                                      return this.MappaMap<string>(value).With(mappaMethodAttributes: attrs);
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle().Which.HasErrors.Should().BeTrue();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorWithArgumentMustBeObjectCreation.Id);
    }

    /// <summary>
    /// Warns when the same static dependency type is declared twice at class level.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeWarnsOnDuplicateClassLevelStaticDependencies()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public static class HelperMapper
                              {
                              }

                              [MappaInterceptor]
                              [MappaStaticDependency(typeof(HelperMapper))]
                              [MappaStaticDependency(typeof(HelperMapper))]
                              public partial class InlineMapper
                              {
                                  public string Map(int value) => this.MappaMap<string>(value);
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle();
        callSites[0].StaticDependencies.Should().ContainSingle();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorDuplicateStaticDependencyType.Id);
    }

    /// <summary>
    /// Errors when the interceptor target type is a <c>Task</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeErrorsOnTaskTargetType()
    {
        const string source = """
                              using System.Threading.Tasks;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public Task<string> Map(int value)
                                  {
                                      return this.MappaMap<Task<string>>(value);
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle().Which.HasErrors.Should().BeTrue();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MethodReturnsTaskType.Id);
    }

    /// <summary>
    /// Non-matching invocations still exercise map-call matching rejection branches.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeIgnoresNonMatchingMapInvocations()
    {
        const string source = """
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      _ = this.MappaMap(value);
                                      _ = this.MappaMap<string, int>(value);
                                      _ = this.MappaMap<string>();
                                      _ = this.MappaMap<string>(value, value, value);
                                      _ = this.NotMappaMap<string>(value);
                                      return this.MappaMap<string>(value);
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].TargetType.ToDisplayString().Should().Be("string");
    }

    /// <summary>
    /// Extra positional <c>With</c> arguments are ignored during collation.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeIgnoresExtraPositionalWithArguments()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      return this.MappaMap<string>(value)
                                          .With(
                                              new MappaSettingsAttribute { DateTimeFormat = "O" },
                                              null,
                                              null,
                                              "ignored");
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        diagnostics.Should().BeEmpty();
        callSites.Should().ContainSingle();
        callSites[0].Settings.Should().ContainSingle()
            .Which.DateTimeFormat.Should().Be("O");
    }

    /// <summary>
    /// Reports when static-dependency arguments are not inline creations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AnalyzeReportsStaticDependenciesVariableArgument()
    {
        const string source = """
                              using Mappa;
                              using Mappa.Attributes;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public static class HelperMapper
                              {
                              }

                              [MappaInterceptor]
                              public partial class InlineMapper
                              {
                                  public string Map(int value)
                                  {
                                      var deps = new MappaStaticDependencyAttribute[]
                                      {
                                          new MappaStaticDependencyAttribute(typeof(HelperMapper)),
                                      };
                                      return this.MappaMap<string>(value).With(mappaStaticDependencies: deps);
                                  }
                              }
                              """;

        var (callSites, diagnostics) = Analyze(source);

        callSites.Should().ContainSingle().Which.HasErrors.Should().BeTrue();
        diagnostics.Should().Contain(diagnostic =>
            diagnostic.Id == MappaDiagnosticDescriptors.MappaInterceptorWithArgumentMustBeObjectCreation.Id);
    }

    private static (System.Collections.Immutable.ImmutableArray<MappaInterceptorCallSite> CallSites, List<Diagnostic> Diagnostics)
        Analyze(string source, string methodName = "MappaMap")
    {
        var compilation = BuildCompilation(source);
        var classDeclaration = compilation.SyntaxTrees
            .SelectMany(syntaxTree => syntaxTree.GetRoot(TestContext.Current.CancellationToken)
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>())
            .Single(declaration => declaration.Identifier.Text == "InlineMapper");
        var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
        var classSymbol = semanticModel.GetDeclaredSymbol(
            classDeclaration,
            TestContext.Current.CancellationToken);
        classSymbol.Should().NotBeNull();

        var diagnostics = new List<Diagnostic>();
        var callSites = MappaInterceptorCallSiteAnalyzer.Analyze(
            classDeclaration,
            classSymbol,
            methodName,
            compilation,
            diagnostics.Add,
            TestContext.Current.CancellationToken);
        return (callSites, diagnostics);
    }
}