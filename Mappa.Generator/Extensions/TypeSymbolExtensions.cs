// <copyright file="TypeSymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections;
using System.Collections.Immutable;

using Mappa.Generator.Exceptions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="ITypeSymbol"/>.
/// </summary>
internal static class TypeSymbolExtensions
{
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
        var listType = compilation.GetTypeByMetadataName(typeof(List<>).FullName);
        var isList = SymbolEqualityComparer.Default.Equals(listType, typeSymbol.OriginalDefinition);
        return isList;
    }

    /// <summary>
    /// Check if the type is <see cref="IDictionary{K,V}"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IDictionary{K,V}"/>.</returns>
    internal static bool IsIDictionary(this ITypeSymbol typeSymbol, Compilation compilation)
    {
        var listType = compilation.GetTypeByMetadataName(typeof(IDictionary<,>).FullName);
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
        var listType = compilation.GetTypeByMetadataName(typeof(Dictionary<,>).FullName);
        var isList = SymbolEqualityComparer.Default.Equals(listType, typeSymbol.OriginalDefinition);
        return isList;
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
    /// Check if the type is <see cref="Tuple"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="IDictionary{K,V}"/>.</returns>
    internal static bool IsTuple(this ITypeSymbol typeSymbol, Compilation compilation)
        => typeSymbol.IsTupleType
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(typeof(Tuple<>).FullName), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(typeof(Tuple<,>).FullName), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(typeof(Tuple<,,>).FullName), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(typeof(Tuple<,,,>).FullName), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(typeof(Tuple<,,,,>).FullName), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(typeof(Tuple<,,,,,>).FullName), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(typeof(Tuple<,,,,,,>).FullName), typeSymbol.OriginalDefinition)
           || SymbolEqualityComparer.Default.Equals(compilation.GetTypeByMetadataName(typeof(Tuple<,,,,,,,>).FullName), typeSymbol.OriginalDefinition);

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

        if (typeSymbol is INamedTypeSymbol namedTypeSymbol && namedTypeSymbol.TypeArguments.Length == 1)
        {
            return namedTypeSymbol.TypeArguments.First();
        }

        throw new MappaGeneratorException($"Cannot obtain element type of \"{typeSymbol.ToDisplayString()}\"");
    }

    /// <summary>
    /// Gets the key and value type of the container.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns>The element type of the key and value of the contain.</returns>
    /// <exception cref="MappaGeneratorException">If <paramref name="typeSymbol"/> is not an array or a generic type.</exception>
    internal static (ITypeSymbol KeyType, ITypeSymbol ValueType) GetKeyAndValueTypes(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol { TypeArguments.Length: 2 } namedTypeSymbol)
        {
            return (namedTypeSymbol.TypeArguments.First(), namedTypeSymbol.TypeArguments.Last());
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

        typeArguments = Array.Empty<ITypeSymbol>().ToImmutableArray();
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
    /// Check if the type is <see cref="DateTime"/>.
    /// </summary>
    /// <param name="typeSymbol">The type symbol.</param>
    /// <returns><c>true</c> if the type symbol is <see cref="DateTime"/>.</returns>
    internal static bool IsDateTime(this ITypeSymbol typeSymbol)
        => typeSymbol.SpecialType == SpecialType.System_DateTime;

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
}