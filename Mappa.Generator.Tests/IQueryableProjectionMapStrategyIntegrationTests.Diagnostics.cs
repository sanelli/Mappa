// <copyright file="IQueryableProjectionMapStrategyIntegrationTests.Diagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

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
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                QueryableProjectionMapAssertionExtensions.QueryableOf(lineItemType),
                QueryableProjectionMapAssertionExtensions.QueryableOf(lineItemDtoType))
            .HaveGeneratedSourceCode();
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
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.Source",
                $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.Target")
            .HaveGeneratedSourceCode();
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

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode();
    }
}