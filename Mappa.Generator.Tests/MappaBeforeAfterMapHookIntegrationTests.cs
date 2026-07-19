// <copyright file="MappaBeforeAfterMapHookIntegrationTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Integration tests for before-map and after-map hooks.
/// </summary>
public sealed partial class MappaBeforeAfterMapHookIntegrationTests
    : MappaGeneratorAbstractUnitTests
{
    private const string SourceNamespace = "Mappa.Generator.Tests.UnitTests.SourceCode";

    /// <summary>
    /// Test all supported hook signatures and phase-specific scope ordering.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanInvokeAllSupportedSignaturesInScopeAndDeclarationOrder()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaBeforeMap(nameof(ClassBeforeNone))]
                                  [MappaBeforeMap(nameof(ClassBeforeContext))]
                                  [MappaBeforeMap(nameof(ClassBeforeRef))]
                                  [MappaBeforeMap(nameof(ClassBeforeRefContext))]
                                  [MappaAfterMap(nameof(ClassAfterNone))]
                                  [MappaAfterMap(nameof(ClassAfterContext))]
                                  [MappaAfterMap(nameof(ClassAfterRef))]
                                  [MappaAfterMap(nameof(ClassAfterRefContext))]
                                  public sealed partial class Mapper
                                  {
                                      [MappaBeforeMap(nameof(MethodBeforeNone))]
                                      [MappaBeforeMap(nameof(MethodBeforeContext))]
                                      [MappaBeforeMap(nameof(MethodBeforeRef))]
                                      [MappaBeforeMap(nameof(MethodBeforeRefContext))]
                                      [MappaAfterMap(nameof(MethodAfterNone))]
                                      [MappaAfterMap(nameof(MethodAfterContext))]
                                      [MappaAfterMap(nameof(MethodAfterRef))]
                                      [MappaAfterMap(nameof(MethodAfterRefContext))]
                                      public partial int Map(int input, MappaContext context);

                                      private void ClassBeforeNone() { }
                                      private void ClassBeforeContext(MappaContext context) { }
                                      private void ClassBeforeRef(ref int input) { }
                                      private void ClassBeforeRefContext(ref int input, MappaContext context) { }
                                      private void MethodBeforeNone() { }
                                      private void MethodBeforeContext(MappaContext context) { }
                                      private void MethodBeforeRef(ref int input) { }
                                      private void MethodBeforeRefContext(ref int input, MappaContext context) { }
                                      private void ClassAfterNone() { }
                                      private void ClassAfterContext(MappaContext context) { }
                                      private void ClassAfterRef(ref int target) { }
                                      private void ClassAfterRefContext(ref int target, MappaContext context) { }
                                      private void MethodAfterNone() { }
                                      private void MethodAfterContext(MappaContext context) { }
                                      private void MethodAfterRef(ref int target) { }
                                      private void MethodAfterRefContext(ref int target, MappaContext context) { }
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasSyntaxNodesCount(18)
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.ClassBeforeNone"))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.ClassBeforeContext", (SyntaxKind.None, "context")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.ClassBeforeRef", (SyntaxKind.RefKeyword, "input")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.ClassBeforeRefContext", (SyntaxKind.RefKeyword, "input"), (SyntaxKind.None, "context")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.MethodBeforeNone"))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.MethodBeforeContext", (SyntaxKind.None, "context")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.MethodBeforeRef", (SyntaxKind.RefKeyword, "input")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.MethodBeforeRefContext", (SyntaxKind.RefKeyword, "input"), (SyntaxKind.None, "context")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_1",
                            initialization => initialization.BeIdentifierNameSyntax("input")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.MethodAfterNone"))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.MethodAfterContext", (SyntaxKind.None, "context")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.MethodAfterRef", (SyntaxKind.RefKeyword, "__mappa_tmp_1")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.MethodAfterRefContext", (SyntaxKind.RefKeyword, "__mappa_tmp_1"), (SyntaxKind.None, "context")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.ClassAfterNone"))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.ClassAfterContext", (SyntaxKind.None, "context")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.ClassAfterRef", (SyntaxKind.RefKeyword, "__mappa_tmp_1")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.ClassAfterRefContext", (SyntaxKind.RefKeyword, "__mappa_tmp_1"), (SyntaxKind.None, "context")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Test an <c>in</c> source is copied before it is passed to a ref hook.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanCopyInSourceBeforeInvokingRefHook()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public static partial class Mapper
                                  {
                                      [MappaBeforeMap(nameof(Before))]
                                      public static partial int Map(in int input);

                                      private static void Before(ref int input) { }
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveMapMethod(
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int).ToString(),
                NullableAnnotation.NotAnnotated,
                RefKind.In,
                false,
                null,
                RefKind.None,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_1",
                            initialization => initialization.BeIdentifierNameSyntax("input")))
                        .HasNextSyntaxNode(node => AssertStaticHook(node, "Before", (SyntaxKind.RefKeyword, "__mappa_tmp_1")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Test explicit types, fields, properties, inheritance, static members, and interface members.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanResolveHooksFromAllSupportedLocations()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public static class ExternalHooks
                                  {
                                      public static void Explicit(ref int input) { }
                                  }

                                  public interface IHooks
                                  {
                                      void InterfaceHook(ref int input);
                                  }

                                  public class BaseHooks
                                  {
                                      public void Inherited(ref int input) { }
                                  }

                                  public sealed class Hooks : BaseHooks, IHooks
                                  {
                                      public static void StaticMember(ref int input) { }
                                      public void InterfaceHook(ref int input) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private readonly Hooks field = new();
                                      private static readonly Hooks StaticHooks = new();
                                      private Hooks Property { get; } = new();
                                      private IHooks InterfaceHooks { get; } = new Hooks();

                                      [MappaBeforeMap(typeof(ExternalHooks), nameof(ExternalHooks.Explicit))]
                                      [MappaBeforeMap(nameof(field), nameof(BaseHooks.Inherited))]
                                      [MappaBeforeMap(nameof(Property), nameof(Hooks.StaticMember))]
                                      [MappaBeforeMap(nameof(InterfaceHooks), nameof(IHooks.InterfaceHook))]
                                      [MappaBeforeMap(nameof(StaticHooks), nameof(IHooks.InterfaceHook))]
                                      public partial int Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(int).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(6)
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, $"global::{SourceNamespace}.ExternalHooks.Explicit", (SyntaxKind.RefKeyword, "input")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.field.Inherited", (SyntaxKind.RefKeyword, "input")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, $"global::{SourceNamespace}.Hooks.StaticMember", (SyntaxKind.RefKeyword, "input")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.InterfaceHooks.InterfaceHook", (SyntaxKind.RefKeyword, "input")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "StaticHooks.InterfaceHook", (SyntaxKind.RefKeyword, "input")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("input"));
                });
    }

    /// <summary>
    /// Test hooks declared on a base interface resolve through a derived interface member.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanResolveHooksDeclaredOnInheritedInterface()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public interface IBaseHooks
                                  {
                                      void Before(ref int input);
                                      void After(ref int target);
                                  }

                                  public interface IDerivedHooks : IBaseHooks
                                  {
                                  }

                                  public sealed class Hooks : IDerivedHooks
                                  {
                                      public void Before(ref int input) { }
                                      public void After(ref int target) { }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private IDerivedHooks Hooks { get; } = new Hooks();

                                      [MappaBeforeMap(nameof(Hooks), nameof(IBaseHooks.Before))]
                                      [MappaAfterMap(nameof(Hooks), nameof(IBaseHooks.After))]
                                      public partial int Map(int input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
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
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.Hooks.Before", (SyntaxKind.RefKeyword, "input")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(int).ToString(),
                            "__mappa_tmp_1",
                            initialization => initialization.BeIdentifierNameSyntax("input")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.Hooks.After", (SyntaxKind.RefKeyword, "__mappa_tmp_1")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    /// <summary>
    /// Test class-level hooks select overloads independently for each mapping method type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanResolveClassHooksIndependentlyForEachMappingMethodType()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  [MappaBeforeMap(nameof(Before))]
                                  [MappaAfterMap(nameof(After))]
                                  public sealed partial class Mapper
                                  {
                                      public partial int MapInt(int input);
                                      public partial string MapString(string input);

                                      private void Before(ref int input) { }
                                      private void Before(ref string input) { }
                                      private void After(ref int target) { }
                                      private void After(ref string target) { }
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var compilationUnitAssertions = generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        AssertTypedClassHookMethod(
            compilationUnitAssertions,
            "MapInt",
            typeof(int).ToString(),
            NullableAnnotation.NotAnnotated,
            "__mappa_tmp_1");
        AssertTypedClassHookMethod(
            compilationUnitAssertions,
            "MapString",
            typeof(string).ToString(),
            NullableAnnotation.NotAnnotated,
            "__mappa_tmp_2");
    }

    /// <summary>
    /// Test after hooks materialize the result of a nullable root strategy.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanMaterializeNullableRootBeforeAfterHook()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaAfterMap(nameof(After))]
                                      public partial long Map(int? input);

                                      private void After(ref long target) { }
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(long).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int?).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(5)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(long).ToString(), "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeIfStatementSyntax(
                            condition => condition.BeMemberAccessExpressionSyntax("input.HasValue"),
                            thenStatement => thenStatement
                                .BeBlockStatement()
                                .AsBlock()
                                .HasSyntaxNodesCount(2)
                                .HasNextSyntaxNode(statement => statement.BeLocalDeclarationStatementSyntax(
                                    typeof(int).ToString(),
                                    "__mappa_tmp_2",
                                    initialization => initialization.BeMemberAccessExpressionSyntax("input.Value")))
                                .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                    left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    right => right.BeIdentifierNameSyntax("__mappa_tmp_2"))),
                            elseStatement => elseStatement
                                .BeBlockStatement()
                                .AsBlock()
                                .HasSyntaxNodesCount(1)
                                .HasNextSyntaxNode(statement => statement.BeThrowStatementSyntax<NullReferenceException>(
                                    argument => argument.BeLiteralExpressionSyntax("\"input\" is null.")))))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            typeof(long).ToString(),
                            "__mappa_tmp_3",
                            initialization => initialization.BeIdentifierNameSyntax("__mappa_tmp_1")))
                        .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.After", (SyntaxKind.RefKeyword, "__mappa_tmp_3")))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
                });
    }

    private static void AssertTypedClassHookMethod(
        CompilationUnitSyntaxAssertions compilationUnitAssertions,
        string methodName,
        string typeName,
        NullableAnnotation nullableAnnotation,
        string targetTemporaryName)
    {
        compilationUnitAssertions.HaveMapMethod(
            "Mapper",
            [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
            methodName,
            [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
            false,
            typeName,
            nullableAnnotation,
            "input",
            typeName,
            null,
            nullableAnnotation,
            RefKind.None,
            false,
            RefKind.None,
            2,
            NullableSetup.Enable,
            PragmaWarning.NoBlock,
            blockSyntaxAssertions =>
            {
                blockSyntaxAssertions
                    .HasSyntaxNodesCount(4)
                    .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.Before", (SyntaxKind.RefKeyword, "input")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        typeName,
                        targetTemporaryName,
                        initialization => initialization.BeIdentifierNameSyntax("input")))
                    .HasNextSyntaxNode(node => AssertInstanceHook(node, "this.After", (SyntaxKind.RefKeyword, targetTemporaryName)))
                    .HasNextSyntaxNode(node => node.BeReturnStatement(targetTemporaryName));
            });
    }

    private static void AssertInstanceHook(
        SyntaxNodeAssertions nodeAssertions,
        string accessIdentifier,
        params (SyntaxKind RefKind, string Identifier)[] arguments)
    {
        nodeAssertions.BeInvocationExpressionSyntaxStatementWithArguments(
            accessIdentifier,
            [.. arguments.Select(argument =>
                (argument.RefKind, (Action<ExpressionSyntaxAssertions>)(expression => expression.BeIdentifierNameSyntax(argument.Identifier))))]);
    }

    private static void AssertStaticHook(
        SyntaxNodeAssertions nodeAssertions,
        string methodName,
        params (SyntaxKind RefKind, string Identifier)[] arguments)
    {
        nodeAssertions.BeInvocationExpressionUsingIdentifierNameSyntaxStatement(
            methodName,
            [.. arguments.Select(argument =>
                (argument.RefKind, (Action<ExpressionSyntaxAssertions>)(expression => expression.BeIdentifierNameSyntax(argument.Identifier))))]);
    }
}