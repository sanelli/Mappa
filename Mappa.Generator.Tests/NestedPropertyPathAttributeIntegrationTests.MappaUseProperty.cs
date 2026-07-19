// <copyright file="NestedPropertyPathAttributeIntegrationTests.MappaUseProperty.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// <see cref="MappaUsePropertyAttribute"/> nested property path integration tests.
/// </summary>
public sealed partial class NestedPropertyPathAttributeIntegrationTests
{
    /// <summary>
    /// Test mapping succeeds with a two-segment target and source property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMappaUsePropertyWithTwoSegmentTargetAndSourcePath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Outer
                                  {
                                      public string Value { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public Outer Outer { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public Outer Outer { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Outer.Value", "Outer.Value")]
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
                block => block
                    .HasSyntaxNodesCount(5)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Outer", "__mappa_tmp_1", value => value.BeMemberAccessExpressionSyntax("input.Outer")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_2", value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Value")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Outer", "__mappa_tmp_3", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Outer", ("Value", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_4", value => value.BeObjectCreationExpressionSyntax(TargetTypeName, ("Outer", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4")));
    }

    /// <summary>
    /// Test mapping succeeds with a three-segment target and source property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMappaUsePropertyWithThreeSegmentTargetAndSourcePath()
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
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Address.City", "Location.Address.City")]
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
                block => block
                    .HasSyntaxNodesCount(5)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.LocationDto?", "__mappa_tmp_1", value => value.BeMemberAccessExpressionSyntax("input.Location")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_2"))
                    .HasNextSyntaxNode(node => node.BeIfStatementSyntax(
                        condition => condition.BeIsPatternExpressionSyntax(
                            expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"),
                            pattern => pattern.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, inner => inner.BeConstantPatternSyntax(null))),
                        thenStatement => thenStatement.BeBlockStatement().AsBlock()
                            .HasSyntaxNodesCount(4)
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.LocationDto", "__mappa_tmp_3", value => value.BeIdentifierNameSyntax("__mappa_tmp_1")))
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_4",
                                value => value.BeBinaryExpressionSyntax(
                                    left => left.BeConditionalAccessExpressionSyntax("__mappa_tmp_3?.Address?.City"),
                                    SyntaxKind.CoalesceExpression,
                                    right => right.BeThrowExpressionSyntax<NullReferenceException>(message => message.BeLiteralExpressionSyntax("\"Location.Address.City\" is null.")))))
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_5", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", ("City", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                            .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement("__mappa_tmp_2", value => value.BeIdentifierNameSyntax("__mappa_tmp_5"))),
                        elseStatement => elseStatement.BeBlockStatement().AsBlock()
                            .HasSyntaxNodesCount(1)
                            .HasNextSyntaxNode(node => node.BeThrowStatementSyntax<NullReferenceException>(message => message.BeLiteralExpressionSyntax("\"__mappa_tmp_1\" is null.")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_6", value => value.BeObjectCreationExpressionSyntax(TargetTypeName, ("Address", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_6")));
    }

