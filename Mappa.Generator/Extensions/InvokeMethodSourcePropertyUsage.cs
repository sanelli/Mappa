// <copyright file="InvokeMethodSourcePropertyUsage.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Extensions;

/// <summary>
/// Extension methods that determine whether an invoked method consumes a source property.
/// </summary>
internal static class InvokeMethodSourcePropertyUsage
{
    /// <summary>
    /// Gets a value indicating whether <paramref name="method"/> uses <paramref name="sourceProperty"/>
    /// when invoked for mapping.
    /// </summary>
    /// <param name="method">The method to invoke.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="sourceProperty">The source property.</param>
    /// <param name="sourceClassType">The source class type.</param>
    /// <param name="nullableEnabled"><c>true</c> if nullable reference types are enabled.</param>
    /// <returns><c>true</c> if the source property is used; otherwise, <c>false</c>.</returns>
    internal static bool UsesSourceProperty(
        this IMethodSymbol method,
        Compilation compilation,
        IPropertySymbol? sourceProperty,
        ITypeSymbol sourceClassType,
        bool nullableEnabled)
        => method.Parameters.Length switch
        {
            0 => false,
            1 => UsesSourcePropertyForSingleParameter(method, compilation, sourceProperty, sourceClassType, nullableEnabled),
            2 => UsesSourcePropertyForTwoParameters(method, compilation, sourceProperty, sourceClassType, nullableEnabled),
            3 => sourceProperty is not null,
            _ => false,
        };

    private static bool UsesSourcePropertyForSingleParameter(
        IMethodSymbol method,
        Compilation compilation,
        IPropertySymbol? sourceProperty,
        ITypeSymbol sourceClassType,
        bool nullableEnabled)
    {
        if (method.ParameterIsMappaContext(compilation, 0))
        {
            return false;
        }

        if (ParameterAcceptsSourceType(method.Parameters[0].Type, compilation, sourceClassType, nullableEnabled))
        {
            return false;
        }

        return sourceProperty is not null &&
               ParameterAcceptsSourceType(method.Parameters[0].Type, compilation, sourceProperty.Type, nullableEnabled);
    }

    private static bool UsesSourcePropertyForTwoParameters(
        IMethodSymbol method,
        Compilation compilation,
        IPropertySymbol? sourceProperty,
        ITypeSymbol sourceClassType,
        bool nullableEnabled)
    {
        if (method.ParameterIsMappaContext(compilation, 1))
        {
            if (ParameterAcceptsSourceType(method.Parameters[0].Type, compilation, sourceClassType, nullableEnabled))
            {
                return false;
            }

            return sourceProperty is not null &&
                   ParameterAcceptsSourceType(method.Parameters[0].Type, compilation, sourceProperty.Type, nullableEnabled);
        }

        return sourceProperty is not null;
    }

    private static bool ParameterAcceptsSourceType(
        ITypeSymbol parameterType,
        Compilation compilation,
        ITypeSymbol sourceType,
        bool nullableEnabled)
        => parameterType.IsEqualTo(sourceType, nullableEnabled) ||
           compilation.HasImplicitConversion(sourceType, parameterType);
}