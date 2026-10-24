// <copyright file="MappaNullSubstituteInvokeMethodIntegrationTests.Diagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Negative / diagnostic paths for <c>[MappaNullSubstitute]</c> InvokeMethod mode.
/// </summary>
public sealed partial class MappaNullSubstituteInvokeMethodIntegrationTests
{
    /// <summary>
    /// Test InvokeMethod is rejected on <see cref="System.Linq.IQueryable{T}"/> projections.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task InvokeMethodIsRejectedOnQueryableProjection()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public static class NullSubstituteHelpers
                                  {
                                      public static string WhenNull(string? value) => value ?? "Other";
                                  }

                                  public class Source
                                  {
                                      public string? Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.Value), InvokeMethod = nameof(NullSubstituteHelpers.WhenNull), InvokeMethodType = typeof(NullSubstituteHelpers))]
                                      public partial IQueryable<Target> Map(IQueryable<Source> query);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.ProjectionMethodHasNullSubstituteInvokeMethod, "Map")
            .NotHaveGeneratedAnySourceCode();
    }

    /// <summary>
    /// Test a non-static InvokeMethodSource field is rejected on a static map method.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NonStaticInvokeMethodSourceIsRejectedOnStaticMapMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Helper
                                  {
                                      public string WhenNull(string? value) => value ?? "Other";
                                  }

                                  public class Source
                                  {
                                      public string? Value { get; set; }
                                      public int Other { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Value { get; set; }
                                      public int Other { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private readonly Helper helper = new();

                                      [MappaNullSubstitute(nameof(Target.Value), InvokeMethod = nameof(Helper.WhenNull), InvokeMethodSource = nameof(helper))]
                                      public static partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.FieldOrPropertyMustBeStatic, "helper")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "Map",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                false,
                TargetTypeName,
                NullableAnnotation.NotAnnotated,
                "input",
                SourceTypeName,
                NullableAnnotation.NotAnnotated,
                1,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "string?",
                                "__mappa_tmp_1",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Other"));
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                TargetTypeName,
                                "__mappa_tmp_3",
                                initializationAssertions =>
                                {
                                    initializationAssertions.BeObjectCreationExpressionSyntax(
                                        TargetTypeName,
                                        ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")),
                                        ("Other", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_3"));
                        });
                });
    }

    /// <summary>
    /// Test a missing <c>InvokeMethodSource</c> reports <see cref="MappaDiagnosticDescriptors.CannotFindFieldOrProperty"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MissingInvokeMethodSourceReportsCannotFindFieldOrProperty()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string? Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.Value), InvokeMethod = "WhenNull", InvokeMethodSource = "missingHelper")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.CannotFindFieldOrProperty, "missingHelper")
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
                    AssertFallbackMapWithoutNullSubstitute(blockSyntaxAssertions);
                });
    }

    /// <summary>
    /// Test a missing InvokeMethod name reports that no suitable method was detected.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MissingInvokeMethodNameReportsCannotDetectSuitableMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string? Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.Value), InvokeMethod = "MissingMethod")]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotDetectSuitableMethodToInvokeForParameter,
                "MissingMethod",
                "Mappa.Generator.Tests.UnitTests.SourceCode.Mapper",
                "Value")
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
                    AssertFallbackMapWithoutNullSubstitute(blockSyntaxAssertions);
                });
    }

    /// <summary>
    /// Test ambiguous InvokeMethod overloads report <see cref="MappaDiagnosticDescriptors.AmbiguousInvokeMethodResolution"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AmbiguousInvokeMethodOverloadsReportAmbiguousResolution()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string? Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Value { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.Value), InvokeMethod = nameof(WhenNull))]
                                      public partial Target Map(Source input);

                                      [MappaIgnore]
                                      private string WhenNull(string? value) => value ?? "A";

                                      [MappaIgnore]
                                      private string WhenNull(string value) => value;
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainAmbiguousInvokeMethodResolutionDiagnostic("WhenNull")
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
                    AssertFallbackMapWithoutNullSubstitute(blockSyntaxAssertions);
                });
    }

    /// <summary>
    /// Test InvokeMethodType referencing a nested type that cannot be resolved via metadata reports
    /// <see cref="MappaDiagnosticDescriptors.CannotDetectType"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task NestedInvokeMethodTypeWithUnresolvedMetadataNameReportsCannotDetectType()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string? Value { get; set; }
                                  }

                                  public class Target
                                  {
                                      public string Value { get; set; }
                                  }

                                  public class HelperContainer
                                  {
                                      public static class NestedHelper
                                      {
                                          public static string WhenNull(string? value) => value ?? "Other";
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaNullSubstitute(nameof(Target.Value), InvokeMethod = nameof(HelperContainer.NestedHelper.WhenNull), InvokeMethodType = typeof(HelperContainer.NestedHelper))]
                                      public partial Target Map(Source input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotDetectType,
                "Mappa.Generator.Tests.UnitTests.SourceCode.HelperContainer.NestedHelper")
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
                    AssertFallbackMapWithoutNullSubstitute(blockSyntaxAssertions);
                });
    }

    private static void AssertFallbackMapWithoutNullSubstitute(BlockSyntaxAssertions blockSyntaxAssertions)
    {
        blockSyntaxAssertions
            .HasSyntaxNodesCount(3)
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    "string?",
                    "__mappa_tmp_1",
                    initializationAssertions => initializationAssertions.BeMemberAccessExpressionSyntax("input.Value"));
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                    TargetTypeName,
                    "__mappa_tmp_2",
                    initializationAssertions =>
                    {
                        initializationAssertions.BeObjectCreationExpressionSyntax(
                            TargetTypeName,
                            ("Value", initAssertions => initAssertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                    });
            })
            .HasNextSyntaxNode(syntaxNodeAssertions =>
            {
                syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_2"));
            });
    }
}