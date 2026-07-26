// <copyright file="TypeSymbolExtensionsTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
using Mappa.Generator.Extensions;
using Mappa.Generator.Tests.Abstractions;

using Microsoft.CodeAnalysis;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="TypeSymbolExtensions"/>.
/// </summary>
public sealed class TypeSymbolExtensionsTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.NormalizeType"/> maps C# aliases to CLR type names.
    /// </summary>
    /// <param name="alias">The type alias.</param>
    /// <param name="expected">The expected normalized name.</param>
    [Theory]
    [InlineData("int", "System.Int32")]
    [InlineData("string", "System.String")]
    [InlineData("bool", "System.Boolean")]
    [InlineData("void", "System.Void")]
    [InlineData("System.DateTime", "System.DateTime")]
    [UnitTest]
    public void NormalizeTypeMapsAliasesToClrNames(string alias, string expected)
    {
        alias.NormalizeType().Should().Be(expected);
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.GetKeyAndValueTypes"/> reads dictionary type arguments.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetKeyAndValueTypesReturnsDictionaryTypeArguments()
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

        var (keyType, valueType) = property.Type.GetKeyAndValueTypes(compilation);

        keyType.ToDisplayString().Should().Be("string");
        valueType.ToDisplayString().Should().Be("int");
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.GetKeyAndValueTypes"/> resolves key and value types from
    /// a non-generic type implementing <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetKeyAndValueTypesResolvesInterfaceTypeArgumentsOnNonGenericType()
    {
        const string source = """
                              using System.Collections.Generic;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class StringIntDictionary : Dictionary<string, int>
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var dictionaryType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.StringIntDictionary");

        var (keyType, valueType) = dictionaryType!.GetKeyAndValueTypes(compilation);

        keyType.ToDisplayString().Should().Be("string");
        valueType.ToDisplayString().Should().Be("int");
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.GetKeyAndValueTypes"/> throws for unsupported types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetKeyAndValueTypesThrowsForUnsupportedType()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Holder
                              {
                                  public int Value { get; set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var holder = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Holder");
        var property = holder!.GetMembers("Value").OfType<IPropertySymbol>().Single();

        var act = () => property.Type.GetKeyAndValueTypes(compilation);

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Cannot obtain key and value types of \"int\"");
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.IsTuple"/> detects value tuples and classic tuples.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsTupleDetectsValueTupleAndClassicTuple()
    {
        const string source = """
                              using System;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Holder
                              {
                                  public (int, string) ValueTuple { get; set; }

                                  public Tuple<int, string> ClassicTuple { get; set; }

                                  public int NotTuple { get; set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var holder = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Holder");
        var valueTuple = holder!.GetMembers("ValueTuple").OfType<IPropertySymbol>().Single();
        var classicTuple = holder.GetMembers("ClassicTuple").OfType<IPropertySymbol>().Single();
        var notTuple = holder.GetMembers("NotTuple").OfType<IPropertySymbol>().Single();

        valueTuple.Type.IsTuple(compilation).Should().BeTrue();
        classicTuple.Type.IsTuple(compilation).Should().BeTrue();
        notTuple.Type.IsTuple(compilation).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.GetElementType"/> returns element types for arrays, generics, and enumerables.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetElementTypeReturnsElementTypeForSupportedContainers()
    {
        const string source = """
                              using System.Collections.Generic;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class IntEnumerable : IEnumerable<int>
                              {
                                  public IEnumerator<int> GetEnumerator() => throw new System.NotImplementedException();

                                  System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new System.NotImplementedException();
                              }

                              public sealed class Holder
                              {
                                  public int[] Array { get; set; }

                                  public List<int> List { get; set; }

                                  public IntEnumerable CustomEnumerable { get; set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var holder = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Holder");
        var array = holder!.GetMembers("Array").OfType<IPropertySymbol>().Single();
        var list = holder.GetMembers("List").OfType<IPropertySymbol>().Single();
        var customEnumerable = holder.GetMembers("CustomEnumerable").OfType<IPropertySymbol>().Single();

        array.Type.GetElementType().ToDisplayString().Should().Be("int");
        list.Type.GetElementType().ToDisplayString().Should().Be("int");
        customEnumerable.Type.GetElementType().ToDisplayString().Should().Be("int");
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.GetElementType"/> throws for unsupported types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void GetElementTypeThrowsForUnsupportedType()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Holder
                              {
                                  public int Value { get; set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var holder = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Holder");
        var property = holder!.GetMembers("Value").OfType<IPropertySymbol>().Single();

        var act = () => property.Type.GetElementType();

        act.Should()
            .Throw<MappaGeneratorException>()
            .WithMessage("Cannot obtain element type of \"int\"");
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.IsMethodValidToMapToTargetSymbolForPolymorphism"/> validates polymorphism helper methods.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsMethodValidToMapToTargetSymbolForPolymorphismValidatesMethodShapes()
    {
        const string source = """
                              using Mappa;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Source
                              {
                              }

                              public static class Helpers
                              {
                                  public static Source MapZeroParameters() => new();

                                  public static Source MapSource(Source source) => source;

                                  public static Source MapSourceWithContext(Source source, MappaContext context) => source;

                                  public Source MapInstance(Source source) => source;

                                  public static Source MapTooMany(Source source, MappaContext context, int extra) => source;
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Source");
        var helpers = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Helpers");
        if (sourceType is null || helpers is null)
        {
            throw new InvalidOperationException("Expected source and helper types to be present in the compilation.");
        }

        var zeroParameters = helpers.GetMembers("MapZeroParameters").OfType<IMethodSymbol>().Single();
        var oneParameter = helpers.GetMembers("MapSource").OfType<IMethodSymbol>().Single();
        var twoParameters = helpers.GetMembers("MapSourceWithContext").OfType<IMethodSymbol>().Single();
        var instanceMethod = helpers.GetMembers("MapInstance").OfType<IMethodSymbol>().Single();
        var tooManyParameters = helpers.GetMembers("MapTooMany").OfType<IMethodSymbol>().Single();

        zeroParameters.IsMethodValidToMapToTargetSymbolForPolymorphism(sourceType, compilation, mustBeStatic: true, nullableEnabled: true, acceptTwoParameters: true).Should().BeTrue();
        oneParameter.IsMethodValidToMapToTargetSymbolForPolymorphism(sourceType, compilation, mustBeStatic: true, nullableEnabled: true, acceptTwoParameters: true).Should().BeTrue();
        twoParameters.IsMethodValidToMapToTargetSymbolForPolymorphism(sourceType, compilation, mustBeStatic: true, nullableEnabled: true, acceptTwoParameters: true).Should().BeTrue();
        instanceMethod.IsMethodValidToMapToTargetSymbolForPolymorphism(sourceType, compilation, mustBeStatic: true, nullableEnabled: true, acceptTwoParameters: true).Should().BeFalse();
        tooManyParameters.IsMethodValidToMapToTargetSymbolForPolymorphism(sourceType, compilation, mustBeStatic: true, nullableEnabled: true, acceptTwoParameters: true).Should().BeFalse();
        oneParameter.IsMethodValidToMapToTargetSymbolForPolymorphism(sourceType, compilation, mustBeStatic: true, nullableEnabled: true, acceptTwoParameters: false).Should().BeTrue();
        twoParameters.IsMethodValidToMapToTargetSymbolForPolymorphism(sourceType, compilation, mustBeStatic: true, nullableEnabled: true, acceptTwoParameters: false).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.IsRelaxedNullabilityMatch"/> allows the supported nullability mismatches.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsRelaxedNullabilityMatchAllowsSupportedMismatches()
    {
        const string source = """
                              #nullable enable

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Sample
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sampleType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Sample")!;
        var nullableSample = sampleType.WithNullableAnnotation(NullableAnnotation.Annotated);
        var nonNullableSample = sampleType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

        nullableSample.IsRelaxedNullabilityMatch(nonNullableSample, isNullableEnabled: true).Should().BeTrue();
        nonNullableSample.IsRelaxedNullabilityMatch(nullableSample, isNullableEnabled: true).Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.IsRelaxedNullabilityMatch"/> rejects unsupported nullability mismatches.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsRelaxedNullabilityMatchRejectsUnsupportedMismatches()
    {
        const string source = """
                              #nullable enable

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Sample
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sampleType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Sample")!;
        var nullableSample = sampleType.WithNullableAnnotation(NullableAnnotation.Annotated);
        var nonNullableSample = sampleType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

        nullableSample.IsRelaxedNullabilityMatch(nullableSample, isNullableEnabled: true).Should().BeFalse();
        nonNullableSample.IsRelaxedNullabilityMatch(nonNullableSample, isNullableEnabled: true).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.IsRelaxedNullabilityMatch"/> is disabled when nullable is disabled.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsRelaxedNullabilityMatchReturnsFalseWhenNullableIsDisabled()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Sample
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sampleType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Sample")!;
        var nullableSample = sampleType.WithNullableAnnotation(NullableAnnotation.Annotated);
        var nonNullableSample = sampleType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

        nullableSample.IsRelaxedNullabilityMatch(nonNullableSample, isNullableEnabled: false).Should().BeFalse();
        nonNullableSample.IsRelaxedNullabilityMatch(nullableSample, isNullableEnabled: false).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.IsNullabilityMatchOrRelaxed"/> prefers exact matches and allows relaxed matches.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsNullabilityMatchOrRelaxedSupportsExactAndRelaxedMatches()
    {
        const string source = """
                              #nullable enable

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Sample
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sampleType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Sample")!;
        var nullableSample = sampleType.WithNullableAnnotation(NullableAnnotation.Annotated);
        var nonNullableSample = sampleType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

        nonNullableSample.IsNullabilityMatchOrRelaxed(nonNullableSample, isNullableEnabled: true).Should().BeTrue();
        nullableSample.IsNullabilityMatchOrRelaxed(nullableSample, isNullableEnabled: true).Should().BeTrue();
        nullableSample.IsNullabilityMatchOrRelaxed(nonNullableSample, isNullableEnabled: true).Should().BeTrue();
        nonNullableSample.IsNullabilityMatchOrRelaxed(nullableSample, isNullableEnabled: true).Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.IsRelaxedNullabilityMatch"/> rejects different underlying types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsRelaxedNullabilityMatchReturnsFalseForDifferentUnderlyingTypes()
    {
        const string source = """
                              #nullable enable

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Sample
                              {
                              }

                              public sealed class OtherSample
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sampleType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Sample")!;
        var otherSampleType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.OtherSample")!;
        var nullableSample = sampleType.WithNullableAnnotation(NullableAnnotation.Annotated);
        var nonNullableOtherSample = otherSampleType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

        nullableSample.IsRelaxedNullabilityMatch(nonNullableOtherSample, isNullableEnabled: true).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.IsRelaxedNullabilityMatch"/> rejects oblivious nullability annotations.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsRelaxedNullabilityMatchReturnsFalseWhenNullableAnnotationIsNone()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Sample
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sampleType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Sample")!;
        var obliviousSample = sampleType.WithNullableAnnotation(NullableAnnotation.None);
        var nonNullableSample = sampleType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        var nullableSample = sampleType.WithNullableAnnotation(NullableAnnotation.Annotated);

        obliviousSample.IsRelaxedNullabilityMatch(nonNullableSample, isNullableEnabled: true).Should().BeFalse();
        nonNullableSample.IsRelaxedNullabilityMatch(obliviousSample, isNullableEnabled: true).Should().BeFalse();
        obliviousSample.IsRelaxedNullabilityMatch(nullableSample, isNullableEnabled: true).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.IsNullabilityMatchOrRelaxed"/> rejects different underlying types.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsNullabilityMatchOrRelaxedReturnsFalseForDifferentUnderlyingTypes()
    {
        const string source = """
                              #nullable enable

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Sample
                              {
                              }

                              public sealed class OtherSample
                              {
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sampleType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Sample")!;
        var otherSampleType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.OtherSample")!;

        sampleType.IsNullabilityMatchOrRelaxed(otherSampleType, isNullableEnabled: true).Should().BeFalse();
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.HasAmbiguousEnumMemberDescriptionMap"/> detects one source
    /// description matching multiple targets when comparison is case-insensitive.
    /// </summary>
    [Fact]
    [UnitTest]
    public void HasAmbiguousEnumMemberDescriptionMapDetectsSourceMatchingMultipleTargets()
    {
        const string source = """
                              using System.ComponentModel;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public enum SourceEnum
                              {
                                  [Description("Shared")]
                                  Alpha,
                              }

                              public enum TargetEnum
                              {
                                  [Description("shared")]
                                  One,
                                  [Description("SHARED")]
                                  Two,
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceEnum = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.SourceEnum")!;
        var targetEnum = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.TargetEnum")!;

        var isAmbiguous = sourceEnum.HasAmbiguousEnumMemberDescriptionMap(
            targetEnum,
            compilation,
            caseInsensitive: true,
            out var ambiguityDetails);

        isAmbiguous.Should().BeTrue();
        ambiguityDetails.Should().Be(
            "Source enum member 'Alpha' matches multiple target members by Description: 'One', 'Two'.");
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.HasAmbiguousEnumMemberDescriptionMap"/> detects multiple
    /// source members mapping to the same target by Description.
    /// </summary>
    [Fact]
    [UnitTest]
    public void HasAmbiguousEnumMemberDescriptionMapDetectsMultipleSourcesMatchingOneTarget()
    {
        const string source = """
                              using System.ComponentModel;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public enum SourceEnum
                              {
                                  [Description("Shared")]
                                  Alpha,
                                  [Description("shared")]
                                  Beta,
                              }

                              public enum TargetEnum
                              {
                                  [Description("SHARED")]
                                  One,
                              }
                              """;

        var compilation = BuildCompilation(source);
        var sourceEnum = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.SourceEnum")!;
        var targetEnum = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.TargetEnum")!;

        var isAmbiguous = sourceEnum.HasAmbiguousEnumMemberDescriptionMap(
            targetEnum,
            compilation,
            caseInsensitive: true,
            out var ambiguityDetails);

        isAmbiguous.Should().BeTrue();
        ambiguityDetails.Should().Be(
            "Target enum member 'One' is matched by multiple source members by Description: 'Alpha', 'Beta'.");
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.IsOrImplementIQueryable"/> recognizes <see cref="System.Linq.IQueryable{T}"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsOrImplementIQueryableRecognizesQueryableType()
    {
        const string source = """
                              using System.Linq;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Holder
                              {
                                  public IQueryable<int> Values { get; set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var holder = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Holder")!;
        var property = holder.GetMembers("Values").OfType<IPropertySymbol>().Single();

        property.Type.IsOrImplementIQueryable(compilation).Should().BeTrue();
        property.Type.IsIQueryable(compilation).Should().BeTrue();
    }

    /// <summary>
    /// Test <see cref="TypeSymbolExtensions.TryGetQueryableElementType"/> returns the element type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetQueryableElementTypeReturnsElementType()
    {
        const string source = """
                              using System.Linq;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Holder
                              {
                                  public IQueryable<string> Values { get; set; }
                              }
                              """;

        var compilation = BuildCompilation(source);
        var holder = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Holder")!;
        var property = holder.GetMembers("Values").OfType<IPropertySymbol>().Single();

        var success = property.Type.TryGetQueryableElementType(compilation, out var elementType);

        success.Should().BeTrue();
        elementType.ToDisplayString().Should().Be("string");
    }
}