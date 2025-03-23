// <copyright file="TypeSymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="ITypeSymbol"/>.
/// </summary>
internal static class TypeSymbolExtensions
{
    private const string ImmutableDictionaryFullName = "System.Collections.Immutable.ImmutableDictionary`2";
    private const string FrozenDictionaryFullName = "System.Collections.Frozen.FrozenDictionary`2";
    private const string SpanFullName = "System.Span`1";
    private const string ReadOnlySpanFullName = "System.ReadOnlySpan`1";
    private const string MemoryFullName = "System.Memory`1";
    private const string ReadOnlyMemoryFullName = "System.ReadOnlyMemory`1";
    private const string StackFullName = "System.Collections.Generic.Stack`1";
    private const string QueueFullName = "System.Collections.Generic.Queue`1";

    private static readonly string Tuple1Fullname = typeof(Tuple<>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(Tuple<>)}");
    private static readonly string Tuple2Fullname = typeof(Tuple<,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(Tuple<,>)}");
    private static readonly string Tuple3Fullname = typeof(Tuple<,,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(Tuple<,,>)}");
    private static readonly string Tuple4Fullname = typeof(Tuple<,,,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(Tuple<,,,>)}");
    private static readonly string Tuple5Fullname = typeof(Tuple<,,,,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(Tuple<,,,,>)}");
    private static readonly string Tuple6Fullname = typeof(Tuple<,,,,,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(Tuple<,,,,,>)}");
    private static readonly string Tuple7Fullname = typeof(Tuple<,,,,,,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(Tuple<,,,,,,>)}");
    private static readonly string Tuple8Fullname = typeof(Tuple<,,,,,,,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(Tuple<,,,,,,,>)}");
    private static readonly string DictionaryFullName = typeof(Dictionary<,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(Dictionary<,>)}");
    private static readonly string ReadOnlyDictionaryFullName = typeof(ReadOnlyDictionary<,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(ReadOnlyDictionary<,>)}");
    private static readonly string ReadOnlyCollectionFullName = typeof(ReadOnlyCollection<>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(ReadOnlyCollection<>)}");
    private static readonly string DictionaryInterfaceFullName = typeof(IDictionary<,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(IDictionary<,>)}");
    private static readonly string ReadOnlyDictionaryInterfaceFullName = typeof(IReadOnlyDictionary<,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(IReadOnlyDictionary<,>)}");
    private static readonly string ListFullName = typeof(List<>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(List<>)}");
    private static readonly string TimeSpanFullName = typeof(TimeSpan).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(TimeSpan)}");
    private static readonly string UriFullName = typeof(Uri).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(Uri)}");
    private static readonly string GuidFullName = typeof(Guid).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(Guid)}");
    private static readonly string DateTimeOffsetFullName = typeof(DateTimeOffset).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(DateTimeOffset)}");
    private static readonly string KeyValuePairFullName = typeof(KeyValuePair<,>).FullName ?? throw new MappaGeneratorException($"Cannot obtain {nameof(Type.FullName)} for {typeof(KeyValuePair<,>)}");

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
    internal static bool IsNullable(this ITypeSymbol typeSymbol)
        => typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

    /// <summary>
    /// Check if the type is an array with rank one.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is an array.</returns>
    internal static bool IsArray(this ITypeSymbol typeSymbol)
        => typeSymbol is IArrayTypeSymbol { Rank: 1 };

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
    /// Gets the name of the property returning the number of items.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns> The name of the property returning the number of items.</returns>
    internal static string GetCountProperty(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol is IArrayTypeSymbol)
        {
            return nameof(Array.Length);
        }

        return nameof(ICollection.Count);
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
        => isNullableEnabled
            ? SymbolEqualityComparer.IncludeNullability.Equals(left, right)
            : SymbolEqualityComparer.Default.Equals(left, right);

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
        return typeSymbol.GetMembers().OfType<IPropertySymbol>();
    }

    /// <summary>
    /// Check if a reference type has the nullable attribute.
    /// </summary>
    /// <param name="typeSymbol">The symbol.</param>
    /// <returns><c>true</c> if the type is a reference nullable type.</returns>
    internal static bool IsReferenceNullable(this ITypeSymbol typeSymbol)
        => typeSymbol is
        {
            IsReferenceType: true,
            NullableAnnotation: NullableAnnotation.Annotated
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
    /// Check if the type is <see cref="Stack{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Stack{T}"/>.</returns>
    internal static bool IsOrImplementStack(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsStack(compilation) || typeSymbol.AllInterfaces.Any(@interface => @interface.IsStack(compilation));

    /// <summary>
    /// Check if the type is <see cref="Queue{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Queue{T}"/>.</returns>
    internal static bool IsQueue(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var queueSymbol = compilation.GetTypeByMetadataName(QueueFullName);
        var isStack = SymbolEqualityComparer.Default.Equals(queueSymbol, typeSymbol.OriginalDefinition);
        return isStack;
    }

    /// <summary>
    /// Check if the type is <see cref="Queue{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="Queue{T}"/>.</returns>
    internal static bool IsOrImplementQueue(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsQueue(compilation) || typeSymbol.AllInterfaces.Any(@interface => @interface.IsQueue(compilation));
}