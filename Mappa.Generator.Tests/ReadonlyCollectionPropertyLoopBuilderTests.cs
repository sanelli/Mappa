// <copyright file="ReadonlyCollectionPropertyLoopBuilderTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;
using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

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

    /// <summary>
    /// Preferred source interface forces an explicit cast before <c>foreach</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceCastsToPreferredExplicitSourceInterfaceBeforeForeach()
    {
        const string source = """
                              using System.Collections;
                              using System.Collections.Generic;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class DualEnumerable : IEnumerable<int>, IEnumerable<string>
                              {
                                  public IEnumerator<int> GetEnumerator()
                                  {
                                      yield return 1;
                                  }

                                  IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

                                  IEnumerator<string> IEnumerable<string>.GetEnumerator()
                                  {
                                      yield return "a";
                                  }
                              }

                              public class Source
                              {
                                  public DualEnumerable Items { get; }
                              }

                              public class Target
                              {
                                  public List<string> Items { get; }
                              }
                              """;

        var code = BuildLoop(
            source,
            ReadonlyCollectionPropertyLoopBuilder.InsertionMethod.Add,
            preferEnumerableOfString: true);

        code.Should().Contain("System.Collections.Generic.IEnumerable<string>");
        code.Should().Contain("foreach (string __mappa_tmp_");
    }

    /// <summary>
    /// Preferred implicit source interface does not emit a cast before <c>foreach</c>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void BuildSourceDoesNotCastWhenPreferredSourceInterfaceIsImplicit()
    {
        const string source = """
                              using System.Collections;
                              using System.Collections.Generic;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class DualEnumerable : IEnumerable<int>, IEnumerable<string>
                              {
                                  public IEnumerator<int> GetEnumerator()
                                  {
                                      yield return 1;
                                  }

                                  IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

                                  IEnumerator<string> IEnumerable<string>.GetEnumerator()
                                  {
                                      yield return "a";
                                  }
                              }

                              public class Source
                              {
                                  public DualEnumerable Items { get; }
                              }

                              public class Target
                              {
                                  public List<int> Items { get; }
                              }
                              """;

        var code = BuildLoop(
            source,
            ReadonlyCollectionPropertyLoopBuilder.InsertionMethod.Add,
            preferEnumerableOfInt: true);

        code.Should().Contain("foreach (int __mappa_tmp_");
        code.Should().NotContain("IEnumerable<int> __mappa_tmp_");
    }

    /// <summary>
    /// <see cref="ReadonlyCollectionPropertyLoopBuilder"/> rejects unsupported length sources.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetLengthPropertyNameThrowsForUnsupportedSourceType()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var method = typeof(ReadonlyCollectionPropertyLoopBuilder).GetMethod(
            "GetLengthPropertyName",
            BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("GetLengthPropertyName was not found.");

        var act = () => method.Invoke(null, [intType]);

        act.Should().Throw<TargetInvocationException>()
            .WithInnerException<MappaGeneratorException>();
    }

    /// <summary>
    /// <see cref="ReadonlyCollectionPropertyLoopBuilder"/> rejects unexpected insertion methods.
    /// </summary>
    [Fact]
    [UnitTest]
    public void AppendElementThrowsForUnexpectedInsertionMethod()
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

        var compilation = BuildCompilation(source);
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target")
            ?? throw new InvalidOperationException("Target was not found.");
        var targetProperty = targetType.GetMembers("Items").OfType<IPropertySymbol>().Single();
        var builderContext = new MappaBuilderContext(compilation);
        var stringBuilder = new PrettyCode.StringBuilder();
        var method = typeof(ReadonlyCollectionPropertyLoopBuilder).GetMethod(
            "AppendElement",
            BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("AppendElement was not found.");
        var invalidInsertion = (ReadonlyCollectionPropertyLoopBuilder.InsertionMethod)999;

        using (builderContext.PushCurrentCompositeTypeTargetName("target"))
        {
            var act = () => method.Invoke(
                null,
                [
                    stringBuilder,
                    builderContext,
                    targetProperty.Type,
                    targetProperty,
                    "element",
                    invalidInsertion,
                    null,
                ]);

            act.Should().Throw<TargetInvocationException>()
                .WithInnerException<MappaGeneratorException>();
        }
    }

    private static string BuildLoop(
        string source,
        ReadonlyCollectionPropertyLoopBuilder.InsertionMethod insertionMethod,
        bool preferEnumerableOfString = false,
        bool preferEnumerableOfInt = false)
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
        INamedTypeSymbol? preferredSourceInterface = null;
        INamedTypeSymbol? preferredTargetInterface = null;
        if (preferEnumerableOfString)
        {
            preferredSourceInterface = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
                .Construct(compilation.GetSpecialType(SpecialType.System_String));
            preferredTargetInterface = preferredSourceInterface;
        }
        else if (preferEnumerableOfInt)
        {
            preferredSourceInterface = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
                .Construct(compilation.GetSpecialType(SpecialType.System_Int32));
            preferredTargetInterface = preferredSourceInterface;
        }

        var sourceElementType = preferredSourceInterface is { TypeArguments.Length: 1 }
            ? preferredSourceInterface.TypeArguments[0]
            : sourceCollectionType.GetElementType();
        var targetElementType = preferredTargetInterface is { TypeArguments.Length: 1 }
            ? preferredTargetInterface.TypeArguments[0]
            : targetCollectionType.GetElementType();
        var elementStrategy = new IdentityMapStrategy(targetElementType, sourceElementType);
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
                globalOptions,
                preferredSourceInterface,
                preferredTargetInterface);

            return code;
        }
    }
}