    /// <summary>
    /// Test mapping succeeds with a three-segment nested target path (Location.Address.City).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMappaUsePropertyWithThreeSegmentNestedTargetPath()
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
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public LocationDto Location { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public LocationDto Location { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Location.Address.City", "Location.Address.City")]
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
                block => block
                    .HasSyntaxNodesCount(7)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.LocationDto", "__mappa_tmp_1", value => value.BeMemberAccessExpressionSyntax("input.Location")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_2", value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Address")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_3", value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_2.City")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_4", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", ("City", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.LocationDto", "__mappa_tmp_5", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.LocationDto", ("Address", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_6", value => value.BeObjectCreationExpressionSyntax(TargetTypeName, ("Location", property => property.BeIdentifierNameSyntax("__mappa_tmp_5")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_6")));
    }

    /// <summary>
    /// Test multiple <see cref="MappaUsePropertyAttribute"/> declarations with different nested paths under the same root target member.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMultipleMappaUsePropertyAttributesWithDifferentNestedPathsSharingRoot()
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
                                      [MappaUseProperty("Address.City", "Address.City")]
                                      [MappaUseProperty("Address.ZipCode", "Address.ZipCode")]
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
                block => block
                    .HasSyntaxNodesCount(6)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_1", value => value.BeMemberAccessExpressionSyntax("input.Address")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_2", value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_1.City")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_3", value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_1.ZipCode")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_4", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", ("City", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")), ("ZipCode", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_5", value => value.BeObjectCreationExpressionSyntax(TargetTypeName, ("Address", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_5")));
    }

    /// <summary>
    /// Test multiple <see cref="MappaUsePropertyAttribute"/> declarations with different nested paths and different root target members.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMultipleMappaUsePropertyAttributesWithDifferentNestedPathsAndDifferentRoots()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; } = null!;
                                  }

                                  public class ContactDto
                                  {
                                      public string Name { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                      public ContactDto Contact { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                      public ContactDto Contact { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Address.City", "Address.City")]
                                      [MappaUseProperty("Contact.Name", "Contact.Name")]
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
                block => block
                    .HasSyntaxNodesCount(8)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_1", value => value.BeMemberAccessExpressionSyntax("input.Address")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_2", value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_1.City")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_3", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", ("City", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.ContactDto", "__mappa_tmp_4", value => value.BeMemberAccessExpressionSyntax("input.Contact")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_5", value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_4.Name")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.ContactDto", "__mappa_tmp_6", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.ContactDto", ("Name", property => property.BeIdentifierNameSyntax("__mappa_tmp_5")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_7", value => value.BeObjectCreationExpressionSyntax(TargetTypeName, ("Address", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")), ("Contact", property => property.BeIdentifierNameSyntax("__mappa_tmp_6")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_7")));
    }

    /// <summary>
    /// Test multiple nested <see cref="MappaUsePropertyAttribute"/> paths that share a target root
    /// but use a different first source segment.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingMultipleMappaUsePropertyAttributesWithDifferentSourceRootThanTargetRoot()
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

                                  public class LocationDto
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  public class Source
                                  {
                                      public LocationDto Location { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Address.City", "Location.Address.City")]
                                      [MappaUseProperty("Address.ZipCode", "Location.Address.ZipCode")]
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
                block => block
                    .HasSyntaxNodesCount(6)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.LocationDto", "__mappa_tmp_1", value => value.BeMemberAccessExpressionSyntax("input.Location")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_2", value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Address.City")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_3", value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Address.ZipCode")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_4", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", ("City", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")), ("ZipCode", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_5", value => value.BeObjectCreationExpressionSyntax(TargetTypeName, ("Address", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_5")));
    }

    /// <summary>
    /// Test mapping fails when multiple <see cref="MappaUsePropertyAttribute"/> declarations target the same exact nested path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MapFailsWhenMultipleMappaUsePropertyAttributesTargetTheSameExactNestedPath()
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
                                      [MappaUseProperty("Address.City", "Address.City")]
                                      [MappaUseProperty("Address.City", "Address.City")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TooManyUsePropertyAttributesForTheSameTargetProperty, "Map", "Address");
    }

    /// <summary>
    /// Test mapping fails when multiple flat <see cref="MappaUsePropertyAttribute"/> declarations target the same property path.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MapFailsWhenMultipleMappaUsePropertyAttributesTargetTheSameExactFlatPath()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public int PropertyA { get; set; }
                                      public int Foo { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string PropertyA { get; set; } = null!;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.PropertyA), nameof(Source.Foo))]
                                      [MappaUseProperty(nameof(Target.PropertyA), nameof(Source.PropertyA))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.TooManyUsePropertyAttributesForTheSameTargetProperty, "Map", "PropertyA");
    }

    /// <summary>
    /// Test swapped flat <see cref="MappaUsePropertyAttribute"/> mappings including identity int-to-int.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapUsingSwappedFlatMappaUsePropertyIncludingIntToIntIdentity()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public enum CountingValues { One, Two, Three }

                                  public class Source
                                  {
                                      public int ParamA { get; set; }
                                      public CountingValues ParamB { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string ParamA { get; set; } = string.Empty;
                                      public int ParamB { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.ParamA), nameof(Source.ParamB))]
                                      [MappaUseProperty(nameof(Target.ParamB), nameof(Source.ParamA))]
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
                block => block
                    .HasSyntaxNodesCount(6)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        "Mappa.Generator.Tests.UnitTests.SourceCode.CountingValues",
                        "__mappa_tmp_1",
                        value => value.BeMemberAccessExpressionSyntax("input.ParamB")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_2"))
                    .HasNextSyntaxNode(node => node.BeSwitchStatementSyntax(
                        value => value.BeIdentifierNameSyntax("__mappa_tmp_1"),
                        (labels, statements) =>
                        {
                            labels.Should().HaveCount(1);
                            labels[0].IsCase();
                            labels[0].AsCase().HasValue(value => value.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.CountingValues.One"));
                            statements.Should().HaveCount(1);
                            statements[0].BeBlockStatement();
                            statements[0].AsBlock()
                                .HasSyntaxNodesCount(2)
                                .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                    "__mappa_tmp_2",
                                    value => value.BeNameOf(argument => argument.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.CountingValues.One"))))
                                .HasNextSyntaxNode(statement => statement.BeBreakStatement());
                        },
                        (labels, statements) =>
                        {
                            labels.Should().HaveCount(1);
                            labels[0].IsCase();
                            labels[0].AsCase().HasValue(value => value.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.CountingValues.Two"));
                            statements.Should().HaveCount(1);
                            statements[0].BeBlockStatement();
                            statements[0].AsBlock()
                                .HasSyntaxNodesCount(2)
                                .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                    "__mappa_tmp_2",
                                    value => value.BeNameOf(argument => argument.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.CountingValues.Two"))))
                                .HasNextSyntaxNode(statement => statement.BeBreakStatement());
                        },
                        (labels, statements) =>
                        {
                            labels.Should().HaveCount(1);
                            labels[0].IsCase();
                            labels[0].AsCase().HasValue(value => value.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.CountingValues.Three"));
                            statements.Should().HaveCount(1);
                            statements[0].BeBlockStatement();
                            statements[0].AsBlock()
                                .HasSyntaxNodesCount(2)
                                .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                    "__mappa_tmp_2",
                                    value => value.BeNameOf(argument => argument.BeMemberAccessExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.CountingValues.Three"))))
                                .HasNextSyntaxNode(statement => statement.BeBreakStatement());
                        },
                        (labels, statements) =>
                        {
                            labels.Should().HaveCount(1);
                            labels[0].IsDefault();
                            statements.Should().HaveCount(1);
                            statements[0].BeBlockStatement();
                            statements[0].AsBlock()
                                .HasSyntaxNodesCount(1)
                                .HasNextSyntaxNode(statement => statement.BeThrowStatementSyntax<ArgumentOutOfRangeException>(
                                    argument => argument.BeLiteralExpressionSyntax("__mappa_tmp_1")));
                        }))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        typeof(int).ToString(),
                        "__mappa_tmp_3",
                        value => value.BeMemberAccessExpressionSyntax("input.ParamA")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        TargetTypeName,
                        "__mappa_tmp_4",
                        value => value.BeObjectCreationExpressionSyntax(
                            TargetTypeName,
                            ("ParamA", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")),
                            ("ParamB", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4")));
    }
}