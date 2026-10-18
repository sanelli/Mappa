// <copyright file="PreferredInterfaceMapStrategyIntegrationTests.Diagnostics.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Assertions;
using Mappa.Generator.Tests.Assertions.Extensions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Diagnostic and coverage-oriented preferred-interface integration tests.
/// </summary>
public sealed partial class PreferredInterfaceMapStrategyIntegrationTests
{
    private const string DualDictionaryTypeDefinition = """
                                                          public sealed class Unmappable
                                                          {
                                                          }

                                                          public sealed class DualDictionary :
                                                              System.Collections.Generic.IDictionary<int, int>,
                                                              System.Collections.Generic.IDictionary<Unmappable, int>
                                                          {
                                                              private readonly System.Collections.Generic.Dictionary<int, int> values = new();

                                                              int System.Collections.Generic.IDictionary<int, int>.this[int key]
                                                              {
                                                                  get => this.values[key];
                                                                  set => this.values[key] = value;
                                                              }

                                                              Unmappable System.Collections.Generic.IDictionary<Unmappable, int>.this[Unmappable key]
                                                              {
                                                                  get => throw new System.NotSupportedException();
                                                                  set => throw new System.NotSupportedException();
                                                              }

                                                              System.Collections.Generic.ICollection<int> System.Collections.Generic.IDictionary<int, int>.Keys => this.values.Keys;
                                                              System.Collections.Generic.ICollection<int> System.Collections.Generic.IDictionary<int, int>.Values => this.values.Values;
                                                              System.Collections.Generic.ICollection<Unmappable> System.Collections.Generic.IDictionary<Unmappable, int>.Keys => throw new System.NotSupportedException();
                                                              System.Collections.Generic.ICollection<int> System.Collections.Generic.IDictionary<Unmappable, int>.Values => throw new System.NotSupportedException();
                                                              int System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int, int>>.Count => this.values.Count;
                                                              int System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Unmappable, int>>.Count => 0;
                                                              bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int, int>>.IsReadOnly => false;
                                                              bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Unmappable, int>>.IsReadOnly => true;
                                                              void System.Collections.Generic.IDictionary<int, int>.Add(int key, int value) => this.values.Add(key, value);
                                                              void System.Collections.Generic.IDictionary<Unmappable, int>.Add(Unmappable key, int value) => throw new System.NotSupportedException();
                                                              void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int, int>>.Add(System.Collections.Generic.KeyValuePair<int, int> item) => this.values.Add(item.Key, item.Value);
                                                              void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Unmappable, int>>.Add(System.Collections.Generic.KeyValuePair<Unmappable, int> item) => throw new System.NotSupportedException();
                                                              void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int, int>>.Clear() => this.values.Clear();
                                                              void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Unmappable, int>>.Clear() => throw new System.NotSupportedException();
                                                              bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int, int>>.Contains(System.Collections.Generic.KeyValuePair<int, int> item) => this.values.ContainsKey(item.Key);
                                                              bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Unmappable, int>>.Contains(System.Collections.Generic.KeyValuePair<Unmappable, int> item) => false;
                                                              bool System.Collections.Generic.IDictionary<int, int>.ContainsKey(int key) => this.values.ContainsKey(key);
                                                              bool System.Collections.Generic.IDictionary<Unmappable, int>.ContainsKey(Unmappable key) => false;
                                                              void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int, int>>.CopyTo(System.Collections.Generic.KeyValuePair<int, int>[] array, int arrayIndex) => ((System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int, int>>)this.values).CopyTo(array, arrayIndex);
                                                              void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Unmappable, int>>.CopyTo(System.Collections.Generic.KeyValuePair<Unmappable, int>[] array, int arrayIndex) => throw new System.NotSupportedException();
                                                              bool System.Collections.Generic.IDictionary<int, int>.Remove(int key) => this.values.Remove(key);
                                                              bool System.Collections.Generic.IDictionary<Unmappable, int>.Remove(Unmappable key) => false;
                                                              bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int, int>>.Remove(System.Collections.Generic.KeyValuePair<int, int> item) => this.values.Remove(item.Key);
                                                              bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<Unmappable, int>>.Remove(System.Collections.Generic.KeyValuePair<Unmappable, int> item) => false;
                                                              bool System.Collections.Generic.IDictionary<int, int>.TryGetValue(int key, out int value) => this.values.TryGetValue(key, out value);
                                                              bool System.Collections.Generic.IDictionary<Unmappable, int>.TryGetValue(Unmappable key, out int value) { value = default; return false; }
                                                              System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int, int>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int, int>>.GetEnumerator() => this.values.GetEnumerator();
                                                              System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<Unmappable, int>> System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<Unmappable, int>>.GetEnumerator() { yield break; }
                                                              System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.values.GetEnumerator();
                                                          }
                                                          """;

