// <copyright file="OneOfWellKnownHelper.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System.Collections.Immutable;

using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Helper methods to inspect [OneOf](https://github.com/mcintyre321/OneOf) <c>OneOf</c> / <c>OneOfBase</c> shapes.
/// </summary>
internal static class OneOfWellKnownHelper
{
    private const string OneOfNamespaceName = "OneOf";
    private const string OneOfTypeName = "OneOf";
    private const string OneOfBaseTypeName = "OneOfBase";
    private const string NoneFullName = "OneOf.Types.None";

    /// <summary>
    /// Returns <c>true</c> when <paramref name="typeSymbol"/> is a constructed <c>OneOf&lt;…&gt;</c>.
    /// </summary>
    /// <param name="typeSymbol">The type to inspect.</param>
    /// <returns><c>true</c> if the type is <c>OneOf</c>.</returns>
    internal static bool IsOneOf(ITypeSymbol typeSymbol)
        => typeSymbol is INamedTypeSymbol namedTypeSymbol
           && IsOneOfDefinition(namedTypeSymbol.OriginalDefinition);

    /// <summary>
    /// Returns <c>true</c> when <paramref name="typeSymbol"/> is <c>OneOfBase&lt;…&gt;</c>
    /// or derives from it.
    /// </summary>
    /// <param name="typeSymbol">The type to inspect.</param>
    /// <returns><c>true</c> if the type is or derives from <c>OneOfBase</c>.</returns>
    internal static bool IsOneOfBase(ITypeSymbol typeSymbol)
        => TryGetOneOfBaseNamedType(typeSymbol, out var oneOfBase) && oneOfBase is not null;

    /// <summary>
    /// Returns <c>true</c> when <paramref name="typeSymbol"/> is <c>OneOf.Types.None</c>.
    /// </summary>
    /// <param name="typeSymbol">The type to inspect.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> if the type is <c>None</c>.</returns>
    internal static bool IsOneOfTypesNone(ITypeSymbol typeSymbol, Compilation compilation)
    {
        var noneType = compilation.GetTypeByMetadataName(NoneFullName);
        if (noneType is null)
        {
            return false;
        }

        return SymbolEqualityComparer.Default.Equals(noneType, typeSymbol.OriginalDefinition);
    }

    /// <summary>
    /// Attempts to get the type arguments of a constructed <c>OneOf&lt;…&gt;</c>.
    /// </summary>
    /// <param name="typeSymbol">The type to inspect.</param>
    /// <param name="typeArguments">The type arguments when successful.</param>
    /// <returns><c>true</c> when <paramref name="typeSymbol"/> is <c>OneOf</c>.</returns>
    internal static bool TryGetOneOfTypeArguments(ITypeSymbol typeSymbol, out ImmutableArray<ITypeSymbol> typeArguments)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol
            && IsOneOfDefinition(namedTypeSymbol.OriginalDefinition)
            && namedTypeSymbol.TypeArguments.Length > 0)
        {
            typeArguments = namedTypeSymbol.TypeArguments;
            return true;
        }

        typeArguments = default;
        return false;
    }

    /// <summary>
    /// Attempts to get the type arguments of the <c>OneOfBase&lt;…&gt;</c> in the base type chain
    /// of <paramref name="typeSymbol"/> (including when the type itself is <c>OneOfBase</c>).
    /// </summary>
    /// <param name="typeSymbol">The type to inspect.</param>
    /// <param name="typeArguments">The type arguments when successful.</param>
    /// <returns><c>true</c> when a <c>OneOfBase</c> was found.</returns>
    internal static bool TryGetOneOfBaseTypeArguments(ITypeSymbol typeSymbol, out ImmutableArray<ITypeSymbol> typeArguments)
    {
        if (TryGetOneOfBaseNamedType(typeSymbol, out var oneOfBase)
            && oneOfBase is not null
            && oneOfBase.TypeArguments.Length > 0)
        {
            typeArguments = oneOfBase.TypeArguments;
            return true;
        }

        typeArguments = default;
        return false;
    }

    /// <summary>
    /// Attempts to get the type arguments of either a <c>OneOf&lt;…&gt;</c> or a <c>OneOfBase&lt;…&gt;</c>
    /// (including derived types).
    /// </summary>
    /// <param name="typeSymbol">The type to inspect.</param>
    /// <param name="typeArguments">The type arguments when successful.</param>
    /// <returns><c>true</c> when the type is a OneOf shape.</returns>
    internal static bool TryGetOneOfOrOneOfBaseTypeArguments(ITypeSymbol typeSymbol, out ImmutableArray<ITypeSymbol> typeArguments)
        => TryGetOneOfTypeArguments(typeSymbol, out typeArguments)
           || TryGetOneOfBaseTypeArguments(typeSymbol, out typeArguments);

    /// <summary>
    /// Returns <c>true</c> when <paramref name="typeSymbol"/> is the Option shape
    /// <c>OneOf&lt;T, None&gt;</c> or <c>OneOfBase&lt;T, None&gt;</c> (value as T0, <c>None</c> as T1).
    /// </summary>
    /// <param name="typeSymbol">The type to inspect.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="valueType">The T0 value type when successful.</param>
    /// <returns><c>true</c> when the type is the Option shape.</returns>
    internal static bool IsOneOfOptionShape(ITypeSymbol typeSymbol, Compilation compilation, out ITypeSymbol valueType)
    {
        valueType = typeSymbol;
        if (!TryGetOneOfOrOneOfBaseTypeArguments(typeSymbol, out var typeArguments)
            || typeArguments.Length != 2
            || !IsOneOfTypesNone(typeArguments[1], compilation))
        {
            return false;
        }

        valueType = typeArguments[0];
        return true;
    }

    /// <summary>
    /// Selects a unique arm index for mapping a OneOf / OneOfBase source to <paramref name="targetType"/>.
    /// Prefers an exact arm match over an assignable arm (base class or interface). Returns <c>false</c>
    /// when zero or more than one matching arm exists.
    /// </summary>
    /// <param name="armTypes">The OneOf type arguments.</param>
    /// <param name="targetType">The mapping target type.</param>
    /// <param name="armIndex">The selected arm index when successful.</param>
    /// <returns><c>true</c> when exactly one arm can be selected.</returns>
    internal static bool TrySelectUniqueArmIndex(
        ImmutableArray<ITypeSymbol> armTypes,
        ITypeSymbol targetType,
        out int armIndex)
    {
        armIndex = -1;
        if (armTypes.IsDefaultOrEmpty)
        {
            return false;
        }

        var exactMatches = new List<int>();
        var assignableMatches = new List<int>();

        for (var index = 0; index < armTypes.Length; index++)
        {
            var armType = armTypes[index];
            if (IsExactArmMatch(armType, targetType))
            {
                exactMatches.Add(index);
                continue;
            }

            if (IsAssignableArmMatch(armType, targetType))
            {
                assignableMatches.Add(index);
            }
        }

        if (exactMatches.Count == 1)
        {
            armIndex = exactMatches[0];
            return true;
        }

        if (exactMatches.Count > 1)
        {
            return false;
        }

        if (assignableMatches.Count == 1)
        {
            armIndex = assignableMatches[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Selects a unique arm index for wrapping <paramref name="sourceType"/> into a OneOf / OneOfBase.
    /// Prefers an exact arm match over an arm that <paramref name="sourceType"/> is assignable to.
    /// Returns <c>false</c> when zero or more than one matching arm exists.
    /// </summary>
    /// <param name="armTypes">The OneOf type arguments.</param>
    /// <param name="sourceType">The value being wrapped.</param>
    /// <param name="armIndex">The selected arm index when successful.</param>
    /// <returns><c>true</c> when exactly one arm can be selected.</returns>
    internal static bool TrySelectUniqueArmIndexForWrap(
        ImmutableArray<ITypeSymbol> armTypes,
        ITypeSymbol sourceType,
        out int armIndex)
    {
        armIndex = -1;
        if (armTypes.IsDefaultOrEmpty)
        {
            return false;
        }

        var exactMatches = new List<int>();
        var assignableMatches = new List<int>();

        for (var index = 0; index < armTypes.Length; index++)
        {
            var armType = armTypes[index];
            if (IsExactArmMatch(armType, sourceType))
            {
                exactMatches.Add(index);
                continue;
            }

            // Source must be assignable to the arm (e.g. Dog → Animal arm).
            if (IsAssignableArmMatch(sourceType, armType))
            {
                assignableMatches.Add(index);
            }
        }

        if (exactMatches.Count == 1)
        {
            armIndex = exactMatches[0];
            return true;
        }

        if (exactMatches.Count > 1)
        {
            return false;
        }

        if (assignableMatches.Count == 1)
        {
            armIndex = assignableMatches[0];
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to resolve the constructed <c>OneOf&lt;…&gt;</c> struct type matching the type arguments
    /// of a <c>OneOfBase</c> (or derived) target.
    /// </summary>
    /// <param name="oneOfBaseType">The <c>OneOfBase</c> or derived type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="optionStructType">The constructed <c>OneOf</c> type when successful.</param>
    /// <returns><c>true</c> when the <c>OneOf</c> type was resolved.</returns>
    internal static bool TryGetOptionStructTypeForOneOfBase(
        ITypeSymbol oneOfBaseType,
        Compilation compilation,
        out INamedTypeSymbol? optionStructType)
    {
        optionStructType = null;
        if (!TryGetOneOfBaseTypeArguments(oneOfBaseType, out var typeArguments)
            || typeArguments.IsDefaultOrEmpty)
        {
            return false;
        }

        var unbound = compilation.GetTypeByMetadataName($"{OneOfNamespaceName}.{OneOfTypeName}`{typeArguments.Length}");
        if (unbound is null)
        {
            return false;
        }

        optionStructType = unbound.Construct([.. typeArguments]);
        return true;
    }

    /// <summary>
    /// Attempts to find how to construct a <c>OneOfBase</c> (or derived) target from a source value
    /// that maps to the given arm.
    /// </summary>
    /// <param name="targetType">The <c>OneOfBase</c> or derived target type.</param>
    /// <param name="armIndex">The selected arm index.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="constructionKind">The construction kind when successful.</param>
    /// <param name="optionStructType">The underlying <c>OneOf</c> struct type when needed for construction.</param>
    /// <returns><c>true</c> when a usable construction path exists.</returns>
    internal static bool TryGetOneOfBaseWrapConstruction(
        ITypeSymbol targetType,
        int armIndex,
        Compilation compilation,
        out OneOfBaseConstructionKind constructionKind,
        out INamedTypeSymbol? optionStructType)
    {
        constructionKind = OneOfBaseConstructionKind.ImplicitFromArm;
        optionStructType = null;

        if (targetType is not INamedTypeSymbol namedTarget
            || !IsOneOfBase(namedTarget)
            || !TryGetOneOfBaseTypeArguments(namedTarget, out var typeArguments)
            || armIndex < 0
            || armIndex >= typeArguments.Length)
        {
            return false;
        }

        var armType = typeArguments[armIndex];
        if (HasPublicImplicitConversionFrom(namedTarget, armType))
        {
            constructionKind = OneOfBaseConstructionKind.ImplicitFromArm;
            return true;
        }

        if (!TryGetOptionStructTypeForOneOfBase(namedTarget, compilation, out optionStructType)
            || optionStructType is null
            || !HasPublicConstructorAccepting(namedTarget, optionStructType))
        {
            return false;
        }

        constructionKind = OneOfBaseConstructionKind.ConstructorFromOneOf;
        return true;
    }

    private static bool HasPublicImplicitConversionFrom(INamedTypeSymbol targetType, ITypeSymbol sourceType)
    {
        foreach (var member in targetType.GetMembers())
        {
            if (member is not IMethodSymbol conversion
                || conversion.MethodKind is not MethodKind.Conversion
                || !conversion.IsStatic
                || conversion.DeclaredAccessibility is not Accessibility.Public
                || conversion.Parameters.Length != 1)
            {
                continue;
            }

            if (IsExactArmMatch(conversion.Parameters[0].Type, sourceType)
                && IsExactArmMatch(conversion.ReturnType, targetType))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasPublicConstructorAccepting(INamedTypeSymbol targetType, ITypeSymbol parameterType)
    {
        foreach (var constructor in targetType.Constructors)
        {
            if (constructor.DeclaredAccessibility != Accessibility.Public
                || constructor.IsStatic
                || constructor.Parameters.Length != 1)
            {
                continue;
            }

            if (IsExactArmMatch(constructor.Parameters[0].Type, parameterType))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsOneOfDefinition(INamedTypeSymbol originalDefinition)
        => originalDefinition.IsGenericType
           && originalDefinition.Name == OneOfTypeName
           && IsInOneOfNamespace(originalDefinition);

    private static bool IsOneOfBaseDefinition(INamedTypeSymbol originalDefinition)
        => originalDefinition.IsGenericType
           && originalDefinition.Name == OneOfBaseTypeName
           && IsInOneOfNamespace(originalDefinition);

    private static bool IsInOneOfNamespace(INamedTypeSymbol typeSymbol)
        => typeSymbol.ContainingNamespace is { IsGlobalNamespace: false } containingNamespace
           && containingNamespace.ContainingNamespace is { IsGlobalNamespace: true }
           && containingNamespace.Name == OneOfNamespaceName;

    private static bool TryGetOneOfBaseNamedType(ITypeSymbol typeSymbol, out INamedTypeSymbol? oneOfBase)
    {
        for (var current = typeSymbol as INamedTypeSymbol; current is not null; current = current.BaseType)
        {
            if (IsOneOfBaseDefinition(current.OriginalDefinition))
            {
                oneOfBase = current;
                return true;
            }
        }

        oneOfBase = null;
        return false;
    }

    private static bool IsExactArmMatch(ITypeSymbol armType, ITypeSymbol targetType)
    {
        var normalizedArm = NormalizeForComparison(armType);
        var normalizedTarget = NormalizeForComparison(targetType);
        return SymbolEqualityComparer.Default.Equals(normalizedArm, normalizedTarget);
    }

    private static bool IsAssignableArmMatch(ITypeSymbol armType, ITypeSymbol targetType)
    {
        if (IsExactArmMatch(armType, targetType))
        {
            return false;
        }

        if (targetType.SpecialType == SpecialType.System_Object)
        {
            return armType.SpecialType != SpecialType.System_Object;
        }

        if (targetType.TypeKind == TypeKind.Interface)
        {
            return armType.AllInterfaces.Any(interfaceType => IsExactArmMatch(interfaceType, targetType));
        }

        for (var current = armType.BaseType; current is not null; current = current.BaseType)
        {
            if (IsExactArmMatch(current, targetType))
            {
                return true;
            }
        }

        return false;
    }

    private static ITypeSymbol NormalizeForComparison(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable
            && nullable.TypeArguments.Length == 1)
        {
            return nullable.TypeArguments[0].WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        }

        return typeSymbol.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
    }
}