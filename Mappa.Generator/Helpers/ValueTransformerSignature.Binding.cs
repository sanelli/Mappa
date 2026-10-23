// <copyright file="ValueTransformerSignature.Binding.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Binding of the type parameters of open generic transformers to the type of a mapping site.
/// </summary>
internal static partial class ValueTransformerSignature
{
    private static bool TryBind(
        ITypeSymbol pattern,
        ITypeSymbol actual,
        IMethodSymbol method,
        Dictionary<ITypeParameterSymbol, ITypeSymbol> bindings,
        bool nullableEnabled)
    {
        switch (pattern)
        {
            case ITypeParameterSymbol typeParameter when IsMethodTypeParameter(method, typeParameter):
                return TryBindTypeParameter(typeParameter, actual, bindings, nullableEnabled);
            case IArrayTypeSymbol patternArray:
                return TryBindArray(patternArray, actual, method, bindings, nullableEnabled);
            case INamedTypeSymbol { IsGenericType: true } patternNamed when ContainsTypeParameter(patternNamed):
                return TryBindNamedType(patternNamed, actual, method, bindings, nullableEnabled);
            default:
                return pattern.IsEqualTo(actual, nullableEnabled);
        }
    }

    private static bool IsMethodTypeParameter(IMethodSymbol method, ITypeParameterSymbol typeParameter)
        => method.TypeParameters.Any(methodTypeParameter => SymbolEqualityComparer.Default.Equals(methodTypeParameter, typeParameter));

    private static bool TryBindArray(
        IArrayTypeSymbol patternArray,
        ITypeSymbol actual,
        IMethodSymbol method,
        Dictionary<ITypeParameterSymbol, ITypeSymbol> bindings,
        bool nullableEnabled)
    {
        return actual is IArrayTypeSymbol actualArray
               && patternArray.Rank == actualArray.Rank
               && AnnotationsMatch(patternArray, actualArray, nullableEnabled)
               && TryBind(patternArray.ElementType, actualArray.ElementType, method, bindings, nullableEnabled);
    }

    private static bool TryBindNamedType(
        INamedTypeSymbol patternNamed,
        ITypeSymbol actual,
        IMethodSymbol method,
        Dictionary<ITypeParameterSymbol, ITypeSymbol> bindings,
        bool nullableEnabled)
    {
        return actual is INamedTypeSymbol actualNamed
               && SymbolEqualityComparer.Default.Equals(patternNamed.OriginalDefinition, actualNamed.OriginalDefinition)
               && AnnotationsMatch(patternNamed, actualNamed, nullableEnabled)
               && TryBindTypeArguments(patternNamed, actualNamed, method, bindings, nullableEnabled);
    }

    private static bool TryBindTypeArguments(
        INamedTypeSymbol patternNamed,
        INamedTypeSymbol actualNamed,
        IMethodSymbol method,
        Dictionary<ITypeParameterSymbol, ITypeSymbol> bindings,
        bool nullableEnabled)
    {
        for (var index = 0; index < patternNamed.TypeArguments.Length; index++)
        {
            if (!TryBind(patternNamed.TypeArguments[index], actualNamed.TypeArguments[index], method, bindings, nullableEnabled))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryBindTypeParameter(
        ITypeParameterSymbol typeParameter,
        ITypeSymbol actual,
        Dictionary<ITypeParameterSymbol, ITypeSymbol> bindings,
        bool nullableEnabled)
    {
        // A pattern such as T? (reference type parameter) only matches a nullable annotated
        // actual type and binds T to the non-annotated version of it.
        var boundType = actual;
        if (nullableEnabled
            && typeParameter.NullableAnnotation == NullableAnnotation.Annotated
            && actual.IsReferenceType)
        {
            if (actual.NullableAnnotation == NullableAnnotation.NotAnnotated)
            {
                return false;
            }

            boundType = actual.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        }

        if (bindings.TryGetValue(typeParameter, out var existing))
        {
            return existing.IsEqualTo(boundType, nullableEnabled);
        }

        bindings[typeParameter] = boundType;
        return true;
    }

    private static bool AnnotationsMatch(ITypeSymbol pattern, ITypeSymbol actual, bool nullableEnabled)
        => !nullableEnabled
           || pattern.NullableAnnotation == NullableAnnotation.None
           || actual.NullableAnnotation == NullableAnnotation.None
           || pattern.NullableAnnotation == actual.NullableAnnotation;
}