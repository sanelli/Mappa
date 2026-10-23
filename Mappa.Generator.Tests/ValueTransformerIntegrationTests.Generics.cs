// <copyright file="ValueTransformerIntegrationTests.Generics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Open generic integration tests for value transformers.
/// </summary>
public sealed partial class ValueTransformerIntegrationTests
{
    /// <summary>
    /// Test an open generic array transformer is applied only to the sites whose element type satisfies its constraints.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyOpenGenericArrayTransformerOnlyWhenConstraintsAreSatisfied()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string[] Names { get; set; } = [];
                                      public int[] Numbers { get; set; } = [];
                                  }

                                  public class Target
                                  {
                                      public string[] Names { get; set; } = [];
                                      public int[] Numbers { get; set; } = [];
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private T[] OnlyReferences<T>(T[] values) where T : class => values;

                                      public partial Target Map(Source input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                $"{SourceNamespace}.Target",
                $"{SourceNamespace}.Source",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(6)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string[]).ToString(), "__mappa_tmp_1", initialization => initialization.BeMemberAccessExpressionSyntax("input.Names")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string[]).ToString(), "__mappa_tmp_2", "this.OnlyReferences", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string[]).ToString(), "__mappa_tmp_3", "this.OnlyReferences", "__mappa_tmp_2"))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(int[]).ToString(), "__mappa_tmp_4", initialization => initialization.BeMemberAccessExpressionSyntax("input.Numbers")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.Target",
                            "__mappa_tmp_5",
                            initialization => initialization.BeObjectCreationExpressionSyntax(
                                $"{SourceNamespace}.Target",
                                ("Names", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                ("Numbers", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_5"));
                });
    }

    /// <summary>
    /// Test a <c>T?</c> open generic transformer is applied only to nullable reference sites.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyNullableOpenGenericTransformerOnlyToNullableReferenceSites()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public string? Nick { get; set; }
                                      public string Name { get; set; } = string.Empty;
                                  }

                                  public class Target
                                  {
                                      public string? Nick { get; set; }
                                      public string Name { get; set; } = string.Empty;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private T? Fallback<T>(T? value) where T : class => value;

                                      public partial Target Map(Source input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                $"{SourceNamespace}.Target",
                $"{SourceNamespace}.Source",
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(6)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax($"{typeof(string)}?", "__mappa_tmp_1", initialization => initialization.BeMemberAccessExpressionSyntax("input.Nick")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Fallback", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_3", "this.Fallback", "__mappa_tmp_2"))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_4", initialization => initialization.BeMemberAccessExpressionSyntax("input.Name")))
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(
                            $"{SourceNamespace}.Target",
                            "__mappa_tmp_5",
                            initialization => initialization.BeObjectCreationExpressionSyntax(
                                $"{SourceNamespace}.Target",
                                ("Nick", property => property.BeIdentifierNameSyntax("__mappa_tmp_3")),
                                ("Name", property => property.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_5"));
                });
    }
}