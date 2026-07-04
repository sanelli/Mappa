// <copyright file="TypeSymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;

using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="ITypeSymbol"/>.
/// </summary>
internal static class TypeSymbolExtensions
{
    private const string ImmutableDictionaryFullName = "System.Collections.Immutable.ImmutableDictionary`2";
    private const string ImmutableDictionaryInterfaceFullName = "System.Collections.Immutable.IImmutableDictionary`2";
    private const string ImmutableSortedDictionaryFullName = "System.Collections.Immutable.ImmutableSortedDictionary`2";
    private const string ImmutableSetInterfaceFullName = "System.Collections.Immutable.IImmutableSet`1";
    private const string ImmutableHashSetFullName = "System.Collections.Immutable.ImmutableHashSet`1";
    private const string ImmutableSortedSetFullName = "System.Collections.Immutable.ImmutableSortedSet`1";
    private const string ImmutableListInterfaceFullName = "System.Collections.Immutable.IImmutableList`1";
    private const string ImmutableArrayFullName = "System.Collections.Immutable.ImmutableArray`1";
    private const string ImmutableListFullName = "System.Collections.Immutable.ImmutableList`1";
    private const string FrozenDictionaryFullName = "System.Collections.Frozen.FrozenDictionary`2";
    private const string FrozenSetFullName = "System.Collections.Frozen.FrozenSet`1";
    private const string SpanFullName = "System.Span`1";
    private const string ReadOnlySpanFullName = "System.ReadOnlySpan`1";
    private const string MemoryFullName = "System.Memory`1";
    private const string ReadOnlyMemoryFullName = "System.ReadOnlyMemory`1";
    private const string StackFullName = "System.Collections.Generic.Stack`1";
    private const string QueueFullName = "System.Collections.Generic.Queue`1";
    private const string Tuple1Fullname = "System.Tuple`1";
    private const string Tuple2Fullname = "System.Tuple`2";
    private const string Tuple3Fullname = "System.Tuple`3";
    private const string Tuple4Fullname = "System.Tuple`4";
    private const string Tuple5Fullname = "System.Tuple`5";
    private const string Tuple6Fullname = "System.Tuple`6";
    private const string Tuple7Fullname = "System.Tuple`7";
    private const string Tuple8Fullname = "System.Tuple`8";
    private const string DictionaryFullName = "System.Collections.Generic.Dictionary`2";
    private const string ReadOnlyDictionaryFullName = "System.Collections.ObjectModel.ReadOnlyDictionary`2";
    private const string ReadOnlyCollectionFullName = "System.Collections.ObjectModel.ReadOnlyCollection`1";
    private const string ReadOnlySetFullName = "System.Collections.ObjectModel.ReadOnlySet`1";
    private const string DictionaryInterfaceFullName = "System.Collections.Generic.IDictionary`2";
    private const string ReadOnlyDictionaryInterfaceFullName = "System.Collections.Generic.IReadOnlyDictionary`2";
    private const string ListFullName = "System.Collections.Generic.List`1";
    private const string TimeSpanFullName = "System.TimeSpan";
    private const string UriFullName = "System.Uri";
    private const string GuidFullName = "System.Guid";
    private const string DateTimeOffsetFullName = "System.DateTimeOffset";
    private const string KeyValuePairFullName = "System.Collections.Generic.KeyValuePair`2";
    private const string SetInterfaceFullName = "System.Collections.Generic.ISet`1";
    private const string ReadOnlySetInterfaceFullName = "System.Collections.Generic.IReadOnlySet`1";
    private const string HashSetFullName = "System.Collections.Generic.HashSet`1";
    private const string ImmutableStackInterfaceFullName = "System.Collections.Immutable.IImmutableStack`1";
    private const string ImmutableStackFullName = "System.Collections.Immutable.ImmutableStack`1";
    private const string ImmutableQueueInterfaceFullName = "System.Collections.Immutable.IImmutableQueue`1";
    private const string ImmutableQueueFullName = "System.Collections.Immutable.ImmutableQueue`1";
    private const string BlockingCollectionFullName = "System.Collections.Concurrent.BlockingCollection`1";
    private const string ConcurrentBagFullName = "System.Collections.Concurrent.ConcurrentBag`1";
    private const string ConcurrentStackFullName = "System.Collections.Concurrent.ConcurrentStack`1";
    private const string ConcurrentQueueFullName = "System.Collections.Concurrent.ConcurrentQueue`1";
    private const string ProducerConsumerCollectionInterfaceFullName = "System.Collections.Concurrent.IProducerConsumerCollection`1";
    private const string ExceptionFullName = "System.Exception";

    /// <summary>
    /// Check if the type is <see cref="Void"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <c>void</c>.</returns>
    internal static bool IsVoid(this ITypeSymbol typeSymbol)
        => typeSymbol.SpecialType == SpecialType.System_Void;

    /// <summary>
    /// Check if the type is <see cref="object"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="object"/>.</returns>
    internal static bool IsObject(this ITypeSymbol typeSymbol)
        => typeSymbol.SpecialType == SpecialType.System_Object;

    /// <summary>
    /// Check if the type is <see cref="Enum"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Enum"/>.</returns>
    internal static bool IsEnum(this ITypeSymbol typeSymbol)
        => typeSymbol is { TypeKind: TypeKind.Enum, BaseType.SpecialType: SpecialType.System_Enum };

    /// <summary>
    /// Check if the type is <see cref="string"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="string"/>.</returns>
    internal static bool IsString(this ITypeSymbol typeSymbol)
        => typeSymbol.SpecialType == SpecialType.System_String;

    /// <summary>
    /// Check if the type is <see cref="Nullable{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Nullable{T}"/>.</returns>
    internal static bool IsValueTypeNullable(this ITypeSymbol typeSymbol)
        => typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

    /// <summary>
    /// Check if the type is an array with rank one.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is an array.</returns>
    internal static bool IsArray(this ITypeSymbol typeSymbol)
        => typeSymbol is IArrayTypeSymbol { Rank: 1 };

    /// <summary>
    /// Gets accessible instance fields declared on the type and its base types.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="within">The symbol within which field access must be accessible.</param>
    /// <returns>The accessible instance fields.</returns>
    internal static IEnumerable<IFieldSymbol> GetAccessibleInstanceFields(
        this ITypeSymbol typeSymbol,
        Compilation compilation,
        ISymbol within)
    {
        var currentType = typeSymbol;
        while (currentType is not null)
        {
            foreach (var field in currentType.GetMembers().OfType<IFieldSymbol>())
            {
                if (field.IsStatic || field.IsConst)
                {
                    continue;
                }

                if (compilation.IsSymbolAccessibleWithin(field, within))
                {
                    yield return field;
                }
            }

            currentType = currentType.BaseType;
        }
    }

    /// <summary>
    /// Check if the type is <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IList{T}"/>.</returns>
    internal static bool IsIList(this ITypeSymbol typeSymbol)
        => typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IList_T;

    /// <summary>
    /// Check if the type is <see cref="List{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="List{T}"/>.</returns>
    internal static bool IsList(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var listType = compilation.GetTypeByMetadataName(ListFullName);
        var isList = SymbolEqualityComparer.Default.Equals(listType, typeSymbol.OriginalDefinition);
        return isList;
    }

    /// <summary>
    /// Check if the type is <see cref="Span{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Span{T}"/>.</returns>
    internal static bool IsSpan(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var spanType = compilation.GetTypeByMetadataName(SpanFullName);
        var isSpan = SymbolEqualityComparer.Default.Equals(spanType, typeSymbol.OriginalDefinition);
        return isSpan;
    }

    /// <summary>
    /// Check if the type is <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ReadOnlySpan{T}"/>.</returns>
    internal static bool IsReadOnlySpan(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var spanType = compilation.GetTypeByMetadataName(ReadOnlySpanFullName);
        var isSpan = SymbolEqualityComparer.Default.Equals(spanType, typeSymbol.OriginalDefinition);
        return isSpan;
    }

    /// <summary>
    /// Check if the type is <see cref="Memory{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Memory{T}"/>.</returns>
    internal static bool IsMemory(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var memoryType = compilation.GetTypeByMetadataName(MemoryFullName);
        var isMemory = SymbolEqualityComparer.Default.Equals(memoryType, typeSymbol.OriginalDefinition);
        return isMemory;
    }

    /// <summary>
    /// Check if the type is <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ReadOnlyMemory{T}"/>.</returns>
    internal static bool IsReadOnlyMemory(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var readOnlyMemoryType = compilation.GetTypeByMetadataName(ReadOnlyMemoryFullName);
        var isMemory = SymbolEqualityComparer.Default.Equals(readOnlyMemoryType, typeSymbol.OriginalDefinition);
        return isMemory;
    }

    /// <summary>
    /// Check if the type is <see cref="IDictionary{K,V}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IDictionary{K,V}"/>.</returns>
    internal static bool IsIDictionary(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var listType = compilation.GetTypeByMetadataName(DictionaryInterfaceFullName);
        var isList = SymbolEqualityComparer.Default.Equals(listType, typeSymbol.OriginalDefinition);
        return isList;
    }

