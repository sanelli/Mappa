// <copyright file="PreferredInterfaceResolverUnitTests.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Diagnostics.Debug;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Tests.Abstractions;
using Mappa.Generator.Tests.Helpers;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;
using Xunit.OpenCategories.V3;

namespace Mappa.Generator.Tests;

/// <summary>
/// Unit tests for <see cref="PreferredInterfaceResolver"/>.
/// </summary>
public sealed class PreferredInterfaceResolverUnitTests
    : MappaGeneratorAbstractUnitTests
{
    /// <summary>
    /// <see cref="PreferredInterfaceResolver.TryGetFamily"/> recognizes dictionary, collection, and enumerable families.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryGetFamilyRecognizesKnownInterfaceFamilies()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var keyValuePair = compilation.GetTypeByMetadataName("System.Collections.Generic.KeyValuePair`2")
            ?.Construct(intType, stringType)
            ?? throw new InvalidOperationException("KeyValuePair`2 was not found.");

        var dictionary = compilation.GetTypeByMetadataName("System.Collections.Generic.IDictionary`2")
            ?? throw new InvalidOperationException("IDictionary`2 was not found.");
        var readOnlyDictionary = compilation.GetTypeByMetadataName("System.Collections.Generic.IReadOnlyDictionary`2")
            ?? throw new InvalidOperationException("IReadOnlyDictionary`2 was not found.");
        var comparable = compilation.GetTypeByMetadataName("System.IComparable`1")
            ?? throw new InvalidOperationException("IComparable`1 was not found.");

        PreferredInterfaceResolver.TryGetFamily(dictionary.Construct(intType, stringType), compilation)
            .Should().Be(PreferredInterfaceFamily.IDictionary);
        PreferredInterfaceResolver.TryGetFamily(readOnlyDictionary.Construct(intType, stringType), compilation)
            .Should().Be(PreferredInterfaceFamily.IReadOnlyDictionary);
        PreferredInterfaceResolver.TryGetFamily(
                compilation.GetSpecialType(SpecialType.System_Collections_Generic_ICollection_T).Construct(intType),
                compilation)
            .Should().Be(PreferredInterfaceFamily.ICollection);
        PreferredInterfaceResolver.TryGetFamily(
                compilation.GetSpecialType(SpecialType.System_Collections_Generic_IList_T).Construct(intType),
                compilation)
            .Should().Be(PreferredInterfaceFamily.IList);
        PreferredInterfaceResolver.TryGetFamily(
                compilation.GetSpecialType(SpecialType.System_Collections_Generic_IReadOnlyList_T).Construct(intType),
                compilation)
            .Should().Be(PreferredInterfaceFamily.IReadOnlyList);
        PreferredInterfaceResolver.TryGetFamily(
                compilation.GetSpecialType(SpecialType.System_Collections_Generic_IReadOnlyCollection_T).Construct(intType),
                compilation)
            .Should().Be(PreferredInterfaceFamily.IReadOnlyCollection);
        PreferredInterfaceResolver.TryGetFamily(
                compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T).Construct(intType),
                compilation)
            .Should().Be(PreferredInterfaceFamily.IEnumerable);
        PreferredInterfaceResolver.TryGetFamily(
                compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T).Construct(keyValuePair),
                compilation)
            .Should().Be(PreferredInterfaceFamily.IEnumerableOfKeyValuePair);

        var set = compilation.GetTypeByMetadataName("System.Collections.Generic.ISet`1")
            ?? throw new InvalidOperationException("ISet`1 was not found.");
        var readOnlySet = compilation.GetTypeByMetadataName("System.Collections.Generic.IReadOnlySet`1")
            ?? throw new InvalidOperationException("IReadOnlySet`1 was not found.");
        var immutableSet = compilation.GetTypeByMetadataName("System.Collections.Immutable.IImmutableSet`1")
            ?? throw new InvalidOperationException("IImmutableSet`1 was not found.");
        var immutableList = compilation.GetTypeByMetadataName("System.Collections.Immutable.IImmutableList`1")
            ?? throw new InvalidOperationException("IImmutableList`1 was not found.");
        var immutableQueue = compilation.GetTypeByMetadataName("System.Collections.Immutable.IImmutableQueue`1")
            ?? throw new InvalidOperationException("IImmutableQueue`1 was not found.");
        var immutableStack = compilation.GetTypeByMetadataName("System.Collections.Immutable.IImmutableStack`1")
            ?? throw new InvalidOperationException("IImmutableStack`1 was not found.");
        var immutableDictionary = compilation.GetTypeByMetadataName("System.Collections.Immutable.IImmutableDictionary`2")
            ?? throw new InvalidOperationException("IImmutableDictionary`2 was not found.");
        var producerConsumer = compilation.GetTypeByMetadataName("System.Collections.Concurrent.IProducerConsumerCollection`1")
            ?? throw new InvalidOperationException("IProducerConsumerCollection`1 was not found.");
        var queryable = compilation.GetTypeByMetadataName("System.Linq.IQueryable`1")
            ?? throw new InvalidOperationException("IQueryable`1 was not found.");

        PreferredInterfaceResolver.TryGetFamily(set.Construct(intType), compilation)
            .Should().Be(PreferredInterfaceFamily.ISet);
        PreferredInterfaceResolver.TryGetFamily(readOnlySet.Construct(intType), compilation)
            .Should().Be(PreferredInterfaceFamily.IReadOnlySet);
        PreferredInterfaceResolver.TryGetFamily(immutableSet.Construct(intType), compilation)
            .Should().Be(PreferredInterfaceFamily.IImmutableSet);
        PreferredInterfaceResolver.TryGetFamily(immutableList.Construct(intType), compilation)
            .Should().Be(PreferredInterfaceFamily.IImmutableList);
        PreferredInterfaceResolver.TryGetFamily(immutableQueue.Construct(intType), compilation)
            .Should().Be(PreferredInterfaceFamily.IImmutableQueue);
        PreferredInterfaceResolver.TryGetFamily(immutableStack.Construct(intType), compilation)
            .Should().Be(PreferredInterfaceFamily.IImmutableStack);
        PreferredInterfaceResolver.TryGetFamily(immutableDictionary.Construct(intType, stringType), compilation)
            .Should().Be(PreferredInterfaceFamily.IImmutableDictionary);
        PreferredInterfaceResolver.TryGetFamily(producerConsumer.Construct(intType), compilation)
            .Should().Be(PreferredInterfaceFamily.IProducerConsumerCollection);
        PreferredInterfaceResolver.TryGetFamily(queryable.Construct(intType), compilation)
            .Should().Be(PreferredInterfaceFamily.IQueryable);
        PreferredInterfaceResolver.TryGetFamily(comparable.Construct(intType), compilation)
            .Should().BeNull();
    }

    /// <summary>
    /// <see cref="PreferredInterfaceResolver.IsConcreteImplementingType"/> accepts closed generic interfaces.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsConcreteImplementingTypeReturnsTrueForClosedGenericInterface()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var enumerable = compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T)
            .Construct(compilation.GetSpecialType(SpecialType.System_Int32));

        PreferredInterfaceResolver.IsConcreteImplementingType(enumerable).Should().BeTrue();
    }

    /// <summary>
    /// Collection-family representative lookup runs for <see cref="ICollection{T}"/> on <see cref="List{T}"/>.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsExplicitlyImplementedUsesAddRepresentativeForICollectionOnList()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var listDefinition = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")
            ?? throw new InvalidOperationException("List`1 was not found.");
        var collectionDefinition = compilation.GetSpecialType(SpecialType.System_Collections_Generic_ICollection_T);
        var listOfInt = listDefinition.Construct(intType);
        var collectionOfInt = collectionDefinition.Construct(intType);

        PreferredInterfaceResolver.IsExplicitlyImplemented(listOfInt, collectionOfInt, compilation)
            .Should()
            .BeFalse();
        PreferredInterfaceResolver.IsImplicitImplementation(listOfInt, collectionOfInt, compilation)
            .Should()
            .BeTrue();
    }

    /// <summary>
    /// Unrecognized interface families fall through to the default representative-member lookup.
    /// </summary>
    [Fact]
    [UnitTest]
    public void IsExplicitlyImplementedUsesDefaultRepresentativeForUnrecognizedFamily()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var listDefinition = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")
            ?? throw new InvalidOperationException("List`1 was not found.");
        var comparableDefinition = compilation.GetTypeByMetadataName("System.IComparable`1")
            ?? throw new InvalidOperationException("IComparable`1 was not found.");
        var listOfInt = listDefinition.Construct(intType);
        var comparableOfInt = comparableDefinition.Construct(intType);

        PreferredInterfaceResolver.IsExplicitlyImplemented(listOfInt, comparableOfInt, compilation)
            .Should()
            .BeFalse();
    }

    /// <summary>
    /// Dictionary key/value pair comparer equality is used when deduplicating candidates.
    /// </summary>
    [Fact]
    [UnitTest]
    public void DictionaryKeyValuePairComparerEqualsMatchingAndMismatchingPairs()
    {
        var compilation = BuildCompilation("namespace Mappa.Generator.Tests.UnitTests.SourceCode { internal class Placeholder { } }");
        var intType = compilation.GetSpecialType(SpecialType.System_Int32);
        var stringType = compilation.GetSpecialType(SpecialType.System_String);
        var comparerType = typeof(Mappa.Generator.Algorithm.StrategyDetectors.CommonStrategyDetectors)
            .GetNestedType("DictionaryKeyValuePairComparer", System.Reflection.BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("DictionaryKeyValuePairComparer was not found.");
        var instance = comparerType
            .GetProperty("Instance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            ?.GetValue(null)
            ?? throw new InvalidOperationException("Comparer instance was not found.");
        var pairType = typeof(ValueTuple<ITypeSymbol, ITypeSymbol>);
        var equals = comparerType.GetMethod(
                "Equals",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance,
                binder: null,
                types: [pairType, pairType],
                modifiers: null)
            ?? throw new InvalidOperationException("Equals was not found.");

        (ITypeSymbol, ITypeSymbol) left = (intType, stringType);
        (ITypeSymbol, ITypeSymbol) rightSame = (intType, stringType);
        (ITypeSymbol, ITypeSymbol) rightDifferent = (stringType, intType);

        equals.Invoke(instance, [left, rightSame]).Should().Be(true);
        equals.Invoke(instance, [left, rightDifferent]).Should().Be(false);
    }

    /// <summary>
    /// Re-entering the same resolution frame returns the recursion-guard outcome.
    /// </summary>
    [Fact]
    [UnitTest]
    public void TryResolveReturnsRecursionGuardWhenResolutionFrameAlreadyActive()
    {
        const string source = """
                              using Mappa.Attributes;
                              using System.Collections.Generic;

                              namespace Mappa.Generator.Tests.UnitTests.SourceCode;

                              [Mappa]
                              public sealed partial class Mapper
                              {
                                  public partial List<int> Map(List<int> input);
                              }
                              """;

        var compilation = BuildCompilation(source);
        var (methodContext, listOfInt) = CreateMethodContext(compilation);
        var session = methodContext.GetOrCreatePreferredInterfaceSession(compilation);
        session.ValidateAttributes(methodContext, compilation, "Map");
        session.ValidateAttributes(methodContext, compilation, "Map");

        session.TryEnterResolution(listOfInt, PreferredInterfaceFamily.IEnumerable, PreferredInterfaceSide.Source)
            .Should()
            .BeTrue();

        var resolved = PreferredInterfaceResolver.TryResolve(
            listOfInt,
            PreferredInterfaceFamily.IEnumerable,
            PreferredInterfaceSide.Source,
            methodContext,
            compilation,
            _ => true,
            out var preferred,
            out var outcome);

        resolved.Should().BeFalse();
        preferred.Should().BeNull();
        outcome.Should().Be(PreferredInterfaceResolutionOutcome.RecursionGuard);

        session.ExitResolution(listOfInt, PreferredInterfaceFamily.IEnumerable, PreferredInterfaceSide.Source);
    }

    private static (MappaMethodGeneratorContext MethodContext, INamedTypeSymbol ListOfInt) CreateMethodContext(
        CSharpCompilation compilation)
    {
        var syntaxTree = compilation.SyntaxTrees[0];
        var classDeclarationSyntax = syntaxTree.GetRoot(TestContext.Current.CancellationToken)
            .DescendantNodes()
            .OfType<ClassDeclarationSyntax>()
            .Single(classSyntax => classSyntax.Identifier.Text == "Mapper");
        var globalOptions = new MappaGlobalOptions(
            TestAnalyzerConfigOptionsProvider.FromEditorConfig("root = true"),
            syntaxTree);
        var classContext = new MappaClassGeneratorContext(
            globalOptions,
            new MappaDebug(globalOptions, _ => { }),
            compilation,
            classDeclarationSyntax);
        var methodDeclarationSyntax = classDeclarationSyntax.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Single(methodSyntax => methodSyntax.Identifier.Text == "Map");
        var mapMethod = new MapMethod(
            methodDeclarationSyntax,
            compilation.GetSemanticModel(syntaxTree),
            nullableEnabled: true,
            TestContext.Current.CancellationToken);
        var methodContext = new MappaMethodGeneratorContext(classContext, new MappaUserSettings(globalOptions), mapMethod);
        var listDefinition = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")
            ?? throw new InvalidOperationException("List`1 was not found.");
        var listOfInt = listDefinition.Construct(compilation.GetSpecialType(SpecialType.System_Int32));
        return (methodContext, listOfInt);
    }
}