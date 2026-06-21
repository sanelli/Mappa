// <copyright file="ReadonlyCollectionPropertyLoopBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="ReadonlyCollectionPropertyLoopBuilder"/>.
/// </summary>
public sealed class ReadonlyCollectionPropertyLoopBuilderTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="ReadonlyCollectionPropertyLoopBuilder"/> emits a foreach loop for non-array, non-IList sources.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceUsesForeachForIEnumerableSource()
    {
        const string source = """
                              using System.Collections.Generic;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public IEnumerable<int> Items { get; }
                              }

                              public class Target
                              {
                                  public List<int> Items { get; }
                              }
                              """;

        var code = BuildLoop(
            source,
            ReadonlyCollectionPropertyLoopBuilder.InsertionMethod.Add);

        code.Should().Contain("foreach (int __mappa_tmp_");
        code.Should().Contain("in __mappa_tmp_1");
        code.Should().Contain("target.Items.Add(__mappa_tmp_");
    }

    /// <summary>
    /// Test <see cref="ReadonlyCollectionPropertyLoopBuilder"/> emits explicit <see cref="ICollection{T}.Add"/> for explicit interface implementations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceUsesExplicitCollectionAddWhenAddIsExplicitlyImplemented()
    {
        const string source = """
                              using System;
                              using System.Collections;
                              using System.Collections.Generic;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int[] Items { get; }
                              }

                              public class TargetCollection : ICollection<int>
                              {
                                  void ICollection<int>.Add(int item) { }

                                  int ICollection<int>.Count => 0;

                                  bool ICollection<int>.IsReadOnly => false;

                                  void ICollection<int>.Clear() { }

                                  bool ICollection<int>.Contains(int item) => false;

                                  void ICollection<int>.CopyTo(int[] array, int arrayIndex) { }

                                  bool ICollection<int>.Remove(int item) => false;

                                  IEnumerator<int> IEnumerable<int>.GetEnumerator() => throw new NotImplementedException();

                                  IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
                              }

                              public class Target
                              {
                                  public TargetCollection Items { get; }
                              }
                              """;

        var code = BuildLoop(
            source,
            ReadonlyCollectionPropertyLoopBuilder.InsertionMethod.Add);

        code.Should().Contain("System.Collections.Generic.ICollection<int> __mappa_tmp_");
        code.Should().Contain(".Add(__mappa_tmp_");
        code.Should().NotContain("target.Items.Add(__mappa_tmp_");
    }

    /// <summary>
    /// Test <see cref="ReadonlyCollectionPropertyLoopBuilder"/> uses <see cref="Array.Length"/> for array sources.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceUsesLengthForArraySource()
    {
        const string source = """
                              using System.Collections.Generic;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                                  public int[] Items { get; }
                              }

                              public class Target
                              {
                                  public List<int> Items { get; }
                              }
                              """;

        var code = BuildLoop(
            source,
            ReadonlyCollectionPropertyLoopBuilder.InsertionMethod.Add);

        code.Should().Contain("for (int __mappa_tmp_");
        code.Should().Contain("< __mappa_tmp_1.Length");
    }

    private static string BuildLoop(
        string source,
        ReadonlyCollectionPropertyLoopBuilder.InsertionMethod insertionMethod)
    {
        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target");
        if (sourceType is null || targetType is null)
        {
            throw new InvalidOperationException("Expected source and target types to be present in the compilation.");
        }

        var sourceProperty = sourceType.GetMembers("Items").OfType<IPropertySymbol>().Single();
        var targetProperty = targetType.GetMembers("Items").OfType<IPropertySymbol>().Single();
        var sourceCollectionType = sourceProperty.Type;
        var targetCollectionType = targetProperty.Type;
        var elementStrategy = new IdentityMapStrategy(
            targetCollectionType.GetElementType(),
            sourceCollectionType.GetElementType());
        var builderContext = new MappaBuilderContext(compilation);
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            compilation.SyntaxTrees[0]);

        using (builderContext.PushCurrentCompositeTypeTargetName("target"))
        {
            var sourceTemporary = builderContext.NextTemporary();
            var (_, code) = ReadonlyCollectionPropertyLoopBuilder.BuildSource(
                sourceCollectionType,
                targetCollectionType,
                targetProperty,
                elementStrategy,
                insertionMethod,
                sourceTemporary,
                builderContext,
                globalOptions);

            return code;
        }
    }
}