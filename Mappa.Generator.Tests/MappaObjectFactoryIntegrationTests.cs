// <copyright file="MappaObjectFactoryIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <c>MappaObjectFactory</c> happy paths.
/// </summary>
public sealed partial class MappaObjectFactoryIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string SourceNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";
    private const string SourceType = $"{SourceNamespace}.Source";
    private const string TargetType = $"{SourceNamespace}.Target";
    private const string NestedSourceType = $"{SourceNamespace}.NestedSource";
    private const string NestedTargetType = $"{SourceNamespace}.NestedTarget";

    /// <summary>
    /// Fully-produced factory <c>(Source)</c> on the mapper is preferred over empty constructor.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingFullyProducedFactoryWithSourceParameter()
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
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public partial Target Map(Source input);

                                      private Target CreateTarget(Source source) => new Target { PropertyA = source.PropertyA.ToString(), PropertyB = source.PropertyB };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_1",
                            init => init.BeInvocationExpressionSyntax(
                                "this.CreateTarget",
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Fully-produced factory <c>(Source, MappaContext)</c> is preferred over empty constructor.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingFullyProducedFactoryWithSourceAndContext()
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
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public partial Target Map(Source input, MappaContext context);

                                      private Target CreateTarget(Source source, MappaContext context) => new Target { PropertyA = source.PropertyA.ToString(), PropertyB = source.PropertyB };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_1",
                            init => init.BeInvocationExpressionSyntax(
                                "this.CreateTarget",
                                arg => arg.BeIdentifierNameSyntax("input"),
                                arg => arg.BeIdentifierNameSyntax("context"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Empty-ctor-like factory <c>()</c> fills properties like the empty-constructor path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingEmptyCtorLikeFactoryWithPropertyAssignments()
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
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public partial Target Map(Source input);

                                      private Target CreateTarget() => new Target();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasSyntaxNodesCount(7)
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
                            init => init.BeInvocationExpressionSyntax("this.CreateTarget")))
                        .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement(
                            left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_4.PropertyA"),
                            right => right.BeIdentifierNameSyntax("__mappa_tmp_2")))
                        .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement(
                            left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_4.PropertyB"),
                            right => right.BeIdentifierNameSyntax("__mappa_tmp_3")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Empty-ctor-like factory <c>(MappaContext)</c> fills properties when the map method has context.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingEmptyCtorLikeFactoryWithContextParameter()
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
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public partial Target Map(Source input, MappaContext context);

                                      private Target CreateTarget(MappaContext context) => new Target();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(7)
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
                            init => init.BeInvocationExpressionSyntax(
                                "this.CreateTarget",
                                arg => arg.BeIdentifierNameSyntax("context"))))
                        .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement(
                            left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_4.PropertyA"),
                            right => right.BeIdentifierNameSyntax("__mappa_tmp_2")))
                        .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement(
                            left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_4.PropertyB"),
                            right => right.BeIdentifierNameSyntax("__mappa_tmp_3")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Parameterized-like factory maps arguments from source properties without leftover assigns.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingParameterizedLikeFactory()
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

                                      public Target(string propertyA, long propertyB)
                                      {
                                          this.PropertyA = propertyA;
                                          this.PropertyB = propertyB;
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public partial Target Map(Source input);

                                      private Target CreateTarget(string propertyA, long propertyB) => new Target(propertyA, propertyB);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                            init => init.BeInvocationExpressionSyntax(
                                "this.CreateTarget",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_3"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Class-level object factory is applied to mapping methods.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingClassLevelFactoryOnly()
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
                                  [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                  public sealed partial class Mapper
                                  {
                                      public partial Target Map(Source input);

                                      private Target CreateTarget(Source source) => new Target { PropertyA = source.PropertyA.ToString() };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_1",
                            init => init.BeInvocationExpressionSyntax(
                                "this.CreateTarget",
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Method-level object factory is applied without a class-level factory.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMethodLevelFactoryOnly()
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
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public partial Target Map(Source input);

                                      private Target CreateTarget(Source source) => new Target { PropertyA = source.PropertyA.ToString() };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_1",
                            init => init.BeInvocationExpressionSyntax(
                                "this.CreateTarget",
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Class and method factories for different target types are both used.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingClassAndMethodFactoriesForDifferentTargetTypes()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class NestedSource
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class NestedTarget
                                  {
                                      public string Value { get; set; }
                                  }

                                  public class Source
                                  {
                                      public NestedSource NestedProperty { get; set; }
                                  }

                                  public class Target
                                  {
                                      public NestedTarget NestedProperty { get; set; }
                                  }

                                  [Mappa]
                                  [MappaObjectFactory(typeof(NestedTarget), nameof(CreateNested))]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public partial Target Map(Source input);

                                      private NestedTarget CreateNested(NestedSource source) => new NestedTarget { Value = source.Value.ToString() };

                                      private Target CreateTarget() => new Target();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                            NestedSourceType,
                            "__mappa_tmp_1",
                            init => init.BeMemberAccessExpressionSyntax("input.NestedProperty")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            NestedTargetType,
                            "__mappa_tmp_2",
                            init => init.BeInvocationExpressionSyntax(
                                "this.CreateNested",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_3",
                            init => init.BeInvocationExpressionSyntax("this.CreateTarget")))
                        .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement(
                            left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_3.NestedProperty"),
                            right => right.BeIdentifierNameSyntax("__mappa_tmp_2")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                });
    }

    /// <summary>
    /// Static type factory is invoked via the fully-qualified type name.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticTypeFactory()
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

                                  public static class FactoryHelpers
                                  {
                                      public static Target Create(Source source) => new Target { PropertyA = source.PropertyA.ToString() };
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), typeof(FactoryHelpers), nameof(FactoryHelpers.Create))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_1",
                            init => init.BeInvocationExpressionSyntax(
                                $"global::{SourceNamespace}.FactoryHelpers.Create",
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Field or property instance factory is invoked via <c>this.field.Method</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingFieldOrPropertyInstanceFactory()
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

                                  public sealed class Dep
                                  {
                                      public Target Create(Source source) => new Target { PropertyA = source.PropertyA.ToString() };
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private readonly Dep dep = new();

                                      [MappaObjectFactory(typeof(Target), nameof(dep), nameof(Dep.Create))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_1",
                            init => init.BeInvocationExpressionSyntax(
                                "this.dep.Create",
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Static factory method reached via field is invoked through the field declared type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingFieldOrPropertyStaticFactoryMethod()
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

                                  public sealed class Dep
                                  {
                                      public static Target Create(Source source) => new Target { PropertyA = source.PropertyA.ToString() };
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private readonly Dep dep = new();

                                      [MappaObjectFactory(typeof(Target), nameof(dep), nameof(Dep.Create))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_1",
                            init => init.BeInvocationExpressionSyntax(
                                $"global::{SourceNamespace}.Dep.Create",
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Static mapper invokes a static factory method without <c>this</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingStaticMapperAndStaticFactoryMethod()
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
                                  public static partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public static partial Target Map(Source input);

                                      private static Target CreateTarget(Source source) => new Target { PropertyA = source.PropertyA.ToString() };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                "Map",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                false,
                TargetType,
                NullableAnnotation.NotAnnotated,
                "input",
                SourceType,
                NullableAnnotation.NotAnnotated,
                1,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_1",
                            init => init.BeInvocationExpressionUsingIdentifierNameSyntax(
                                "CreateTarget",
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Factory may return a type derived from the mapping target type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingFactoryReturningDerivedTargetType()
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

                                  public class DerivedTarget : Target
                                  {
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public partial Target Map(Source input);

                                      private DerivedTarget CreateTarget() => new DerivedTarget();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                            TargetType,
                            "__mappa_tmp_3",
                            init => init.BeInvocationExpressionSyntax("this.CreateTarget")))
                        .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement(
                            left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_3.PropertyA"),
                            right => right.BeIdentifierNameSyntax("__mappa_tmp_2")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                });
    }

    /// <summary>
    /// Object factory is preferred over a mapping constructor <c>Target(Source)</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task FactoryIsPreferredOverMappingConstructor()
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

                                      public Target(Source source)
                                      {
                                          this.PropertyA = source.PropertyA.ToString();
                                      }

                                      public Target()
                                      {
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public partial Target Map(Source input);

                                      private Target CreateTarget(Source source) => new Target { PropertyA = "factory" };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_1",
                            init => init.BeInvocationExpressionSyntax(
                                "this.CreateTarget",
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Nested property construction uses a factory registered for the nested target type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNestedPropertyUsingRegisteredFactory()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class NestedSource
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class NestedTarget
                                  {
                                      public string Value { get; set; }
                                  }

                                  public class Source
                                  {
                                      public NestedSource NestedProperty { get; set; }
                                  }

                                  public class Target
                                  {
                                      public NestedTarget NestedProperty { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(NestedTarget), nameof(CreateNested))]
                                      public partial Target Map(Source input);

                                      private NestedTarget CreateNested(NestedSource source) => new NestedTarget { Value = source.Value.ToString() };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            NestedSourceType,
                            "__mappa_tmp_1",
                            init => init.BeMemberAccessExpressionSyntax("input.NestedProperty")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            NestedTargetType,
                            "__mappa_tmp_2",
                            init => init.BeInvocationExpressionSyntax(
                                "this.CreateNested",
                                arg => arg.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_3",
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                ("NestedProperty", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                });
    }
}