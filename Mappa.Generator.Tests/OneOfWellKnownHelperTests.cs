// <copyright file="OneOfWellKnownHelperTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Helpers;
using Mappa.Generator.Models.Strategies;
using Mappa.Generator.Tests.Abstractions;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="OneOfWellKnownHelper"/> edge and defensive paths.
/// </summary>
public sealed class OneOfWellKnownHelperTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// Returns false when the OneOf.Types.None metadata type is missing from the compilation.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsOneOfTypesNoneReturnsFalseWhenNoneTypeIsMissing()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class Empty
                              {
                              }
                              """;
        var compilation = BuildCompilationWithoutOneOf(source);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);

        OneOfWellKnownHelper.IsOneOfTypesNone(stringType, compilation).Should().BeFalse();
    }

    /// <summary>
    /// Returns false for an empty arm list when selecting an unwrap arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TrySelectUniqueArmIndexReturnsFalseForEmptyArms()
    {
        var compilation = BuildCompilationWithOneOf();
        var stringType = compilation.GetSpecialType(SpecialType.System_String);

        var selected = OneOfWellKnownHelper.TrySelectUniqueArmIndex(
            ImmutableArray<ITypeSymbol>.Empty,
            stringType,
            out var armIndex);

        selected.Should().BeFalse();
        armIndex.Should().Be(-1);
    }

    /// <summary>
    /// Returns false for an empty arm list when selecting a wrap arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TrySelectUniqueArmIndexForWrapReturnsFalseForEmptyArms()
    {
        var compilation = BuildCompilationWithOneOf();
        var stringType = compilation.GetSpecialType(SpecialType.System_String);

        var selected = OneOfWellKnownHelper.TrySelectUniqueArmIndexForWrap(
            ImmutableArray<ITypeSymbol>.Empty,
            stringType,
            out var armIndex);

        selected.Should().BeFalse();
        armIndex.Should().Be(-1);
    }

    /// <summary>
    /// Returns false when wrap has more than one exact arm match.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TrySelectUniqueArmIndexForWrapReturnsFalseForDuplicateExactArms()
    {
        var compilation = BuildCompilationWithOneOf();
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var arms = ImmutableArray.Create<ITypeSymbol>(stringType, stringType);

        var selected = OneOfWellKnownHelper.TrySelectUniqueArmIndexForWrap(arms, stringType, out var armIndex);

        selected.Should().BeFalse();
        armIndex.Should().Be(-1);
    }

    /// <summary>
    /// Returns false when wrap has no matching arm.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TrySelectUniqueArmIndexForWrapReturnsFalseWhenNoArmMatches()
    {
        var compilation = BuildCompilationWithOneOf();
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var arms = ImmutableArray.Create<ITypeSymbol>(stringType);

        var selected = OneOfWellKnownHelper.TrySelectUniqueArmIndexForWrap(arms, intType, out var armIndex);

        selected.Should().BeFalse();
        armIndex.Should().Be(-1);
    }

    /// <summary>
    /// Selects a unique assignable wrap arm when there is no exact match.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TrySelectUniqueArmIndexForWrapSelectsAssignableArm()
    {
        var compilation = BuildCompilationWithAnimals();
        var animal = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Animal")
            ?? throw new InvalidOperationException("Animal was not found.");
        var dog = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Dog")
            ?? throw new InvalidOperationException("Dog was not found.");
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var arms = ImmutableArray.Create<ITypeSymbol>(animal, stringType);

        var selected = OneOfWellKnownHelper.TrySelectUniqueArmIndexForWrap(arms, dog, out var armIndex);

        selected.Should().BeTrue();
        armIndex.Should().Be(0);
    }

    /// <summary>
    /// Selects a unique arm when the target is <see cref="object"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TrySelectUniqueArmIndexSelectsAssignableObjectTarget()
    {
        var compilation = BuildCompilationWithOneOf();
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var objectType = compilation.GetSpecialType(SpecialType.System_Object);
        var arms = ImmutableArray.Create<ITypeSymbol>(stringType);

        var selected = OneOfWellKnownHelper.TrySelectUniqueArmIndex(arms, objectType, out var armIndex);

        selected.Should().BeTrue();
        armIndex.Should().Be(0);
    }

    /// <summary>
    /// Matches nullable value types to their underlying arm type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TrySelectUniqueArmIndexMatchesNullableValueTypeToUnderlyingArm()
    {
        var compilation = BuildCompilationWithOneOf();
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var nullableInt = compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(intType);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var arms = ImmutableArray.Create<ITypeSymbol>(intType, stringType);

        var selected = OneOfWellKnownHelper.TrySelectUniqueArmIndex(arms, nullableInt, out var armIndex);

        selected.Should().BeTrue();
        armIndex.Should().Be(0);
    }

    /// <summary>
    /// Returns false when resolving the Option struct for a non-OneOfBase type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetOptionStructTypeForOneOfBaseReturnsFalseForNonOneOfBase()
    {
        var compilation = BuildCompilationWithOneOf();
        var stringType = compilation.GetSpecialType(SpecialType.System_String);

        var resolved = OneOfWellKnownHelper.TryGetOptionStructTypeForOneOfBase(
            stringType,
            compilation,
            out var optionStructType);

        resolved.Should().BeFalse();
        optionStructType.Should().BeNull();
    }

    /// <summary>
    /// Returns false when the constructed OneOf arity metadata type is missing.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetOptionStructTypeForOneOfBaseReturnsFalseWhenOneOfArityIsMissing()
    {
        const string source = """
                              namespace OneOf
                              {
                                  public class OneOfBase<T0, T1, T2, T3, T4, T5, T6, T7, T8>
                                  {
                                  }
                              }

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode
                              {
                                  public sealed class Huge : OneOf.OneOfBase<string, int, bool, long, byte, short, uint, ulong, char>
                                  {
                                  }
                              }
                              """;
        var compilation = BuildCompilationWithoutOneOf(source);
        var huge = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.Huge")
            ?? throw new InvalidOperationException("Huge was not found.");

        var resolved = OneOfWellKnownHelper.TryGetOptionStructTypeForOneOfBase(
            huge,
            compilation,
            out var optionStructType);

        resolved.Should().BeFalse();
        optionStructType.Should().BeNull();
    }

    /// <summary>
    /// Returns false when the arm index is out of range for wrap construction.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetOneOfBaseWrapConstructionReturnsFalseForInvalidArmIndex()
    {
        var compilation = BuildCompilationWithOneOf();
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.StringOrInt")
            ?? throw new InvalidOperationException("StringOrInt was not found.");

        var resolved = OneOfWellKnownHelper.TryGetOneOfBaseWrapConstruction(
            targetType,
            armIndex: 99,
            compilation,
            out _,
            out var optionStructType);

        resolved.Should().BeFalse();
        optionStructType.Should().BeNull();
    }

    /// <summary>
    /// Detects public implicit conversion construction for a OneOfBase-derived type.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetOneOfBaseWrapConstructionSelectsImplicitFromArm()
    {
        var compilation = BuildCompilationWithOneOf();
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.StringOrInt")
            ?? throw new InvalidOperationException("StringOrInt was not found.");

        var resolved = OneOfWellKnownHelper.TryGetOneOfBaseWrapConstruction(
            targetType,
            armIndex: 0,
            compilation,
            out var constructionKind,
            out var optionStructType);

        resolved.Should().BeTrue();
        constructionKind.Should().Be(OneOfBaseConstructionKind.ImplicitFromArm);
        optionStructType.Should().BeNull();
    }

    /// <summary>
    /// Detects constructor-from-OneOf construction when no implicit conversion exists.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetOneOfBaseWrapConstructionSelectsConstructorFromOneOf()
    {
        var compilation = BuildCompilationWithConstructorOnlyOneOfBase();
        var targetType = compilation.GetTypeByMetadataName("Mappa.Generator.Tests.UnitTests.SourceCode.CtorOnly")
            ?? throw new InvalidOperationException("CtorOnly was not found.");

        var resolved = OneOfWellKnownHelper.TryGetOneOfBaseWrapConstruction(
            targetType,
            armIndex: 0,
            compilation,
            out var constructionKind,
            out var optionStructType);

        resolved.Should().BeTrue();
        constructionKind.Should().Be(OneOfBaseConstructionKind.ConstructorFromOneOf);
        optionStructType.Should().NotBeNull();
    }

    private static CSharpCompilation BuildCompilationWithOneOf()
    {
        const string source = """
                              using OneOf;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class StringOrInt : OneOfBase<string, int>
                              {
                                  public StringOrInt(OneOf<string, int> input)
                                      : base(input)
                                  {
                                  }

                                  public static implicit operator StringOrInt(string value)
                                      => new StringOrInt(value);
                              }
                              """;
        return BuildCompilation(source);
    }

    private static CSharpCompilation BuildCompilationWithAnimals()
    {
        const string source = """
                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public class Animal
                              {
                              }

                              public sealed class Dog : Animal
                              {
                              }
                              """;
        return BuildCompilation(source);
    }

    private static CSharpCompilation BuildCompilationWithConstructorOnlyOneOfBase()
    {
        const string source = """
                              using OneOf;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              public sealed class CtorOnly : OneOfBase<string, int>
                              {
                                  public CtorOnly(OneOf<string, int> input)
                                      : base(input)
                                  {
                                  }
                              }
                              """;
        return BuildCompilation(source);
    }

    private static CSharpCompilation BuildCompilationWithoutOneOf(string source)
    {
        var frameworkPath = Path.GetDirectoryName(typeof(Attribute).GetTypeInfo().Assembly.Location)
            ?? throw new InvalidOperationException("Framework path was not found.");
        var metadataReferences = new List<PortableExecutableReference>
        {
            MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "netstandard.dll")),
            MetadataReference.CreateFromFile(Path.Combine(frameworkPath, "System.Runtime.dll")),
        };

        return CSharpCompilation.Create(
            "OneOfHelperNoOneOfTests",
            [CSharpSyntaxTree.ParseText(source)],
            metadataReferences,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}