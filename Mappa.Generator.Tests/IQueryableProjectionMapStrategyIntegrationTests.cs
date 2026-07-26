// <copyright file="IQueryableProjectionMapStrategyIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for <see cref="QueryableProjectionMapStrategy"/>.
/// </summary>
public sealed partial class IQueryableProjectionMapStrategyIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string OrderType = $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.Order";
    private const string OrderDtoType = $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.OrderDto";
    private const string LambdaParameterName = "__mappa_tmp_1";

    /// <summary>
    /// Test an extension method can project <see cref="System.Linq.IQueryable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectUsingExtensionMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

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
                true,
                "query",
                OrderType,
                OrderDtoType,
                LambdaParameterName,
                elementExpression => elementExpression.BeObjectCreationExpressionSyntax(
                    OrderDtoType,
                    ("Id", property => property.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Id")),
                    ("Name", property => property.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Name"))));
    }

    /// <summary>
    /// Test an instance partial method can project <see cref="System.Linq.IQueryable{T}"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectUsingInstancePartialMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

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
                                  public sealed partial class Mapper
                                  {
                                      public partial IQueryable<OrderDto> Map(IQueryable<Order> input);
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
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "Map",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                false,
                "input",
                OrderType,
                OrderDtoType,
                LambdaParameterName,
                elementExpression => elementExpression.BeObjectCreationExpressionSyntax(
                    OrderDtoType,
                    ("Id", property => property.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Id")),
                    ("Name", property => property.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Name"))));
    }

    /// <summary>
    /// Test constructor-parameter mapping is emitted inside the projection expression.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectUsingConstructorParameters()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Order
                                  {
                                      public int Id { get; set; }
                                      public string Name { get; set; } = null!;
                                  }

                                  public class OrderDto
                                  {
                                      public OrderDto(int id, string name)
                                      {
                                          this.Id = id;
                                          this.Name = name;
                                      }

                                      public int Id { get; }
                                      public string Name { get; }
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
                true,
                "query",
                OrderType,
                $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.OrderDto",
                LambdaParameterName,
                elementExpression => elementExpression.BeObjectCreationExpressionSyntax(
                    $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.OrderDto",
                    argument => argument.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Id"),
                    argument => argument.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Name")));
    }

    /// <summary>
    /// Test nested object mapping is supported in projections.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectUsingNestedSourcePropertyPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Address
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class Order
                                  {
                                      public Address Address { get; set; } = null!;
                                  }

                                  public class OrderDto
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
                                  #nullable restore
                                  """;

        const string addressDtoType = $"{QueryableProjectionMapAssertionExtensions.TestNamespace}.AddressDto";

        Action<ExpressionSyntaxAssertions> addressPropertyAssertions = property => property.BeObjectCreationExpressionSyntax(
            addressDtoType,
            ("City", city => city.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Address.City")));

        Action<ExpressionSyntaxAssertions> nestedElementExpressionAssertions = elementExpression => elementExpression.BeObjectCreationExpressionSyntax(
            OrderDtoType,
            ("Address", addressPropertyAssertions));

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
                true,
                "query",
                OrderType,
                OrderDtoType,
                LambdaParameterName,
                nestedElementExpressionAssertions);
    }

    /// <summary>
    /// Test enum properties are mapped using switch expressions in projections.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectEnumProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum SourceStatus
                                  {
                                      Active,
                                      Inactive,
                                  }

                                  public enum TargetStatus
                                  {
                                      Active,
                                      Inactive,
                                  }

                                  public class Order
                                  {
                                      public SourceStatus Status { get; set; }
                                  }

                                  public class OrderDto
                                  {
                                      public TargetStatus Status { get; set; }
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        Action<ExpressionSyntaxAssertions> statusPropertyAssertions = property =>
        {
            property.Subject.Should().BeOfType<SwitchExpressionSyntax>();
            ((SwitchExpressionSyntax)property.Subject).Arms.Should().HaveCount(3);
        };

        Action<ExpressionSyntaxAssertions> enumElementExpressionAssertions = elementExpression => elementExpression.BeObjectCreationExpressionSyntax(
            OrderDtoType,
            ("Status", statusPropertyAssertions));

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
                true,
                "query",
                OrderType,
                OrderDtoType,
                LambdaParameterName,
                enumElementExpressionAssertions);
    }

    /// <summary>
    /// Test nullable reference properties are mapped in projections.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanProjectNullableReferenceProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Order
                                  {
                                      public string? Name { get; set; }
                                  }

                                  public class OrderDto
                                  {
                                      public string? Name { get; set; }
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);
                                  }
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
                true,
                "query",
                OrderType,
                OrderDtoType,
                LambdaParameterName,
                elementExpression => elementExpression.BeObjectCreationExpressionSyntax(
                    OrderDtoType,
                    ("Name", property => property.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Name"))));
    }

    /// <summary>
    /// Test mapping <see cref="System.Linq.IQueryable{T}"/> to a concrete collection emits a warning and cannot be mapped.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task QueryableSourceToListEmitsMaterializationWarning()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
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
                                      public static partial List<OrderDto> Map(IQueryable<Order> query);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostic(Mappa.Generator.Diagnostics.MappaDiagnosticDescriptors.IQueryableMappedAsCollection, "Map")
            .HaveDiagnostic(
                Mappa.Generator.Diagnostics.MappaDiagnosticDescriptors.CannotIdentifyStrategy,
                QueryableProjectionMapAssertionExtensions.QueryableOf(OrderType),
                "System.Collections.Generic.List<Mappa.Generator.Tests.UnitTests.SourceCode.OrderDto>")
            .HaveGeneratedSourceCode();
    }
}