    /// <summary>
    /// Check if the type is <see cref="IReadOnlyDictionary{K,V}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IReadOnlyDictionary{K,V}"/>.</returns>
    internal static bool IsIReadOnlyDictionary(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var listType = compilation.GetTypeByMetadataName(ReadOnlyDictionaryInterfaceFullName);
        var isList = SymbolEqualityComparer.Default.Equals(listType, typeSymbol.OriginalDefinition);
        return isList;
    }

    /// <summary>
    /// Check if the type is <see cref="Dictionary{K,V}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Dictionary{K,V}"/>.</returns>
    internal static bool IsDictionary(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var dictionaryType = compilation.GetTypeByMetadataName(DictionaryFullName);
        var isDictionary = SymbolEqualityComparer.Default.Equals(dictionaryType, typeSymbol.OriginalDefinition);
        return isDictionary;
    }

    /// <summary>
    /// Check if the type is <see cref="ReadOnlyDictionary{K,V}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ReadOnlyDictionary{K,V}"/>.</returns>
    internal static bool IsReadOnlyDictionary(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var dictionaryType = compilation.GetTypeByMetadataName(ReadOnlyDictionaryFullName);
        var isDictionary = SymbolEqualityComparer.Default.Equals(dictionaryType, typeSymbol.OriginalDefinition);
        return isDictionary;
    }

    /// <summary>
    /// Check if the type is <see cref="ReadOnlyCollection{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ReadOnlyCollection{K}"/>.</returns>
    internal static bool IsReadOnlyCollection(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var collectionSymbol = compilation.GetTypeByMetadataName(ReadOnlyCollectionFullName);
        var isCollection = SymbolEqualityComparer.Default.Equals(collectionSymbol, typeSymbol.OriginalDefinition);
        return isCollection;
    }

    /// <summary>
    /// Check if the type is <c>ReadOnlySet{T}</c>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <c>ReadOnlySet{T}</c>..</returns>
    internal static bool IsReadOnlySet(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var setSymbol = compilation.GetTypeByMetadataName(ReadOnlySetFullName);
        var isCollection = SymbolEqualityComparer.Default.Equals(setSymbol, typeSymbol.OriginalDefinition);
        return isCollection;
    }

    /// <summary>
    /// Check if the type is <see cref="ImmutableDictionary{K,V}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ImmutableDictionary{K,V}"/>.</returns>
    internal static bool IsImmutableDictionary(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var dictionaryType = compilation.GetTypeByMetadataName(ImmutableDictionaryFullName);
        var isDictionary = SymbolEqualityComparer.Default.Equals(dictionaryType, typeSymbol.OriginalDefinition);
        return isDictionary;
    }

    /// <summary>
    /// Check if the type is <see cref="IImmutableDictionary{K,V}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IImmutableDictionary{K,V}"/>.</returns>
    internal static bool IsIImmutableDictionary(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var dictionaryType = compilation.GetTypeByMetadataName(ImmutableDictionaryInterfaceFullName);
        var isDictionary = SymbolEqualityComparer.Default.Equals(dictionaryType, typeSymbol.OriginalDefinition);
        return isDictionary;
    }

    /// <summary>
    /// Check if the type is <see cref="ImmutableSortedDictionary{K,V}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ImmutableSortedDictionary{K,V}"/>.</returns>
    internal static bool IsImmutableSortedDictionary(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var dictionaryType = compilation.GetTypeByMetadataName(ImmutableSortedDictionaryFullName);
        var isDictionary = SymbolEqualityComparer.Default.Equals(dictionaryType, typeSymbol.OriginalDefinition);
        return isDictionary;
    }

    /// <summary>
    /// Check if the type is <see cref="ImmutableSortedSet{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ImmutableSortedSet{T}"/>.</returns>
    internal static bool IsImmutableSortedSet(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var setType = compilation.GetTypeByMetadataName(ImmutableSortedSetFullName);
        var isSet = SymbolEqualityComparer.Default.Equals(setType, typeSymbol.OriginalDefinition);
        return isSet;
    }

    /// <summary>
    /// Check if the type is <see cref="ImmutableHashSet{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ImmutableHashSet{T}"/>.</returns>
    internal static bool IsImmutableHashSet(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var setType = compilation.GetTypeByMetadataName(ImmutableHashSetFullName);
        var isSet = SymbolEqualityComparer.Default.Equals(setType, typeSymbol.OriginalDefinition);
        return isSet;
    }

    /// <summary>
    /// Check if the type is <see cref="IImmutableSet{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IImmutableSet{T}"/>.</returns>
    internal static bool IsIImmutableSet(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var setType = compilation.GetTypeByMetadataName(ImmutableSetInterfaceFullName);
        var isSet = SymbolEqualityComparer.Default.Equals(setType, typeSymbol.OriginalDefinition);
        return isSet;
    }

    /// <summary>
    /// Check if the type is <see cref="FrozenDictionary{K,V}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="FrozenDictionary{K,V}"/>.</returns>
    internal static bool IsFrozenDictionary(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var dictionaryType = compilation.GetTypeByMetadataName(FrozenDictionaryFullName);
        var isDictionary = SymbolEqualityComparer.Default.Equals(dictionaryType, typeSymbol.OriginalDefinition);
        return isDictionary;
    }

    /// <summary>
    /// Check if the type is <see cref="FrozenSet{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="FrozenSet{T}"/>.</returns>
    internal static bool IsFrozenSet(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var frozenSetType = compilation.GetTypeByMetadataName(FrozenSetFullName);
        var isFrozenSet = SymbolEqualityComparer.Default.Equals(frozenSetType, typeSymbol.OriginalDefinition);
        return isFrozenSet;
    }

    /// <summary>
    /// Check if the type is <see cref="IImmutableList{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IImmutableList{T}"/>.</returns>
    internal static bool IsIImmutableList(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var listType = compilation.GetTypeByMetadataName(ImmutableListInterfaceFullName);
        var isSet = SymbolEqualityComparer.Default.Equals(listType, typeSymbol.OriginalDefinition);
        return isSet;
    }

    /// <summary>
    /// Check if the type is <see cref="ImmutableArray{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ImmutableArray{T}"/>.</returns>
    internal static bool IsImmutableArray(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var listType = compilation.GetTypeByMetadataName(ImmutableArrayFullName);
        var isSet = SymbolEqualityComparer.Default.Equals(listType, typeSymbol.OriginalDefinition);
        return isSet;
    }

    /// <summary>
    /// Check if the type is <see cref="ImmutableList{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ImmutableList{T}"/>.</returns>
    internal static bool IsImmutableList(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var listType = compilation.GetTypeByMetadataName(ImmutableListFullName);
        var isSet = SymbolEqualityComparer.Default.Equals(listType, typeSymbol.OriginalDefinition);
        return isSet;
    }

    /// <summary>
    /// Check if the type is <see cref="ICollection{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ICollection{T}"/>.</returns>
    internal static bool IsICollection(this ITypeSymbol typeSymbol)
        => typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_ICollection_T;

    /// <summary>
    /// Check if the type is <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IReadOnlyCollection{T}"/>.</returns>
    internal static bool IsIReadOnlyCollection(this ITypeSymbol typeSymbol)
        => typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IReadOnlyCollection_T;

    /// <summary>
    /// Check if the type is <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IEnumerable{T}"/>.</returns>
    internal static bool IsIEnumerable(this ITypeSymbol typeSymbol)
        => typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T;

    /// <summary>
    /// Check if the type is <see cref="IEnumerable{T}"/> or implements <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is or implements <see cref="IEnumerable{T}"/>, <c>false</c> otherwise.</returns>
    internal static bool IsOrImplementIEnumerable(this ITypeSymbol typeSymbol)
        => typeSymbol.IsIEnumerable() || typeSymbol.AllInterfaces.Any(@interface => @interface.IsIEnumerable());

