// <copyright file="MethodSymbolExtensionsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="MethodSymbolExtensions"/>.
/// </summary>
public sealed class MethodSymbolExtensionsTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="MethodSymbolExtensions.GetIDictionaryInterfaceAddAccessMode"/> returns <see cref="InterfaceMethodAccessMode.Direct"/>
    /// for <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetIDictionaryInterfaceAddAccessModeReturnsDirectForDictionary()
    {
        const string source = """
                              using System.Collections.Generic;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Holder
                              {
                                  public Dictionary<string, int> Values { get; set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var holder = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Holder");
        var property = holder!.GetMembers("Values").OfType<IPropertySymbol>().Single();

        property.Type.GetIDictionaryInterfaceAddAccessMode(compilation)
            .Should().Be(InterfaceMethodAccessMode.Direct);
    }

    /// <summary>
    /// Test <see cref="MethodSymbolExtensions.GetIDictionaryInterfaceAddAccessMode"/> returns <see cref="InterfaceMethodAccessMode.InterfaceExplicit"/>
    /// for a generic type that implements <see cref="IDictionary{TKey,TValue}.Add"/> explicitly.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetIDictionaryInterfaceAddAccessModeReturnsInterfaceExplicitForGenericExplicitAdd()
    {
        const string source = """
                              using System.Collections.Generic;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public partial class Target<K, V> : IDictionary<K, V>
                              {
                                  void IDictionary<K, V>.Add(K key, V value) { }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var target = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Target`2");
        var stringType = compilation.GetTypeByMetadataName("System.String");
        var intType = compilation.GetTypeByMetadataName("System.Int32");

        stringType.Should().NotBeNull();
        intType.Should().NotBeNull();

        target!.Construct(stringType, intType)
            .GetIDictionaryInterfaceAddAccessMode(compilation)
            .Should().Be(InterfaceMethodAccessMode.InterfaceExplicit);
    }

    /// <summary>
    /// Test <see cref="MethodSymbolExtensions.GetIDictionaryInterfaceAddAccessMode"/> returns <see cref="InterfaceMethodAccessMode.InterfaceExplicit"/>
    /// for <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey,TValue}"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetIDictionaryInterfaceAddAccessModeReturnsInterfaceExplicitForConcurrentDictionary()
    {
        const string source = """
                              using System.Collections.Concurrent;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Holder
                              {
                                  public ConcurrentDictionary<string, int> Values { get; set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var holder = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Holder");
        var property = holder!.GetMembers("Values").OfType<IPropertySymbol>().Single();

        property.Type.GetIDictionaryInterfaceAddAccessMode(compilation)
            .Should().Be(InterfaceMethodAccessMode.InterfaceExplicit);
    }
}