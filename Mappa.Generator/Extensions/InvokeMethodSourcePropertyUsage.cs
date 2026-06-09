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
    {
        switch (method.Parameters.Length)
        {
            case 0:
                return false;

            case 1:
                if (method.ParameterIsMappaContext(compilation, 0))
                {
                    return false;
                }

                if (method.Parameters[0].Type.IsEqualTo(sourceClassType, nullableEnabled) ||
                    compilation.HasImplicitConversion(sourceClassType, method.Parameters[0].Type))
                {
                    return false;
                }

                return sourceProperty is not null &&
                       (method.Parameters[0].Type.IsEqualTo(sourceProperty.Type, nullableEnabled) ||
                        compilation.HasImplicitConversion(sourceProperty.Type, method.Parameters[0].Type));

            case 2:
                if (method.ParameterIsMappaContext(compilation, 1))
                {
                    if (method.Parameters[0].Type.IsEqualTo(sourceClassType, nullableEnabled) ||
                        compilation.HasImplicitConversion(sourceClassType, method.Parameters[0].Type))
                    {
                        return false;
                    }

                    return sourceProperty is not null &&
                           (method.Parameters[0].Type.IsEqualTo(sourceProperty.Type, nullableEnabled) ||
                            compilation.HasImplicitConversion(sourceProperty.Type, method.Parameters[0].Type));
                }

                return sourceProperty is not null;

            case 3:
                return sourceProperty is not null;

            default:
                return false;
        }
    }
}