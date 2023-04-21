// <copyright file="TypeSymbolExtensions.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

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
}