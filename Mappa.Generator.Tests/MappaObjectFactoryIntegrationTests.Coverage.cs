// <copyright file="MappaObjectFactoryIntegrationTests.Coverage.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Coverage-oriented integration tests for <c>MappaObjectFactory</c> edge paths.
/// </summary>
public sealed partial class MappaObjectFactoryIntegrationTests
{
    /// <summary>
    /// Empty-ctor-like factory with <c>MappaAssignToContext</c> writes the target member into context.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanAssignMappedPropertyToContextUsingObjectFactory()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
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
                                      [MappaAssignToContext("outKey", nameof(Target.PropertyA))]
                                      public partial Target Map(Source input, MappaContext context);

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
            .HaveDefaultMapMethodWithContext(
                TargetType,
                NullableAnnotation.NotAnnotated,
                SourceType,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(6)
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
                        .HasNextSyntaxNode(node => node.BeAssignToContextStatement("context", "outKey", "__mappa_tmp_3", "PropertyA"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                });
    }

    /// <summary>
    /// Empty-ctor-like factory fills a get-only collection property after the factory call.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapReadonlyCollectionPropertyUsingEmptyCtorLikeFactory()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int[] PropertyA { get; set; }
                                  }

                                  public class Target
                                  {
                                      public Target()
                                      {
                                          this.PropertyA = new List<string>();
                                      }

                                      public ICollection<string> PropertyA { get; }
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
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_1",
                            init => init.BeInvocationExpressionSyntax("this.CreateTarget")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int[]).ToString(),
                            "__mappa_tmp_2",
                            init => init.BeMemberAccessExpressionSyntax("input.PropertyA")))
                        .HasNextSyntaxNode(node => node.BeForStatementSyntax(
                            declarationAssertion => declarationAssertion.BeAssignmentFromConstant("int", "__mappa_tmp_3", 0),
                            conditionAssertion => conditionAssertion.BeBinaryExpressionSyntax(
                                leftExpressionAssertions => leftExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                SyntaxKind.LessThanToken,
                                rightExpressionAssertions => rightExpressionAssertions.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Length")),
                            incrementorAssertion => incrementorAssertion.BePrefixUnaryExpressionSyntax(SyntaxKind.PlusPlusToken, expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3")),
                            statementSyntaxBaseAssertions => statementSyntaxBaseAssertions
                                .BeBlockStatement()
                                .AsBlock()
                                .HasSyntaxNodesCount(3)
                                .HasNextSyntaxNode(forStatementAssertion =>
                                {
                                    forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                        typeof(int).ToString(),
                                        "__mappa_tmp_4",
                                        initializerAssertions => initializerAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_2", "__mappa_tmp_3"));
                                })
                                .HasNextSyntaxNode(forStatementAssertion =>
                                {
                                    forStatementAssertion.BeLocalDeclarationStatementSyntax(
                                        typeof(string).ToString(),
                                        "__mappa_tmp_5",
                                        initializerAssertions => initializerAssertions.BeInvocationExpressionSyntax("__mappa_tmp_4.ToString"));
                                })
                                .HasNextSyntaxNode(forStatementAssertion =>
                                {
                                    forStatementAssertion.BeInvocationExpressionSyntaxStatement(
                                        "__mappa_tmp_1.PropertyA.Add",
                                        firstParameter => firstParameter.BeIdentifierNameSyntax("__mappa_tmp_5"));
                                })))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Instance factory reached via a property is invoked through <c>this.Property.Method</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingPropertyInstanceFactory()
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
                                      private Dep Dependency { get; } = new();

                                      [MappaObjectFactory(typeof(Target), nameof(Dependency), nameof(Dep.Create))]
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
                                "this.Dependency.Create",
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Static factory reached via a property is invoked through the property declared type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingPropertyStaticFactoryMethod()
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
                                      private Dep Dependency { get; } = new();

                                      [MappaObjectFactory(typeof(Target), nameof(Dependency), nameof(Dep.Create))]
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
    /// When the same factory name exists on a derived type and its base, the derived method is preferred.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PrefersMostDerivedFactoryMethodInTypeHierarchy()
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

                                  public class BaseHelpers
                                  {
                                      public static Target Create(Source source) => new Target { PropertyA = "base" };
                                  }

                                  public sealed class DerivedHelpers : BaseHelpers
                                  {
                                      public static new Target Create(Source source) => new Target { PropertyA = source.PropertyA.ToString() };
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), typeof(DerivedHelpers), nameof(DerivedHelpers.Create))]
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
                                $"global::{SourceNamespace}.DerivedHelpers.Create",
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Ambiguous parameterized factory overloads report MP00063 and fall back to constructor mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AmbiguousParameterizedFactoriesFallBackWithMp00063()
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

                                      private Target CreateTarget(string propertyA) => new Target { PropertyA = propertyA };
                                      private Target CreateTarget(long propertyB) => new Target { PropertyB = propertyB };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ObjectFactoryMethodNotFound, "Map", TargetType, "CreateTarget")
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
    /// A context-only factory on a map method without <c>MappaContext</c> reports MP00063 and falls back.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ContextOnlyFactoryWithoutContextParameterFallsBackWithMp00063()
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
                                      public partial Target Map(Source input);

                                      private Target CreateTarget(MappaContext context) => new Target();
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ObjectFactoryMethodNotFound, "Map", TargetType, "CreateTarget")
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
    /// Parameterized factory whose parameters cannot be mapped falls back to empty-constructor mapping.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task UnmappableParameterizedFactoryFallsBackToEmptyConstructor()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System;
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

                                      private Target CreateTarget(Guid id) => new Target { PropertyA = id.ToString() };
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
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                ("PropertyA", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_2")),
                                ("PropertyB", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Assign-to-context without a context parameter on a factory map reports MP00036.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AssignToContextWithoutContextParameterOnFactoryReportsMp00036()
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
                                      [MappaAssignToContext("outKey", nameof(Target.PropertyA))]
                                      public partial Target Map(Source input);

                                      private Target CreateTarget(Source source) => new Target { PropertyA = source.PropertyA.ToString() };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotUseMappaAssignToContextAttributeWithoutContextParameter,
                "Map",
                "outKey")
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
    /// Duplicate assign-to-context keys on a factory map report MP00037.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DuplicateAssignToContextKeysOnFactoryReportMp00037()
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
                                      [MappaAssignToContext("dup", nameof(Target.PropertyA))]
                                      [MappaAssignToContext("dup", nameof(Target.PropertyB))]
                                      public partial Target Map(Source input, MappaContext context);

                                      private Target CreateTarget(Source source) => new Target
                                      {
                                          PropertyA = source.PropertyA.ToString(),
                                          PropertyB = source.PropertyB,
                                      };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MultipleMappaAssignToContextAttributesUseTheSameContextKey,
                "Map",
                "dup")
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
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Missing assign-to-context target member on a factory map reports MP00035.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MissingAssignToContextTargetMemberOnFactoryReportsMp00035()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
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
                                      [MappaAssignToContext("outKey", "Missing")]
                                      public partial Target Map(Source input, MappaContext context);

                                      private Target CreateTarget(Source source) => new Target { PropertyA = source.PropertyA.ToString() };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.MappaAssignToContextTargetMemberDoesNotExistOrIsNotAccessible,
                "Map",
                "outKey",
                "Missing",
                TargetType)
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
                                arg => arg.BeIdentifierNameSyntax("input"))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Nested factory type metadata cannot be resolved via <c>typeof</c> display name (MP00063 fallback).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NestedFactoryTypeCannotBeResolvedFallsBackWithMp00063()
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

                                  public sealed class FactoryContainer
                                  {
                                      public static class NestedFactories
                                      {
                                          public static Target Create(Source source) => new Target { PropertyA = source.PropertyA.ToString(), PropertyB = source.PropertyB };
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), typeof(FactoryContainer.NestedFactories), nameof(FactoryContainer.NestedFactories.Create))]
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
    /// Empty-ctor-like factory that cannot map a required property falls back; empty constructor also fails.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task EmptyCtorLikeFactoryWithUnmappedRequiredPropertyDoesNotGenerateMap()
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
                                      public required string Missing { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaObjectFactory(typeof(Target), nameof(CreateTarget))]
                                      public partial Target Map(Source input);

                                      private Target CreateTarget() => new Target { Missing = "x" };
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotIdentifyStrategy, SourceType, TargetType);
    }

    /// <summary>
    /// Nested empty-ctor-like factory (not reusable as a map method) fills nested properties after the call.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapNestedPropertyUsingEmptyCtorLikeFactory()
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

                                      private NestedTarget CreateNested() => new NestedTarget();
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
                            NestedSourceType,
                            "__mappa_tmp_1",
                            init => init.BeMemberAccessExpressionSyntax("input.NestedProperty")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_2",
                            init => init.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Value")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_3",
                            init => init.BeInvocationExpressionSyntax($"__mappa_tmp_2.{nameof(this.ToString)}")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            NestedTargetType,
                            "__mappa_tmp_4",
                            init => init.BeInvocationExpressionSyntax("this.CreateNested")))
                        .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement(
                            left => left.BeMemberAccessExpressionSyntax("__mappa_tmp_4.Value"),
                            right => right.BeIdentifierNameSyntax("__mappa_tmp_3")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetType,
                            "__mappa_tmp_5",
                            init => init.BeObjectCreationExpressionSyntax(
                                TargetType,
                                ("NestedProperty", prop => prop.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_5"));
                });
    }
}