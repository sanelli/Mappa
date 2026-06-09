// <copyright file="MappaInvokeMethodAttributeTests.MappaContext.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="MappaInvokeMethodAttribute"/> with <see cref="MappaContext"/>.
/// </summary>
public sealed partial class MappaInvokeMethodAttributeTests
{
    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting a local method with
    /// three parameters: source type, source property type, and <see cref="MappaContext"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticLocalMethodWithThreeParametersIncludingMappaContext()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source, int propertyA, MappaContext mappaContext)
                                      {
                                          return $"{source.PropertyB}/{propertyA}/{mappaContext.Keys.Count}";
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input"),
                                    secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    thirdParameterAssertions => thirdParameterAssertions.BeIdentifierNameSyntax("context")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting a local method with
    /// source type and <see cref="MappaContext"/> parameters.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticLocalMethodWithSourceAndMappaContext()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source, MappaContext mappaContext)
                                      {
                                          return $"{source.PropertyB}/{mappaContext.Keys.Count}";
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input"),
                                    secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("context")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> targeting a local method with
    /// a single <see cref="MappaContext"/> parameter.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticLocalMethodWithMappaContextOnly()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(MappaContext mappaContext)
                                      {
                                          return mappaContext.Keys.Count.ToString();
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("context")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> still maps using source and source-property
    /// parameters when the map method provides <see cref="MappaContext"/> but the invoked method does not use it.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingNonStaticLocalMethodWithTwoParametersWhenMapMethodProvidesMappaContext()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source, int propertyA)
                                      {
                                          return $"{source.PropertyB}/{propertyA}";
                                      }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input"),
                                    secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_3",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyB"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                "__mappa_tmp_4",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                                        ("PropertyA", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                        ("PropertyB", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement("__mappa_tmp_4");
                        });
                });
    }

    /// <summary>
    /// Test method resolution prefers a three-parameter exact match over a two-parameter exact match
    /// when <see cref="MappaContext"/> is available.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSelectThreeParameterExactOverTwoParameterExactWhenContextAvailable()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source, int propertyA, MappaContext mappaContext) => "three";

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source, int propertyA) => "two";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input"),
                                    secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    thirdParameterAssertions => thirdParameterAssertions.BeIdentifierNameSyntax("context")));
                        });
                });
    }

    /// <summary>
    /// Test method resolution prefers a two-parameter source-and-property match over source-and-context
    /// when <see cref="MappaContext"/> is available.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSelectTwoParameterExactOverSourceWithContextWhenContextAvailable()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source, int propertyA) => "two";

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source, MappaContext mappaContext) => "source-context";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input"),
                                    secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                        });
                });
    }

    /// <summary>
    /// Test method resolution prefers source-and-context over source-only
    /// when <see cref="MappaContext"/> is available.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSelectSourceWithContextOverSourceOnlyWhenContextAvailable()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source, MappaContext mappaContext) => "source-context";

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source) => "source";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input"),
                                    secondParameterAssertions => secondParameterAssertions.BeIdentifierNameSyntax("context")));
                        });
                });
    }

    /// <summary>
    /// Test method resolution prefers source-only over context-only
    /// when <see cref="MappaContext"/> is available.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSelectSourceOnlyOverContextOnlyWhenBothAvailable()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source) => "source";

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(MappaContext mappaContext) => "context";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        });
                });
    }

    /// <summary>
    /// Test method resolution prefers context-only over no-parameter
    /// when <see cref="MappaContext"/> is available.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanSelectContextOnlyOverNoParametersWhenContextAvailable()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input, MappaContext context);

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(MappaContext mappaContext) => "context";

                                      [MappaIgnore]
                                      public string CustomMapPropertyA() => "none";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethodWithContext(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("context")));
                        });
                });
    }

    /// <summary>
    /// Test method resolution falls back to source-only when the map method does not provide
    /// <see cref="MappaContext"/> even if a source-and-context overload exists.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanFallBackToSourceOnlyWhenContextUnavailable()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source, MappaContext mappaContext) => "source-context";

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source) => "source";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "Mappa.Generator.Tests.UnitTests.SourceCode.Target",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.Source",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.PropertyA"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeInvocationExpressionSyntax(
                                    "this.CustomMapPropertyA",
                                    firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("input")));
                        });
                });
    }

    /// <summary>
    /// Test a diagnostic is reported when only context-requiring overloads exist
    /// and the map method does not provide <see cref="MappaContext"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CannotMapWhenOnlyContextRequiringOverloadExistsAndMapMethodHasNoContext()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int PropertyB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; }
                                      public long PropertyB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute(nameof(Target.PropertyA), nameof(CustomMapPropertyA))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      public string CustomMapPropertyA(Source source, MappaContext mappaContext) => "source-context";
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotDetectSuitableMethodToInvokeForParameter, "CustomMapPropertyA", "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper", "PropertyA")
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotMapNonRequiredProperty, "Mappa.Generator.Tests.UnitTests.SourceCode.Target", "PropertyA");
    }
}