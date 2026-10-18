// <copyright file="IQueryableProjectionMapStrategyIntegrationTests.FollowUps.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for #283 queryable projection follow-ups.
/// </summary>
public sealed partial class IQueryableProjectionMapStrategyIntegrationTests
{
    private const string ChildDtoType = $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.ChildDto";
    private const string NestedLambdaParameterName = "__mappa_tmp_2";

    /// <summary>
    /// Test nested collection properties emit Select + ToList inside the projection.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectNestedCollectionPropertyWithSelect()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Child
                                  {
                                      public int Id { get; set; }
                                  }

                                  public class ChildDto
                                  {
                                      public int Id { get; set; }
                                  }

                                  public class Order
                                  {
                                      public int Id { get; set; }
                                      public List<Child> Children { get; set; } = null!;
                                  }

                                  public class OrderDto
                                  {
                                      public int Id { get; set; }
                                      public List<ChildDto> Children { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveQueryableProjectionMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                "ProjectToDto",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                true,
                "query",
                OrderType,
                OrderDtoType,
                LambdaParameterName,
                ElementExpressionAssertions);

        static void ChildDtoAssertions(ExpressionSyntaxAssertions body)
        {
            body.BeObjectCreationExpressionSyntax(
                ChildDtoType,
                ("Id", id => id.BeMemberAccessExpressionSyntax($"{NestedLambdaParameterName}.Id")));
        }

        static void SelectLambdaAssertions(ExpressionSyntaxAssertions lambda)
        {
            lambda.BeSimpleLambdaExpressionSyntax(NestedLambdaParameterName, ChildDtoAssertions);
        }

        static void SelectAssertions(ExpressionSyntaxAssertions selectArgument)
        {
            selectArgument.BeInvocationExpressionSyntax(
                "global::System.Linq.Enumerable.Select",
                source => source.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Children"),
                SelectLambdaAssertions);
        }

        static void ChildrenAssertions(ExpressionSyntaxAssertions children)
        {
            children.BeInvocationExpressionSyntax(
                "global::System.Linq.Enumerable.ToList",
                SelectAssertions);
        }

        static void ElementExpressionAssertions(ExpressionSyntaxAssertions elementExpression)
        {
            elementExpression.BeObjectCreationExpressionSyntax(
                OrderDtoType,
                ("Id", property => property.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Id")),
                ("Children", ChildrenAssertions));
        }
    }

    /// <summary>
    /// Test tuple element mappings are projected as tuple expressions.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectTupleProperties()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Order
                                  {
                                      public (int Value, string Label) Values { get; set; }
                                  }

                                  public class OrderDto
                                  {
                                      public (string Value, string Label) Values { get; set; }
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveQueryableProjectionMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                "ProjectToDto",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                true,
                "query",
                OrderType,
                OrderDtoType,
                LambdaParameterName,
                ElementExpressionAssertions);

        static void Item1Assertions(ExpressionSyntaxAssertions item1)
        {
            item1.BeInvocationExpressionSyntax($"{LambdaParameterName}.Values.Item1.ToString");
        }

        static void ValuesAssertions(ExpressionSyntaxAssertions values)
        {
            values.BeTupleExpressionSyntax(
                Item1Assertions,
                item2 => item2.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Values.Item2"));
        }

        static void ElementExpressionAssertions(ExpressionSyntaxAssertions elementExpression)
        {
            elementExpression.BeObjectCreationExpressionSyntax(
                OrderDtoType,
                ("Values", ValuesAssertions));
        }
    }

    /// <summary>
    /// Test string-to-Guid mappings are projected with Parse.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectStringToGuidProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Order
                                  {
                                      public string Id { get; set; } = null!;
                                  }

                                  public class OrderDto
                                  {
                                      public Guid Id { get; set; }
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveQueryableProjectionMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                "ProjectToDto",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                true,
                "query",
                OrderType,
                OrderDtoType,
                LambdaParameterName,
                ElementExpressionAssertions);

        static void IdAssertions(ExpressionSyntaxAssertions id)
        {
            id.BeInvocationExpressionSyntax(
                "System.Guid.Parse",
                argument => argument.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Id"));
        }

        static void ElementExpressionAssertions(ExpressionSyntaxAssertions elementExpression)
        {
            elementExpression.BeObjectCreationExpressionSyntax(
                OrderDtoType,
                ("Id", IdAssertions));
        }
    }

