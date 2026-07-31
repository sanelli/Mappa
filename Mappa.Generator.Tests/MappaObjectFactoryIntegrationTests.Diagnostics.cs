// <copyright file="MappaObjectFactoryIntegrationTests.Diagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Diagnostic integration tests for <c>MappaObjectFactory</c>.
/// </summary>
public sealed partial class MappaObjectFactoryIntegrationTests
{
    /// <summary>
    /// Duplicate method-level factories for the same target type report MP00062.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DuplicateMethodLevelFactoriesReportMp00062()
    {
        const string sourceCode = """
                                  #nullable enable
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
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTargetA))]
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTargetB))]
                                      public partial Target Map(Source input);

                                      private Target CreateTargetA(Source source) => new Target();
                                      private Target CreateTargetB(Source source) => new Target();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostic(MappaDiagnosticDescriptors.DuplicateObjectFactoryForTargetType, "Map", TargetType)
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Duplicate class-level factories for the same target type report MP00062.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DuplicateClassLevelFactoriesReportMp00062()
    {
        const string sourceCode = """
                                  #nullable enable
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
                                  [MappaObjectFactory(typeof(Target), nameof(CreateTargetA))]
                                  [MappaObjectFactory(typeof(Target), nameof(CreateTargetB))]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);

                                      private Target CreateTargetA(Source source) => new Target();
                                      private Target CreateTargetB(Source source) => new Target();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostic(MappaDiagnosticDescriptors.DuplicateObjectFactoryForTargetType, "Map", TargetType)
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Class and method factories for the same target type report MP00062.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ClassAndMethodFactoriesForSameTypeReportMp00062()
    {
        const string sourceCode = """
                                  #nullable enable
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
                                  [MappaObjectFactory(typeof(Target), nameof(CreateTargetClass))]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTargetMethod))]
                                      public partial Target Map(Source input);

                                      private Target CreateTargetClass(Source source) => new Target();
                                      private Target CreateTargetMethod(Source source) => new Target();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostic(MappaDiagnosticDescriptors.DuplicateObjectFactoryForTargetType, "Map", TargetType)
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Unresolved factory method name reports MP00063 and falls back to constructor mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task UnresolvedFactoryMethodFallsBackToConstructorWithMp00063()
    {
        const string sourceCode = """
                                  #nullable enable
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
                                      [MappaObjectFactory(typeof(Target), "MissingFactory")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ObjectFactoryMethodNotFound, "Map", TargetType, "MissingFactory")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_1",
                            init => init.BeMemberAccessExpressionSyntax("input.PropertyA")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_2",
                            init => init.BeInvocationExpressionSyntax($"__mappa_tmp_1.{nameof(this.ToString)}")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_3",
                            init => init.BeMemberAccessExpressionSyntax("input.PropertyB")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_4",
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                ("PropertyA", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                ("PropertyB", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Missing field or property for factory location reports MP00063 and falls back to constructor.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MissingFieldOrPropertyForFactoryFallsBackWithMp00063()
    {
        const string sourceCode = """
                                  #nullable enable
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

                                  public sealed class Dep
                                  {
                                      public Target Create(Source source) => new Target();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), "missingDep", nameof(Dep.Create))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ObjectFactoryMethodNotFound, "Map", TargetType, "Create")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_1",
                            init => init.BeMemberAccessExpressionSyntax("input.PropertyA")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_2",
                            init => init.BeInvocationExpressionSyntax($"__mappa_tmp_1.{nameof(this.ToString)}")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_3",
                            init => init.BeMemberAccessExpressionSyntax("input.PropertyB")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_4",
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                ("PropertyA", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                ("PropertyB", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Static map with non-static field for an instance factory reports MP00063 and falls back.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task StaticMapWithNonStaticFieldInstanceFactoryReportsMp00063()
    {
        const string sourceCode = """
                                  #nullable enable
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

                                  public sealed class Dep
                                  {
                                      public Target Create(Source source) => new Target();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private readonly Dep dep = new();

                                      [MappaObjectFactory(typeof(Target), nameof(dep), nameof(Dep.Create))]
                                      public static partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ObjectFactoryMethodNotFound, "Map", TargetType, "Create")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultStaticMapMethod(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                1,
                false,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_1",
                            init => init.BeMemberAccessExpressionSyntax("input.PropertyA")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_2",
                            init => init.BeInvocationExpressionSyntax($"__mappa_tmp_1.{nameof(this.ToString)}")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_3",
                            init => init.BeMemberAccessExpressionSyntax("input.PropertyB")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_4",
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                ("PropertyA", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                ("PropertyB", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Projection method with a method-level object factory reports MP00064.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ProjectionMethodWithMethodLevelObjectFactoryReportsMp00064()
    {
        const string sourceCode = """
                                  #nullable enable
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
                                      [MappaObjectFactory(typeof(OrderDto), nameof(CreateDto))]
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);

                                      private static OrderDto CreateDto() => new OrderDto();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodHasObjectFactory, "ProjectToDto")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Projection method with a class-level object factory reports MP00064.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ProjectionMethodWithClassLevelObjectFactoryReportsMp00064()
    {
        const string sourceCode = """
                                  #nullable enable
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
                                  [MappaObjectFactory(typeof(OrderDto), nameof(CreateDto))]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);

                                      private static OrderDto CreateDto() => new OrderDto();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodHasObjectFactory, "ProjectToDto")
            .NotHaveGeneratedAnySourceCode();
    }
}