    private const string DualListTargetTypeDefinition = """
                                                          public sealed class Unmappable
                                                          {
                                                          }

                                                          public sealed class DualList : System.Collections.Generic.List<int>, System.Collections.Generic.IEnumerable<Unmappable>
                                                          {
                                                              System.Collections.Generic.IEnumerator<Unmappable> System.Collections.Generic.IEnumerable<Unmappable>.GetEnumerator()
                                                              {
                                                                  yield break;
                                                              }
                                                          }
                                                          """;

    private const string NestedDualEnumerableTypeDefinition = """
                                                                public sealed class NestedDualEnumerable :
                                                                    System.Collections.Generic.IEnumerable<NestedDualEnumerable>,
                                                                    System.Collections.Generic.IEnumerable<int>
                                                                {
                                                                    public System.Collections.Generic.IEnumerator<int> GetEnumerator()
                                                                    {
                                                                        yield return 1;
                                                                    }

                                                                    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

                                                                    System.Collections.Generic.IEnumerator<NestedDualEnumerable> System.Collections.Generic.IEnumerable<NestedDualEnumerable>.GetEnumerator()
                                                                    {
                                                                        yield break;
                                                                    }
                                                                }
                                                                """;

    private const string PublicInterfaceEnumeratorTypeDefinition = """
                                                                    public sealed class PublicInterfaceEnumerator :
                                                                        System.Collections.Generic.IEnumerable<int>
                                                                    {
                                                                        public System.Collections.Generic.IEnumerator<int> GetEnumerator()
                                                                        {
                                                                            yield return 1;
                                                                        }

                                                                        System.Collections.Generic.IEnumerator<int> System.Collections.Generic.IEnumerable<int>.GetEnumerator()
                                                                            => this.GetEnumerator();

                                                                        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                                                                            => this.GetEnumerator();
                                                                    }
                                                                    """;

    /// <summary>
    /// Reports MP00085 when the implementing type argument is missing.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceImplementingTypeInvalid()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSourcePreferredInterfaceImplementation(default!, typeof(IEnumerable<int>))]
                                      public partial List<int> Map(List<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(MappaDiagnosticDescriptors.PreferredInterfaceImplementingTypeInvalid);
    }

