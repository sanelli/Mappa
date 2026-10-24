// <copyright file="ReplaceNullWithEmptyContainerIntegrationTests.Variants.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Additional variants for <c>ReplaceNullWithEmptyContainer</c>.
/// </summary>
public sealed partial class ReplaceNullWithEmptyContainerIntegrationTests
{
    private const string SourceTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Source";
    private const string TargetTypeName = "Mappa.Generator.Tests.UnitTests.SourceCode.Target";
    private const string LambdaParameterName = "__mappa_tmp_1";

    /// <summary>
    /// Test a null dictionary source maps to <c>new Dictionary&lt;,&gt;()</c> when the setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullDictionaryWithEmptyDictionaryWhenSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial Dictionary<string, string> Map(Dictionary<int, int>? input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(Dictionary<string, string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<int, int>).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(Dictionary<string, string>).ToString(),
                        rightAssertions => rightAssertions.BeObjectCreationExpressionSyntax(typeof(Dictionary<string, string>).ToString()));
                });
    }

    /// <summary>
    /// Test a null source maps to <see cref="ImmutableList{T}.Empty"/> when the setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullWithImmutableListEmptyWhenSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using System.Collections.Immutable;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial ImmutableList<string> Map(List<int>? input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(ImmutableList<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(List<int>).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(ImmutableList<string>).ToString(),
                        rightAssertions => rightAssertions.BeMemberAccessExpressionSyntax(
                            "global::System.Collections.Immutable.ImmutableList<string>.Empty"));
                });
    }

    /// <summary>
    /// Test a null source maps to <see cref="ImmutableQueue{T}.Empty"/> when the setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullWithImmutableQueueEmptyWhenSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using System.Collections.Immutable;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial ImmutableQueue<string> Map(IEnumerable<int>? input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(ImmutableQueue<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(ImmutableQueue<string>).ToString(),
                        rightAssertions => rightAssertions.BeMemberAccessExpressionSyntax(
                            "global::System.Collections.Immutable.ImmutableQueue<string>.Empty"));
                });
    }

    /// <summary>
    /// Test a null <see cref="IImmutableList{T}"/> maps to <c>[]</c> when the setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullIImmutableListWithEmptyCollectionExpressionWhenSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using System.Collections.Immutable;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial IImmutableList<string> Map(IEnumerable<int>? input);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(IImmutableList<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(IEnumerable<int>).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(IImmutableList<string>).ToString(),
                        rightAssertions => rightAssertions.BeCollectionExpressionSyntax());
                });
    }

    /// <summary>
    /// Test null container replacement under <c>#nullable disable</c>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullListWithEmptyListWhenNullableDisabled()
    {
        const string sourceCode = """
                                  #nullable disable
                                  using System.Collections.Generic;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial List<string> Map(List<int> input);
                                  }
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                typeof(List<string>).ToString(),
                NullableAnnotation.None,
                typeof(List<int>).ToString(),
                NullableAnnotation.None,
                NullableSetup.Disable,
                AssertNullThenEmptyListThenMapped);
    }

    /// <summary>
    /// Test nested collection projection emits a null-to-empty ternary when the setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullNestedCollectionInQueryableProjectionWhenSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using System.Linq;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  public class Source
                                  {
                                      public List<int>? Values { get; set; }
                                  }

                                  public class Target
                                  {
                                      public List<string> Values { get; set; }
                                  }

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial IQueryable<Target> Map(IQueryable<Source> query);
                                  }
                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, TestContext.Current.CancellationToken).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveQueryableProjectionMapMethod(
                "Mapper",
                [SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword, SyntaxKind.PartialKeyword],
                "Map",
                [SyntaxKind.PublicKeyword, SyntaxKind.PartialKeyword],
                false,
                "query",
                SourceTypeName,
                TargetTypeName,
                LambdaParameterName,
                ElementExpressionAssertions);

        static void ElementExpressionAssertions(ExpressionSyntaxAssertions elementExpression)
        {
            elementExpression.BeObjectCreationExpressionSyntax(
                TargetTypeName,
                ("Values", ValuesAssertions));
        }

        static void ValuesAssertions(ExpressionSyntaxAssertions values)
        {
            values.BeConditionalExpressionSyntax(
                condition => condition.BeBinaryExpressionSyntax(
                    left => left.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Values"),
                    SyntaxKind.EqualsExpression,
                    right => right.BeLiteralExpressionSyntax(null)),
                whenTrue => whenTrue.BeObjectCreationExpressionSyntax(typeof(List<string>).ToString()),
                whenFalse => whenFalse.BeInvocationExpressionSyntax(
                    "global::System.Linq.Enumerable.ToList",
                    select => select.BeInvocationExpressionSyntax(
                        "global::System.Linq.Enumerable.Select",
                        source => source.BeMemberAccessExpressionSyntax($"{LambdaParameterName}.Values"),
                        lambda => lambda.BeSimpleLambdaExpressionSyntax(
                            "__mappa_tmp_2",
                            body => body.BeInvocationExpressionSyntax("__mappa_tmp_2.ToString")))));
        }
    }
}