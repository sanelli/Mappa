// <copyright file="InvokeMethodResolution.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Models;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Helpers;

/// <summary>
/// The result of resolving an invoke method candidate.
/// </summary>
internal enum InvokeMethodResolutionResult
{
    /// <summary>
    /// No matching method was found.
    /// </summary>
    NotFound,

    /// <summary>
    /// Exactly one matching method was found.
    /// </summary>
    Success,

    /// <summary>
    /// Multiple matching methods were found.
    /// </summary>
    Ambiguous,
}

/// <summary>
/// Static requirement for invoke-method resolution.
/// </summary>
internal enum InvokeMethodStaticRequirement
{
    /// <summary>
    /// Static or instance methods are allowed.
    /// </summary>
    StaticOrNotStatic,

    /// <summary>
    /// Only static methods are allowed.
    /// </summary>
    Static,

    /// <summary>
    /// Only instance methods are allowed.
    /// </summary>
    NotStatic,
}

/// <summary>
/// Resolves invoke methods for polymorphism defaults and <see cref="Mappa.Attributes.MappaInvokeMethodAttribute"/>.
/// </summary>
internal static class InvokeMethodResolution
{
    /// <summary>
    /// Resolves a polymorphism default invoke method.
    /// </summary>
    /// <param name="invokeMethodTypeSymbol">The type on which to locate methods.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="expectedSourceType">The expected source type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="mustBeStatic"><c>true</c> when the method must be static.</param>
    /// <param name="nullableEnabled"><c>true</c> when nullable is enabled.</param>
    /// <param name="acceptTwoParameters"><c>true</c> when a context parameter overload is allowed.</param>
    /// <param name="method">The resolved method when successful.</param>
    /// <param name="ambiguityDetails">Ambiguity details when resolution is ambiguous.</param>
    /// <returns>The resolution result.</returns>
    internal static InvokeMethodResolutionResult TryResolvePolymorphismInvokeMethod(
        ITypeSymbol invokeMethodTypeSymbol,
        string methodName,
        ITypeSymbol expectedSourceType,
        Compilation compilation,
        bool mustBeStatic,
        bool nullableEnabled,
        bool acceptTwoParameters,
        out IMethodSymbol? method,
        out string ambiguityDetails)
    {
        var candidates = invokeMethodTypeSymbol
            .LocateMethods(methodName)
            .Where(candidate => candidate.IsMethodValidToMapToTargetSymbolForPolymorphism(
                expectedSourceType,
                compilation,
                mustBeStatic,
                nullableEnabled,
                acceptTwoParameters))
            .ToArray();

        return ResolveUniqueCandidate(
            candidates,
            methodName,
            invokeMethodTypeSymbol.ToDisplayString(),
            out method,
            out ambiguityDetails);
    }

    /// <summary>
    /// Resolves an invoke method for <see cref="Mappa.Attributes.MappaInvokeMethodAttribute"/>.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="mapClass">The mapper class.</param>
    /// <param name="methods">Candidate methods located by name.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceClassType">The source class type.</param>
    /// <param name="sourceProperty">The source property, if any.</param>
    /// <param name="nullableEnabled"><c>true</c> when nullable is enabled.</param>
    /// <param name="staticRequirement">The static requirement.</param>
    /// <param name="rootMapMethod">The root map method.</param>
    /// <param name="method">The resolved method when successful.</param>
    /// <param name="ambiguityDetails">Ambiguity details when resolution is ambiguous.</param>
    /// <returns>The resolution result.</returns>
    internal static InvokeMethodResolutionResult TryResolveMappaInvokeMethod(
        Compilation compilation,
        ITypeSymbol mapClass,
        IMethodSymbol[] methods,
        string methodName,
        ITypeSymbol targetType,
        ITypeSymbol sourceClassType,
        IPropertySymbol? sourceProperty,
        bool nullableEnabled,
        InvokeMethodStaticRequirement staticRequirement,
        MapMethod rootMapMethod,
        out IMethodSymbol? method,
        out string ambiguityDetails)
    {
        method = null;
        ambiguityDetails = string.Empty;

        var methodsWithTheRightNameAndReturnType = methods
            .Where(candidate =>
                candidate.Name.Equals(methodName, StringComparison.Ordinal) &&
                compilation.IsSymbolAccessibleWithin(candidate, mapClass) &&
                (candidate.ReturnType.IsEqualTo(targetType, nullableEnabled) ||
                 compilation.HasImplicitConversion(candidate.ReturnType, targetType)) &&
                staticRequirement switch
                {
                    InvokeMethodStaticRequirement.StaticOrNotStatic => true,
                    InvokeMethodStaticRequirement.Static => candidate.IsStatic,
                    InvokeMethodStaticRequirement.NotStatic => !candidate.IsStatic,
                    _ => false,
                })
            .ToArray();

        if (methodsWithTheRightNameAndReturnType.Length == 0)
        {
            return InvokeMethodResolutionResult.NotFound;
        }

        var rootProvidesMappaContext = rootMapMethod.ProvideMappaContextWhenInvoked();
        var typeDisplayName = mapClass.ToDisplayString();

        bool IsExactSourceType(ITypeSymbol parameterType)
            => parameterType.IsEqualTo(sourceClassType, nullableEnabled);

        bool IsImplicitSourceType(ITypeSymbol parameterType)
            => IsExactSourceType(parameterType) ||
               compilation.HasImplicitConversion(sourceClassType, parameterType);

        bool IsExactSourcePropertyType(ITypeSymbol parameterType)
            => sourceProperty is not null &&
               parameterType.IsEqualTo(sourceProperty.Type, nullableEnabled);

        bool IsImplicitSourcePropertyType(ITypeSymbol parameterType)
            => sourceProperty is not null &&
               (IsExactSourcePropertyType(parameterType) ||
                compilation.HasImplicitConversion(sourceProperty.Type, parameterType));

        if (rootProvidesMappaContext && sourceProperty is not null)
        {
            var tier1Result = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                candidate => candidate.Parameters.Length == 3 &&
                             IsExactSourceType(candidate.Parameters[0].Type) &&
                             IsExactSourcePropertyType(candidate.Parameters[1].Type) &&
                             candidate.ParameterIsMappaContext(compilation, 2));
            if (tier1Result.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tier1Result.Method;
                ambiguityDetails = tier1Result.AmbiguityDetails;
                return tier1Result.Status;
            }
        }

