// <copyright file="ValueTransformerSignature.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// Validates value transformer signatures and matches transformers with mapping site types.
/// </summary>
internal static partial class ValueTransformerSignature
{
    /// <summary>
    /// Validates the shape of a transformer method: <c>T M(T)</c>, <c>T M(in T)</c>,
    /// <c>T M(T, MappaContext)</c> or <c>T M(in T, MappaContext)</c>.
    /// </summary>
    /// <param name="method">The method to validate.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="reason">The reason the signature is not valid, when invalid.</param>
    /// <returns><c>true</c> when the signature is valid; otherwise <c>false</c>.</returns>
    internal static bool TryValidate(IMethodSymbol method, Compilation compilation, out string reason)
    {
        var invalidReason = GetReturnProblem(method, compilation)
                            ?? GetParametersProblem(method, compilation)
                            ?? GetTypeProblem(method);
        reason = invalidReason ?? string.Empty;
        return invalidReason is null;
    }

    /// <summary>
    /// Determines whether <paramref name="transformer"/> can be applied to a value of type <paramref name="siteType"/>.
    /// Open generic transformers bind their type parameters to <paramref name="siteType"/>.
    /// </summary>
    /// <param name="transformer">The transformer.</param>
    /// <param name="siteType">The type of the value at the mapping site.</param>
    /// <param name="nullableEnabled"><c>true</c> when nullable is enabled.</param>
    /// <param name="compilation">The compilation.</param>
    /// <returns><c>true</c> when the transformer matches the site type, nullability included.</returns>
    internal static bool IsMatch(
        ValueTransformer transformer,
        ITypeSymbol siteType,
        bool nullableEnabled,
        Compilation compilation)
    {
        var method = transformer.Method;
        var parameterType = method.Parameters[0].Type;
        if (!method.IsGenericMethod)
        {
            return parameterType.IsEqualTo(siteType, nullableEnabled);
        }

        var bindings = new Dictionary<ITypeParameterSymbol, ITypeSymbol>(SymbolEqualityComparer.Default);
        return TryBind(parameterType, siteType, method, bindings, nullableEnabled)
               && bindings.All(binding => SatisfiesConstraints(binding.Key, binding.Value, compilation));
    }

    private static string? GetReturnProblem(IMethodSymbol method, Compilation compilation)
    {
        if (method.MethodKind is not MethodKind.Ordinary)
        {
            return "it is not an ordinary method";
        }

        if (method.IsAsync || method.ReturnsAnyTaskType(compilation))
        {
            return "asynchronous transformers are not supported";
        }

        if (method.IsVoid())
        {
            return "it must return a value";
        }

        return method.ReturnsByRef || method.ReturnsByRefReadonly
            ? "it must not return by reference"
            : null;
    }

    private static string? GetParametersProblem(IMethodSymbol method, Compilation compilation)
    {
        if (method.Parameters.Length is not (1 or 2))
        {
            return "it must have one parameter, or one parameter and a MappaContext";
        }

        if (method.Parameters[0].RefKind is not (RefKind.None or RefKind.In))
        {
            return "the value parameter must not be 'ref' or 'out'";
        }

        return method.Parameters.Length == 2 && !IsContextParameterPassedByValue(method, compilation)
            ? "the second parameter must be a MappaContext passed by value"
            : null;
    }

    private static bool IsContextParameterPassedByValue(IMethodSymbol method, Compilation compilation)
        => method.ParameterIsMappaContext(compilation, 1)
           && method.Parameters[1].RefKind is RefKind.None or RefKind.In;

    private static string? GetTypeProblem(IMethodSymbol method)
    {
        if (!SymbolEqualityComparer.IncludeNullability.Equals(method.Parameters[0].Type, method.ReturnType))
        {
            return "the return type must be the same as the value parameter type, nullability included";
        }

        return method.IsGenericMethod && !AllTypeParametersAreUsedByValueParameter(method)
            ? "every generic type parameter must be inferable from the value parameter"
            : null;
    }

    private static bool AllTypeParametersAreUsedByValueParameter(IMethodSymbol method)
    {
        var used = new HashSet<ITypeParameterSymbol>(SymbolEqualityComparer.Default);
        CollectTypeParameters(method.Parameters[0].Type, used);
        return method.TypeParameters.All(used.Contains);
    }

    private static void CollectTypeParameters(ITypeSymbol type, HashSet<ITypeParameterSymbol> used)
    {
        switch (type)
        {
            case ITypeParameterSymbol typeParameter:
                used.Add(typeParameter);
                break;
            case IArrayTypeSymbol arrayType:
                CollectTypeParameters(arrayType.ElementType, used);
                break;
            case INamedTypeSymbol namedType:
                foreach (var typeArgument in namedType.TypeArguments)
                {
                    CollectTypeParameters(typeArgument, used);
                }

                break;
        }
    }

    private static bool ContainsTypeParameter(ITypeSymbol type)
    {
        var used = new HashSet<ITypeParameterSymbol>(SymbolEqualityComparer.Default);
        CollectTypeParameters(type, used);
        return used.Count > 0;
    }
}