    /// <summary>
    /// Check if the type is <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>.</returns>
    internal static bool IsIEnumerableOfKeyValuePairs(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsIEnumerable() && typeSymbol.GetElementType().IsKeyValuePair(compilation);

    /// <summary>
    /// Check if the type is <see cref="IEnumerable{T}"/> or implements <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is or implements <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey,TValue}"/>, <c>false</c> otherwise.</returns>
    internal static bool IsOrImplementIEnumerableOfKeyValuePair(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsIEnumerableOfKeyValuePairs(compilation) || typeSymbol.AllInterfaces.Any(@interface => @interface.IsIEnumerableOfKeyValuePairs(compilation));

    /// <summary>
    /// Check if the type is <see cref="ICollection{T}"/> or implements <see cref="ICollection{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is or implements <see cref="ICollection{T}"/>, <c>false</c> otherwise.</returns>
    internal static bool IsOrImplementICollection(this ITypeSymbol typeSymbol)
        => typeSymbol.IsICollection() || typeSymbol.ImplementICollection();

    /// <summary>
    /// Check if the type is a collection that can be filled after constructor invocation
    /// on a get-only or inaccessible-setter property.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol can be filled post-construction.</returns>
    internal static bool IsPostInitializationCollectionType(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsOrImplementIDictionary(compilation)
           || typeSymbol.IsOrImplementICollection()
           || typeSymbol.IsOrDerivedFromStack(compilation)
           || typeSymbol.IsOrDerivedFromConcurrentStack(compilation)
           || typeSymbol.IsOrDerivedFromQueue(compilation)
           || typeSymbol.IsOrImplementConcurrentQueue(compilation)
           || typeSymbol.IsOrDerivedFromConcurrentBag(compilation)
           || typeSymbol.IsOrDerivedFromBlockingCollection(compilation);

    /// <summary>
    /// Check if the type is <see cref="ICollection{T}"/> or implements <see cref="ICollection{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is or implements <see cref="ICollection{T}"/>, <c>false</c> otherwise.</returns>
    internal static bool ImplementICollection(this ITypeSymbol typeSymbol)
        => typeSymbol.AllInterfaces.Any(@interface => @interface.IsICollection());

    /// <summary>
    /// Check if the type is <see cref="IReadOnlyCollection{T}"/> or implements <see cref="IReadOnlyCollection{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is or implements <see cref="ICollection{T}"/>, <c>false</c> otherwise.</returns>
    internal static bool IsOrImplementIReadOnlyCollection(this ITypeSymbol typeSymbol)
        => typeSymbol.IsIReadOnlyCollection() || typeSymbol.AllInterfaces.Any(@interface => @interface.IsIReadOnlyCollection());

    /// <summary>
    /// Check if the type is <see cref="IList{T}"/> or implements <see cref="IList{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is or implements <see cref="IList{T}"/>, <c>false</c> otherwise.</returns>
    internal static bool IsOrImplementIList(this ITypeSymbol typeSymbol)
        => typeSymbol.IsIList() || typeSymbol.AllInterfaces.Any(@interface => @interface.IsIList());

    /// <summary>
    /// Check if the type is <see cref="IDictionary{K,V}"/> or implements <see cref="IDictionary{K,V}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol implements <see cref="IDictionary{K,V}"/>.</returns>
    internal static bool IsOrImplementIDictionary(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsIDictionary(compilation) || typeSymbol.AllInterfaces.Any(@interface => @interface.IsIDictionary(compilation));

    /// <summary>
    /// Check if the type is <see cref="IReadOnlyDictionary{K,V}"/> or implements <see cref="IReadOnlyDictionary{K,V}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol implements <see cref="IDictionary{K,V}"/>.</returns>
    internal static bool IsOrImplementIReadOnlyDictionary(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsIReadOnlyDictionary(compilation) || typeSymbol.AllInterfaces.Any(@interface => @interface.IsIReadOnlyDictionary(compilation));

    /// <summary>
    /// Check if the type is <see cref="Tuple"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IDictionary{K,V}"/>.</returns>
    internal static bool IsTuple(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsTupleType
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(Tuple1Fullname), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(Tuple2Fullname), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(Tuple3Fullname), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(Tuple4Fullname), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(Tuple5Fullname), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(Tuple6Fullname), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(Tuple7Fullname), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(Tuple8Fullname), typeSymbol.OriginalDefinition);

    /// <summary>
    /// Gets the element type of the container.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns>The element type of the container.</returns>
    /// <exception cref="MappaGeneratorException">If <paramref name="typeSymbol"/> is not an array or a generic type.</exception>
    internal static ITypeSymbol GetElementType(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            return arrayTypeSymbol.ElementType;
        }

        if (typeSymbol is INamedTypeSymbol { TypeArguments.Length: 1 } namedTypeSymbol)
        {
            return namedTypeSymbol.TypeArguments.First();
        }

        if (typeSymbol is INamedTypeSymbol nonGenericNamedTypeSymbol)
        {
            foreach (var @interface in nonGenericNamedTypeSymbol.AllInterfaces)
            {
                if (@interface is not null && @interface.IsGenericType && @interface.IsIEnumerable())
                {
                    return @interface.TypeArguments.First();
                }
            }
        }

        throw new MappaGeneratorException($"Cannot obtain element type of \"{typeSymbol.ToDisplayString()}\"");
    }

    /// <summary>
    /// Gets the key and value type of the container.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The element type of the key and value of the container.</returns>
    /// <exception cref="MappaGeneratorException">If <paramref name="typeSymbol"/> is not an array or a generic type.</exception>
    internal static (ITypeSymbol KeyType, ITypeSymbol ValueType) GetKeyAndValueTypes(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        if (typeSymbol is INamedTypeSymbol { TypeArguments.Length: 2 } namedTypeSymbol)
        {
            return (namedTypeSymbol.TypeArguments.First(), namedTypeSymbol.TypeArguments.Last());
        }

        if (typeSymbol is INamedTypeSymbol { TypeArguments.Length: 1 } mightBeEnumerable)
        {
            var enumerableElementType = mightBeEnumerable.GetElementType();
            if (enumerableElementType.IsKeyValuePair(compilation))
            {
                return enumerableElementType.GetKeyAndValueTypes(compilation);
            }
        }

        // The type might be non-generic but still implement an IDictionary
        // so we need to check all the interfaces to get the type argument of
        // the first IDictionary{TKey, TValue}.
        if (typeSymbol is INamedTypeSymbol symbol)
        {
            foreach (var @interface in symbol.AllInterfaces)
            {
                if (@interface is not null && @interface.IsGenericType && @interface.IsIDictionary(compilation))
                {
                    return (@interface.TypeArguments.First(), @interface.TypeArguments.Last());
                }

                if (@interface is not null && @interface.IsGenericType && @interface.IsIEnumerableOfKeyValuePairs(compilation))
                {
                    return @interface.GetKeyAndValueTypes(compilation);
                }
            }
        }

        throw new MappaGeneratorException($"Cannot obtain key and value types of \"{typeSymbol.ToDisplayString()}\"");
    }

    /// <summary>
    /// Check if the type is <see cref="System.Collections.Concurrent.IProducerConsumerCollection{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="System.Collections.Concurrent.IProducerConsumerCollection{T}"/>.</returns>
    internal static bool IsIProducerConsumerCollection(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var interfaceType = compilation.GetTypeByMetadataName(ProducerConsumerCollectionInterfaceFullName);
        var isProducerConsumerCollectionInterface = SymbolEqualityComparer.Default.Equals(interfaceType, typeSymbol.OriginalDefinition);
        return isProducerConsumerCollectionInterface;
    }

    /// <summary>
    /// Gets all the type parameters of this type.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="typeArguments">The type parameters associated to <paramref name="typeSymbol"/>.</param>
    /// <returns><c>true</c> if the element contains at least one type parameter.</returns>
    internal static bool TryGetTypeArguments(this ITypeSymbol typeSymbol, out ImmutableArray<ITypeSymbol> typeArguments)
    {
        if (typeSymbol is INamedTypeSymbol { TypeArguments.Length: > 0 } namedTypeSymbol)
        {
            typeArguments = namedTypeSymbol.TypeArguments;
            return true;
        }

        typeArguments = [];
        return false;
    }

    /// <summary>
    /// Check if the type is a numeric type.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is numeric.</returns>
    internal static bool IsNumeric(this ITypeSymbol typeSymbol)
    {
        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Decimal:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Check if the type is <see cref="byte"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="byte"/>.</returns>
    internal static bool IsByte(this ITypeSymbol typeSymbol)
    {
        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Byte:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Check if the type is a boolean type.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is boolean.</returns>
    internal static bool IsBoolean(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.SpecialType switch
        {
            SpecialType.System_Boolean => true,
            _ => false,
        };
    }

    /// <summary>
    /// Check if the type is <see cref="long"/> type.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="long"/>, <c>false</c> otherwise.</returns>
    internal static bool IsLong(this ITypeSymbol typeSymbol)
    {
        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Int64:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Check if the type is <see cref="double"/> type.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="double"/>, <c>false</c> otherwise.</returns>
    internal static bool IsDouble(this ITypeSymbol typeSymbol)
    {
        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Double:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Check if the type is <see cref="double"/> type
    /// or a numeric type implicitly convertible.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="double"/> or implicitply convertible to <see cref="double"/>, <c>false</c> otherwise.</returns>
    internal static bool IsDoubleOrNumericImplicitlyConvertible(this ITypeSymbol typeSymbol)
    {
        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Check if the type is <see cref="long"/> type or a compatible smaller
    /// numeric type.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="long"/> type or a compatible smaller
    /// numeric type, <c>false</c> otherwise.</returns>
    internal static bool IsLongOrNumericCanBeImplicitlyCastedToLong(this ITypeSymbol typeSymbol)
    {
        switch (typeSymbol.SpecialType)
        {
            case SpecialType.System_Int64:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_SByte:
            case SpecialType.System_Byte:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Check if the type is <see cref="DateTime"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="DateTime"/>.</returns>
    internal static bool IsDateTime(this ITypeSymbol typeSymbol)
        => typeSymbol.SpecialType == SpecialType.System_DateTime;

    /// <summary>
    /// Check if the type is <see cref="DateTime"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="DateTime"/>.</returns>
    internal static bool IsDateTimeOffset(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var dateTimeOffsetType = compilation.GetTypeByMetadataName(DateTimeOffsetFullName);
        var isDateTimeOffsetType = SymbolEqualityComparer.Default.Equals(dateTimeOffsetType, typeSymbol.OriginalDefinition);
        return isDateTimeOffsetType;
    }

    /// <summary>
    /// Check if the type is <see cref="Nullable{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="innerType">The actual generic type inside <see cref="Nullable{T}"/>.</param>
    /// <param name="nullableEnabled"><c>true</c> if nullable enabled.</param>
    /// <returns><c>true</c> if the type symbol is <c>void</c>.</returns>
    internal static bool IsNullableGenericType(
        this ITypeSymbol typeSymbol,
        ITypeSymbol innerType,
        bool nullableEnabled)
    {
        bool isNullableT = typeSymbol is
            { IsDefinition: false, OriginalDefinition.SpecialType: SpecialType.System_Nullable_T };
        if (!isNullableT)
        {
            return false;
        }

        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
        {
            return false;
        }

        if (namedTypeSymbol.Arity is not 1)
        {
            return false;
        }

        var actualTypeParameter = namedTypeSymbol.TypeArguments.Single();
        var typeParameterIsTheSame = actualTypeParameter.IsEqualTo(innerType, nullableEnabled);
        return typeParameterIsTheSame;
    }

    /// <summary>
    /// Check if two types are the same using the nullability flag.
    /// </summary>
    /// <param name="left">The first type.</param>
    /// <param name="right">The second type.</param>
    /// <param name="isNullableEnabled"><c>true</c> if nullable is enabled.</param>
    /// <returns><c>true</c> if the types are the same.</returns>
    internal static bool IsEqualTo(this ITypeSymbol left, ITypeSymbol right, bool isNullableEnabled)
    {
        if (SymbolEqualityComparer.Default.Equals(left, right))
        {
            if (isNullableEnabled)
            {
                if (left.NullableAnnotation == NullableAnnotation.None
                    || right.NullableAnnotation == NullableAnnotation.None)
                {
                    return true;
                }

                return left.NullableAnnotation == right.NullableAnnotation;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the type is <c>void</c>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation used to obtain the required types.</param>
    /// <returns><c>true</c> if the type symbol is <c>void</c>.</returns>
    internal static bool IsAnyTaskType(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var task = compilation.GetTypeSymbol<Task>();
        var taskGeneric = compilation.GetTypeSymbol(typeof(Task<>));
        var valueTask = compilation.GetTypeSymbol<ValueTask>();
        var valueTaskGeneric = compilation.GetTypeSymbol(typeof(ValueTask<>));

        if (!typeSymbol.IsDefinition)
        {
            typeSymbol = typeSymbol.OriginalDefinition;
        }

        return SymbolEqualityComparer.Default.Equals(typeSymbol, task)
               || SymbolEqualityComparer.Default.Equals(typeSymbol, taskGeneric)
               || SymbolEqualityComparer.Default.Equals(typeSymbol, valueTask)
               || SymbolEqualityComparer.Default.Equals(typeSymbol, valueTaskGeneric);
    }

    /// <summary>
    /// Get all the enum names and values.
    /// </summary>
    /// <param name="typeSymbol">The type symbol representing an enumeration.</param>
    /// <returns>A sequence of enumeration name and values.</returns>
    internal static IEnumerable<(string Name, object Value)> GetEnumValues(this ITypeSymbol typeSymbol)
    {
        foreach (var member in typeSymbol.GetMembers())
        {
            if (member is IFieldSymbol { ConstantValue: not null } fieldSymbol)
            {
                yield return (fieldSymbol.Name, fieldSymbol.ConstantValue);
            }
        }
    }

    /// <summary>
    /// Gets the enum member names shared by source and target enums when mapping by member name.
    /// </summary>
    /// <param name="sourceType">The source enum type.</param>
    /// <param name="targetType">The target enum type.</param>
    /// <returns>The shared enum member names in ascending order.</returns>
    internal static string[] GetSharedEnumMemberNamesByName(this ITypeSymbol sourceType, ITypeSymbol targetType)
        => sourceType.GetEnumValues().Select(enumValue => enumValue.Name)
            .Intersect(targetType.GetEnumValues().Select(enumValue => enumValue.Name))
            .OrderBy(name => name)
            .ToArray();

    /// <summary>
    /// Gets the source enum member names that have no matching name in the target enum.
    /// </summary>
    /// <param name="sourceType">The source enum type.</param>
    /// <param name="targetType">The target enum type.</param>
    /// <returns>The unmapped source enum member names in ascending order.</returns>
    internal static string[] GetUnmappedEnumMemberNamesByName(this ITypeSymbol sourceType, ITypeSymbol targetType)
        => sourceType.GetEnumValues().Select(enumValue => enumValue.Name)
            .Except(targetType.GetEnumValues().Select(enumValue => enumValue.Name))
            .OrderBy(name => name)
            .ToArray();

    /// <summary>
    /// Gets source and target enum member name pairs shared by underlying numeric value.
    /// </summary>
    /// <param name="sourceType">The source enum type.</param>
    /// <param name="targetType">The target enum type.</param>
    /// <returns>The shared enum member mappings in ascending source member name order.</returns>
    internal static (string SourceMemberName, string TargetMemberName)[] GetSharedEnumMemberMappingsByValue(
        this ITypeSymbol sourceType,
        ITypeSymbol targetType)
    {
        var targetMembersByValue = targetType.GetEnumValues()
            .GroupBy(enumValue => Convert.ToDecimal(enumValue.Value, CultureInfo.InvariantCulture))
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(enumValue => enumValue.Name).First().Name);

        return sourceType.GetEnumValues()
            .Select(sourceEnumValue => (SourceEnumValue: sourceEnumValue, Value: Convert.ToDecimal(sourceEnumValue.Value, CultureInfo.InvariantCulture)))
            .Where(pair => targetMembersByValue.ContainsKey(pair.Value))
            .OrderBy(pair => pair.SourceEnumValue.Name)
            .Select(pair => (pair.SourceEnumValue.Name, targetMembersByValue[pair.Value]))
            .ToArray();
    }

    /// <summary>
    /// Gets the source enum member names that have no matching numeric value in the target enum.
    /// </summary>
    /// <param name="sourceType">The source enum type.</param>
    /// <param name="targetType">The target enum type.</param>
    /// <returns>The unmapped source enum member names in ascending order.</returns>
    internal static string[] GetUnmappedEnumMemberNamesByValue(this ITypeSymbol sourceType, ITypeSymbol targetType)
    {
        var targetValues = new HashSet<decimal>(
            targetType.GetEnumValues().Select(enumValue => Convert.ToDecimal(enumValue.Value, CultureInfo.InvariantCulture)));

        return sourceType.GetEnumValues()
            .Where(sourceEnumValue => !targetValues.Contains(Convert.ToDecimal(sourceEnumValue.Value, CultureInfo.InvariantCulture)))
            .Select(sourceEnumValue => sourceEnumValue.Name)
            .OrderBy(name => name)
            .ToArray();
    }

    /// <summary>
    /// Gets the Description attribute value for an enum field, if present and non-empty.
    /// </summary>
    /// <param name="fieldSymbol">The enum field symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>The description text, or <c>null</c> if missing or empty.</returns>
    internal static string? GetEnumMemberDescription(this IFieldSymbol fieldSymbol, Compilation compilation)
    {
        var descriptionAttributeSymbol = compilation.GetTypeByMetadataName("System.ComponentModel.DescriptionAttribute");

        foreach (var attribute in fieldSymbol.GetAttributes())
        {
            var attributeClass = attribute.AttributeClass;
            if (attributeClass is null)
            {
                continue;
            }

            var isDescriptionAttribute = descriptionAttributeSymbol is not null
                && SymbolEqualityComparer.Default.Equals(attributeClass, descriptionAttributeSymbol);
            isDescriptionAttribute |= attributeClass.Name == "DescriptionAttribute"
                && attributeClass.ContainingNamespace.ToDisplayString() == "System.ComponentModel";
            if (!isDescriptionAttribute)
            {
                continue;
            }

            if (attribute.ConstructorArguments.Length > 0
                && attribute.ConstructorArguments[0].Value is string description
                && !string.IsNullOrWhiteSpace(description))
            {
                return description;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets enum member names and Description attribute values for members that have a non-empty description.
    /// </summary>
    /// <param name="typeSymbol">The enum type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>Member names paired with description text.</returns>
    internal static (string Name, string Description)[] GetEnumMembersWithDescriptions(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.GetEnumValues()
            .Select(enumValue => typeSymbol.GetMembers(enumValue.Name).OfType<IFieldSymbol>().First())
            .Select(fieldSymbol => (fieldSymbol.Name, Description: fieldSymbol.GetEnumMemberDescription(compilation)))
            .Where(member => member.Description is not null)
            .Select(member => (member.Name, member.Description!))
            .OrderBy(member => member.Name)
            .ToArray();

    /// <summary>
    /// Gets enum member names that lack a non-empty Description attribute.
    /// </summary>
    /// <param name="typeSymbol">The enum type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns>Member names missing a Description attribute.</returns>
    internal static string[] GetEnumMemberNamesMissingDescription(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.GetEnumValues()
            .Select(enumValue => typeSymbol.GetMembers(enumValue.Name).OfType<IFieldSymbol>().First())
            .Where(fieldSymbol => fieldSymbol.GetEnumMemberDescription(compilation) is null)
            .Select(fieldSymbol => fieldSymbol.Name)
            .OrderBy(name => name)
            .ToArray();

    /// <summary>
    /// Gets duplicate Description values within an enum.
    /// </summary>
    /// <param name="typeSymbol">The enum type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="caseInsensitive">Whether description comparison is case-insensitive.</param>
    /// <returns>Formatted duplicate description groups.</returns>
    internal static string[] GetDuplicateDescriptionGroups(this ITypeSymbol typeSymbol, Compilation compilation, bool caseInsensitive)
    {
        var comparer = caseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var membersByDescription = typeSymbol.GetEnumMembersWithDescriptions(compilation)
            .GroupBy(member => member.Description, comparer)
            .Where(group => group.Count() > 1)
            .Select(group => string.Join(", ", group.Select(member => $"'{member.Name}'")))
            .OrderBy(group => group, StringComparer.Ordinal)
            .ToArray();

        return membersByDescription;
    }

    /// <summary>
    /// Gets source and target enum member name pairs matched by member name case-insensitively.
    /// </summary>
    /// <param name="sourceType">The source enum type.</param>
    /// <param name="targetType">The target enum type.</param>
    /// <returns>The shared enum member mappings in ascending source member name order.</returns>
    internal static (string SourceMemberName, string TargetMemberName)[] GetSharedEnumMemberMappingsByNameCaseInsensitive(
        this ITypeSymbol sourceType,
        ITypeSymbol targetType)
    {
        var targetMemberNames = targetType.GetEnumValues().Select(enumValue => enumValue.Name).ToArray();
        return sourceType.GetEnumValues()
            .Select(sourceEnumValue => (SourceName: sourceEnumValue.Name, TargetName: targetMemberNames.FirstOrDefault(targetName => string.Equals(sourceEnumValue.Name, targetName, StringComparison.OrdinalIgnoreCase))))
            .Where(pair => pair.TargetName is not null)
            .OrderBy(pair => pair.SourceName)
            .Select(pair => (pair.SourceName, pair.TargetName!))
            .ToArray();
    }

    /// <summary>
    /// Gets source and target enum member name pairs matched by Description attribute value.
    /// </summary>
    /// <param name="sourceType">The source enum type.</param>
    /// <param name="targetType">The target enum type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="caseInsensitive">Whether description comparison is case-insensitive.</param>
    /// <returns>The shared enum member mappings in ascending source member name order.</returns>
    internal static (string SourceMemberName, string TargetMemberName)[] GetSharedEnumMemberMappingsByDescription(
        this ITypeSymbol sourceType,
        ITypeSymbol targetType,
        Compilation compilation,
        bool caseInsensitive)
    {
        var comparer = caseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var targetMembersByDescription = targetType.GetEnumMembersWithDescriptions(compilation)
            .GroupBy(member => member.Description, comparer)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(member => member.Name).First().Name,
                comparer);

        return sourceType.GetEnumMembersWithDescriptions(compilation)
            .Where(sourceMember => targetMembersByDescription.ContainsKey(sourceMember.Description))
            .OrderBy(sourceMember => sourceMember.Name)
            .Select(sourceMember => (sourceMember.Name, targetMembersByDescription[sourceMember.Description]))
            .ToArray();
    }

    /// <summary>
    /// Gets the source enum member names that have no matching Description in the target enum.
    /// </summary>
    /// <param name="sourceType">The source enum type.</param>
    /// <param name="targetType">The target enum type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="caseInsensitive">Whether description comparison is case-insensitive.</param>
    /// <returns>The unmapped source enum member names in ascending order.</returns>
    internal static string[] GetUnmappedEnumMemberNamesByDescription(
        this ITypeSymbol sourceType,
        ITypeSymbol targetType,
        Compilation compilation,
        bool caseInsensitive)
    {
        var comparer = caseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var targetDescriptions = new HashSet<string>(
            targetType.GetEnumMembersWithDescriptions(compilation).Select(member => member.Description),
            comparer);

        return sourceType.GetEnumMembersWithDescriptions(compilation)
            .Where(sourceMember => !targetDescriptions.Contains(sourceMember.Description))
            .Select(sourceMember => sourceMember.Name)
            .OrderBy(name => name)
            .ToArray();
    }

    /// <summary>
    /// Determines whether case-insensitive enum member name mapping between source and target is ambiguous.
    /// </summary>
    /// <param name="sourceType">The source enum type.</param>
    /// <param name="targetType">The target enum type.</param>
    /// <param name="ambiguityDetails">The ambiguity details when mapping is ambiguous.</param>
    /// <returns><c>true</c> if mapping is ambiguous; otherwise, <c>false</c>.</returns>
    internal static bool HasAmbiguousEnumMemberNameCaseInsensitiveMap(
        this ITypeSymbol sourceType,
        ITypeSymbol targetType,
        out string ambiguityDetails)
    {
        ambiguityDetails = string.Empty;
        var sourceMemberNames = sourceType.GetEnumValues().Select(enumValue => enumValue.Name).ToArray();
        var targetMemberNames = targetType.GetEnumValues().Select(enumValue => enumValue.Name).ToArray();
        var mappings = new List<(string SourceMemberName, string TargetMemberName)>();

        foreach (var sourceMemberName in sourceMemberNames)
        {
            var targetMatches = targetMemberNames
                .Where(targetMemberName => string.Equals(sourceMemberName, targetMemberName, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (targetMatches.Length > 1)
            {
                ambiguityDetails = $"Source enum member '{sourceMemberName}' matches multiple target members: {string.Join(", ", targetMatches.Select(name => $"'{name}'"))}.";
                return true;
            }

            if (targetMatches.Length == 1)
            {
                mappings.Add((sourceMemberName, targetMatches[0]));
            }
        }

        var duplicateTargetMappings = mappings
            .GroupBy(mapping => mapping.TargetMemberName, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateTargetMappings is not null)
        {
            var sourceMembers = duplicateTargetMappings.Select(mapping => mapping.SourceMemberName).ToArray();
            ambiguityDetails = $"Target enum member '{duplicateTargetMappings.Key}' is matched by multiple source members: {string.Join(", ", sourceMembers.Select(name => $"'{name}'"))}.";
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether Description-based enum mapping between source and target is ambiguous.
    /// </summary>
    /// <param name="sourceType">The source enum type.</param>
    /// <param name="targetType">The target enum type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="caseInsensitive">Whether description comparison is case-insensitive.</param>
    /// <param name="ambiguityDetails">The ambiguity details when mapping is ambiguous.</param>
    /// <returns><c>true</c> if mapping is ambiguous; otherwise, <c>false</c>.</returns>
    internal static bool HasAmbiguousEnumMemberDescriptionMap(
        this ITypeSymbol sourceType,
        ITypeSymbol targetType,
        Compilation compilation,
        bool caseInsensitive,
        out string ambiguityDetails)
    {
        ambiguityDetails = string.Empty;
        var comparer = caseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var sourceMembers = sourceType.GetEnumMembersWithDescriptions(compilation);
        var targetMembers = targetType.GetEnumMembersWithDescriptions(compilation);
        var mappings = new List<(string SourceMemberName, string TargetMemberName)>();

        foreach (var sourceMember in sourceMembers)
        {
            var targetMatches = targetMembers
                .Where(targetMember => comparer.Equals(sourceMember.Description, targetMember.Description))
                .ToArray();
            if (targetMatches.Length > 1)
            {
                ambiguityDetails = $"Source enum member '{sourceMember.Name}' matches multiple target members by Description: {string.Join(", ", targetMatches.Select(member => $"'{member.Name}'"))}.";
                return true;
            }

            if (targetMatches.Length == 1)
            {
                mappings.Add((sourceMember.Name, targetMatches[0].Name));
            }
        }

        var duplicateTargetMappings = mappings
            .GroupBy(mapping => mapping.TargetMemberName, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateTargetMappings is not null)
        {
            var sourceMembersMatched = duplicateTargetMappings.Select(mapping => mapping.SourceMemberName).ToArray();
            ambiguityDetails = $"Target enum member '{duplicateTargetMappings.Key}' is matched by multiple source members by Description: {string.Join(", ", sourceMembersMatched.Select(name => $"'{name}'"))}.";
            return true;
        }

        return false;
    }

    /// <summary>
    /// Formats a C# string literal for generated code.
    /// </summary>
    /// <param name="value">The string value.</param>
    /// <returns>The escaped C# string literal.</returns>
    internal static string ToCSharpStringLiteral(string value)
        => $"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t")}\"";

    /// <summary>
    /// Gets the list of accessible constructors for <paramref name="typeSymbol"/>.
    /// </summary>
    /// <param name="typeSymbol">The symbol for which you require the list of constructors.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="accessibleWithin">The symbol from which the constructor should be accessible.</param>
    /// <param name="numberOfArguments">The number of arguments; <c>null</c> if any number of parameters is acceptable.</param>
    /// <returns>The list of accessible constructor for <paramref name="typeSymbol"/>.</returns>
    /// <exception cref="MappaGeneratorException">If <paramref name="typeSymbol"/> is not of type <see cref="INamedTypeSymbol"/>.</exception>
    internal static IMethodSymbol[] GetAccessibleConstructors(
        this ITypeSymbol typeSymbol,
        Compilation compilation,
        ISymbol accessibleWithin,
        int? numberOfArguments = null)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            var constructors = namedTypeSymbol
                .Constructors
                .Where(methodSymbol => compilation.IsSymbolAccessibleWithin(methodSymbol, accessibleWithin));

            if (numberOfArguments is not null)
            {
                constructors = constructors.Where(methodSymbol => methodSymbol.Parameters.Length == numberOfArguments);
            }

            return constructors.ToArray();
        }

        throw new MappaGeneratorException($"Cannot detect constructors for type \"{typeSymbol.ToDisplayString()}\"");
    }

    /// <summary>
    /// Gets the properties of the type.
    /// </summary>
    /// <param name="typeSymbol">Get the symbol properties.</param>
    /// <returns>The symbol properties.</returns>
    internal static IEnumerable<IPropertySymbol> GetTypeProperties(this ITypeSymbol typeSymbol)
    {
        HashSet<string> propertyNames = new();
        List<IPropertySymbol> properties = new();

        if (typeSymbol.TypeKind == TypeKind.Interface)
        {
            properties.AddRange(typeSymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(property => propertyNames.Add(property.Name)));

            foreach (var currentType in typeSymbol.AllInterfaces)
            {
                properties.AddRange(currentType
                    .GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(property => propertyNames.Add(property.Name)));
            }
        }
        else
        {
            ITypeSymbol? currentType = typeSymbol;
            while (currentType is not null)
            {
                properties.AddRange(currentType
                    .GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(property => propertyNames.Add(property.Name)));

                currentType = currentType.BaseType;
            }
        }

        return properties;
    }

    /// <summary>
    /// Check if a reference type has the nullable attribute or the nullable attribute
    /// is not set because the reference <c>#nullable</c> flag is not enabled.
    /// </summary>
    /// <param name="typeSymbol">The symbol.</param>
    /// <returns><c>true</c> if the type is a reference nullable type.</returns>
    internal static bool IsReferenceNullable(this ITypeSymbol typeSymbol)
        => typeSymbol is
        {
            IsReferenceType: true,
            NullableAnnotation: NullableAnnotation.Annotated or NullableAnnotation.None,
        };

    /// <summary>
    /// Obtain the display string without the nullable question mark
    /// for reference type.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns>The display string without the question mark for reference types.</returns>
    internal static string ToDisplayNameWithoutNullableAnnotation(this ITypeSymbol typeSymbol)
    {
        var displayString = typeSymbol.ToDisplayString();
        return typeSymbol.NullableAnnotation == NullableAnnotation.Annotated
            ? displayString.Substring(0, displayString.Length - 1)
            : displayString;
    }

    /// <summary>
    /// Check if the type is <see cref="TimeSpan"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="TimeSpan"/>.</returns>
    internal static bool IsTimeSpan(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var timeSpanName = compilation.GetTypeByMetadataName(TimeSpanFullName);
        var isTimeSpan = SymbolEqualityComparer.Default.Equals(timeSpanName, typeSymbol.OriginalDefinition);
        return isTimeSpan;
    }

    /// <summary>
    /// Check if the type is TimeOnly.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is TimeOnly.</returns>
    internal static bool IsTimeOnly(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var timeOnlyType = compilation.GetTypeByMetadataName("System.TimeOnly");
        var isTimeOnly = SymbolEqualityComparer.Default.Equals(timeOnlyType, typeSymbol.OriginalDefinition);
        return isTimeOnly;
    }

    /// <summary>
    /// Check if the type is DateOnly.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is DateOnly.</returns>
    internal static bool IsDateOnly(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var dateOnlyType = compilation.GetTypeByMetadataName("System.DateOnly");
        var isDateOnly = SymbolEqualityComparer.Default.Equals(dateOnlyType, typeSymbol.OriginalDefinition);
        return isDateOnly;
    }

    /// <summary>
    /// Check if the type is <see cref="Uri"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Uri"/>.</returns>
    internal static bool IsUri(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var uriType = compilation.GetTypeByMetadataName(UriFullName);
        var isUri = SymbolEqualityComparer.Default.Equals(uriType, typeSymbol.OriginalDefinition);
        return isUri;
    }

    /// <summary>
    /// Check if the type is <see cref="Guid"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Guid"/>.</returns>
    internal static bool IsGuid(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var guidType = compilation.GetTypeByMetadataName(GuidFullName);
        var isGuid = SymbolEqualityComparer.Default.Equals(guidType, typeSymbol.OriginalDefinition);
        return isGuid;
    }

    /// <summary>
    /// Check if the type is <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="KeyValuePair{TKey,TValue}"/>.</returns>
    internal static bool IsKeyValuePair(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var keyValuePairSymbol = compilation.GetTypeByMetadataName(KeyValuePairFullName);
        var isKeyValuePair = SymbolEqualityComparer.Default.Equals(keyValuePairSymbol, typeSymbol.OriginalDefinition);
        return isKeyValuePair;
    }

    /// <summary>
    /// Check if the type is <see cref="Stack{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Stack{T}"/>.</returns>
    internal static bool IsStack(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var stackSymbol = compilation.GetTypeByMetadataName(StackFullName);
        var isStack = SymbolEqualityComparer.Default.Equals(stackSymbol, typeSymbol.OriginalDefinition);
        return isStack;
    }

    /// <summary>
    /// Check if the type is or implements <see cref="Stack{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is or implements <see cref="Stack{T}"/>.</returns>
    internal static bool IsOrDerivedFromStack(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var stackSymbol = compilation.GetTypeByMetadataName(StackFullName);
        if (SymbolEqualityComparer.Default.Equals(stackSymbol, typeSymbol.OriginalDefinition))
        {
            return true;
        }

        INamedTypeSymbol? baseType = typeSymbol.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(stackSymbol, baseType.OriginalDefinition))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Check if the type is <see cref="Queue{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Queue{T}"/>.</returns>
    internal static bool IsQueue(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var queueSymbol = compilation.GetTypeByMetadataName(QueueFullName);
        var isQueue = SymbolEqualityComparer.Default.Equals(queueSymbol, typeSymbol.OriginalDefinition);
        return isQueue;
    }

    /// <summary>
    /// Check if the type is or implement <see cref="Queue{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is or implement <see cref="Queue{T}"/>.</returns>
    internal static bool IsOrDerivedFromQueue(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var queueType = compilation.GetTypeByMetadataName(QueueFullName);
        if (SymbolEqualityComparer.Default.Equals(queueType, typeSymbol.OriginalDefinition))
        {
            return true;
        }

        INamedTypeSymbol? baseType = typeSymbol.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(queueType, baseType.OriginalDefinition))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Check if the type is <see cref="BlockingCollection{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="BlockingCollection{T}"/>.</returns>
    internal static bool IsBlockingCollection(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var blockingCollectionSymbol = compilation.GetTypeByMetadataName(BlockingCollectionFullName);
        var isBlockingCollections = SymbolEqualityComparer.Default.Equals(blockingCollectionSymbol, typeSymbol.OriginalDefinition);
        return isBlockingCollections;
    }

    /// <summary>
    /// Check if the type is or derived from <see cref="BlockingCollection{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is or implement <see cref="BlockingCollection{T}"/>.</returns>
    internal static bool IsOrDerivedFromBlockingCollection(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsBlockingCollection(compilation) || typeSymbol.IsDerivedFromBlockingCollection(compilation);

    /// <summary>
    /// Check if the type is derived from <see cref="BlockingCollection{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is or implement <see cref="BlockingCollection{T}"/>.</returns>
    internal static bool IsDerivedFromBlockingCollection(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var blockingCollectionSymbol = compilation.GetTypeByMetadataName(BlockingCollectionFullName);
        INamedTypeSymbol? baseType = typeSymbol.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(blockingCollectionSymbol, baseType.OriginalDefinition))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Check if the type is <see cref="ConcurrentBag{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ConcurrentBag{T}"/>.</returns>
    internal static bool IsConcurrentBag(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var blockingCollectionSymbol = compilation.GetTypeByMetadataName(ConcurrentBagFullName);
        var isConcurrentBags = SymbolEqualityComparer.Default.Equals(blockingCollectionSymbol, typeSymbol.OriginalDefinition);
        return isConcurrentBags;
    }

    /// <summary>
    /// Check if the type is or implement <see cref="ConcurrentBag{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is or implement <see cref="ConcurrentBag{T}"/>.</returns>
    internal static bool IsOrDerivedFromConcurrentBag(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var blockingCollectionSymbol = compilation.GetTypeByMetadataName(ConcurrentBagFullName);
        if (SymbolEqualityComparer.Default.Equals(blockingCollectionSymbol, typeSymbol.OriginalDefinition))
        {
            return true;
        }

        INamedTypeSymbol? baseType = typeSymbol.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(blockingCollectionSymbol, baseType.OriginalDefinition))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Check if the type is or is derived from <see cref="ConcurrentStack{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is or implement <see cref="ConcurrentStack{T}"/>.</returns>
    internal static bool IsOrDerivedFromConcurrentStack(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var blockingCollectionSymbol = compilation.GetTypeByMetadataName(ConcurrentStackFullName);
        if (SymbolEqualityComparer.Default.Equals(blockingCollectionSymbol, typeSymbol.OriginalDefinition))
        {
            return true;
        }

        INamedTypeSymbol? baseType = typeSymbol.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(blockingCollectionSymbol, baseType.OriginalDefinition))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Check if the type is or implement <see cref="ConcurrentQueue{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is or implement <see cref="ConcurrentQueue{T}"/>.</returns>
    internal static bool IsOrImplementConcurrentQueue(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var blockingCollectionSymbol = compilation.GetTypeByMetadataName(ConcurrentQueueFullName);
        if (SymbolEqualityComparer.Default.Equals(blockingCollectionSymbol, typeSymbol.OriginalDefinition))
        {
            return true;
        }

        INamedTypeSymbol? baseType = typeSymbol.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(blockingCollectionSymbol, baseType.OriginalDefinition))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Check if the type is <see cref="ISet{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ISet{T}"/>.</returns>
    internal static bool IsISet(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var setType = compilation.GetTypeByMetadataName(SetInterfaceFullName);
        var isSetType = SymbolEqualityComparer.Default.Equals(setType, typeSymbol.OriginalDefinition);
        return isSetType;
    }

    /// <summary>
    /// Check if the type implements <see cref="ISet{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ISet{T}"/>.</returns>
    internal static bool ImplementISet(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.AllInterfaces.Any(@interface => @interface.IsISet(compilation));

    /// <summary>
    /// Check if the type is <see cref="ISet{T}"/> or implements <see cref="ISet{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol implements <see cref="ISet{T}"/>.</returns>
    internal static bool IsOrImplementISet(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsISet(compilation) || typeSymbol.ImplementISet(compilation);

    /// <summary>
    /// Check if the type is <c>IReadOnlySet{T}</c>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <c>IReadOnlySet{T}</c>.</returns>
    internal static bool IsIReadOnlySet(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var readonlySetType = compilation.GetTypeByMetadataName(ReadOnlySetInterfaceFullName);
        var isSetType = SymbolEqualityComparer.Default.Equals(readonlySetType, typeSymbol.OriginalDefinition);
        return isSetType;
    }

    /// <summary>
    /// Check if the type is <see cref="HashSet{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="HashSet{T}"/>.</returns>
    internal static bool IsHashSet(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var hashsetType = compilation.GetTypeByMetadataName(HashSetFullName);
        var isSetType = SymbolEqualityComparer.Default.Equals(hashsetType, typeSymbol.OriginalDefinition);
        return isSetType;
    }

    /// <summary>
    /// Check if the type is <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IReadOnlyList{T}"/>.</returns>
    internal static bool IsIReadOnlyList(this ITypeSymbol typeSymbol)
        => typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IReadOnlyList_T;

    /// <summary>
    /// Check if the type is <see cref="IImmutableQueue{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IImmutableQueue{T}"/>.</returns>
    internal static bool IsIImmutableQueue(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var queueType = compilation.GetTypeByMetadataName(ImmutableQueueInterfaceFullName);
        var isQueue = SymbolEqualityComparer.Default.Equals(queueType, typeSymbol.OriginalDefinition);
        return isQueue;
    }

    /// <summary>
    /// Check if the type is <see cref="ImmutableQueue{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ImmutableQueue{T}"/>.</returns>
    internal static bool IsImmutableQueue(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var queueType = compilation.GetTypeByMetadataName(ImmutableQueueFullName);
        var isQueue = SymbolEqualityComparer.Default.Equals(queueType, typeSymbol.OriginalDefinition);
        return isQueue;
    }

    /// <summary>
    /// Check if the type is <see cref="IImmutableStack{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IImmutableStack{T}"/>.</returns>
    internal static bool IsIImmutableStack(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var stackType = compilation.GetTypeByMetadataName(ImmutableStackInterfaceFullName);
        var isStack = SymbolEqualityComparer.Default.Equals(stackType, typeSymbol.OriginalDefinition);
        return isStack;
    }

    /// <summary>
    /// Check if the type is <see cref="ImmutableStack{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="ImmutableStack{T}"/>.</returns>
    internal static bool IsImmutableStack(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var stackType = compilation.GetTypeByMetadataName(ImmutableStackFullName);
        var isStack = SymbolEqualityComparer.Default.Equals(stackType, typeSymbol.OriginalDefinition);
        return isStack;
    }

    /// <summary>
    /// Check if <paramref name="namedTypeSymbol"/> has a constructor with empty parameters.
    /// </summary>
    /// <param name="namedTypeSymbol">The symbol to check has a constructor without parameters.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="accessibleFromMethod">Optional method, if provided (and not <c>null</c>) we will check that the constructor can be invoked from <paramref name="accessibleFromMethod"/>.</param>
    /// <returns><c>true</c> if <paramref name="namedTypeSymbol"/> has a constructor with no parameters, <c>false</c> otherwise.</returns>
    internal static bool HasNamedTypeSymbolAccessibleZeroParametersConstructor(
        this INamedTypeSymbol namedTypeSymbol,
        Compilation compilation,
        IMethodSymbol? accessibleFromMethod = null)
    {
        var constructor = namedTypeSymbol.Constructors.FirstOrDefault(constructor => constructor.Parameters.Length == 0);
        if (constructor is null)
        {
            return false;
        }

        return accessibleFromMethod == null || compilation.IsSymbolAccessibleWithin(constructor, accessibleFromMethod.ContainingSymbol);
    }

    /// <summary>
    /// Check if <paramref name="namedTypeSymbol"/> has a constructor with one parameter of type integer.
    /// </summary>
    /// <param name="namedTypeSymbol">The symbol to check has a constructor with a single integer parameter.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="accessibleFromMethod">Optional method, if provided (and not <c>null</c>) we will check that the constructor can be invoked from <paramref name="accessibleFromMethod"/>.</param>
    /// <returns><c>true</c> if <paramref name="namedTypeSymbol"/> has a constructor with one integer parameter, <c>false</c> otherwise.</returns>
    internal static bool HasNamedTypeSymbolAccessibleSingleIntegerParametersConstructor(
        this INamedTypeSymbol namedTypeSymbol,
        Compilation compilation,
        IMethodSymbol? accessibleFromMethod = null)
    {
        var constructor = namedTypeSymbol.Constructors.FirstOrDefault(constructor =>
            constructor.Parameters.Length == 1 &&
            constructor.Parameters[0].Type.SpecialType is SpecialType.System_Int32);
        if (constructor is null)
        {
            return false;
        }

        return accessibleFromMethod == null || compilation.IsSymbolAccessibleWithin(constructor, accessibleFromMethod.ContainingSymbol);
    }

    /// <summary>
    /// Check if <paramref name="namedTypeSymbol"/> has a constructor with one parameter of type string.
    /// </summary>
    /// <param name="namedTypeSymbol">The symbol to check has a constructor with a single string parameter.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="accessibleFromMethod">Optional method, if provided (and not <c>null</c>) we will check that the constructor can be invoked from <paramref name="accessibleFromMethod"/>.</param>
    /// <returns><c>true</c> if <paramref name="namedTypeSymbol"/> has a constructor with one string parameter, <c>false</c> otherwise.</returns>
    internal static bool HasNamedTypeSymbolAccessibleSingleStringParametersConstructor(
        this INamedTypeSymbol namedTypeSymbol,
        Compilation compilation,
        IMethodSymbol? accessibleFromMethod = null)
    {
        var constructor = namedTypeSymbol.Constructors.FirstOrDefault(constructor =>
            constructor.Parameters.Length == 1 &&
            constructor.Parameters[0].Type.IsString());
        if (constructor is null)
        {
            return false;
        }

        return accessibleFromMethod == null || compilation.IsSymbolAccessibleWithin(constructor, accessibleFromMethod.ContainingSymbol);
    }

    /// <summary>
    /// Check if <paramref name="typeSymbol"/> has a constructor with empty parameters.
    /// </summary>
    /// <param name="typeSymbol">The symbol to check has a constructor without parameters.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="accessibleFromMethod">Optional method, if provided (and not <c>null</c>) we will check that the constructor can be invoked from <paramref name="accessibleFromMethod"/>.</param>
    /// <returns><c>true</c> if <paramref name="typeSymbol"/> has a constructor with no parameters, <c>false</c> otherwise.</returns>
    internal static bool HasSymbolAccessibleZeroParametersConstructor(
        this ITypeSymbol typeSymbol,
        Compilation compilation,
        IMethodSymbol? accessibleFromMethod = null)
        => typeSymbol.TypeKind != TypeKind.Interface &&
           typeSymbol is INamedTypeSymbol namedTypeSymbol &&
           namedTypeSymbol.HasNamedTypeSymbolAccessibleZeroParametersConstructor(compilation, accessibleFromMethod);

    /// <summary>
    /// Check if <paramref name="typeSymbol"/> has a constructor with one parameter of type integer.
    /// </summary>
    /// <param name="typeSymbol">The symbol to check has a constructor with one integer parameter.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="accessibleFromMethod">Optional method, if provided (and not <c>null</c>) we will check that the constructor can be invoked from <paramref name="accessibleFromMethod"/>.</param>
    /// <returns><c>true</c> if <paramref name="typeSymbol"/> has a constructor with one integer parameter, <c>false</c> otherwise.</returns>
    internal static bool HasSymbolAccessibleSingleIntegerParametersConstructor(
        this ITypeSymbol typeSymbol,
        Compilation compilation,
        IMethodSymbol? accessibleFromMethod = null)
        => typeSymbol.TypeKind != TypeKind.Interface &&
           typeSymbol is INamedTypeSymbol namedTypeSymbol &&
           namedTypeSymbol.HasNamedTypeSymbolAccessibleSingleIntegerParametersConstructor(compilation, accessibleFromMethod);

    /// <summary>
    /// Normalize a type name (e.g. <c>"string"</c>
    /// become  <c>"System.String"</c>).
    /// </summary>
    /// <param name="type">The type name to be normalised.</param>
    /// <returns>The normalised name of the type.</returns>
    internal static string NormalizeType(this string type)
        => type switch
        {
            "sbyte" => typeof(sbyte).ToString(),
            "short" => typeof(short).ToString(),
            "int" => typeof(int).ToString(),
            "long" => typeof(long).ToString(),
            "byte" => typeof(byte).ToString(),
            "ushort" => typeof(ushort).ToString(),
            "uint" => typeof(uint).ToString(),
            "ulong" => typeof(ulong).ToString(),
            "float" => typeof(float).ToString(),
            "double" => typeof(double).ToString(),
            "string" => typeof(string).ToString(),
            "char" => typeof(char).ToString(),
            "decimal" => typeof(decimal).ToString(),
            "nint" => typeof(nint).ToString(),
            "nuint" => typeof(nuint).ToString(),
            "void" => typeof(void).ToString(),
            "bool" => typeof(bool).ToString(),
            _ => type,
        };

    /// <summary>
    /// Check if <paramref name="typeSymbol"/> is either <see cref="IsValueTypeNullable"/>
    /// or <see cref="IsReferenceNullable"/>.
    /// </summary>
    /// <param name="typeSymbol">The type to evaluate.</param>
    /// <returns><c>true</c> if <paramref name="typeSymbol"/> is either <see cref="IsValueTypeNullable"/>
    /// or <see cref="IsReferenceNullable"/>, <c>false</c> otherwise.</returns>
    internal static bool IsNullable(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsValueTypeNullable() || typeSymbol.IsReferenceNullable();
    }

    /// <summary>
    /// Get the type inside the nullable.
    /// For reference type it returns the type itself.
    /// </summary>
    /// <param name="typeSymbol">The type to evaluate.</param>
    /// <returns>The type inside the nullable.</returns>
    internal static ITypeSymbol GetTypeInsideNullable(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsReferenceNullable())
        {
            return typeSymbol;
        }

        if (typeSymbol.IsValueTypeNullable())
        {
            return typeSymbol.GetElementType();
        }

        return typeSymbol;
    }

    /// <summary>
    /// Check if <paramref name="typeSymbol"/> implements <paramref name="interfaceSymbol"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol to check.</param>
    /// <param name="interfaceSymbol">The interface symbol.</param>
    /// <returns><c>true</c> if <paramref name="typeSymbol"/> implements <paramref name="interfaceSymbol"/>, <c>false</c> otherwise.</returns>
    internal static bool IsImplementingInterface(this ITypeSymbol typeSymbol, ITypeSymbol interfaceSymbol)
        => typeSymbol.AllInterfaces.Any(@interface =>
                SymbolEqualityComparer.Default.Equals(@interface, interfaceSymbol));

    /// <summary>
    /// Check if <paramref name="typeSymbol"/> implements <paramref name="baseClassSymbol"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol to check.</param>
    /// <param name="baseClassSymbol">The base class symbol.</param>
    /// <returns><c>true</c> if <paramref name="typeSymbol"/> is derived from <paramref name="baseClassSymbol"/> or they are the same type, <c>false</c> otherwise.</returns>
    internal static bool IsDerivedFromClass(this ITypeSymbol typeSymbol, ITypeSymbol baseClassSymbol)
    {
        if (SymbolEqualityComparer.Default.Equals(typeSymbol, baseClassSymbol))
        {
            return true;
        }

        var baseType = typeSymbol.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(baseType, baseClassSymbol))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Check if <paramref name="typeSymbol"/> is implementing interface <paramref name="baseSymbol"/>
    /// or derived from class <paramref name="baseSymbol"/> or <paramref name="typeSymbol"/> or if <paramref name="baseSymbol"/>
    /// are the same type.
    /// </summary>
    /// <param name="typeSymbol">The symbol to check.</param>
    /// <param name="baseSymbol">The parent class or interface.</param>
    /// <returns><c>true</c> if <paramref name="typeSymbol"/> is implementing interface <paramref name="baseSymbol"/>
    /// or derived from class <paramref name="baseSymbol"/> or <paramref name="typeSymbol"/> or if <paramref name="baseSymbol"/>
    /// are the same type, <c>false</c> otherwise.</returns>
    internal static bool IsImplementingOrIsDerivedFromClass(this ITypeSymbol typeSymbol, ITypeSymbol baseSymbol)
        => baseSymbol.TypeKind switch
        {
            TypeKind.Interface => typeSymbol.IsImplementingInterface(baseSymbol),
            _ => typeSymbol.IsDerivedFromClass(baseSymbol),
        };

    /// <summary>
    /// Check if the symbol is an exception.
    /// </summary>
    /// <param name="typeSymbol">The symbol to validate.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type is a valid type that can thrown, <c>false</c> otherwise.</returns>
    internal static bool CanBeThrown(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var exceptionSymbol = compilation.GetTypeByMetadataName(ExceptionFullName);
        if (SymbolEqualityComparer.Default.Equals(exceptionSymbol, typeSymbol.OriginalDefinition))
        {
            return true;
        }

        INamedTypeSymbol? baseType = typeSymbol.BaseType;
        while (baseType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(exceptionSymbol, baseType.OriginalDefinition))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Obtain the metadata name of <paramref name="typeSymbol"/> for use with
    /// <see cref="Compilation.GetTypeByMetadataName(string)"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns>The metadata name of <paramref name="typeSymbol"/>.</returns>
    internal static string GetMetadataName(this INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.ContainingType is { } containingType)
        {
            return containingType.GetMetadataName() + "+" + typeSymbol.MetadataName;
        }

        return typeSymbol.ContainingNamespace.IsGlobalNamespace
            ? typeSymbol.MetadataName
            : typeSymbol.ContainingNamespace.ToDisplayString() + "." + typeSymbol.MetadataName;
    }

    /// <summary>
    /// Obtain all methods declared in <paramref name="typeSymbol"/> and its base types.
    /// Methods are sorted by most derived class first.
    /// </summary>
    /// <param name="typeSymbol">The type symbol to investigate.</param>
    /// <returns>The methods in the type hierarchy.</returns>
    internal static IEnumerable<IMethodSymbol> GetMethodsInTypeHierarchy(this ITypeSymbol typeSymbol)
    {
        ITypeSymbol? currentType = typeSymbol;
        while (currentType is not null)
        {
            foreach (var method in currentType.GetMembers().OfType<IMethodSymbol>())
            {
                yield return method;
            }

            currentType = currentType.BaseType;
        }
    }

    /// <summary>
    /// Locate all the methods in the type hierarchy with the given name.
    /// Methods are sorted by most derived class first.
    /// </summary>
    /// <param name="typeSymbol">The type symbol to investigate.</param>
    /// <param name="methodName">The method name.</param>
    /// <returns>The methods with name <paramref name="methodName"/> in the hierarchy.</returns>
    internal static IMethodSymbol[] LocateMethods(this ITypeSymbol typeSymbol, string methodName)
    {
        var methods = new List<IMethodSymbol>();
        ITypeSymbol? currentType = typeSymbol;
        while (currentType is not null)
        {
            var currentTypeMethods = currentType
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.Name.Equals(methodName, StringComparison.Ordinal));

            methods.AddRange(currentTypeMethods);
            currentType = currentType.BaseType;
        }

        return [.. methods];
    }

    /// <summary>
    /// Check if method <paramref name="methodSymbol"/> is a method with the
    /// given characteristic to be used for mapping.
    /// </summary>
    /// <param name="methodSymbol">The method to validate.</param>
    /// <param name="expectedSourceType">The expected source type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="mustBeStatic"><c>true</c> when the method must be static, <c>false</c> otherwise.</param>
    /// <param name="nullableEnabled"><c>true</c> when nullable is enabled, <c>false</c> otherwise.</param>
    /// <param name="acceptTwoParameters"><c>true</c> when mapping method can provide a context parameter, <c>false</c> otherwise.</param>
    /// <returns><c>true</c> when the <paramref name="methodSymbol"/> is a method with the expected constraints, <c>false</c> otherwise.</returns>
    internal static bool IsMethodValidToMapToTargetSymbolForPolymorphism(
        this IMethodSymbol methodSymbol,
        ITypeSymbol expectedSourceType,
        Compilation compilation,
        bool mustBeStatic,
        bool nullableEnabled,
        bool acceptTwoParameters)
    {
        return (!mustBeStatic || methodSymbol.IsStatic) && methodSymbol.Parameters.Length switch
        {
            0 => true,
            1 => methodSymbol.AreParametersRefModifiersValid()
                 && methodSymbol.Parameters[0].Type.IsEqualTo(expectedSourceType, nullableEnabled),
            2 when acceptTwoParameters => methodSymbol.AreParametersRefModifiersValid()
                                          && IsFirstParameterOk()
                                          && IsSecondParameterOk(),
            _ => false,
        };

        bool IsFirstParameterOk() => methodSymbol.Parameters[0].Type.IsEqualTo(expectedSourceType, nullableEnabled);
        bool IsSecondParameterOk() => methodSymbol.SecondParameterIsMappaContext(compilation);
    }
}