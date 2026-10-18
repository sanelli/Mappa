// <copyright file="IQueryableProjectionSqliteIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;
using System.Reflection;

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;
using Mappa.Generator.Tests.Helpers;
using Mappa.Generator.Tests.Models;

using Microsoft.CodeAnalysis;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Mappa.Generator.Tests;

/// <summary>
/// Runtime integration tests verifying queryable projections translate against EF Core Sqlite.
/// </summary>
public sealed class IQueryableProjectionSqliteIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string SqliteNamespace = "Mappa.Generator.Tests.UnitTests.SqliteProjection";
    private const string OrderTypeName = $"{SqliteNamespace}.Order";
    private const string OrderDtoTypeName = $"{SqliteNamespace}.OrderDto";
    private const string ChildTypeName = $"{SqliteNamespace}.Child";
    private const string ChildDtoTypeName = $"{SqliteNamespace}.ChildDto";
    private const string LambdaParameterName = "__mappa_tmp_1";
    private const string NestedLambdaParameterName = "__mappa_tmp_2";

    /// <summary>
    /// Test a flat projection translates and executes against EF Core Sqlite.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanTranslateAndExecuteFlatProjectionAgainstSqlite()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SqliteProjection;

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

        var generatedResults = await RunMappaGeneratorWithEfReferencesAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                OrderTypeName,
                OrderDtoTypeName,
                LambdaParameterName,
                ElementExpressionAssertions);

        var assembly = CompileToAssembly(generatedResults.OutputCompilation);
        var orderType = assembly.GetType(OrderTypeName)!;
        var orderDtoType = assembly.GetType(OrderDtoTypeName)!;

        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync(CancellationToken.None).ConfigureAwait(true);
        var options = new DbContextOptionsBuilder<FlatProjectionSqliteDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new FlatProjectionSqliteDbContext(options, orderType);
        try
        {
            await context.Database.EnsureCreatedAsync(CancellationToken.None).ConfigureAwait(true);

            var order = Activator.CreateInstance(orderType)!;
            orderType.GetProperty("Id")!.SetValue(order, 7);
            orderType.GetProperty("Name")!.SetValue(order, "Sqlite order");
            context.Add(order);
            await context.SaveChangesAsync(CancellationToken.None).ConfigureAwait(true);

            var projected = InvokeProjectToDto(assembly, SetAsQueryable(context, orderType));
            var sql = projected.ToQueryString();
            sql.Should().Contain("Name");
            sql.Should().Contain("Id");

            var results = Materialize(projected, orderDtoType);
            results.Count.Should().Be(1);
            orderDtoType.GetProperty("Id")!.GetValue(results[0]).Should().Be(7);
            orderDtoType.GetProperty("Name")!.GetValue(results[0]).Should().Be("Sqlite order");
        }
        finally
        {
            await context.DisposeAsync().ConfigureAwait(true);
            await connection.DisposeAsync().ConfigureAwait(true);
        }

        static void ElementExpressionAssertions(ExpressionSyntaxAssertions elementExpression)
        {
            elementExpression.BeObjectCreationExpressionSyntax(
                OrderDtoTypeName,
                ("Id", property => property.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Id")),
                ("Name", property => property.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Name")));
        }
    }

    /// <summary>
    /// Test a nested-collection projection translates and executes against EF Core Sqlite.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanTranslateAndExecuteNestedCollectionProjectionAgainstSqlite()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SqliteProjection;

                                  public class Child
                                  {
                                      public int Id { get; set; }
                                      public int OrderId { get; set; }
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
                                  """;

        var generatedResults = await RunMappaGeneratorWithEfReferencesAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

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
                OrderTypeName,
                OrderDtoTypeName,
                LambdaParameterName,
                ElementExpressionAssertions);

        var assembly = CompileToAssembly(generatedResults.OutputCompilation);
        var orderType = assembly.GetType(OrderTypeName)!;
        var childType = assembly.GetType(ChildTypeName)!;
        var orderDtoType = assembly.GetType(OrderDtoTypeName)!;

        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync(CancellationToken.None).ConfigureAwait(true);
        var options = new DbContextOptionsBuilder<NestedProjectionSqliteDbContext>()
            .UseSqlite(connection)
            .Options;
        var context = new NestedProjectionSqliteDbContext(options, orderType, childType);
        try
        {
            await context.Database.EnsureCreatedAsync(CancellationToken.None).ConfigureAwait(true);

            var order = Activator.CreateInstance(orderType)!;
            orderType.GetProperty("Id")!.SetValue(order, 3);
            var children = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(childType))!;
            orderType.GetProperty("Children")!.SetValue(order, children);

            var child = Activator.CreateInstance(childType)!;
            childType.GetProperty("Id")!.SetValue(child, 11);
            childType.GetProperty("OrderId")!.SetValue(child, 3);
            children.Add(child);

            context.Add(order);
            await context.SaveChangesAsync(CancellationToken.None).ConfigureAwait(true);

            var projected = InvokeProjectToDto(assembly, SetAsQueryable(context, orderType));
            var sql = projected.ToQueryString();
            sql.Should().NotBeNullOrWhiteSpace();

            var results = Materialize(projected, orderDtoType);
            results.Count.Should().Be(1);
            var projectedChildren = (IList)orderDtoType.GetProperty("Children")!.GetValue(results[0])!;
            projectedChildren.Count.Should().Be(1);
            projectedChildren[0]!.GetType().GetProperty("Id")!.GetValue(projectedChildren[0]).Should().Be(11);
        }
        finally
        {
            await context.DisposeAsync().ConfigureAwait(true);
            await connection.DisposeAsync().ConfigureAwait(true);
        }

        static void ChildDtoAssertions(ExpressionSyntaxAssertions body)
        {
            body.BeObjectCreationExpressionSyntax(
                ChildDtoTypeName,
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
                OrderDtoTypeName,
                ("Id", property => property.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Id")),
                ("Children", ChildrenAssertions));
        }
    }

    private static Task<GeneratedResults> RunMappaGeneratorWithEfReferencesAsync(string source, CancellationToken cancellationToken)
    {
        var generator = new MappaGenerator();
        var compilation = BuildCompilation(source)
            .AddReferences(MetadataReference.CreateFromFile(typeof(Queryable).Assembly.Location));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var diagnostics,
            cancellationToken);

        GeneratedSourceDumpHelper.TryDumpGeneratedSources(driver);
        return Task.FromResult(new GeneratedResults(driver, outputCompilation, diagnostics.ToArray()));
    }

    private static Assembly CompileToAssembly(Compilation compilation)
    {
        using var stream = new MemoryStream();
        var emitResult = compilation.Emit(stream);
        emitResult.Success.Should().BeTrue(string.Join(Environment.NewLine, emitResult.Diagnostics));
        return Assembly.Load(stream.ToArray());
    }

    private static IQueryable SetAsQueryable(DbContext context, Type entityType)
    {
        var setMethod = typeof(DbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(method => method.Name == nameof(DbContext.Set)
                              && method.IsGenericMethodDefinition
                              && method.GetParameters().Length == 0);
        return (IQueryable)setMethod.MakeGenericMethod(entityType).Invoke(context, null)!;
    }

    private static IQueryable InvokeProjectToDto(Assembly assembly, IQueryable ordersQueryable)
    {
        var mapperType = assembly.GetType($"{SqliteNamespace}.Mapper")
                         ?? throw new InvalidOperationException("Mapper type was not found.");
        var projectMethod = mapperType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(method => method.Name == "ProjectToDto");
        return (IQueryable)projectMethod.Invoke(null, [ordersQueryable])!;
    }

    private static IList Materialize(IQueryable queryable, Type elementType)
    {
        var toList = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))!.MakeGenericMethod(elementType);
        return (IList)toList.Invoke(null, [queryable])!;
    }

    private sealed class FlatProjectionSqliteDbContext : DbContext
    {
        private readonly Type[] entityTypes;

        public FlatProjectionSqliteDbContext(DbContextOptions<FlatProjectionSqliteDbContext> options, params Type[] entityTypes)
            : base(options)
        {
            this.entityTypes = entityTypes;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in this.entityTypes)
            {
                modelBuilder.Entity(entityType);
            }
        }
    }

    private sealed class NestedProjectionSqliteDbContext(
        DbContextOptions<NestedProjectionSqliteDbContext> options,
        params Type[] entityTypes)
        : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in entityTypes)
            {
                modelBuilder.Entity(entityType);
            }
        }
    }
}