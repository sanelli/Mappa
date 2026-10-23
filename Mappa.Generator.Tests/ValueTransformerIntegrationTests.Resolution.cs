// <copyright file="ValueTransformerIntegrationTests.Resolution.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Resolution integration tests for value transformers: accessors, overrides and inherited dependencies.
/// </summary>
public sealed partial class ValueTransformerIntegrationTests
{
    /// <summary>
    /// Test <c>FieldName</c> combined with a static transformer method of the field type invokes it through the declared type.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanApplyStaticTransformerOfFieldTypeUsingFieldName()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class Normalizer
                                  {
                                      public static string Normalize(string value) => value.Trim();
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      private readonly Normalizer normalizer = new();

                                      [MappaApplyTransformer(nameof(Normalizer.Normalize), FieldName = nameof(normalizer))]
                                      public partial string Map(int input);
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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", $"global::{SourceNamespace}.Normalizer.Normalize", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test a transformer overridden by the mapper is discovered once, through the most derived override.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DiscoversOverriddenTransformerOnlyOnce()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public abstract class MapperBase
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      protected virtual string Trim(string value) => value;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper : MapperBase
                                  {
                                      [MappaIgnore]
                                      [MappaTransformer]
                                      protected override string Trim(string value) => value;

                                      public partial string Map(int input);
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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Trim", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test <c>[MappaApplyTransformer]</c> resolves an overridden method once, replacing every method it overrides.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ApplyTransformerReplacesOverriddenMethodsWithTheirOverrides()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public abstract class MapperBase
                                  {
                                      [MappaIgnore]
                                      protected virtual string Normalize(string value) => value;
                                  }

                                  public abstract class MapperMiddle : MapperBase
                                  {
                                      [MappaIgnore]
                                      protected override string Normalize(string value) => value;
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper : MapperMiddle
                                  {
                                      [MappaIgnore]
                                      protected override string Normalize(string value) => value;

                                      [MappaApplyTransformer(nameof(Normalize))]
                                      public partial string Map(int input);
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
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                        .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.Normalize", "__mappa_tmp_1"))
                        .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
                });
    }

    /// <summary>
    /// Test dependency transformers declared by <c>[MappaDependency]</c> members of the base class are applied
    /// (properties first, then fields) to every map method whose site types match.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task AppliesInheritedMappaDependencyTransformersToEveryMapMethod()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public sealed class PropertyDependency
                                  {
                                      [MappaTransformer]
                                      public string PropertyStep(string value) => value;
                                  }

                                  public sealed class FieldDependency
                                  {
                                      [MappaTransformer]
                                      public long FieldStep(long value) => value;
                                  }

                                  public abstract class MapperBase
                                  {
                                      [MappaDependency]
                                      protected PropertyDependency BaseProperty { get; } = new();

                                      [MappaDependency]
                                      protected readonly FieldDependency baseField = new();
                                  }

                                  [Mappa]
                                  [MappaSettings(MappaTransformerFromDependency = BooleanSetting.Enable)]
                                  public sealed partial class Mapper : MapperBase
                                  {
                                      public partial string Map(int input);

                                      public partial string MapAgain(long input);
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
            typeof(string).ToString(),
            typeof(int).ToString(),
            blockSyntaxAssertions =>
            {
                blockSyntaxAssertions
                    .HasSyntaxNodesCount(3)
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_1", initialization => initialization.BeInvocationExpressionSyntax("input.ToString")))
                    .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_2", "this.BaseProperty.PropertyStep", "__mappa_tmp_1"))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_2"));
            });

        AssertMapperMethod(
            unit,
            "MapAgain",
            typeof(string).ToString(),
            typeof(long).ToString(),
            blockSyntaxAssertions =>
            {
                blockSyntaxAssertions
                    .HasSyntaxNodesCount(4)
                    .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(long).ToString(), "__mappa_tmp_3", "this.baseField.FieldStep", "input"))
                    .HasNextSyntaxNode(node => node.BeLocalDeclarationStatementSyntax(typeof(string).ToString(), "__mappa_tmp_4", initialization => initialization.BeInvocationExpressionSyntax("__mappa_tmp_3.ToString")))
                    .HasNextSyntaxNode(node => AssertInstanceTransformer(node, typeof(string).ToString(), "__mappa_tmp_5", "this.BaseProperty.PropertyStep", "__mappa_tmp_4"))
                    .HasNextSyntaxNode(node => node.BeReturnStatement("__mappa_tmp_5"));
            });
    }
}