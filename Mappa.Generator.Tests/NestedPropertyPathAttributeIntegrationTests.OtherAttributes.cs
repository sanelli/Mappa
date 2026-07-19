// <copyright file="NestedPropertyPathAttributeIntegrationTests.OtherAttributes.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Nested property path integration tests for attributes other than <see cref="MappaUsePropertyAttribute"/>.
/// </summary>
public sealed partial class NestedPropertyPathAttributeIntegrationTests
{
    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> with a nested target property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMappaInvokeMethodWithNestedTargetPropertyPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaInvokeMethodAttribute("Address.City", nameof(CustomMapCity))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      public string CustomMapCity(string city) => city.ToUpperInvariant();
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
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(6)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                            "__mappa_tmp_1",
                            expression => expression.BeMemberAccessExpressionSyntax("input.Address")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_2",
                            expression => expression.BeMemberAccessExpressionSyntax("__mappa_tmp_1.City")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_3",
                            expression => expression.BeInvocationExpressionSyntax(
                                "this.CustomMapCity",
                                argument => argument.BeIdentifierNameSyntax("__mappa_tmp_2"))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                            "__mappa_tmp_4",
                            expression => expression.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                                ("City", value => value.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetTypeName,
                            "__mappa_tmp_5",
                            expression => expression.BeObjectCreationExpressionSyntax(
                                TargetTypeName,
                                ("Address", value => value.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_5"));
                });
    }

    /// <summary>
    /// Test <see cref="MappaInvokeMethodAttribute"/> with a nested source property path on a flat target member.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMappaUsePropertyWithNestedSourcePropertyPathOnFlatTargetMember()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class LocationDto
                                  {
                                      public AddressDto? Address { get; set; }
                                  }

                                  public class Source
                                  {
                                      public LocationDto? Location { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.City), "Location.Address.City")]
                                      public partial Target Map(Source input);
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
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_1",
                            expression => expression.BeBinaryExpressionSyntax(
                                left => left.BeConditionalAccessExpressionSyntax("input.Location?.Address?.City"),
                                SyntaxKind.CoalesceExpression,
                                right => right.BeThrowExpressionSyntax<NullReferenceException>(
                                    message => message.BeLiteralExpressionSyntax("\"Location.Address.City\" is null.")))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetTypeName,
                            "__mappa_tmp_2",
                            expression => expression.BeObjectCreationExpressionSyntax(
                                TargetTypeName,
                                ("City", value => value.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test <see cref="MappaAssignFromConstantAttribute"/> with a nested target property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanAssignFromConstantUsingNestedTargetPropertyPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignFromConstant("Address.City", "London")]
                                      public partial Target Map(Source input, MappaContext context);
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
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                            "__mappa_tmp_1",
                            expression => expression.BeMemberAccessExpressionSyntax("input.Address")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_2",
                            expression => expression.BeLiteralExpressionSyntax("London")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                            "__mappa_tmp_3",
                            expression => expression.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                                ("City", value => value.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetTypeName,
                            "__mappa_tmp_4",
                            expression => expression.BeObjectCreationExpressionSyntax(
                                TargetTypeName,
                                ("Address", value => value.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Test <see cref="MappaAssignFromContextAttribute"/> with a nested target property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanAssignFromContextUsingNestedTargetPropertyPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignFromContext("Address.City", "city")]
                                      public partial Target Map(Source input, MappaContext context);
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
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                            "__mappa_tmp_1",
                            expression => expression.BeMemberAccessExpressionSyntax("input.Address")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_2",
                            expression => expression.BeCastExpressionSyntax(
                                typeof(string).ToString(),
                                cast => cast.BeElementAccessExpressionSyntaxWithLiteralSyntax("context", "city"))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                            "__mappa_tmp_3",
                            expression => expression.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                                ("City", value => value.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetTypeName,
                            "__mappa_tmp_4",
                            expression => expression.BeObjectCreationExpressionSyntax(
                                TargetTypeName,
                                ("Address", value => value.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }

    /// <summary>
    /// Test <see cref="MappaAssignToContextAttribute"/> with a nested target property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanAssignToContextUsingNestedTargetPropertyPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAssignToContext("caboom", "Address.City")]
                                      public partial Target Map(Source input, MappaContext context);
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
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                            "__mappa_tmp_1",
                            expression => expression.BeMemberAccessExpressionSyntax("input.Address")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetTypeName,
                            "__mappa_tmp_2",
                            expression => expression.BeObjectCreationExpressionSyntax(
                                TargetTypeName,
                                ("Address", value => value.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                        .HasNextSyntaxNode(node => node.BeAssignToContextStatement("context", "caboom", "__mappa_tmp_2", "Address.City"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test <see cref="MappaIgnoreTargetPropertyAttribute"/> with a nested target property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanIgnoreNestedTargetPropertyPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                      public string ZipCode { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnoreTargetProperty("Address.ZipCode")]
                                      public partial Target Map(Source input);
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
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                            "__mappa_tmp_1",
                            expression => expression.BeMemberAccessExpressionSyntax("input.Address")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(string).ToString(),
                            "__mappa_tmp_2",
                            expression => expression.BeMemberAccessExpressionSyntax("__mappa_tmp_1.City")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                            "__mappa_tmp_3",
                            expression => expression.BeObjectCreationExpressionSyntax(
                                "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                                ("City", value => value.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            TargetTypeName,
                            "__mappa_tmp_4",
                            expression => expression.BeObjectCreationExpressionSyntax(
                                TargetTypeName,
                                ("Address", value => value.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4"));
                });
    }
}