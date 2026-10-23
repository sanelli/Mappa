// <copyright file="ValueTransformerIntegrationTests.Ordering.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Ordering integration tests for value transformers.
/// </summary>
public sealed partial class ValueTransformerIntegrationTests
{
    /// <summary>
    /// Test transformers are chained as apply attributes, local, base hierarchy (most derived first)
    /// and dependency transformers, on both the source and the returned value.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task OrdersTransformersAcrossApplyLocalBaseAndDependencyTiers()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Dependency
                                  {
                                      [MappaTransformer]
                                      public string DependencyStep(string value) => value;
                                  }

                                  public abstract class GrandBase
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      protected string GrandBaseStep(string value) => value;
                                  }

                                  public abstract class MapperBase : GrandBase
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      protected string BaseStep(string value) => value;
                                  }

                                  [Mappa]
                                  [MappaSettings(MappaTransformerFromDependency = BooleanSetting.Enable)]
                                  public sealed partial class Mapper : MapperBase
                                  {
                                      [MappaDependency]
                                      private readonly Dependency dependency = new();

                                      [MappaApplyTransformer(nameof(ApplyFirst))]
                                      [MappaApplyTransformer(nameof(ApplySecond))]
                                      public partial string Map(int input);

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string LocalFirst(string value) => value;

                                      [MappaIgnore]
                                      private string ApplyFirst(string value) => value;

                                      [MappaIgnore]
                                      private string ApplySecond(string value) => value;

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string LocalSecond(string value) => value;
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                typeof(int).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(9)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.ApplyFirst", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_3", "this.ApplySecond", "__mappa_tmp_2"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_4", "this.LocalFirst", "__mappa_tmp_3"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_5", "this.LocalSecond", "__mappa_tmp_4"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_6", "this.BaseStep", "__mappa_tmp_5"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_7", "this.GrandBaseStep", "__mappa_tmp_6"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_8", "this.dependency.DependencyStep", "__mappa_tmp_7"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_8"));
                });
    }

    /// <summary>
    /// Test a transformer matching more than one tier is applied only once per side.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AppliesTransformerRegisteredInMultipleTiersOnlyOncePerSide()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaApplyTransformer(nameof(Trim))]
                                      public partial string Map(string input);

                                      [MappaIgnore]
                                      [MappaTransformer]
                                      private string Trim(string value) => value.Trim();
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .NotHaveCompilationErrors()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .HaveDefaultMapMethod(
                typeof(string).ToString(),
                typeof(string).ToString(),
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_1", "this.Trim", "input"))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Trim", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }
}