        if (sourceProperty is not null)
        {
            var tier2Result = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                candidate => candidate.Parameters.Length == 2 &&
                             IsExactSourceType(candidate.Parameters[0].Type) &&
                             IsExactSourcePropertyType(candidate.Parameters[1].Type));
            if (tier2Result.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tier2Result.Method;
                ambiguityDetails = tier2Result.AmbiguityDetails;
                return tier2Result.Status;
            }
        }

        if (rootProvidesMappaContext && sourceProperty is not null)
        {
            var tier3Result = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                candidate => candidate.Parameters.Length == 3 &&
                             IsImplicitSourceType(candidate.Parameters[0].Type) &&
                             IsImplicitSourcePropertyType(candidate.Parameters[1].Type) &&
                             candidate.ParameterIsMappaContext(compilation, 2));
            if (tier3Result.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tier3Result.Method;
                ambiguityDetails = tier3Result.AmbiguityDetails;
                return tier3Result.Status;
            }
        }

        if (sourceProperty is not null)
        {
            var tier4Result = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                candidate => candidate.Parameters.Length == 2 &&
                             IsImplicitSourceType(candidate.Parameters[0].Type) &&
                             IsImplicitSourcePropertyType(candidate.Parameters[1].Type));
            if (tier4Result.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tier4Result.Method;
                ambiguityDetails = tier4Result.AmbiguityDetails;
                return tier4Result.Status;
            }
        }

        if (rootProvidesMappaContext)
        {
            var tier5Result = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                candidate => candidate.Parameters.Length == 2 &&
                             IsExactSourceType(candidate.Parameters[0].Type) &&
                             candidate.ParameterIsMappaContext(compilation, 1));
            if (tier5Result.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tier5Result.Method;
                ambiguityDetails = tier5Result.AmbiguityDetails;
                return tier5Result.Status;
            }
        }

        var tier6Result = PickFromTier(
            methodsWithTheRightNameAndReturnType,
            methodName,
            typeDisplayName,
            candidate => candidate.Parameters.Length == 1 &&
                         IsExactSourceType(candidate.Parameters[0].Type));
        if (tier6Result.Status is not InvokeMethodResolutionResult.NotFound)
        {
            method = tier6Result.Method;
            ambiguityDetails = tier6Result.AmbiguityDetails;
            return tier6Result.Status;
        }

        if (rootProvidesMappaContext)
        {
            var tier7Result = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                candidate => candidate.Parameters.Length == 2 &&
                             IsImplicitSourceType(candidate.Parameters[0].Type) &&
                             candidate.ParameterIsMappaContext(compilation, 1));
            if (tier7Result.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tier7Result.Method;
                ambiguityDetails = tier7Result.AmbiguityDetails;
                return tier7Result.Status;
            }
        }

        var tier8Result = PickFromTier(
            methodsWithTheRightNameAndReturnType,
            methodName,
            typeDisplayName,
            candidate => candidate.Parameters.Length == 1 &&
                         compilation.HasImplicitConversion(sourceClassType, candidate.Parameters[0].Type));
        if (tier8Result.Status is not InvokeMethodResolutionResult.NotFound)
        {
            method = tier8Result.Method;
            ambiguityDetails = tier8Result.AmbiguityDetails;
            return tier8Result.Status;
        }

        if (rootProvidesMappaContext && sourceProperty is not null)
        {
            var tier9Result = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                candidate => candidate.Parameters.Length == 2 &&
                             IsExactSourcePropertyType(candidate.Parameters[0].Type) &&
                             candidate.ParameterIsMappaContext(compilation, 1));
            if (tier9Result.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tier9Result.Method;
                ambiguityDetails = tier9Result.AmbiguityDetails;
                return tier9Result.Status;
            }
        }

        if (sourceProperty is not null)
        {
            var tier10Result = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                candidate => candidate.Parameters.Length == 1 &&
                             IsExactSourcePropertyType(candidate.Parameters[0].Type));
            if (tier10Result.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tier10Result.Method;
                ambiguityDetails = tier10Result.AmbiguityDetails;
                return tier10Result.Status;
            }
        }

        if (rootProvidesMappaContext && sourceProperty is not null)
        {
            var tier11Result = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                candidate => candidate.Parameters.Length == 2 &&
                             IsImplicitSourcePropertyType(candidate.Parameters[0].Type) &&
                             candidate.ParameterIsMappaContext(compilation, 1));
            if (tier11Result.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tier11Result.Method;
                ambiguityDetails = tier11Result.AmbiguityDetails;
                return tier11Result.Status;
            }
        }

        if (sourceProperty is not null)
        {
            var tier12Result = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                candidate => candidate.Parameters.Length == 1 &&
                             compilation.HasImplicitConversion(sourceProperty.Type, candidate.Parameters[0].Type));
            if (tier12Result.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tier12Result.Method;
                ambiguityDetails = tier12Result.AmbiguityDetails;
                return tier12Result.Status;
            }
        }

        if (rootProvidesMappaContext)
        {
            var tier13Result = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                candidate => candidate.Parameters.Length == 1 &&
                             candidate.ParameterIsMappaContext(compilation, 0));
            if (tier13Result.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tier13Result.Method;
                ambiguityDetails = tier13Result.AmbiguityDetails;
                return tier13Result.Status;
            }
        }

        var tier14Result = PickFromTier(
            methodsWithTheRightNameAndReturnType,
            methodName,
            typeDisplayName,
            candidate => candidate.Parameters.Length == 0);
        if (tier14Result.Status is not InvokeMethodResolutionResult.NotFound)
        {
            method = tier14Result.Method;
            ambiguityDetails = tier14Result.AmbiguityDetails;
            return tier14Result.Status;
        }

        return InvokeMethodResolutionResult.NotFound;
    }

    private static TierPickResult PickFromTier(
        IMethodSymbol[] pool,
        string methodName,
        string typeDisplayName,
        Func<IMethodSymbol, bool> predicate)
    {
        var matches = pool.Where(predicate).ToArray();
        if (matches.Length == 0)
        {
            return new TierPickResult(InvokeMethodResolutionResult.NotFound, null, string.Empty);
        }

        var result = ResolveUniqueCandidate(matches, methodName, typeDisplayName, out var resolvedMethod, out var details);
        return new TierPickResult(result, resolvedMethod, details);
    }

    private static InvokeMethodResolutionResult ResolveUniqueCandidate(
        IMethodSymbol[] candidates,
        string methodName,
        string typeDisplayName,
        out IMethodSymbol? method,
        out string ambiguityDetails)
    {
        method = null;
        ambiguityDetails = string.Empty;

        if (candidates.Length == 0)
        {
            return InvokeMethodResolutionResult.NotFound;
        }

        if (candidates.Length == 1)
        {
            method = candidates[0];
            return InvokeMethodResolutionResult.Success;
        }

        var preferredContainingType = candidates[0].ContainingType;
        var filteredCandidates = candidates
            .Where(candidate => SymbolEqualityComparer.Default.Equals(candidate.ContainingType, preferredContainingType))
            .ToArray();

        if (filteredCandidates.Length == 1)
        {
            method = filteredCandidates[0];
            return InvokeMethodResolutionResult.Success;
        }

        ambiguityDetails = FormatAmbiguousInvokeMethodCandidates(
            methodName,
            typeDisplayName,
            filteredCandidates.Length > 1 ? filteredCandidates : candidates);
        return InvokeMethodResolutionResult.Ambiguous;
    }

    private static string FormatAmbiguousInvokeMethodCandidates(
        string methodName,
        string typeDisplayName,
        IMethodSymbol[] candidates)
        => $"multiple methods named '{methodName}' in '{typeDisplayName}' match: {string.Join(", ", candidates.Select(candidate => candidate.ToDisplayString()))}";

    private sealed class TierPickResult
    {
        internal TierPickResult(InvokeMethodResolutionResult status, IMethodSymbol? method, string ambiguityDetails)
        {
            this.Status = status;
            this.Method = method;
            this.AmbiguityDetails = ambiguityDetails;
        }

        internal InvokeMethodResolutionResult Status { get; }

        internal IMethodSymbol? Method { get; }

        internal string AmbiguityDetails { get; }
    }
}