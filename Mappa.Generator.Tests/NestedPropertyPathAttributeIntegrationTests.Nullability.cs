// <copyright file="NestedPropertyPathAttributeIntegrationTests.Nullability.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Nullability integration tests for nested property path source chains.
/// </summary>
public sealed partial class NestedPropertyPathAttributeIntegrationTests
{
    /// <summary>
    /// Test chained source reads use conditional access when nullable reference types are enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapChainedSourcePathWithNullableEnableUsingConditionalAccess()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string? City { get; set; }
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
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("string?", "__mappa_tmp_4", value => value.BeConditionalAccessExpressionSyntax("__mappa_tmp_3?.Address?.City")))
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_5", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", ("City", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                            .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement("__mappa_tmp_2", value => value.BeIdentifierNameSyntax("__mappa_tmp_5"))),
                        elseStatement => elseStatement.BeBlockStatement().AsBlock()
                            .HasSyntaxNodesCount(1)
                            .HasNextSyntaxNode(node => node.BeThrowStatementSyntax<NullReferenceException>(message => message.BeLiteralExpressionSyntax("\"__mappa_tmp_1\" is null.")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_6", value => value.BeObjectCreationExpressionSyntax(TargetTypeName, ("Address", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_6")));
    }

    /// <summary>
    /// Test chained source reads use conditional access for reference types when nullable reference types are disabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapChainedSourcePathWithNullableDisableUsingConditionalAccess()
    {
        const string sourceCode = """
                                  #nullable disable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class AddressDto
                                  {
                                      public string City { get; set; }
                                  }

                                  public class LocationDto
                                  {
                                      public AddressDto Address { get; set; }
                                  }

                                  public class Source
                                  {
                                      public LocationDto Location { get; set; }
                                  }

                                  public class Target
                                  {
                                      public AddressDto Address { get; set; }
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
                NullableAnnotation.None,
                SourceTypeName,
                NullableAnnotation.None,
                1,
                NullableSetup.Disable,
                PragmaWarning.NoBlock,
                block => block
                    .HasSyntaxNodesCount(3)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_1"))
                    .HasNextSyntaxNode(node => node.BeIfStatementSyntax(
                        condition => condition.BeIsPatternExpressionSyntax(
                            expression => expression.BeIdentifierNameSyntax("input"),
                            pattern => pattern.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, inner => inner.BeConstantPatternSyntax(null))),
                        thenStatement => thenStatement.BeBlockStatement().AsBlock()
                            .HasSyntaxNodesCount(6)
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(SourceTypeName, "__mappa_tmp_2", value => value.BeIdentifierNameSyntax("input")))
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.LocationDto", "__mappa_tmp_3", value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_2.Location")))
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_4"))
                            .HasNextSyntaxNode(node => node.BeIfStatementSyntax(
                                condition => condition.BeIsPatternExpressionSyntax(
                                    expression => expression.BeIdentifierNameSyntax("__mappa_tmp_3"),
                                    pattern => pattern.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, inner => inner.BeConstantPatternSyntax(null))),
                                thenStatement => thenStatement.BeBlockStatement().AsBlock()
                                    .HasSyntaxNodesCount(4)
                                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.LocationDto", "__mappa_tmp_5", value => value.BeIdentifierNameSyntax("__mappa_tmp_3")))
                                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_6", value => value.BeConditionalAccessExpressionSyntax("__mappa_tmp_5?.Address?.City")))
                                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_7", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", ("City", property => property.BeIdentifierNameSyntax("__mappa_tmp_6")))))
                                    .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement("__mappa_tmp_4", value => value.BeIdentifierNameSyntax("__mappa_tmp_7"))),
                                elseStatement => elseStatement.BeBlockStatement().AsBlock()
                                    .HasSyntaxNodesCount(1)
                                    .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement(
                                        "__mappa_tmp_4",
                                        value => value.BeCastExpressionSyntax(
                                            "Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto",
                                            cast => cast.BeLiteralExpressionSyntax(null))))))
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_8", value => value.BeObjectCreationExpressionSyntax(TargetTypeName, ("Address", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                            .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement("__mappa_tmp_1", value => value.BeIdentifierNameSyntax("__mappa_tmp_8"))),
                        elseStatement => elseStatement.BeBlockStatement().AsBlock()
                            .HasSyntaxNodesCount(1)
                            .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement(
                                "__mappa_tmp_1",
                                value => value.BeCastExpressionSyntax(TargetTypeName, cast => cast.BeLiteralExpressionSyntax(null))))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1")));
    }

    /// <summary>
    /// Test chained source reads use conditional access for nullable value type segments.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapChainedSourcePathThroughNullableValueTypeSegment()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Outer
                                  {
                                      public int? Code { get; set; }
                                  }

                                  public class Source
                                  {
                                      public Outer? Outer { get; set; }
                                  }

                                  public class Target
                                  {
                                      public int? Code { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.Code), "Outer.Code")]
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
                    .HasSyntaxNodesCount(3)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        "int?",
                        "__mappa_tmp_1",
                        value => value.BeConditionalAccessExpressionSyntax("input.Outer?.Code")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        TargetTypeName,
                        "__mappa_tmp_2",
                        value => value.BeObjectCreationExpressionSyntax(
                            TargetTypeName,
                            ("Code", property => property.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2")));
    }

    /// <summary>
    /// Test chained source reads use plain member access for non-nullable value type segments.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapChainedSourcePathThroughNonNullableValueTypeSegment()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Outer
                                  {
                                      public int Code { get; set; }
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
                                      [MappaUseProperty("Outer.Code", "Outer.Code")]
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
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_2", value => value.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Code")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Outer", "__mappa_tmp_3", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Outer", ("Code", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_4", value => value.BeObjectCreationExpressionSyntax(TargetTypeName, ("Outer", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4")));
    }

    /// <summary>
    /// Test mixed nullable and non-nullable reference segments under <c>#nullable enable</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapChainedSourcePathWithMixedNullableAndNonNullableReferences()
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
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.LocationDto", "__mappa_tmp_1", value => value.BeMemberAccessExpressionSyntax("input.Location")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        typeof(string).ToString(),
                        "__mappa_tmp_2",
                        value => value.BeBinaryExpressionSyntax(
                            left => left.BeConditionalAccessExpressionSyntax("__mappa_tmp_1.Address?.City"),
                            SyntaxKind.CoalesceExpression,
                            right => right.BeThrowExpressionSyntax<NullReferenceException>(message => message.BeLiteralExpressionSyntax("\"Location.Address.City\" is null.")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", "__mappa_tmp_3", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.AddressDto", ("City", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_4", value => value.BeObjectCreationExpressionSyntax(TargetTypeName, ("Address", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_4")));
    }

    /// <summary>
    /// Test mixed reference, value type, and <see cref="Nullable{T}"/> segments under <c>#nullable enable</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapChainedSourcePathWithMixedReferencesValueTypesAndNullableValueTypes()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public struct Metrics
                                  {
                                      public int? Score { get; set; }
                                  }

                                  public class Container
                                  {
                                      public Metrics Metrics { get; set; }
                                  }

                                  public class Source
                                  {
                                      public Container? Container { get; set; }
                                  }

                                  public class Target
                                  {
                                      public Metrics Metrics { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty("Metrics.Score", "Container.Metrics.Score")]
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
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Container?", "__mappa_tmp_1", value => value.BeMemberAccessExpressionSyntax("input.Container")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Metrics", "__mappa_tmp_2"))
                    .HasNextSyntaxNode(node => node.BeIfStatementSyntax(
                        condition => condition.BeIsPatternExpressionSyntax(
                            expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"),
                            pattern => pattern.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, inner => inner.BeConstantPatternSyntax(null))),
                        thenStatement => thenStatement.BeBlockStatement().AsBlock()
                            .HasSyntaxNodesCount(4)
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Container", "__mappa_tmp_3", value => value.BeIdentifierNameSyntax("__mappa_tmp_1")))
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("int?", "__mappa_tmp_4", value => value.BeConditionalAccessExpressionSyntax("__mappa_tmp_3?.Metrics.Score")))
                            .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Metrics", "__mappa_tmp_5", value => value.BeObjectCreationExpressionSyntax("Mappa.Generator.Tests.UnitTests.SourceCode.Metrics", ("Score", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                            .HasNextSyntaxNode(node => node.BeAssignmentExpressionStatement("__mappa_tmp_2", value => value.BeIdentifierNameSyntax("__mappa_tmp_5"))),
                        elseStatement => elseStatement.BeBlockStatement().AsBlock()
                            .HasSyntaxNodesCount(1)
                            .HasNextSyntaxNode(node => node.BeThrowStatementSyntax<NullReferenceException>(message => message.BeLiteralExpressionSyntax("\"__mappa_tmp_1\" is null.")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(TargetTypeName, "__mappa_tmp_6", value => value.BeObjectCreationExpressionSyntax(TargetTypeName, ("Metrics", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_6")));
    }

    /// <summary>
    /// Test a non-nullable reference chain ending in <see cref="Nullable{T}"/> appends <c>?? throw</c> for a non-nullable target.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMapChainedSourcePathWithNullableValueTypeLeafOntoNonNullableTarget()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Outer
                                  {
                                      public int? Code { get; set; }
                                  }

                                  public class Source
                                  {
                                      public Outer Outer { get; set; } = null!;
                                  }

                                  public class Target
                                  {
                                      public int Code { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaUseProperty(nameof(Target.Code), "Outer.Code")]
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
                    .HasSyntaxNodesCount(3)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        "int?",
                        "__mappa_tmp_1",
                        value => value.BeBinaryExpressionSyntax(
                            left => left.BeMemberAccessExpressionSyntax("input.Outer.Code"),
                            SyntaxKind.CoalesceExpression,
                            right => right.BeThrowExpressionSyntax<NullReferenceException>(message => message.BeLiteralExpressionSyntax("\"Outer.Code\" is null.")))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        TargetTypeName,
                        "__mappa_tmp_2",
                        value => value.BeObjectCreationExpressionSyntax(
                            TargetTypeName,
                            ("Code", property => property.BeIdentifierNameSyntax("__mappa_tmp_1")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2")));
    }
}