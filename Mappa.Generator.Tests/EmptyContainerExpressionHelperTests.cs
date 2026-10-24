// <copyright file="EmptyContainerExpressionHelperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="EmptyContainerExpressionHelper"/>.
/// </summary>
public sealed class EmptyContainerExpressionHelperTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="EmptyContainerExpressionHelper.IsEnabled"/> only accepts <see cref="BooleanSetting.Enable"/>.
    /// </summary>
    /// <param name="setting">The setting value.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [InlineData(BooleanSetting.Enable, true)]
    [InlineData(BooleanSetting.Disable, false)]
    [InlineData(BooleanSetting.Undefined, false)]
    [UnitTest]
    public void IsEnabledOnlyForEnable(BooleanSetting setting, bool expected)
    {
        EmptyContainerExpressionHelper.IsEnabled(setting).Should().Be(expected);
    }

    /// <summary>
    /// Test well-known empty expressions and allow-listed <c>[]</c> shapes.
    /// </summary>
    /// <param name="metadataName">The target type metadata name.</param>
    /// <param name="typeArgumentCount">Number of type arguments to construct.</param>
    /// <param name="expectedEmpty">The expected empty expression.</param>
    [Theory]
    [InlineData("System.Collections.Immutable.ImmutableArray`1", 1, "global::System.Collections.Immutable.ImmutableArray<int>.Empty")]
    [InlineData("System.Collections.Immutable.ImmutableStack`1", 1, "global::System.Collections.Immutable.ImmutableStack<int>.Empty")]
    [InlineData("System.Collections.Immutable.IImmutableStack`1", 1, "global::System.Collections.Immutable.ImmutableStack<int>.Empty")]
    [InlineData("System.Collections.Immutable.ImmutableQueue`1", 1, "global::System.Collections.Immutable.ImmutableQueue<int>.Empty")]
    [InlineData("System.Collections.Immutable.IImmutableQueue`1", 1, "global::System.Collections.Immutable.ImmutableQueue<int>.Empty")]
    [InlineData("System.Collections.Immutable.ImmutableList`1", 1, "global::System.Collections.Immutable.ImmutableList<int>.Empty")]
    [InlineData("System.Collections.Immutable.ImmutableHashSet`1", 1, "global::System.Collections.Immutable.ImmutableHashSet<int>.Empty")]
    [InlineData("System.Collections.Immutable.ImmutableSortedSet`1", 1, "global::System.Collections.Immutable.ImmutableSortedSet<int>.Empty")]
    [InlineData("System.Collections.Immutable.ImmutableDictionary`2", 2, "global::System.Collections.Immutable.ImmutableDictionary<int, int>.Empty")]
    [InlineData("System.Collections.Immutable.IImmutableDictionary`2", 2, "global::System.Collections.Immutable.ImmutableDictionary<int, int>.Empty")]
    [InlineData("System.Collections.Immutable.ImmutableSortedDictionary`2", 2, "global::System.Collections.Immutable.ImmutableSortedDictionary<int, int>.Empty")]
    [InlineData("System.Collections.Frozen.FrozenSet`1", 1, "global::System.Collections.Frozen.FrozenSet<int>.Empty")]
    [InlineData("System.Collections.Frozen.FrozenDictionary`2", 2, "global::System.Collections.Frozen.FrozenDictionary<int, int>.Empty")]
    [InlineData("System.Collections.ObjectModel.ReadOnlyCollection`1", 1, "new global::System.Collections.ObjectModel.ReadOnlyCollection<int>(global::System.Array.Empty<int>())")]
    [InlineData("System.Collections.ObjectModel.ReadOnlySet`1", 1, "new global::System.Collections.ObjectModel.ReadOnlySet<int>(global::System.Collections.Immutable.ImmutableHashSet<int>.Empty)")]
    [InlineData("System.Collections.ObjectModel.ReadOnlyDictionary`2", 2, "new global::System.Collections.ObjectModel.ReadOnlyDictionary<int, int>(new global::System.Collections.Generic.Dictionary<int, int>())")]
    [InlineData("System.Collections.Generic.IList`1", 1, "[]")]
    [InlineData("System.Collections.Generic.ICollection`1", 1, "[]")]
    [InlineData("System.Collections.Generic.IEnumerable`1", 1, "[]")]
    [InlineData("System.Collections.Generic.IReadOnlyList`1", 1, "[]")]
    [InlineData("System.Collections.Generic.IReadOnlyCollection`1", 1, "[]")]
    [InlineData("System.Collections.Generic.ISet`1", 1, "[]")]
    [InlineData("System.Collections.Generic.IReadOnlySet`1", 1, "[]")]
    [InlineData("System.Collections.Immutable.IImmutableList`1", 1, "[]")]
    [InlineData("System.Collections.Immutable.IImmutableSet`1", 1, "[]")]
    [InlineData("System.Collections.Generic.IDictionary`2", 2, "[]")]
    [InlineData("System.Collections.Generic.IReadOnlyDictionary`2", 2, "[]")]
    [InlineData("System.Collections.Generic.List`1", 1, "new System.Collections.Generic.List<int>()")]
    [InlineData("System.Collections.Generic.HashSet`1", 1, "new System.Collections.Generic.HashSet<int>()")]
    [InlineData("System.Collections.Concurrent.ConcurrentBag`1", 1, "new System.Collections.Concurrent.ConcurrentBag<int>()")]
    [InlineData("System.Collections.Concurrent.IProducerConsumerCollection`1", 1, null)]
    [InlineData("System.String", 0, null)]
    [UnitTest]
    public void TryGetEmptyExpressionResolvesOrRejectsTargetTypes(
        string metadataName,
        int typeArgumentCount,
        string? expectedEmpty)
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var targetType = GetConstructedType(compilation, metadataName, typeArgumentCount);

        var success = EmptyContainerExpressionHelper.TryGetEmptyExpression(
            targetType,
            compilation,
            mapMethodSymbol: null,
            out var emptyExpression);

        if (expectedEmpty is null)
        {
            success.Should().BeFalse();
            emptyExpression.Should().BeEmpty();
        }
        else
        {
            success.Should().BeTrue();
            emptyExpression.Should().Be(expectedEmpty);
        }
    }

    /// <summary>
    /// Test Span / Memory family types are excluded from empty-container substitution.
    /// </summary>
    /// <param name="metadataName">The excluded type metadata name.</param>
    [Theory]
    [InlineData("System.Span`1")]
    [InlineData("System.ReadOnlySpan`1")]
    [InlineData("System.Memory`1")]
    [InlineData("System.ReadOnlyMemory`1")]
    [UnitTest]
    public void TryGetEmptyExpressionRejectsSpanAndMemoryFamily(string metadataName)
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var targetType = GetConstructedType(compilation, metadataName, typeArgumentCount: 1);

        var success = EmptyContainerExpressionHelper.TryGetEmptyExpression(
            targetType,
            compilation,
            mapMethodSymbol: null,
            out var emptyExpression);

        success.Should().BeFalse();
        emptyExpression.Should().BeEmpty();
    }

    /// <summary>
    /// Test <see cref="EmptyContainerExpressionHelper.ShouldReplaceNullWithEmpty"/> for collection, dictionary, and other strategies.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ShouldReplaceNullWithEmptyDependsOnStrategyAndSetting()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var listType = GetConstructedType(compilation, "System.Collections.Generic.List`1", 1);
        var dictionaryType = GetConstructedType(compilation, "System.Collections.Generic.Dictionary`2", 2);
        var elementStrategy = new IdentityMapStrategy(listType, listType);

        var enabledCollection = new CollectionToCollectionMapStrategy(
            listType,
            listType,
            elementStrategy,
            methodSymbol: null,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            EnumerableConcreteTypeSetting.Undefined,
            BooleanSetting.Enable);
        EmptyContainerExpressionHelper.ShouldReplaceNullWithEmpty(enabledCollection, compilation, null).Should().BeTrue();

        var disabledCollection = new CollectionToCollectionMapStrategy(
            listType,
            listType,
            elementStrategy,
            methodSymbol: null,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            EnumerableConcreteTypeSetting.Undefined,
            BooleanSetting.Disable);
        EmptyContainerExpressionHelper.ShouldReplaceNullWithEmpty(disabledCollection, compilation, null).Should().BeFalse();

        var enabledDictionary = new DictionaryToDictionaryMapStrategy(
            dictionaryType,
            dictionaryType,
            elementStrategy,
            elementStrategy,
            DictionaryAssignmentSetting.Undefined,
            BooleanSetting.Enable);
        EmptyContainerExpressionHelper.ShouldReplaceNullWithEmpty(enabledDictionary, compilation, null).Should().BeTrue();

        var disabledDictionary = new DictionaryToDictionaryMapStrategy(
            dictionaryType,
            dictionaryType,
            elementStrategy,
            elementStrategy,
            DictionaryAssignmentSetting.Undefined,
            BooleanSetting.Disable);
        EmptyContainerExpressionHelper.ShouldReplaceNullWithEmpty(disabledDictionary, compilation, null).Should().BeFalse();

        EmptyContainerExpressionHelper.ShouldReplaceNullWithEmpty(elementStrategy, compilation, null).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="EmptyContainerExpressionHelper.ShouldReplaceNullWithEmpty"/> is false when enabled
    /// but the target has no empty expression.
    /// </summary>
    [Fact]
    [UnitTest]
    public void ShouldReplaceNullWithEmptyIsFalseWhenEnabledButEmptyExpressionUnavailable()
    {
        var compilation = BuildCompilation(
            """
            namespace Mappa.Generator.Tests.UnitTests.SourceCode
            {
                internal sealed class NoEmptyCtorDict : System.Collections.Generic.Dictionary<int, int>
                {
                    public NoEmptyCtorDict(int capacity)
                        : base(capacity)
                    {
                    }
                }
            }
            """);
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.NoEmptyCtorDict");
        targetType.Should().NotBeNull();
        var sourceType = GetConstructedType(compilation, "System.Collections.Generic.Dictionary`2", 2);
        var elementStrategy = new IdentityMapStrategy(targetType, sourceType);

        var enabledDictionary = new DictionaryToDictionaryMapStrategy(
            targetType,
            sourceType,
            elementStrategy,
            elementStrategy,
            DictionaryAssignmentSetting.Undefined,
            BooleanSetting.Enable);

        EmptyContainerExpressionHelper.ShouldReplaceNullWithEmpty(enabledDictionary, compilation, null).Should().BeFalse();
        EmptyContainerExpressionHelper.TryGetEmptyExpression(targetType, compilation, null, out var empty).Should().BeFalse();
        empty.Should().BeEmpty();
    }

    /// <summary>
    /// Test <c>IEnumerable&lt;KeyValuePair&lt;,&gt;&gt;</c> uses the allow-listed empty collection expression.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetEmptyExpressionResolvesIEnumerableOfKeyValuePairs()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var keyValuePair = compilation.GetTypeByMetadataName("System.Collections.Generic.KeyValuePair`2");
        keyValuePair.Should().NotBeNull();
        var constructedPair = keyValuePair.Construct(intType, intType);
        var enumerable = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1");
        enumerable.Should().NotBeNull();
        var targetType = enumerable.Construct(constructedPair);

        var success = EmptyContainerExpressionHelper.TryGetEmptyExpression(
            targetType,
            compilation,
            mapMethodSymbol: null,
            out var emptyExpression);

        success.Should().BeTrue();
        emptyExpression.Should().Be("[]");
    }

    /// <summary>
    /// Test abstract container types and structs without usable empty construction are rejected or use <c>new()</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetEmptyExpressionHandlesAbstractAndStructTargets()
    {
        var compilation = BuildCompilation(
            """
            namespace Mappa.Generator.Tests.UnitTests.SourceCode
            {
                internal abstract class AbstractList : System.Collections.Generic.List<int>
                {
                }

                internal struct EmptyStruct
                {
                }
            }
            """);
        var abstractList = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.AbstractList");
        var emptyStruct = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.EmptyStruct");
        abstractList.Should().NotBeNull();
        emptyStruct.Should().NotBeNull();

        EmptyContainerExpressionHelper.TryGetEmptyExpression(abstractList, compilation, null, out var abstractEmpty)
            .Should().BeFalse();
        abstractEmpty.Should().BeEmpty();

        EmptyContainerExpressionHelper.TryGetEmptyExpression(emptyStruct, compilation, null, out var structEmpty)
            .Should().BeTrue();
        structEmpty.Should().Be("new Mappa.Generator.Tests.UnitTests.SourceCode.EmptyStruct()");

        var concreteEnum = compilation.GetTypeByMetadataName("System.DayOfWeek");
        concreteEnum.Should().NotBeNull();
        EmptyContainerExpressionHelper.TryGetEmptyExpression(concreteEnum, compilation, null, out var enumEmpty)
            .Should().BeFalse();
        enumEmpty.Should().BeEmpty();

        var staticClassCompilation = BuildCompilation(
            """
            namespace Mappa.Generator.Tests.UnitTests.SourceCode
            {
                internal static class StaticOnly
                {
                }
            }
            """);
        var staticOnly = staticClassCompilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.StaticOnly");
        staticOnly.Should().NotBeNull();
        EmptyContainerExpressionHelper.TryGetEmptyExpression(staticOnly, staticClassCompilation, null, out var staticEmpty)
            .Should().BeFalse();
        staticEmpty.Should().BeEmpty();
    }

    private static ITypeSymbol GetConstructedType(Compilation compilation, string metadataName, int typeArgumentCount)
    {
        var type = compilation.GetTypeByMetadataName(metadataName);
        type.Should().NotBeNull($"Type '{metadataName}' should resolve in the test compilation.");
        if (typeArgumentCount == 0)
        {
            return type;
        }

        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var typeArguments = Enumerable.Repeat<ITypeSymbol>(intType, typeArgumentCount).ToArray();
        return type.Construct(typeArguments);
    }
}