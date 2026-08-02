// <copyright file="IQueryableProjectionEntityFrameworkIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Models;

namespace Mappa.Generator.Tests;

/// <summary>
/// Runtime integration tests for queryable projections with Entity Framework Core-compatible queryables.
/// </summary>
public sealed class IQueryableProjectionEntityFrameworkIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string EntityFrameworkNamespace = "Mappa.Generator.Tests.UnitTests.EntityFramework";
    private const string OrderType = $"{EntityFrameworkNamespace}.Order";
    private const string OrderDtoType = $"{EntityFrameworkNamespace}.OrderDto";
    private const string LambdaParameterName = "__mappa_tmp_1";

    /// <summary>
    /// Test a generated projection can be executed against an <see cref="IQueryable{T}"/> data source.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanExecuteProjectionAgainstQueryableDataSource()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.EntityFramework;

                                  public class Order
                                  {
                                      public int Id { get; set; }
                                      public string Name { get; set; } = null!;
                                  }

                                  public class OrderDto
                                  {
                                      public int Id { get; set; }
                                      public string Name { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorWithQueryableReferenceAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .NotHaveCompilationErrors()
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
                elementExpression => elementExpression.BeObjectCreationExpressionSyntax(
                    OrderDtoType,
                    ("Id", property => property.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Id")),
                    ("Name", property => property.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Name"))));

        var assembly = CompileToAssembly(generatedResults.OutputCompilation);
        var mapperType = assembly.GetType($"{EntityFrameworkNamespace}.Mapper")
            ?? throw new InvalidOperationException("Mapper type was not found.");
        var orderType = assembly.GetType($"{EntityFrameworkNamespace}.Order")
            ?? throw new InvalidOperationException("Order type was not found.");
        var orderDtoType = assembly.GetType($"{EntityFrameworkNamespace}.OrderDto")
            ?? throw new InvalidOperationException("OrderDto type was not found.");

        var order = Activator.CreateInstance(orderType)!;
        orderType.GetProperty("Id")!.SetValue(order, 42);
        orderType.GetProperty("Name")!.SetValue(order, "Test order");

        var listType = typeof(List<>).MakeGenericType(orderType);
        var orders = (IList)Activator.CreateInstance(listType)!;
        listType.GetMethod("Add")!.Invoke(orders, [order]);

        var asQueryableMethod = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == nameof(Queryable.AsQueryable) && method.IsGenericMethodDefinition && method.GetParameters().Length == 1)
            .MakeGenericMethod(orderType);
        var ordersQueryable = (IQueryable)asQueryableMethod.Invoke(null, [orders])!
            ?? throw new InvalidOperationException("Orders queryable was not created.");

        var projectMethod = mapperType.GetMethod(
            "ProjectToDto",
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            [ordersQueryable.GetType()],
            modifiers: null)
            ?? throw new InvalidOperationException("ProjectToDto method was not found.");

        var projectedQueryable = (IQueryable)projectMethod.Invoke(null, [ordersQueryable])!
            ?? throw new InvalidOperationException("Projected queryable was not created.");

        var selectFinder = new SelectExpressionFinder();
        selectFinder.Visit(projectedQueryable.Expression);
        selectFinder.SelectCall.Should().NotBeNull();
        selectFinder.SelectCall!.Arguments.Count.Should().Be(2);
        var projectionLambda = selectFinder.SelectCall.Arguments[1] as LambdaExpression
            ?? (selectFinder.SelectCall.Arguments[1] as UnaryExpression)?.Operand as LambdaExpression;
        projectionLambda.Should().NotBeNull();

        var toListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))!.MakeGenericMethod(orderDtoType);
        var results = (IList)toListMethod.Invoke(null, [projectedQueryable])!
            ?? throw new InvalidOperationException("Projected results were not materialized.");

        results.Count.Should().Be(1);
        results[0]!.Should().BeOfType(orderDtoType);
        orderDtoType.GetProperty("Id")!.GetValue(results[0]).Should().Be(42);
        orderDtoType.GetProperty("Name")!.GetValue(results[0]).Should().Be("Test order");
    }

    private static Task<GeneratedResults> RunMappaGeneratorWithQueryableReferenceAsync(string source, CancellationToken cancellationToken)
    {
        var generator = new MappaGenerator();
        var compilation = BuildCompilation(source)
            .AddReferences(MetadataReference.CreateFromFile(typeof(System.Linq.Queryable).Assembly.Location));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var diagnostics,
            cancellationToken);

        return Task.FromResult(new GeneratedResults(driver, outputCompilation, diagnostics.ToArray()));
    }

    private static Assembly CompileToAssembly(Compilation compilation)
    {
        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        emitResult.Success.Should().BeTrue(string.Join(Environment.NewLine, emitResult.Diagnostics));
        return Assembly.Load(stream.ToArray());
    }

    private sealed class SelectExpressionFinder : ExpressionVisitor
    {
        internal MethodCallExpression? SelectCall { get; private set; }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == nameof(Queryable.Select)
                && node.Method.DeclaringType == typeof(Queryable))
            {
                this.SelectCall = node;
            }

            return base.VisitMethodCall(node);
        }
    }
}