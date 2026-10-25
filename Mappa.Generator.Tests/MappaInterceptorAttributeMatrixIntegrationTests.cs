// <copyright file="MappaInterceptorAttributeMatrixIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Interceptor-route coverage for supported method attributes supplied via <c>With</c>.
/// </summary>
public sealed class MappaInterceptorAttributeMatrixIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string InterceptorsNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";
    private const string SourceType = "Mappa.Generator.Tests.UnitTests.SourceCode.Source";
    private const string TargetType = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";

    /// <summary>
    /// <c>MappaUseProperty</c> remaps a differently named source property on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanUsePropertyViaWith()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Foo { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Target Map(Source value)
                                      {
                                          return this.MappaMap<Target>(value)
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new MappaUsePropertyAttribute("Value", "Foo"),
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
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethod(
                        TargetType,
                        NullableAnnotation.NotAnnotated,
                        "__MappaIntercept_0",
                        false,
                        [("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false)],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveInterceptsLocationAttribute()
                                .HaveBody(blockAssertions =>
                                {
                                    blockAssertions
                                        .HasSyntaxNodesCount(4)
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                SourceType + "?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    SourceType,
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "int",
                                                "__mappa_tmp_1",
                                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Foo"));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                TargetType,
                                                "__mappa_tmp_2",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    TargetType,
                                                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                                });
                        });
            });
    }

    /// <summary>
    /// <c>MappaAssignFromConstant</c> assigns a constant target member on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanAssignFromConstantViaWith()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Target Map(Source value)
                                      {
                                          return this.MappaMap<Target>(value)
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new MappaAssignFromConstantAttribute("Value", 42),
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
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethod(
                        TargetType,
                        NullableAnnotation.NotAnnotated,
                        "__MappaIntercept_0",
                        false,
                        [("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false)],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveInterceptsLocationAttribute()
                                .HaveBody(blockAssertions =>
                                {
                                    blockAssertions
                                        .HasSyntaxNodesCount(4)
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                SourceType + "?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    SourceType,
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "int",
                                                "__mappa_tmp_1",
                                                init => init.BeLiteralExpressionSyntax(42));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                TargetType,
                                                "__mappa_tmp_2",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    TargetType,
                                                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                                });
                        });
            });
    }

    /// <summary>
    /// <c>MappaAssignFromContext</c> reads a context item for a target member on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanAssignFromContextViaWith()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                  }

                                  public class Target
                                  {
                                      public int Value { get; set; }
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Target Map(Source value, MappaContext context)
                                      {
                                          return this.MappaMap<Target>(value, context)
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new MappaAssignFromContextAttribute("Value", "key"),
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
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethod(
                        TargetType,
                        NullableAnnotation.NotAnnotated,
                        "__MappaIntercept_0",
                        false,
                        [
                            ("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false),
                            (typeof(MappaContext).FullName ?? "Mappa.MappaContext", NullableAnnotation.NotAnnotated, "context", RefKind.None, false),
                        ],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveInterceptsLocationAttribute()
                                .HaveBody(blockAssertions =>
                                {
                                    blockAssertions
                                        .HasSyntaxNodesCount(4)
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                SourceType + "?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    SourceType,
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "int",
                                                "__mappa_tmp_1",
                                                init => init.BeCastExpressionSyntax(
                                                    "int",
                                                    casted => casted.BeElementAccessExpressionSyntaxWithLiteralSyntax("context", "key")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                TargetType,
                                                "__mappa_tmp_2",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    TargetType,
                                                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                                });
                        });
            });
    }

    /// <summary>
    /// <c>MappaNullSubstitute</c> with <c>AssignValue</c> substitutes null source members on the interceptor route.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanNullSubstituteViaWith()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string? Name { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Name { get; set; }
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Target Map(Source value)
                                      {
                                          return this.MappaMap<Target>(value)
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new MappaNullSubstituteAttribute("Name") { AssignValue = "n/a" },
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
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethod(
                        TargetType,
                        NullableAnnotation.NotAnnotated,
                        "__MappaIntercept_0",
                        false,
                        [("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false)],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveInterceptsLocationAttribute()
                                .HaveBody(blockAssertions =>
                                {
                                    blockAssertions
                                        .HasSyntaxNodesCount(5)
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                SourceType + "?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    SourceType,
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "string?",
                                                "__mappa_tmp_1",
                                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Name"));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "string",
                                                "__mappa_tmp_2",
                                                init => init.BeBinaryExpressionSyntax(
                                                    left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                                    SyntaxKind.QuestionQuestionToken,
                                                    right => right.BeLiteralExpressionSyntax("n/a")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                TargetType,
                                                "__mappa_tmp_3",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    TargetType,
                                                    ("Name", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_2"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                                });
                        });
            });
    }

    /// <summary>
    /// <c>MappaBeforeMap</c> invokes a hook before the interceptor mapping body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanBeforeMapViaWith()
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
                                                      new MappaBeforeMapAttribute(nameof(Before)),
                                                  });
                                      }

                                      private static void Before(ref int input) { }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaInterceptorGeneratorAsync(
            sourceCode,
            InterceptorsNamespace,
            TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeLocalDeclarationStatementSyntax(
                                "int",
                                "__mappaTypedSource",
                                init => init.BeCastExpressionSyntax(
                                    "int",
                                    casted => casted.BeIdentifierNameSyntax("source")));
                        })
                        .HasNextSyntaxNode(node =>
                        {
                            node.BeInvocationExpressionUsingIdentifierNameSyntaxStatement(
                                "Before",
                                (SyntaxKind.RefKeyword, arg => arg.BeIdentifierNameSyntax("__mappaTypedSource")));
                        })
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappaTypedSource"));
                });
    }

    /// <summary>
    /// Field parity: <c>MappaUseProperty</c> resolves a source field when <c>MapSourceFields</c> is enabled via <c>With</c>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanUsePropertyAgainstSourceFieldViaWith()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int SourceValue;
                                  }

                                  public class Target
                                  {
                                      public int TargetValue { get; set; }
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Target Map(Source value)
                                      {
                                          return this.MappaMap<Target>(value)
                                              .With(new MappaSettingsAttribute { MapSourceFields = BooleanSetting.Enable })
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new MappaUsePropertyAttribute("TargetValue", "SourceValue"),
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
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethod(
                        TargetType,
                        NullableAnnotation.NotAnnotated,
                        "__MappaIntercept_0",
                        false,
                        [("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false)],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveInterceptsLocationAttribute()
                                .HaveBody(blockAssertions =>
                                {
                                    blockAssertions
                                        .HasSyntaxNodesCount(4)
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                SourceType + "?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    SourceType,
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "int",
                                                "__mappa_tmp_1",
                                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.SourceValue"));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                TargetType,
                                                "__mappa_tmp_2",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    TargetType,
                                                    ("TargetValue", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                                });
                        });
            });
    }

    /// <summary>
    /// Field parity: <c>MappaIgnoreTargetProperty</c> ignores a target field when field mapping is enabled via <c>With</c>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanIgnoreTargetFieldViaWith()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int Keep;
                                      public int Drop;
                                  }

                                  public class Target
                                  {
                                      public int Keep;
                                      public int Drop;
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Target Map(Source value)
                                      {
                                          return this.MappaMap<Target>(value)
                                              .With(new MappaSettingsAttribute
                                              {
                                                  MapSourceFields = BooleanSetting.Enable,
                                                  MapTargetFields = BooleanSetting.Enable,
                                              })
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new MappaIgnoreTargetPropertyAttribute("Drop"),
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
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethod(
                        TargetType,
                        NullableAnnotation.NotAnnotated,
                        "__MappaIntercept_0",
                        false,
                        [("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false)],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveInterceptsLocationAttribute()
                                .HaveBody(blockAssertions =>
                                {
                                    blockAssertions
                                        .HasSyntaxNodesCount(4)
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                SourceType + "?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    SourceType,
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "int",
                                                "__mappa_tmp_1",
                                                init => init.BeMemberAccessExpressionSyntax("__mappaTypedSource.Keep"));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                TargetType,
                                                "__mappa_tmp_2",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    TargetType,
                                                    ("Keep", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                                });
                        });
            });
    }

    /// <summary>
    /// Field parity: <c>MappaAssignFromConstant</c> writes a target field when <c>MapTargetFields</c> is enabled via <c>With</c>.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanAssignFromConstantToTargetFieldViaWith()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                  }

                                  public class Target
                                  {
                                      public int Value;
                                  }

                                  [MappaInterceptor]
                                  public partial class InlineMapper
                                  {
                                      public Target Map(Source value)
                                      {
                                          return this.MappaMap<Target>(value)
                                              .With(new MappaSettingsAttribute { MapTargetFields = BooleanSetting.Enable })
                                              .With(
                                                  mappaMethodAttributes: new IMappaMethodAttribute[]
                                                  {
                                                      new MappaAssignFromConstantAttribute("Value", 7),
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
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithGeneratedCompilationUnit()
            .HaveCommentHeader()
            .HaveClass("InlineMapper", classAssertions =>
            {
                classAssertions
                    .HaveMethod(
                        TargetType,
                        NullableAnnotation.NotAnnotated,
                        "__MappaIntercept_0",
                        false,
                        [("object", NullableAnnotation.NotAnnotated, "source", RefKind.None, false)],
                        methodAssertions =>
                        {
                            methodAssertions
                                .HaveInterceptsLocationAttribute()
                                .HaveBody(blockAssertions =>
                                {
                                    blockAssertions
                                        .HasSyntaxNodesCount(4)
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                SourceType + "?",
                                                "__mappaTypedSource",
                                                init => init.BeCastExpressionSyntax(
                                                    SourceType,
                                                    casted => casted.BeIdentifierNameSyntax("source")));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                "int",
                                                "__mappa_tmp_1",
                                                init => init.BeLiteralExpressionSyntax(7));
                                        })
                                        .HasNextSyntaxNode(node =>
                                        {
                                            node.BeLocalDeclarationStatementSyntax(
                                                TargetType,
                                                "__mappa_tmp_2",
                                                init => init.BeObjectCreationExpressionSyntax(
                                                    TargetType,
                                                    ("Value", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"))));
                                        })
                                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                                });
                        });
            });
    }
}