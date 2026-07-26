// <copyright file="ProjectionCapabilityAnalyzerTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Algorithm;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="ProjectionCapabilityAnalyzer"/>.
/// </summary>
public sealed class ProjectionCapabilityAnalyzerTests
{
    /// <summary>
    /// Test shallow-copy identity strategies are supported.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsSupportedReturnsTrueForShallowCopyIdentity()
    {
        var compilation = CSharpCompilation.Create("TestAssembly");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var strategy = new IdentityMapStrategy(
            intType,
            intType,
            IdentityMapDeepCopySetting.ShallowCopy,
            requiresMemberwiseClone: false,
            nestedFieldStrategies: []);

        ProjectionCapabilityAnalyzer.IsSupported(strategy).Should().BeTrue();
    }

    /// <summary>
    /// Test deep-copy identity strategies are not supported for projections.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsSupportedReturnsFalseForDeepCopyIdentity()
    {
        var compilation = CSharpCompilation.Create("TestAssembly");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);

        var strategy = new IdentityMapStrategy(
            intType,
            intType,
            IdentityMapDeepCopySetting.DeepCopy);

        ProjectionCapabilityAnalyzer.IsSupported(strategy).Should().BeFalse();
    }

    /// <summary>
    /// Test collection-to-collection strategies are not supported for projections.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsSupportedReturnsFalseForCollectionToCollectionMapStrategy()
    {
        var compilation = CSharpCompilation.Create("TestAssembly");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var listType = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")!.Construct(intType);
        var elementStrategy = new IdentityMapStrategy(
            intType,
            intType,
            IdentityMapDeepCopySetting.ShallowCopy,
            requiresMemberwiseClone: false,
            nestedFieldStrategies: []);
        var strategy = new CollectionToCollectionMapStrategy(
            listType,
            listType,
            elementStrategy,
            methodSymbol: null,
            BooleanSetting.Undefined,
            BooleanSetting.Undefined,
            EnumerableConcreteTypeSetting.Undefined);

        ProjectionCapabilityAnalyzer.IsSupported(strategy).Should().BeFalse();
    }
}