    /// <summary>
    /// Reports MP00084 when the preferred interface argument is missing.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceMissingPreferredClosedInterface()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSourcePreferredInterfaceImplementation(typeof(List<int>), default!)]
                                      public partial List<int> Map(List<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceNotClosedGenericInterface,
                "Microsoft.CodeAnalysis.TypedConstant");
    }

    /// <summary>
    /// Reports MP00084 when the preferred argument is an array type (non-named type symbol display path).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceNotClosedGenericInterfaceForArrayType()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSourcePreferredInterfaceImplementation(typeof(List<int>), typeof(int[]))]
                                      public partial List<int> Map(List<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceNotClosedGenericInterface,
                "int[]");
    }

    /// <summary>
    /// Reports MP00081 when the implementing type is a closed generic interface.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceNotImplementedWhenImplementingTypeIsClosedGenericInterface()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSourcePreferredInterfaceImplementation(typeof(IEnumerable<int>), typeof(IEnumerable<int>))]
                                      public partial List<int> Map(List<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceNotImplemented,
                "System.Collections.Generic.IEnumerable<int>",
                "System.Collections.Generic.IEnumerable<int>")
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceAttributeNotNeeded,
                "System.Collections.Generic.IEnumerable<int>",
                "System.Collections.Generic.IEnumerable<int>")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("input")));
                });
    }

    /// <summary>
    /// Reports MP00080 when the implementing type is an unbound generic interface.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceImplementingTypeNotConcreteForUnboundGeneric()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSourcePreferredInterfaceImplementation(typeof(IEnumerable<>), typeof(IEnumerable<int>))]
                                      public partial List<int> Map(List<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceImplementingTypeNotConcrete,
                "System.Collections.Generic.IEnumerable<>");
    }

    /// <summary>
    /// Reports MP00080 when the implementing type is a non-generic interface.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceImplementingTypeNotConcreteForNonGenericInterface()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSourcePreferredInterfaceImplementation(typeof(IDisposable), typeof(IEnumerable<int>))]
                                      public partial List<int> Map(List<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceImplementingTypeNotConcrete,
                "System.IDisposable");
    }

    /// <summary>
    /// Reports MP00084 when the preferred type is not a closed generic collection interface.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceNotClosedGenericInterface()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSourcePreferredInterfaceImplementation(typeof(List<int>), typeof(string))]
                                      public partial List<int> Map(List<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceNotClosedGenericInterface,
                "string");
    }

    /// <summary>
    /// Reports MP00084 when the preferred interface family is unrecognized.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceNotClosedGenericInterfaceForUnrecognizedFamily()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSourcePreferredInterfaceImplementation(typeof(List<int>), typeof(IComparable<int>))]
                                      public partial List<int> Map(List<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceNotClosedGenericInterface,
                "System.IComparable<int>");
    }

    /// <summary>
    /// Reports MP00081 when the implementing type does not implement the preferred interface.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceNotImplemented()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{DualEnumerableTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        [MappaSourcePreferredInterfaceImplementation(typeof(DualEnumerable), typeof(ICollection<int>))]
                                        public partial List<int> Map(DualEnumerable input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceNotImplemented,
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualEnumerable",
                "System.Collections.Generic.ICollection<int>")
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceAttributeNotNeeded,
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualEnumerable",
                "System.Collections.Generic.ICollection<int>")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualEnumerable",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "System.Collections.Generic.List<int>",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax("System.Collections.Generic.List<int>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Reports MP00082 when a preferred-interface attribute is never consulted.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceAttributeNotNeeded()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{DualEnumerableTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        [MappaSourcePreferredInterfaceImplementation(typeof(DualEnumerable), typeof(IEnumerable<int>))]
                                        public partial List<int> Map(List<int> input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceAttributeNotNeeded,
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualEnumerable",
                "System.Collections.Generic.IEnumerable<int>")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("input")));
                });
    }

    /// <summary>
    /// Covers immutable-queue family recognition during attribute validation.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceNotImplementedForImmutableQueueFamily()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;
                                  using System.Collections.Immutable;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSourcePreferredInterfaceImplementation(typeof(List<int>), typeof(IImmutableQueue<int>))]
                                      public partial List<int> Map(List<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(2)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceNotImplemented,
                "System.Collections.Generic.List<int>",
                "System.Collections.Immutable.IImmutableQueue<int>")
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceAttributeNotNeeded,
                "System.Collections.Generic.List<int>",
                "System.Collections.Immutable.IImmutableQueue<int>")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("input")));
                });
    }

    /// <summary>
    /// Covers immutable-stack and producer-consumer family recognition during attribute validation.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredInterfaceNotImplementedForImmutableStackAndProducerConsumerFamilies()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Concurrent;
                                  using System.Collections.Generic;
                                  using System.Collections.Immutable;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      [MappaSourcePreferredInterfaceImplementation(typeof(List<int>), typeof(IImmutableStack<int>))]
                                      [MappaTargetPreferredInterfaceImplementation(typeof(List<int>), typeof(IProducerConsumerCollection<int>))]
                                      public partial List<int> Map(List<int> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(4)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceNotImplemented,
                "System.Collections.Generic.List<int>",
                "System.Collections.Immutable.IImmutableStack<int>")
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceNotImplemented,
                "System.Collections.Generic.List<int>",
                "System.Collections.Concurrent.IProducerConsumerCollection<int>")
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceAttributeNotNeeded,
                "System.Collections.Generic.List<int>",
                "System.Collections.Immutable.IImmutableStack<int>")
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceAttributeNotNeeded,
                "System.Collections.Generic.List<int>",
                "System.Collections.Concurrent.IProducerConsumerCollection<int>")
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(1)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("input")));
                });
    }

    /// <summary>
    /// Generic preferred-interface attributes force the explicit source interface.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task GenericSourceAttributeForcesExplicitIEnumerableOfString()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{DualEnumerableTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        [MappaSourcePreferredInterfaceImplementation<DualEnumerable, IEnumerable<string>>]
                                        public partial List<string> Map(DualEnumerable input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.List<string>",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualEnumerable",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(4)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "System.Collections.Generic.List<string>",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax("System.Collections.Generic.List<string>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "System.Collections.Generic.IEnumerable<string>",
                                "__mappa_tmp_2",
                                initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("input")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(string).ToString(),
                                "__mappa_tmp_3",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_2"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Reports MP00079 when a preferred target interface cannot be used.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredTargetInterfaceCannotBeUsed()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{DualListTargetTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        [MappaTargetPreferredInterfaceImplementation(typeof(DualList), typeof(IEnumerable<Unmappable>))]
                                        public partial DualList Map(List<int> input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceCannotBeUsed,
                "System.Collections.Generic.IEnumerable<Mappa.Generator.Tests.UnitTests.SourceCode.Unmappable>",
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualList");
    }

    /// <summary>
    /// Reports MP00079 when a preferred dictionary source interface cannot be used.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredDictionarySourceInterfaceCannotBeUsed()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{DualDictionaryTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        [MappaSourcePreferredInterfaceImplementation(typeof(DualDictionary), typeof(IDictionary<Unmappable, int>))]
                                        public partial Dictionary<int, int> Map(DualDictionary input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceCannotBeUsed,
                "System.Collections.Generic.IDictionary<Mappa.Generator.Tests.UnitTests.SourceCode.Unmappable, int>",
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualDictionary");
    }

    /// <summary>
    /// Nested dual enumerable mapping exercises preferred-interface recursion guard during usability probes.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MapsNestedDualEnumerableUsingUniqueUsableImplicitInterface()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{NestedDualEnumerableTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        public partial List<int> Map(NestedDualEnumerable input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.NestedDualEnumerable",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "System.Collections.Generic.List<int>",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax("System.Collections.Generic.List<int>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Nested dual enumerable to nested list mapping re-enters preferred resolution (recursion guard).
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MapsNestedDualEnumerableToListOfListUsingRecursionGuard()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{NestedDualEnumerableTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        public partial List<List<int>> Map(NestedDualEnumerable input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.List<System.Collections.Generic.List<int>>",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.NestedDualEnumerable",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "System.Collections.Generic.List<System.Collections.Generic.List<int>>",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax("System.Collections.Generic.List<System.Collections.Generic.List<int>>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                initializerAssertions => initializerAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                "System.Collections.Generic.List<int>",
                                                "__mappa_tmp_4",
                                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax(
                                                    "System.Collections.Generic.List<int>",
                                                    parameterAssertions => parameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_3"))))
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Reports MP00079 when a preferred dictionary target interface cannot be used.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task ReportsPreferredDictionaryTargetInterfaceCannotBeUsed()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{DualDictionaryTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        [MappaTargetPreferredInterfaceImplementation(typeof(DualDictionary), typeof(IDictionary<Unmappable, int>))]
                                        public partial DualDictionary Map(Dictionary<int, int> input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .HaveDiagnostics(1)
            .HaveDiagnostic(
                MappaDiagnosticDescriptors.PreferredInterfaceCannotBeUsed,
                "System.Collections.Generic.IDictionary<Mappa.Generator.Tests.UnitTests.SourceCode.Unmappable, int>",
                "Mappa.Generator.Tests.UnitTests.SourceCode.DualDictionary");
    }

    /// <summary>
    /// Public <c>IEnumerator&lt;T&gt;</c> return type with an explicit interface method maps without a cast.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MapsTypeWithPublicInterfaceEnumeratorWithoutCast()
    {
        const string sourceCode = $$"""
                                    #nullable enable

                                    using Mappa.Attributes;
                                    using System.Collections.Generic;

                                    namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                    {{PublicInterfaceEnumeratorTypeDefinition}}

                                    [Mappa]
                                    public sealed partial class Mapper
                                    {
                                        public partial List<int> Map(PublicInterfaceEnumerator input);
                                    }

                                    #nullable restore
                                    """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.List<int>",
                NullableAnnotation.NotAnnotated,
                "Mappa.Generator.Tests.UnitTests.SourceCode.PublicInterfaceEnumerator",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "System.Collections.Generic.List<int>",
                                "__mappa_tmp_1",
                                initializerAssertions => initializerAssertions.BeObjectCreationExpressionSyntax("System.Collections.Generic.List<int>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(int).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(1)
                                        .HasNextSyntaxNode(foreachStatementAssertions =>
                                            foreachStatementAssertions.BeInvocationExpressionSyntaxStatement(
                                                "__mappa_tmp_1.Add",
                                                firstParameterAssertions => firstParameterAssertions.BeIdentifierNameSyntax("__mappa_tmp_2")))))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeReturnStatement(expressionAssertions => expressionAssertions
                                .BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }

    /// <summary>
    /// Maps <see cref="System.Collections.Generic.IEnumerable{T}"/> of key-value pairs using the KVP preferred family.
    /// </summary>
    /// <returns>The async task.</returns>
    [Fact]
    [IntegrationTest]
    public async Task MapsIEnumerableOfKeyValuePairsToIReadOnlyDictionary()
    {
        const string sourceCode = """
                                  #nullable enable

                                  using Mappa.Attributes;
                                  using System.Collections.Generic;

                                  namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                                  [Mappa]
                                  public sealed partial class Mapper
                                  {
                                      public partial IReadOnlyDictionary<int, string> Map(IEnumerable<KeyValuePair<int, string>> input);
                                  }

                                  #nullable restore
                                  """;

        var generatedResults = await RunMappaGeneratorAsync(sourceCode, CancellationToken.None).ConfigureAwait(true);

        generatedResults.Should()
            .NotHaveDiagnostics()
            .HaveGeneratedSourceCode()
            .WithCompilationUnit()
            .NotBeNull().And
            .HaveDefaultMapMethod(
                "System.Collections.Generic.IReadOnlyDictionary<int, string>",
                NullableAnnotation.NotAnnotated,
                "System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int, string>>",
                NullableAnnotation.NotAnnotated,
                blockSyntaxAssertions =>
                {
                    blockSyntaxAssertions
                        .HasSyntaxNodesCount(3)
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                            syntaxNodeAssertions.BeLocalDeclarationStatementSyntax(
                                "System.Collections.Generic.Dictionary<int, string>",
                                "__mappa_tmp_1",
                                assertions => assertions.BeObjectCreationExpressionSyntax("System.Collections.Generic.Dictionary<int, string>")))
                        .HasNextSyntaxNode(syntaxNodeAssertions =>
                        {
                            syntaxNodeAssertions.BeForEachStatementSyntax(
                                typeof(KeyValuePair<int, string>).ToString(),
                                "__mappa_tmp_2",
                                expressionAssertions => expressionAssertions.BeIdentifierNameSyntax("input"),
                                statementAssertions =>
                                {
                                    statementAssertions
                                        .BeBlockStatement()
                                        .AsBlock()
                                        .HasSyntaxNodesCount(3)
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(int).ToString(),
                                                "__mappa_tmp_3",
                                                assertions => assertions.BeMemberAccessExpressionSyntax($"__mappa_tmp_2.{nameof(KeyValuePair<int, string>.Key)}")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeLocalDeclarationStatementSyntax(
                                                typeof(string).ToString(),
                                                "__mappa_tmp_4",
                                                assertions => assertions.BeMemberAccessExpressionSyntax($"__mappa_tmp_2.{nameof(KeyValuePair<int, string>.Value)}")))
                                        .HasNextSyntaxNode(forEachStatementAssertions =>
                                            forEachStatementAssertions.BeAssignmentExpressionStatement(
                                                leftExpressionAssertions => leftExpressionAssertions.BeElementAccessExpressionSyntaxWithIdentifierNameSyntax("__mappa_tmp_1", "__mappa_tmp_3"),
                                                rightExpressionAssertions => rightExpressionAssertions.BeIdentifierNameSyntax("__mappa_tmp_4")));
                                });
                        })
                        .HasNextSyntaxNode(syntaxNodeAssertions => syntaxNodeAssertions.BeReturnStatement(assertions => assertions.BeIdentifierNameSyntax("__mappa_tmp_1")));
                });
    }
}