// <copyright file="MappaBeforeAfterMapHookIntegrationTests.Diagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

using Microsoft.CodeAnalysis.CSharp;

namespace Mappa.Generator.Tests;

/// <summary>
/// Diagnostic integration tests for before-map and after-map hooks.
/// </summary>
public sealed partial class MappaBeforeAfterMapHookIntegrationTests
{
    /// <summary>
    /// Test a context hook is ignored when the mapping method does not provide context.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ContextHookIsIgnoredWhenMappingMethodDoesNotProvideContext()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaBeforeMap(nameof(Before))]
                                      public partial int Map(int input);

                                      private void Before(MappaContext context) { }
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.HookMethodNotFound,
                "Map",
                "before-map",
                "Before")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement("input"));
                });
    }

    /// <summary>
    /// Test unsupported hook signatures are warned and omitted without suppressing the map.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task InvalidHookSignaturesAreIgnoredWithoutSuppressingCoreMap()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class ExternalHooks
                                  {
                                      private static void Inaccessible(ref int input) { }
                                      public void Instance(ref int input) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaBeforeMap(nameof(NonVoid))]
                                      [MappaBeforeMap(nameof(MissingRef))]
                                      [MappaBeforeMap(nameof(WrongType))]
                                      [MappaAfterMap(nameof(WrongCount))]
                                      [MappaAfterMap(nameof(WrongOrder))]
                                      [MappaAfterMap(typeof(ExternalHooks), "Inaccessible")]
                                      [MappaAfterMap(typeof(ExternalHooks), nameof(ExternalHooks.Instance))]
                                      public partial int Map(int input, MappaContext context);

                                      private int NonVoid() => 0;
                                      private void MissingRef(int input) { }
                                      private void WrongType(ref long input) { }
                                      private void WrongCount(ref int input, MappaContext context, int extra) { }
                                      private void WrongOrder(MappaContext context, ref int input) { }
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(7)
            .HaveDiagnostic(MappaDiagnosticDescriptors.HookMethodNotFound, "Map", "before-map", "NonVoid")
            .HaveDiagnostic(MappaDiagnosticDescriptors.HookMethodNotFound, "Map", "before-map", "MissingRef")
            .HaveDiagnostic(MappaDiagnosticDescriptors.HookMethodNotFound, "Map", "before-map", "WrongType")
            .HaveDiagnostic(MappaDiagnosticDescriptors.HookMethodNotFound, "Map", "after-map", "WrongCount")
            .HaveDiagnostic(MappaDiagnosticDescriptors.HookMethodNotFound, "Map", "after-map", "WrongOrder")
            .HaveDiagnostic(MappaDiagnosticDescriptors.HookMethodNotFound, "Map", "after-map", "Inaccessible")
            .HaveDiagnostic(MappaDiagnosticDescriptors.HookMethodNotFound, "Map", "after-map", "Instance")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethodWithContext(
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement("input"));
                });
    }

    /// <summary>
    /// Test duplicate phase registrations warn and retain only the first hook in effective order.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DuplicateRegistrationsWarnAndInvokeEachHookOnce()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Hooks
                                  {
                                      public void Before(ref int input) { }
                                      public void After(ref int target) { }
                                  }

                                  [Mappa]
                                  [MappaBeforeMap("classHooks", nameof(Hooks.Before))]
                                  [MappaAfterMap("classHooks", nameof(Hooks.After))]
                                  public sealed partial class Mapper
                                  {
                                      private readonly Hooks classHooks = new();
                                      private readonly Hooks methodHooks = new();

                                      [MappaBeforeMap(nameof(methodHooks), nameof(Hooks.Before))]
                                      [MappaAfterMap(nameof(methodHooks), nameof(Hooks.After))]
                                      public partial int Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.DuplicateMapHookRegistration,
                "Map",
                "before-map",
                $"{SourceNamespace}.Hooks.Before(ref int)")
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.DuplicateMapHookRegistration,
                "Map",
                "after-map",
                $"{SourceNamespace}.Hooks.After(ref int)")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.classHooks.Before", (SyntaxKind.RefKeyword, "input")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_1",
                            initialization => initialization.BeIdentifierNameSyntax("input")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.methodHooks.After", (SyntaxKind.RefKeyword, "__mappa_tmp_1")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Test unresolved hooks are skipped without suppressing resolved siblings.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task UnresolvedHooksDoNotSuppressResolvedSiblings()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaBeforeMap(nameof(ValidBefore))]
                                      [MappaBeforeMap("MissingBefore")]
                                      [MappaAfterMap("MissingAfter")]
                                      [MappaAfterMap(nameof(ValidAfter))]
                                      public partial long Map(long input);

                                      private void ValidBefore(ref long input) { }
                                      private void ValidAfter(ref long target) { }
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(MappaDiagnosticDescriptors.HookMethodNotFound, "Map", "before-map", "MissingBefore")
            .HaveDiagnostic(MappaDiagnosticDescriptors.HookMethodNotFound, "Map", "after-map", "MissingAfter")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(long).ToString(),
                typeof(long).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.ValidBefore", (SyntaxKind.RefKeyword, "input")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(long).ToString(),
                            "__mappa_tmp_1",
                            initialization => initialization.BeIdentifierNameSyntax("input")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.ValidAfter", (SyntaxKind.RefKeyword, "__mappa_tmp_1")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Test type and member lookup failures retain their established diagnostics.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task LookupFailuresRetainEstablishedDiagnostics()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class HookContainer
                                  {
                                      public static class NestedHooks
                                      {
                                          public static void Before(ref int input) { }
                                      }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaBeforeMap(typeof(HookContainer.NestedHooks), nameof(HookContainer.NestedHooks.Before))]
                                      [MappaAfterMap("missing", nameof(HookContainer.NestedHooks.Before))]
                                      public partial int Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotDetectType,
                $"{SourceNamespace}.HookContainer.NestedHooks")
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.CannotFindFieldOrProperty,
                "missing")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement("input"));
                });
    }

    /// <summary>
    /// Test static mapping restrictions report hook-specific and established member diagnostics.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task StaticMappingRestrictionsReportExpectedDiagnostics()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Hooks
                                  {
                                      public void Instance(ref int input) { }
                                      public void Instance(ref long input) { }
                                      public void Instance(ref string input) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private readonly Hooks hooks = new();

                                      [MappaBeforeMap(nameof(LocalInstance))]
                                      public static partial int MapLocal(int input);

                                      [MappaBeforeMap(nameof(hooks), nameof(Hooks.Instance))]
                                      public static partial long MapMember(long input);

                                      [MappaBeforeMap(typeof(Hooks), nameof(Hooks.Instance))]
                                      public partial string MapExplicit(string input);

                                      private void LocalInstance(ref int input) { }
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(3)
            .HaveDiagnostic(MappaDiagnosticDescriptors.HookMethodNotFound, "MapLocal", "before-map", "LocalInstance")
            .HaveDiagnostic(MappaDiagnosticDescriptors.FieldOrPropertyMustBeStatic, "hooks")
            .HaveDiagnostic(MappaDiagnosticDescriptors.HookMethodNotFound, "MapExplicit", "before-map", "Instance")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "MapLocal",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                false,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                "input",
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                3,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement("input"));
                })
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "MapMember",
                [SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword, SyntaxKind.PartialKeyword],
                false,
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                "input",
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                3,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement("input"));
                })
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "MapExplicit",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                "input",
                typeof(string).ToString(),
                NullableAnnotation.NotAnnotated,
                3,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement("input"));
                });
    }

    /// <summary>
    /// Test ambiguous winning-tier overloads retain MP00042 and omit the hook.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AmbiguousSameTierHookReportsMp00042()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaBeforeMap(nameof(Before))]
                                      public partial int Map(int input);

                                      private void Before(ref int input) { }
                                      private void Before<T>(ref int input) { }
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .ContainAmbiguousInvokeMethodResolutionDiagnostic("Before")
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(node => node.BeReturnStatement("input"));
                });
    }
}