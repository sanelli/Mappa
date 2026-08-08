// <copyright file="IQueryableProjectionMapStrategyIntegrationTests.Diagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Diagnostic integration tests for <see cref="Mappa.Generator.Models.Strategies.QueryableProjectionMapStrategy"/>.
/// </summary>
public sealed partial class IQueryableProjectionMapStrategyIntegrationTests
{
    /// <summary>
    /// Test before-map hooks are rejected on projection methods.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task BeforeMapHookIsRejectedOnProjectionMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa;
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
                                      [MappaBeforeMap(nameof(Before))]
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);

                                      private static void Before(ref IQueryable<Order> query) { }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodHasBeforeOrAfterMapHooks, "ProjectToDto")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Test after-map hooks are rejected on projection methods.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AfterMapHookIsRejectedOnProjectionMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa;
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
                                      [MappaAfterMap(nameof(After))]
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);

                                      private static void After(ref IQueryable<OrderDto> query) { }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodHasBeforeOrAfterMapHooks, "ProjectToDto")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Test class-level before-map hooks are rejected on projection methods.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ClassLevelBeforeMapHookIsRejectedOnProjectionMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa;
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
                                  [MappaBeforeMap(nameof(Before))]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);

                                      private static void Before(ref IQueryable<Order> query) { }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodHasBeforeOrAfterMapHooks, "ProjectToDto")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Test class-level after-map hooks are rejected on projection methods.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ClassLevelAfterMapHookIsRejectedOnProjectionMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa;
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
                                  [MappaAfterMap(nameof(After))]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);

                                      private static void After(ref IQueryable<OrderDto> query) { }
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodHasBeforeOrAfterMapHooks, "ProjectToDto")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Test <see cref="MappaContext"/> parameters are rejected on projection methods.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MappaContextParameterIsRejectedOnProjectionMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa;
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
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query, MappaContext context);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodHasMappaContextParameter, "ProjectToDto")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Test collection properties are rejected inside projection element maps.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CollectionPropertyIsRejectedInProjectionElementMap()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class LineItem
                                  {
                                      public int Id { get; set; }
                                  }

                                  public class LineItemDto
                                  {
                                      public int Id { get; set; }
                                  }

                                  public class Order
                                  {
                                      public IQueryable<LineItem> Items { get; set; } = null!;
                                  }

                                  public class OrderDto
                                  {
                                      public IQueryable<LineItemDto> Items { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
                                  #nullable restore
                                  """;

        const string lineItemType = $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.LineItem";
        const string lineItemDtoType = $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.LineItemDto";

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                QueryableProjectionMapAssertionExtensions.QueryableOf(lineItemType),
                QueryableProjectionMapAssertionExtensions.QueryableOf(lineItemDtoType))
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                "ProjectToDto",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                true,
                QueryableProjectionMapAssertionExtensions.QueryableOf(OrderDtoType),
                NullableAnnotation.NotAnnotated,
                "query",
                QueryableProjectionMapAssertionExtensions.QueryableOf(OrderType),
                NullableAnnotation.NotAnnotated,
                1,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement());
                });
    }

    /// <summary>
    /// Test polymorphic element maps are rejected for projections.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task PolymorphicElementMapIsRejectedInProjection()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public abstract class Source
                                  {
                                  }

                                  public sealed class SourceDog : Source
                                  {
                                      public int Id { get; set; }
                                  }

                                  public abstract class Target
                                  {
                                  }

                                  public sealed class TargetDog : Target
                                  {
                                      public int Id { get; set; }
                                  }

                                  [Mappa]
                                  [MappaTypeMapping(typeof(TargetDog), typeof(SourceDog))]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<Target> ProjectToDto(this IQueryable<Source> query);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.Source",
                $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.Target")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                "ProjectToDto",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                true,
                QueryableProjectionMapAssertionExtensions.QueryableOf($"{QueryableProjectionMapAssertionExtensions.TestNamespace}.Target"),
                NullableAnnotation.NotAnnotated,
                "query",
                QueryableProjectionMapAssertionExtensions.QueryableOf($"{QueryableProjectionMapAssertionExtensions.TestNamespace}.Source"),
                NullableAnnotation.NotAnnotated,
                1,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement());
                });
    }

    /// <summary>
    /// Test user map methods that require <see cref="MappaContext"/> are inlined when the context parameter is optional for the nested map.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectUsingUserMapMethodForNestedProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class ItemSource
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class ItemTarget
                                  {
                                      public int Value { get; set; }
                                  }

                                  public class Order
                                  {
                                      public ItemSource Item { get; set; } = null!;
                                  }

                                  public class OrderDto
                                  {
                                      public ItemTarget Item { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public partial ItemTarget MapItem(ItemSource input, MappaContext context)
                                      {
                                          return new ItemTarget { Value = input.Value + 1 };
                                      }

                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
                                  #nullable restore
                                  """;

        const string itemSourceType = $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.ItemSource";
        const string itemTargetType = $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.ItemTarget";
        const string projectionLambdaParameterName = "__mappa_tmp_3";

        Action<ExpressionSyntaxAssertions> itemPropertyAssertions = property => property.BeObjectCreationExpressionSyntax(
            itemTargetType,
            ("Value", value => value.BeMemberAccessExpressionSyntax($"{projectionLambdaParameterName}.Item.Value")));

        Action<ExpressionSyntaxAssertions> elementExpressionAssertions = elementExpression => elementExpression.BeObjectCreationExpressionSyntax(
            OrderDtoType,
            ("Item", itemPropertyAssertions));

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveCommentHeader()
            .HaveFileScopedNamespace(fileScopedNamespaceDeclarationSyntaxAssertions =>
            {
                fileScopedNamespaceDeclarationSyntaxAssertions
                    .HaveClasses(1)
                    .HaveClass(
                        "Mapper",
                        classDeclarationSyntaxAssertions =>
                        {
                            classDeclarationSyntaxAssertions
                                .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
                                .HaveMethods(2)
                                .HaveMethod(
                                    itemTargetType,
                                    NullableAnnotation.NotAnnotated,
                                    "MapItem",
                                    false,
                                    [
                                        (itemSourceType, NullableAnnotation.NotAnnotated, "input", RefKind.None, false),
                                        (typeof(Mappa.MappaContext).FullName ?? string.Empty, NullableAnnotation.NotAnnotated, "context", RefKind.None, false),
                                    ],
                                    methodDeclarationSyntaxAssertions =>
                                    {
                                        methodDeclarationSyntaxAssertions
                                            .HaveNullabilityAnnotation(NullableSetup.Enable)
                                            .HaveGeneratedCodeAttribute(attributeSyntaxAssertions => attributeSyntaxAssertions.BeMappaGeneratedCodeAttribute())
                                            .HaveDebuggerNonUserCodeAttribute()
                                            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword)
                                            .HaveBody(blockSyntaxAssertions =>
                                            {
                                                blockSyntaxAssertions
                                                    .HasSyntaxNodesCount(3)
                                                    .HasNextSyntaxNode(syntaxNodeAssertions =>
                                                    {
                                                        syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                            typeof(int).ToString(),
                                                            "__mappa_tmp_1",
                                                            initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value"));
                                                    })
                                                    .HasNextSyntaxNode(syntaxNodeAssertions =>
                                                    {
                                                        syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                                            itemTargetType,
                                                            "__mappa_tmp_2",
                                                            initializationAssertions =>
                                                            {
                                                                initializationAssertions.BeObjectCreationExpressionSyntax(
                                                                    itemTargetType,
                                                                    ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                                                            });
                                                    })
                                                    .HasNextSyntaxNode(syntaxNodeAssertions =>
                                                    {
                                                        syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
                                                    });
                                            });
                                    })
                                .HaveMethod(
                                    QueryableProjectionMapAssertionExtensions.QueryableOf(OrderDtoType),
                                    NullableAnnotation.NotAnnotated,
                                    "ProjectToDto",
                                    true,
                                    [(QueryableProjectionMapAssertionExtensions.QueryableOf(OrderType), NullableAnnotation.NotAnnotated, "query", RefKind.None, false)],
                                    methodDeclarationSyntaxAssertions =>
                                    {
                                        methodDeclarationSyntaxAssertions
                                            .HaveNullabilityAnnotation(NullableSetup.Enable)
                                            .HavePragmaWarningDisableAnnotation(PragmaWarning.NoBlock)
                                            .HaveRequiresDynamicCodeAttribute(QueryableProjectionMapAssertionExtensions.RequiresDynamicCodeMessage)
                                            .HaveGeneratedCodeAttribute(attributeSyntaxAssertions => attributeSyntaxAssertions.BeMappaGeneratedCodeAttribute())
                                            .HaveDebuggerNonUserCodeAttribute()
                                            .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
                                            .HaveBody(blockSyntaxAssertions =>
                                            {
                                                blockSyntaxAssertions
                                                    .HasSyntaxNodesCount(1)
                                                    .HasNextSyntaxNode(syntaxNodeAssertions =>
                                                    {
                                                        syntaxNodeAssertions.BeReturnStatement(returnExpressionAssertions =>
                                                        {
                                                            returnExpressionAssertions.BeInvocationExpressionSyntax(
                                                                "global::System.Linq.Queryable.Select",
                                                                queryArgumentAssertions => queryArgumentAssertions.BeIdentifierNameSyntax("query"),
                                                                lambdaArgumentAssertions => lambdaArgumentAssertions.BeSimpleLambdaExpressionSyntax(
                                                                    projectionLambdaParameterName,
                                                                    elementExpressionAssertions));
                                                        });
                                                    });
                                            });
                                    });
                        });
            });
    }
}