// <copyright file="ValueTransformerIntegrationTests.Nesting.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Nested mapping sites integration tests for value transformers.
/// </summary>
public sealed partial class ValueTransformerIntegrationTests
{
    /// <summary>
    /// Test class transformers are not applied around a call to another map method generated for the same mapper,
    /// because the invoked method already applies them on its own root source and return value.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task SkipsClassTransformersAroundCallsToSelfGeneratedMapMethods()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Inner
                                  {
                                      public int A { get; set; }
                                  }

                                  public class InnerTarget
                                  {
                                      public int A { get; set; }
                                  }

                                  public class Source
                                  {
                                      public Inner Child { get; set; } = new();
                                  }

                                  public class Target
                                  {
                                      public InnerTarget Child { get; set; } = new();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private Inner FixInner(Inner value) => value;

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private InnerTarget FixInnerTarget(InnerTarget value) => value;

                                      public partial Target Map(Source input);

                                      public partial InnerTarget MapInner(Inner input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        var unit = generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit();

        AssertMapperMethod(
            unit,
            "Map",
            $"{SourceNamespace}.Target",
            $"{SourceNamespace}.Source",
            blockSyntaxAssertions =>
            {
                blockSyntaxAssertions
                    .HasSyntaxNodesCount(4)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax($"{SourceNamespace}.Inner", "__mappa_tmp_1", initialization => initialization.BeMemberAccessExpressionSyntax("input.Child")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        $"{SourceNamespace}.InnerTarget",
                        "__mappa_tmp_2",
                        initialization => initialization.BeInvocationExpressionSyntax("this.MapInner", expression => expression.BeIdentifierNameSyntax("__mappa_tmp_1"))))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        $"{SourceNamespace}.Target",
                        "__mappa_tmp_3",
                        initialization => initialization.BeObjectCreationExpressionSyntax(
                            $"{SourceNamespace}.Target",
                            ("Child", property => property.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_3"));
            });

        AssertMapperMethod(
            unit,
            "MapInner",
            $"{SourceNamespace}.InnerTarget",
            $"{SourceNamespace}.Inner",
            blockSyntaxAssertions =>
            {
                blockSyntaxAssertions
                    .HasSyntaxNodesCount(5)
                    .HasNextSyntaxNode(node => AssertInstanceTransformer(node, $"{SourceNamespace}.Inner", "__mappa_tmp_4", "this.FixInner", "input"))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_5", initialization => initialization.BeMemberAccessExpressionSyntax("__mappa_tmp_4.A")))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                        $"{SourceNamespace}.InnerTarget",
                        "__mappa_tmp_6",
                        initialization => initialization.BeObjectCreationExpressionSyntax(
                            $"{SourceNamespace}.InnerTarget",
                            ("A", property => property.BeIdentifierNameSyntax("__mappa_tmp_5")))))
                    .HasNextSyntaxNode(node => AssertInstanceTransformer(node, $"{SourceNamespace}.InnerTarget", "__mappa_tmp_7", "this.FixInnerTarget", "__mappa_tmp_6"))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_7"));
            },
            2);
    }

    /// <summary>
    /// Test a transformer applied by a nullable wrapper element strategy is not applied a second time
    /// by the nullable wrapper itself.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotApplyTheSameTransformerTwiceThroughNullableWrapper()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Inner
                                  {
                                      public int A { get; set; }
                                  }

                                  public class InnerTarget
                                  {
                                      public int A { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private Inner? FixInner(Inner? value) => value;

                                      public partial InnerTarget? Map(Inner? input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "Map",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                $"{SourceNamespace}.InnerTarget",
                NullableAnnotation.Annotated,
                "input",
                $"{SourceNamespace}.Inner",
                null,
                NullableAnnotation.Annotated,
                RefKind.None,
                false,
                RefKind.None,
                1,
                NullableSetup.Enable,
                PragmaWarning.NoBlock,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax($"{SourceNamespace}.InnerTarget?", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeIfStatementSyntax(
                            condition => condition.BeIsPatternExpressionSyntax(
                                expression => expression.BeIdentifierNameSyntax("input"),
                                pattern => pattern.BeUnaryPatternSyntax(SyntaxKind.NotKeyword, argument => argument.BeConstantPatternSyntax(null))),
                            thenStatement => thenStatement
                                .BeBlockStatement()
                                .AsBlock()
                                .HasSyntaxNodesCount(5)
                                .HasNextSyntaxNode(statement => statement.BeLocalDeclarationStatementSyntax($"{SourceNamespace}.Inner", "__mappa_tmp_2", initialization => initialization.BeIdentifierNameSyntax("input")))
                                .HasNextSyntaxNode(statement => AssertInstanceTransformer(statement, $"{SourceNamespace}.Inner", "__mappa_tmp_3", "this.FixInner", "__mappa_tmp_2"))
                                .HasNextSyntaxNode(statement => statement.BeLocalDeclarationStatementSyntax(typeof(int).ToString(), "__mappa_tmp_4", initialization => initialization.BeMemberAccessExpressionSyntax("__mappa_tmp_3.A")))
                                .HasNextSyntaxNode(statement => statement.BeLocalDeclarationStatementSyntax(
                                    $"{SourceNamespace}.InnerTarget?",
                                    "__mappa_tmp_5",
                                    initialization => initialization.BeObjectCreationExpressionSyntax(
                                        $"{SourceNamespace}.InnerTarget",
                                        ("A", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                                .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                    left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    right => right.BeIdentifierNameSyntax("__mappa_tmp_5"))),
                            elseStatement => elseStatement
                                .BeBlockStatement()
                                .AsBlock()
                                .HasSyntaxNodesCount(1)
                                .HasNextSyntaxNode(statement => statement.BeAssignmentExpressionStatement(
                                    left => left.BeIdentifierNameSyntax("__mappa_tmp_1"),
                                    right => right.BeCastExpressionSyntax($"{SourceNamespace}.InnerTarget?", expression => expression.BeLiteralExpressionSyntax(null))))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_1"));
                });
    }

    private static void AssertMapperMethod(
        CompilationUnitSyntaxAssertions unit,
        string methodName,
        string returnType,
        string parameterType,
        Action<BlockSyntaxAssertions> bodyAssertion,
        int expectedGeneratedCount = 2)
        => unit.HaveMapMethod(
            "Mapper",
            [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
            methodName,
            [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
            false,
            returnType,
            NullableAnnotation.NotAnnotated,
            "input",
            parameterType,
            null,
            NullableAnnotation.NotAnnotated,
            RefKind.None,
            false,
            RefKind.None,
            expectedGeneratedCount,
            NullableSetup.Enable,
            PragmaWarning.NoBlock,
            bodyAssertion);
}