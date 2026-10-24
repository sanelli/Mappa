// <copyright file="ReplaceNullWithEmptyContainerIntegrationTests.Coverage.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Collections.ObjectModel;

using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Extra empty-container shapes for coverage of <c>EmptyContainerExpressionHelper</c>.
/// </summary>
public sealed partial class ReplaceNullWithEmptyContainerIntegrationTests
{
    /// <summary>
    /// Test null-to-empty for remaining well-known immutable / frozen / readonly targets.
    /// </summary>
    /// <param name="targetTypeSource">Target type text written into the mapper method signature.</param>
    /// <param name="sourceTypeSource">Source type text written into the mapper method signature.</param>
    /// <param name="targetTypeAssert">Expected generated return type for <c>HaveDefaultMapMethod</c>.</param>
    /// <param name="sourceTypeAssert">Expected generated parameter type for <c>HaveDefaultMapMethod</c>.</param>
    /// <param name="emptyKind"><c>Collection</c> for <c>[]</c>, otherwise member-access <c>.Empty</c>.</param>
    /// <param name="emptyMemberAccess">Fully-qualified <c>.Empty</c> path when <paramref name="emptyKind"/> is not collection.</param>
    /// <returns>The async task.</returns>
    [Theory]
    [InlineData(
        "ImmutableArray<string>",
        "List<int>?",
        "System.Collections.Immutable.ImmutableArray`1[System.String]",
        "System.Collections.Generic.List`1[System.Int32]",
        "MemberAccess",
        "global::System.Collections.Immutable.ImmutableArray<string>.Empty")]
    [InlineData(
        "ImmutableHashSet<string>",
        "List<int>?",
        "System.Collections.Immutable.ImmutableHashSet`1[System.String]",
        "System.Collections.Generic.List`1[System.Int32]",
        "MemberAccess",
        "global::System.Collections.Immutable.ImmutableHashSet<string>.Empty")]
    [InlineData(
        "ImmutableSortedSet<string>",
        "List<int>?",
        "System.Collections.Immutable.ImmutableSortedSet`1[System.String]",
        "System.Collections.Generic.List`1[System.Int32]",
        "MemberAccess",
        "global::System.Collections.Immutable.ImmutableSortedSet<string>.Empty")]
    [InlineData(
        "ImmutableStack<string>",
        "IEnumerable<int>?",
        "System.Collections.Immutable.ImmutableStack`1[System.String]",
        "System.Collections.Generic.IEnumerable`1[System.Int32]",
        "MemberAccess",
        "global::System.Collections.Immutable.ImmutableStack<string>.Empty")]
    [InlineData(
        "IImmutableStack<string>",
        "IEnumerable<int>?",
        "System.Collections.Immutable.IImmutableStack`1[System.String]",
        "System.Collections.Generic.IEnumerable`1[System.Int32]",
        "MemberAccess",
        "global::System.Collections.Immutable.ImmutableStack<string>.Empty")]
    [InlineData(
        "ImmutableDictionary<string, string>",
        "Dictionary<int, int>?",
        "System.Collections.Immutable.ImmutableDictionary`2[System.String,System.String]",
        "System.Collections.Generic.Dictionary`2[System.Int32,System.Int32]",
        "MemberAccess",
        "global::System.Collections.Immutable.ImmutableDictionary<string, string>.Empty")]
    [InlineData(
        "IImmutableDictionary<string, string>",
        "Dictionary<int, int>?",
        "System.Collections.Immutable.IImmutableDictionary`2[System.String,System.String]",
        "System.Collections.Generic.Dictionary`2[System.Int32,System.Int32]",
        "MemberAccess",
        "global::System.Collections.Immutable.ImmutableDictionary<string, string>.Empty")]
    [InlineData(
        "ImmutableSortedDictionary<string, string>",
        "Dictionary<int, int>?",
        "System.Collections.Immutable.ImmutableSortedDictionary`2[System.String,System.String]",
        "System.Collections.Generic.Dictionary`2[System.Int32,System.Int32]",
        "MemberAccess",
        "global::System.Collections.Immutable.ImmutableSortedDictionary<string, string>.Empty")]
    [InlineData(
        "FrozenSet<string>",
        "List<int>?",
        "System.Collections.Frozen.FrozenSet`1[System.String]",
        "System.Collections.Generic.List`1[System.Int32]",
        "MemberAccess",
        "global::System.Collections.Frozen.FrozenSet<string>.Empty")]
    [InlineData(
        "FrozenDictionary<string, string>",
        "Dictionary<int, int>?",
        "System.Collections.Frozen.FrozenDictionary`2[System.String,System.String]",
        "System.Collections.Generic.Dictionary`2[System.Int32,System.Int32]",
        "MemberAccess",
        "global::System.Collections.Frozen.FrozenDictionary<string, string>.Empty")]
    [InlineData(
        "IList<string>",
        "IEnumerable<int>?",
        "System.Collections.Generic.IList`1[System.String]",
        "System.Collections.Generic.IEnumerable`1[System.Int32]",
        "Collection",
        "")]
    [InlineData(
        "ICollection<string>",
        "IEnumerable<int>?",
        "System.Collections.Generic.ICollection`1[System.String]",
        "System.Collections.Generic.IEnumerable`1[System.Int32]",
        "Collection",
        "")]
    [InlineData(
        "ISet<string>",
        "IEnumerable<int>?",
        "System.Collections.Generic.ISet`1[System.String]",
        "System.Collections.Generic.IEnumerable`1[System.Int32]",
        "Collection",
        "")]
    [InlineData(
        "IReadOnlySet<string>",
        "IEnumerable<int>?",
        "System.Collections.Generic.IReadOnlySet`1[System.String]",
        "System.Collections.Generic.IEnumerable`1[System.Int32]",
        "Collection",
        "")]
    [InlineData(
        "IImmutableSet<string>",
        "IEnumerable<int>?",
        "System.Collections.Immutable.IImmutableSet`1[System.String]",
        "System.Collections.Generic.IEnumerable`1[System.Int32]",
        "Collection",
        "")]
    [InlineData(
        "IDictionary<string, string>",
        "Dictionary<int, int>?",
        "System.Collections.Generic.IDictionary`2[System.String,System.String]",
        "System.Collections.Generic.Dictionary`2[System.Int32,System.Int32]",
        "Collection",
        "")]
    [InlineData(
        "IReadOnlyDictionary<string, string>",
        "Dictionary<int, int>?",
        "System.Collections.Generic.IReadOnlyDictionary`2[System.String,System.String]",
        "System.Collections.Generic.Dictionary`2[System.Int32,System.Int32]",
        "Collection",
        "")]
    [IntegrationTest]
    public async Task CanReplaceNullWithEmptyForWellKnownAndInterfaceTargets(
        string targetTypeSource,
        string sourceTypeSource,
        string targetTypeAssert,
        string sourceTypeAssert,
        string emptyKind,
        string emptyMemberAccess)
    {
        var usings = """
                     using System.Collections.Generic;
                     using System.Collections.Immutable;
                     using System.Collections.Frozen;
                     using System.Collections.ObjectModel;
                     using Mappa.Attributes;
                     """;

        var sourceCode = $$"""
                           #nullable enable
                           {{usings}}

                           namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                           [Mappa]
                           public sealed partial class Mapper
                           {
                               [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                               public partial {{targetTypeSource}} Map({{sourceTypeSource}} input);
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
                targetTypeAssert,
                NullableAnnotation.NotAnnotated,
                sourceTypeAssert,
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        targetTypeAssert,
                        rightAssertions =>
                        {
                            if (emptyKind == "Collection")
                            {
                                rightAssertions.BeCollectionExpressionSyntax();
                            }
                            else
                            {
                                rightAssertions.BeMemberAccessExpressionSyntax(emptyMemberAccess);
                            }
                        });
                });
    }

    /// <summary>
    /// Test <see cref="ReadOnlyCollection{T}"/> empty uses Array.Empty wrapper construction.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullWithEmptyReadOnlyCollectionWhenSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using System.Collections.ObjectModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial ReadOnlyCollection<string> Map(List<int>? input);
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
                typeof(ReadOnlyCollection<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(List<int>).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(ReadOnlyCollection<string>).ToString(),
                        rightAssertions => rightAssertions.BeObjectCreationExpressionSyntax(
                            typeof(ReadOnlyCollection<string>).ToString(),
                            arg => arg.BeInvocationExpressionSyntax("global::System.Array.Empty<string>")));
                });
    }

    /// <summary>
    /// Test <see cref="ReadOnlySet{T}"/> empty uses ImmutableHashSet.Empty wrapper construction.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullWithEmptyReadOnlySetWhenSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using System.Collections.ObjectModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial ReadOnlySet<string> Map(List<int>? input);
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
                typeof(ReadOnlySet<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(List<int>).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(ReadOnlySet<string>).ToString(),
                        rightAssertions => rightAssertions.BeObjectCreationExpressionSyntax(
                            typeof(ReadOnlySet<string>).ToString(),
                            arg => arg.BeMemberAccessExpressionSyntax(
                                "global::System.Collections.Immutable.ImmutableHashSet<string>.Empty")));
                });
    }

    /// <summary>
    /// Test <see cref="ReadOnlyDictionary{TKey,TValue}"/> empty uses an empty Dictionary wrapper.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullWithEmptyReadOnlyDictionaryWhenSettingEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Generic;
                                  using System.Collections.ObjectModel;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial ReadOnlyDictionary<string, string> Map(Dictionary<int, int>? input);
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
                typeof(ReadOnlyDictionary<string, string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Dictionary<int, int>).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(ReadOnlyDictionary<string, string>).ToString(),
                        rightAssertions => rightAssertions.BeObjectCreationExpressionSyntax(
                            typeof(ReadOnlyDictionary<string, string>).ToString(),
                            arg => arg.BeObjectCreationExpressionSyntax(typeof(Dictionary<string, string>).ToString())));
                });
    }

    /// <summary>
    /// Test <see cref="IProducerConsumerCollection{T}"/> targets are excluded from empty substitution.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task DoesNotReplaceNullIProducerConsumerCollectionBecauseItIsExcluded()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using System.Collections.Concurrent;
                                  using System.Collections.Generic;
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable)]
                                      public partial IProducerConsumerCollection<string> Map(List<int>? input);
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
                typeof(IProducerConsumerCollection<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(List<int>).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                typeof(IProducerConsumerCollection<string>).ToString(),
                                "__mappa_tmp_1");
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeIfStatementSyntax(
                                conditionAssertions =>
                                {
                                    conditionAssertions.BeIsPatternExpressionSyntax(
                                        expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                        patternAssertions =>
                                        {
                                            patternAssertions.BeUnaryPatternSyntax(
                                                SyntaxKind.NotKeyword,
                                                unaryPatternSyntax => unaryPatternSyntax.BeConstantPatternSyntax(null));
                                        });
                                },
                                thenBranchAssertions =>
                                {
                                    thenBranchAssertions.BeBlockStatement();
                                },
                                elseStatementAssertions =>
                                {
                                    elseStatementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(statementAssertions =>
                                        {
                                            statementAssertions.BeThrowStatementSyntax<NullReferenceException>(
                                                expressionAssertions => expressionAssertions.BeLiteralExpressionSyntax("\"input\" is null."));
                                        });
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeReturnStatement(assertion => assertion.BeIdentifierNameSyntax("__mappa_tmp_1"));
                        });
                });
    }

    /// <summary>
    /// Test a null <see cref="Queue{T}"/> source maps to <c>new Queue&lt;T&gt;()</c> when the setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullQueueWithEmptyQueueWhenSettingEnabled()
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
                                      public partial Queue<string> Map(Queue<int>? input);
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
                typeof(Queue<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Queue<int>).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(Queue<string>).ToString(),
                        rightAssertions => rightAssertions.BeObjectCreationExpressionSyntax(typeof(Queue<string>).ToString()));
                });
    }

    /// <summary>
    /// Test a null <see cref="Stack{T}"/> source maps to <c>new Stack&lt;T&gt;()</c> when the setting is enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullStackWithEmptyStackWhenSettingEnabled()
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
                                      public partial Stack<string> Map(Stack<int>? input);
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
                typeof(Stack<string>).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(Stack<int>).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(Stack<string>).ToString(),
                        rightAssertions => rightAssertions.BeObjectCreationExpressionSyntax(typeof(Stack<string>).ToString()));
                });
    }

    /// <summary>
    /// Test null-to-empty for arrays still applies when <c>FastCollections</c> is also enabled.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task CanReplaceNullArrayWithEmptyWhenFastCollectionsIsAlsoEnabled()
    {
        const string sourceCode = """
                                  #nullable enable
                                  using Mappa.Attributes;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSettings(ReplaceNullWithEmptyContainer = BooleanSetting.Enable, FastCollections = BooleanSetting.Enable)]
                                      public partial string[] Map(int[]? input);
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
                typeof(string[]).ToString(),
                NullableAnnotation.NotAnnotated,
                typeof(int[]).ToString(),
                NullableAnnotation.Annotated,
                blockSyntaxAssertions =>
                {
                    AssertNullThenEmptyThenMapped(
                        blockSyntaxAssertions,
                        typeof(string[]).ToString(),
                        rightAssertions => rightAssertions.BeCollectionExpressionSyntax());
                });
    }
}