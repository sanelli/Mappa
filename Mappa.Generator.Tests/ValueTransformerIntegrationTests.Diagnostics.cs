// <copyright file="ValueTransformerIntegrationTests.Diagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Diagnostic integration tests for value transformers.
/// </summary>
public sealed partial class ValueTransformerIntegrationTests
{
    /// <summary>
    /// Test transformers with invalid signatures are reported and ignored without suppressing valid siblings.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task InvalidTransformerSignaturesAreReportedAndIgnored()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Threading.Tasks;
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial string Map(int input);

                                      [MappaTransformer]
                                      private string Valid(string value) => value.Trim();

                                      [MappaTransformer]
                                      private void ReturnsVoid(string value) { }

                                      [MappaTransformer]
                                      private string DifferentReturnType(long value) => string.Empty;

                                      [MappaTransformer]
                                      private string ByRef(ref string value) => value;

                                      [MappaTransformer]
                                      private string WrongSecondParameter(string value, int notContext) => value;

                                      [MappaTransformer]
                                      private string TooManyParameters(string value, MappaContext context, int extra) => value;

                                      [MappaTransformer]
                                      private async Task<string> Asynchronous(Task<string> value) => await value;

                                      [MappaTransformer]
                                      private string UninferableTypeParameter<T>(string value) => value;
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(7)
            .HaveDiagnostic(MappaDiagnosticDescriptors.InvalidTransformerSignature, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper.ReturnsVoid(string)", "it must return a value")
            .HaveDiagnostic(MappaDiagnosticDescriptors.InvalidTransformerSignature, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper.DifferentReturnType(long)", "the return type must be the same as the value parameter type, nullability included")
            .HaveDiagnostic(MappaDiagnosticDescriptors.InvalidTransformerSignature, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper.ByRef(ref string)", "the value parameter must not be 'ref' or 'out'")
            .HaveDiagnostic(MappaDiagnosticDescriptors.InvalidTransformerSignature, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper.WrongSecondParameter(string, int)", "the second parameter must be a MappaContext passed by value")
            .HaveDiagnostic(MappaDiagnosticDescriptors.InvalidTransformerSignature, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper.TooManyParameters(string, Mappa.MappaContext, int)", "it must have one parameter, or one parameter and a MappaContext")
            .HaveDiagnostic(MappaDiagnosticDescriptors.InvalidTransformerSignature, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper.Asynchronous(System.Threading.Tasks.Task<string>)", "asynchronous transformers are not supported")
            .HaveDiagnostic(MappaDiagnosticDescriptors.InvalidTransformerSignature, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper.UninferableTypeParameter<T>(string)", "every generic type parameter must be inferable from the value parameter")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Valid", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test a missing source member path is an error and the invalid attribute is not applied.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MissingSourcePathOnApplyTransformerIsAnError()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string Name { get; set; } = string.Empty;
                                  }

                                  public class Target
                                  {
                                      public string Name { get; set; } = string.Empty;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaApplyTransformer("Missing.Path", nameof(Normalize))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      private string Normalize(string value) => value.Trim();
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ApplyTransformerSourceMemberNotFound, "Map", "Missing.Path", $"{SourceNamespace}.Source")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                $"{SourceNamespace}.Target",
                $"{SourceNamespace}.Source",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement());
                });
    }

    /// <summary>
    /// Test specifying both a class type and a field name warns and skips the attribute.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ClassTypeAndFieldNameConflictWarnsAndSkipsTheAttribute()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public static class Normalizers
                                  {
                                      public static string Normalize(string value) => value.Trim();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private readonly object normalizer = new();

                                      [MappaApplyTransformer(typeof(Normalizers), nameof(Normalizers.Normalize), FieldName = nameof(normalizer))]
                                      public partial string Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ApplyTransformerClassTypeAndFieldNameConflict, "Map", "Normalize")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(2)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Test an unresolved or inaccessible transformer method warns and skips only that attribute.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task UnresolvedApplyTransformerMethodWarnsAndSkipsTheAttribute()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public static class Normalizers
                                  {
                                      private static string Hidden(string value) => value;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaApplyTransformer("Missing")]
                                      [MappaApplyTransformer(nameof(Valid))]
                                      [MappaApplyTransformer(typeof(Normalizers), "Hidden")]
                                      [MappaApplyTransformer(nameof(WrongSignature))]
                                      public partial string Map(int input);

                                      private string Valid(string value) => value.Trim();

                                      private int WrongSignature(string value) => 0;
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(3)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ApplyTransformerMethodNotFound, "Map", "Missing")
            .HaveDiagnostic(MappaDiagnosticDescriptors.ApplyTransformerMethodNotFound, "Map", "Hidden")
            .HaveDiagnostic(MappaDiagnosticDescriptors.ApplyTransformerMethodNotFound, "Map", "WrongSignature")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Valid", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test an unknown <c>FieldName</c> is reported and the attribute is skipped.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task UnknownFieldNameOnApplyTransformerIsReported()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaApplyTransformer("Normalize", FieldName = "missingField")]
                                      public partial string Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotFindFieldOrProperty, "missingField")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement());
                });
    }

    /// <summary>
    /// Test a static map method cannot use an instance transformer reached through an instance field.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task StaticMapMethodCannotApplyTransformerFromInstanceField()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Normalizer
                                  {
                                      public string Normalize(string value) => value.Trim();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private readonly Normalizer normalizer = new();

                                      [MappaApplyTransformer(nameof(Normalizer.Normalize), FieldName = nameof(normalizer))]
                                      public static partial string Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.FieldOrPropertyMustBeStatic, "normalizer")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultStaticMapMethod(
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                1,
                false,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement());
                });
    }

    /// <summary>
    /// Test <c>[MappaApplyTransformer]</c> is rejected on a queryable projection method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ApplyTransformerIsRejectedOnProjectionMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Order
                                  {
                                      public int Id { get; set; }
                                      public string Name { get; set; } = string.Empty;
                                  }

                                  public class OrderDto
                                  {
                                      public int Id { get; set; }
                                      public string Name { get; set; } = string.Empty;
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      [MappaApplyTransformer(nameof(Normalize))]
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);

                                      [MappaIgnore]
                                      private static string Normalize(string value) => value.Trim();
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodHasApplyTransformer, "ProjectToDto")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Test class-level transformers do not invalidate a projection method and are never applied inside it.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ProjectionIgnoresClassTransformersWithoutInvalidatingTheMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Order
                                  {
                                      public int Id { get; set; }
                                      public string Name { get; set; } = string.Empty;
                                  }

                                  public class OrderDto
                                  {
                                      public int Id { get; set; }
                                      public string Name { get; set; } = string.Empty;
                                  }

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      public static partial IQueryable<OrderDto> ProjectToDto(this IQueryable<Order> query);

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private static string Normalize(string value) => value.Trim();

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private static int Clamp(int value) => value;
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
                "query",
                $"{SourceNamespace}.Order",
                $"{SourceNamespace}.OrderDto",
                "__mappa_tmp_1",
                elementExpression => elementExpression.BeObjectCreationExpressionSyntax(
                    $"{SourceNamespace}.OrderDto",
                    ("Id", property => property.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Id")),
                    ("Name", property => property.BeMemberAccessExpressionSyntax("__mappa_tmp_1.Name"))));
    }

    /// <summary>
    /// Test transformers returning by reference are reported as invalid and ignored.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task TransformersReturningByReferenceAreReportedAndIgnored()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private string stored = string.Empty;

                                      public partial string Map(int input);

                                      [MappaTransformer]
                                      private ref string RefReturn(string value) => ref stored;

                                      [MappaTransformer]
                                      private ref readonly string RefReadonlyReturn(string value) => ref stored;

                                      [MappaTransformer]
                                      private string Valid(string value) => value;
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.InvalidTransformerSignature, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper.RefReturn(string)", "it must not return by reference")
            .HaveDiagnostic(MappaDiagnosticDescriptors.InvalidTransformerSignature, "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper.RefReadonlyReturn(string)", "it must not return by reference")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Valid", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }
}