    /// <summary>
    /// Test <see cref="Mappa.Attributes.MappaProjectionAttribute"/> with Disable skips the projection strategy.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MappaProjectionDisableFallsThroughToCollectionMapping()
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
                                      [MappaProjection(Enabled = BooleanSetting.Disable)]
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                QueryableProjectionMapAssertionExtensions.QueryableOf(OrderType),
                QueryableProjectionMapAssertionExtensions.QueryableOf(OrderDtoType))
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
                BlockSyntaxAssertions);

        static void BlockSyntaxAssertions(BlockSyntaxAssertions blockSyntaxAssertions)
        {
            blockSyntaxAssertions
                .HasSyntaxNodesCount(1)
                .HasNextSyntaxNode(node => node.BeReturnStatement());
        }
    }

    /// <summary>
    /// Test projection-specific Guid format overrides the non-projection Guid format.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ProjectionGuidFormatOverridesGuidFormat()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System;
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Order
                                  {
                                      public string Id { get; set; } = null!;
                                  }

                                  public class OrderDto
                                  {
                                      public Guid Id { get; set; }
                                  }

                                  [Mappa]
                                  [MappaSettings(GuidFormat = "D", ProjectionGuidFormat = "N")]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveQueryableProjectionMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                "ProjectToDto",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                true,
                "query",
                OrderType,
                OrderDtoType,
                LambdaParameterName,
                ElementExpressionAssertions);

        static void IdAssertions(ExpressionSyntaxAssertions id)
        {
            id.BeInvocationExpressionSyntax(
                "System.Guid.ParseExact",
                argument => argument.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Id"),
                format => format.BeLiteralExpressionSyntax("N"));
        }

        static void ElementExpressionAssertions(ExpressionSyntaxAssertions elementExpression)
        {
            elementExpression.BeObjectCreationExpressionSyntax(
                OrderDtoType,
                ("Id", IdAssertions));
        }
    }

    /// <summary>
    /// Test inlining a map method into a projection reports MP00086.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ProjectingNestedPropertyViaMapMethodReportsStructuralReplacementWarning()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Child
                                  {
                                      public int Id { get; set; }
                                  }

                                  public class ChildDto
                                  {
                                      public int Id { get; set; }
                                  }

                                  public class Order
                                  {
                                      public Child Child { get; set; } = null!;
                                  }

                                  public class OrderDto
                                  {
                                      public ChildDto Child { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial ChildDto MapChild(Child source);

                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
                                  #nullable restore
                                  """;

        const string projectionLambdaParameterName = "__mappa_tmp_3";

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodMapReplacedByStructuralStrategy, "ProjectToDto", "MapChild")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveCommentHeader()
            .HaveFileScopedNamespace(FileScopedNamespaceAssertions);

        static void ChildAssertions(ExpressionSyntaxAssertions child)
        {
            child.BeObjectCreationExpressionSyntax(
                ChildDtoType,
                ("Id", id => id.BeMemberAccessExpressionSyntax($"{projectionLambdaParameterName}.Child.Id")));
        }

        static void ElementExpressionAssertions(ExpressionSyntaxAssertions elementExpression)
        {
            elementExpression.BeObjectCreationExpressionSyntax(
                OrderDtoType,
                ("Child", ChildAssertions));
        }

        static void FileScopedNamespaceAssertions(
            FileScopedNamespaceDeclarationSyntaxAssertions fileScopedNamespaceDeclarationSyntaxAssertions)
        {
            fileScopedNamespaceDeclarationSyntaxAssertions
                .HaveClasses(1)
                .HaveClass("Mapper", ClassAssertions);
        }

        static void ClassAssertions(ClassDeclarationSyntaxAssertions classDeclarationSyntaxAssertions)
        {
            classDeclarationSyntaxAssertions
                .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
                .HaveMethods(2)
                .HaveMethod(
                    QueryableProjectionMapAssertionExtensions.QueryableOf(OrderDtoType),
                    NullableAnnotation.NotAnnotated,
                    "ProjectToDto",
                    true,
                    [(QueryableProjectionMapAssertionExtensions.QueryableOf(OrderType), NullableAnnotation.NotAnnotated, "query", RefKind.None, false)],
                    MethodAssertions);
        }

        static void MethodAssertions(MethodDeclarationSyntaxAssertions methodDeclarationSyntaxAssertions)
        {
            methodDeclarationSyntaxAssertions
                .HaveNullabilityAnnotation(NullableSetup.Enable)
                .HavePragmaWarningDisableAnnotation(PragmaWarning.NoBlock)
                .HaveRequiresDynamicCodeAttribute(QueryableProjectionMapAssertionExtensions.RequiresDynamicCodeMessage)
                .HaveGeneratedCodeAttribute(attributeSyntaxAssertions => attributeSyntaxAssertions.BeMappaGeneratedCodeAttribute())
                .HaveDebuggerNonUserCodeAttribute()
                .HaveModifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword)
                .HaveBody(BodyAssertions);
        }

        static void BodyAssertions(BlockSyntaxAssertions blockSyntaxAssertions)
        {
            blockSyntaxAssertions
                .HasSyntaxNodesCount(1)
                .HasNextSyntaxNode(ReturnStatementAssertions);
        }

        static void ReturnStatementAssertions(SyntaxNodeAssertions syntaxNodeAssertions)
        {
            syntaxNodeAssertions.BeReturnStatement(ReturnExpressionAssertions);
        }

        static void ReturnExpressionAssertions(ExpressionSyntaxAssertions returnExpressionAssertions)
        {
            returnExpressionAssertions.BeInvocationExpressionSyntax(
                "global::System.Linq.Queryable.Select",
                queryArgumentAssertions => queryArgumentAssertions.BeIdentifierNameSyntax("query"),
                LambdaArgumentAssertions);
        }

        static void LambdaArgumentAssertions(ExpressionSyntaxAssertions lambdaArgumentAssertions)
        {
            lambdaArgumentAssertions.BeSimpleLambdaExpressionSyntax(
                projectionLambdaParameterName,
                ElementExpressionAssertions);
        }
    }
}