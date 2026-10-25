// <copyright file="MappaInterceptorDiagnosticsIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for negative diagnostics on the <c>[MappaInterceptor]</c> route.
/// </summary>
public sealed class MappaInterceptorDiagnosticsIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string InterceptorsNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";

    /// <summary>
    /// Non-partial interceptor classes report a diagnostic and emit no sources.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NonPartialInterceptorClassReportsDiagnostic()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public class InlineMapper
                                  {
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappaInterceptorClassIsNotPartial, "InlineMapper")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Non-<c>new</c> <c>With</c> arguments report a dedicated diagnostic while stubs still emit.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NonObjectCreationWithArgumentReportsDiagnostic()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public int Map(int value)
                                      {
                                          var settings = new MappaSettingsAttribute();
                                          return this.MappaMap<int>(value).With(settings);
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappaInterceptorWithArgumentMustBeObjectCreation, "settings")
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubMethodsOnly("InlineMapper");
    }

    /// <summary>
    /// Duplicate non-<c>AllowMultiple</c> attributes across <c>With</c> report errors.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DuplicateNonMultipleAttributesReportDiagnostic()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public int Map(int value)
                                      {
                                          return this.MappaMap<int>(value)
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new MappaMustMapTargetPropertyAttribute("A"),
                                                      new MappaMustMapTargetPropertyAttribute("B"),
                                                  });
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaInterceptorDuplicateNonMultipleAttribute,
                "Mappa.Attributes.MappaMustMapTargetPropertyAttribute")
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubMethodsOnly("InlineMapper");
    }

    /// <summary>
    /// Multiple settings across <c>With</c> report a warning and still generate when mapping is valid.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MultipleSettingsReportWarningAndStillGenerate()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public int Map(int value)
                                      {
                                          return this.MappaMap<int>(value)
                                              .With(new MappaSettingsAttribute { DateTimeFormat = "first" })
                                              .With(new MappaSettingsAttribute { DateTimeFormat = "second" });
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappaInterceptorMultipleSettings)
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
                "__MappaIntercept_0",
                "int",
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    "int",
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappaTypedSource"));
                });
    }

    /// <summary>
    /// Duplicate static dependency types report a warning.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DuplicateStaticDependencyTypesReportWarning()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public static class HelperMapper
                                  {
                                      public static long Map(int value) => value;
                                  }

                                  [MappaInterceptor]
                                  [MappaStaticDependency(typeof(HelperMapper))]
                                  public partial class InlineMapper
                                  {
                                      public int Map(int value)
                                      {
                                          return this.MappaMap<int>(value)
                                              .With(
                                                  mappaStaticDependencies: new MappaStaticDependencyAttribute[]
                                                  {
                                                      new MappaStaticDependencyAttribute(typeof(HelperMapper)),
                                                  });
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaInterceptorDuplicateStaticDependencyType,
                "Mappa.Generator.Tests.UnitTests.SourceCode.HelperMapper")
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubsAndMethod(
                "InlineMapper",
                "__MappaIntercept_0",
                "int",
                hasContext: false,
                expectedMethodCount: 3,
                blockAssertions =>
                {
                    blockAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    "int",
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappaTypedSource"));
                });
    }

    /// <summary>
    /// <c>IQueryable</c> call sites are rejected on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task QueryableCallSiteReportsDiagnostic()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public IQueryable<int> Map(IQueryable<int> value)
                                      {
                                          return this.MappaMap<IQueryable<int>>(value);
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MappaInterceptorQueryableNotSupported)
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubMethodsOnly("InlineMapper");
    }

    /// <summary>
    /// Task target types report the existing task-return diagnostic.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TaskTargetReportsDiagnostic()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Threading.Tasks;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Task Map(int value)
                                      {
                                          return this.MappaMap<Task>(value);
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.MethodReturnsTaskType, "MappaMap")
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubMethodsOnly("InlineMapper");
    }

    /// <summary>
    /// Projection attributes via <c>With</c> are unsupported on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task UnsupportedProjectionAttributeReportsDiagnostic()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public int Map(int value)
                                      {
                                          return this.MappaMap<int>(value)
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new MappaProjectionAttribute { Enabled = BooleanSetting.Enable },
                                                  });
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaInterceptorUnsupportedFeature,
                "MappaProjectionAttribute")
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubMethodsOnly("InlineMapper");
    }

    /// <summary>
    /// Non-marker attribute types in the method-attributes array report a diagnostic.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task InvalidMethodAttributeTypeReportsDiagnostic()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public int Map(int value)
                                      {
                                          return this.MappaMap<int>(value)
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new ObsoleteAttribute(),
                                                  });
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaInterceptorInvalidMethodAttributeType,
                "System.ObsoleteAttribute")
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveInterceptorStubMethodsOnly("InlineMapper");
    }
}