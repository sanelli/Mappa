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
    /// Resolves a before-map or after-map hook method.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="mapClass">The mapper class.</param>
    /// <param name="lookupType">The type on which hook methods were located.</param>
    /// <param name="methods">Candidate methods located by name.</param>
    /// <param name="methodName">The hook method name.</param>
    /// <param name="mappedValueType">The source type for a before hook or target type for an after hook.</param>
    /// <param name="nullableEnabled"><c>true</c> when nullable is enabled.</param>
    /// <param name="staticRequirement">The static requirement.</param>
    /// <param name="mapMethodProvidesContext"><c>true</c> when the mapping method provides a context.</param>
    /// <param name="method">The resolved method when successful.</param>
    /// <param name="ambiguityDetails">Ambiguity details when resolution is ambiguous.</param>
    /// <returns>The resolution result.</returns>
    internal static InvokeMethodResolutionResult TryResolveMapHook(
        Compilation compilation,
        ITypeSymbol mapClass,
        ITypeSymbol lookupType,
        IMethodSymbol[] methods,
        string methodName,
        ITypeSymbol mappedValueType,
        bool nullableEnabled,
        InvokeMethodStaticRequirement staticRequirement,
        bool mapMethodProvidesContext,
        out IMethodSymbol? method,
        out string ambiguityDetails)
    {
        method = null;
        ambiguityDetails = string.Empty;

        var candidates = methods
            .Where(candidate =>
                candidate.Name.Equals(methodName, StringComparison.Ordinal) &&
                candidate.IsVoid() &&
                compilation.IsSymbolAccessibleWithin(candidate, mapClass) &&
                staticRequirement switch
                {
                    InvokeMethodStaticRequirement.StaticOrNotStatic => true,
                    InvokeMethodStaticRequirement.Static => candidate.IsStatic,
                    InvokeMethodStaticRequirement.NotStatic => !candidate.IsStatic,
                    _ => false,
                })
            .ToArray();

        bool IsMappedValueParameter(IParameterSymbol parameter)
            => parameter.RefKind is RefKind.Ref &&
               parameter.Type.IsEqualTo(mappedValueType, nullableEnabled);

        bool IsContextParameter(IMethodSymbol candidate, int index)
            => candidate.Parameters[index].RefKind is RefKind.None &&
               candidate.ParameterIsMappaContext(compilation, index);

        if (mapMethodProvidesContext)
        {
            var mappedValueAndContextResult = PickFromTier(
                candidates,
                methodName,
                lookupType.ToDisplayString(),
                candidate => candidate.Parameters.Length == 2 &&
                             IsMappedValueParameter(candidate.Parameters[0]) &&
                             IsContextParameter(candidate, 1));
            if (mappedValueAndContextResult.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = mappedValueAndContextResult.Method;
                ambiguityDetails = mappedValueAndContextResult.AmbiguityDetails;
                return mappedValueAndContextResult.Status;
            }
        }

        var mappedValueResult = PickFromTier(
            candidates,
            methodName,
            lookupType.ToDisplayString(),
            candidate => candidate.Parameters.Length == 1 &&
                         IsMappedValueParameter(candidate.Parameters[0]));
        if (mappedValueResult.Status is not InvokeMethodResolutionResult.NotFound)
        {
            method = mappedValueResult.Method;
            ambiguityDetails = mappedValueResult.AmbiguityDetails;
            return mappedValueResult.Status;
        }

        if (mapMethodProvidesContext)
        {
            var contextResult = PickFromTier(
                candidates,
                methodName,
                lookupType.ToDisplayString(),
                candidate => candidate.Parameters.Length == 1 &&
                             IsContextParameter(candidate, 0));
            if (contextResult.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = contextResult.Method;
                ambiguityDetails = contextResult.AmbiguityDetails;
                return contextResult.Status;
            }
        }

        var parameterlessResult = PickFromTier(
            candidates,
            methodName,
            lookupType.ToDisplayString(),
            candidate => candidate.Parameters.Length == 0);
        method = parameterlessResult.Method;
        ambiguityDetails = parameterlessResult.AmbiguityDetails;
        return parameterlessResult.Status;
    }

    /// <summary>
    /// Resolves an invoke method for <see cref="Mappa.Attributes.MappaNullSubstituteAttribute"/> InvokeMethod mode.
    /// Eligible signatures are <c>T Method(T?)</c> / <c>T Method(T)</c> / <c>T Method(in T)</c>
    /// and the same with a trailing <c>MappaContext</c> when the map method provides context.
    /// </summary>
    /// <param name="compilation">The compilation.</param>
    /// <param name="mapClass">The mapper class (for accessibility).</param>
    /// <param name="lookupTypeDisplayName">Display name used in ambiguity messages.</param>
    /// <param name="methods">Candidate methods located by name.</param>
    /// <param name="methodName">The method name.</param>
    /// <param name="sourceMemberType">The source member type <c>T</c> being substituted.</param>
    /// <param name="nullableEnabled"><c>true</c> when nullable is enabled.</param>
    /// <param name="staticRequirement">The static requirement.</param>
    /// <param name="rootMapMethod">The root map method.</param>
    /// <param name="method">The resolved method when successful.</param>
    /// <param name="ambiguityDetails">Ambiguity details when resolution is ambiguous.</param>
    /// <returns>The resolution result.</returns>
    internal static InvokeMethodResolutionResult TryResolveNullSubstituteInvokeMethod(
        Compilation compilation,
        ITypeSymbol mapClass,
        string lookupTypeDisplayName,
        IMethodSymbol[] methods,
        string methodName,
        ITypeSymbol sourceMemberType,
        bool nullableEnabled,
        InvokeMethodStaticRequirement staticRequirement,
        MapMethod rootMapMethod,
        out IMethodSymbol? method,
        out string ambiguityDetails)
    {
        method = null;
        ambiguityDetails = string.Empty;

        var rootProvidesMappaContext = rootMapMethod.ProvideMappaContextWhenInvoked();
        var expectedReturnType = GetNullSubstituteExpectedReturnType(sourceMemberType);
        var candidates = methods
            .Where(candidate =>
                candidate.Name.Equals(methodName, StringComparison.Ordinal) &&
                compilation.IsSymbolAccessibleWithin(candidate, mapClass) &&
                MatchesStaticRequirement(candidate, staticRequirement) &&
                IsNullSubstituteInvokeMethodSignature(
                    candidate,
                    compilation,
                    sourceMemberType,
                    expectedReturnType,
                    nullableEnabled,
                    rootProvidesMappaContext))
            .ToArray();

        // Prefer most-derived declaring type, then prefer overloads with MappaContext when available.
        if (rootProvidesMappaContext)
        {
            var withContext = PickFromTier(
                candidates,
                methodName,
                lookupTypeDisplayName,
                candidate => candidate.Parameters.Length == 2);
            if (withContext.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = withContext.Method;
                ambiguityDetails = withContext.AmbiguityDetails;
                return withContext.Status;
            }
        }

        return ResolveUniqueCandidate(
            candidates.Where(candidate => candidate.Parameters.Length == 1).ToArray(),
            methodName,
            lookupTypeDisplayName,
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

        var methodsWithTheRightNameAndReturnType = FilterCandidatesByNameReturnTypeAndStatic(
                methods,
                compilation,
                mapClass,
                methodName,
                targetType,
                nullableEnabled,
                staticRequirement)
            .ToArray();

        if (methodsWithTheRightNameAndReturnType.Length == 0)
        {
            return InvokeMethodResolutionResult.NotFound;
        }

        var rootProvidesMappaContext = rootMapMethod.ProvideMappaContextWhenInvoked();
        var typeDisplayName = mapClass.ToDisplayString();
        var sourcePropertyType = sourceProperty?.Type;

        foreach (var tierPredicate in BuildMappaInvokeMethodResolutionTiers(
                     compilation,
                     sourceClassType,
                     sourcePropertyType,
                     nullableEnabled,
                     rootProvidesMappaContext))
        {
            var tierResult = PickFromTier(
                methodsWithTheRightNameAndReturnType,
                methodName,
                typeDisplayName,
                tierPredicate);
            if (tierResult.Status is not InvokeMethodResolutionResult.NotFound)
            {
                method = tierResult.Method;
                ambiguityDetails = tierResult.AmbiguityDetails;
                return tierResult.Status;
            }
        }

        return InvokeMethodResolutionResult.NotFound;
    }

    private static IEnumerable<Func<IMethodSymbol, bool>> BuildMappaInvokeMethodResolutionTiers(
        Compilation compilation,
        ITypeSymbol sourceClassType,
        ITypeSymbol? sourcePropertyType,
        bool nullableEnabled,
        bool rootProvidesMappaContext)
    {
        foreach (var tier in BuildExactAndImplicitSourcePropertyTiers(
                     compilation,
                     sourceClassType,
                     sourcePropertyType,
                     nullableEnabled,
                     rootProvidesMappaContext))
        {
            yield return tier;
        }

        foreach (var tier in BuildSourceOnlyResolutionTiers(
                     compilation,
                     sourceClassType,
                     nullableEnabled,
                     rootProvidesMappaContext))
        {
            yield return tier;
        }

        foreach (var tier in BuildPropertyOnlyAndEmptyResolutionTiers(
                     compilation,
                     sourcePropertyType,
                     nullableEnabled,
                     rootProvidesMappaContext))
        {
            yield return tier;
        }
    }

    private static IEnumerable<Func<IMethodSymbol, bool>> BuildExactAndImplicitSourcePropertyTiers(
        Compilation compilation,
        ITypeSymbol sourceClassType,
        ITypeSymbol? sourcePropertyType,
        bool nullableEnabled,
        bool rootProvidesMappaContext)
    {
        if (sourcePropertyType is null)
        {
            yield break;
        }

        bool IsExactSourceType(ITypeSymbol parameterType)
            => parameterType.IsEqualTo(sourceClassType, nullableEnabled);

        bool IsImplicitSourceType(ITypeSymbol parameterType)
            => IsExactSourceType(parameterType) ||
               compilation.HasImplicitConversion(sourceClassType, parameterType);

        bool IsExactSourcePropertyType(ITypeSymbol parameterType)
            => parameterType.IsEqualTo(sourcePropertyType, nullableEnabled);

        bool IsImplicitSourcePropertyType(ITypeSymbol parameterType)
            => IsExactSourcePropertyType(parameterType) ||
               compilation.HasImplicitConversion(sourcePropertyType, parameterType);

        if (rootProvidesMappaContext)
        {
            yield return candidate => candidate.Parameters.Length == 3 &&
                                      IsExactSourceType(candidate.Parameters[0].Type) &&
                                      IsExactSourcePropertyType(candidate.Parameters[1].Type) &&
                                      candidate.ParameterIsMappaContext(compilation, 2);
        }

        yield return candidate => candidate.Parameters.Length == 2 &&
                                  IsExactSourceType(candidate.Parameters[0].Type) &&
                                  IsExactSourcePropertyType(candidate.Parameters[1].Type);

        if (rootProvidesMappaContext)
        {
            yield return candidate => candidate.Parameters.Length == 3 &&
                                      IsImplicitSourceType(candidate.Parameters[0].Type) &&
                                      IsImplicitSourcePropertyType(candidate.Parameters[1].Type) &&
                                      candidate.ParameterIsMappaContext(compilation, 2);
        }

        yield return candidate => candidate.Parameters.Length == 2 &&
                                  IsImplicitSourceType(candidate.Parameters[0].Type) &&
                                  IsImplicitSourcePropertyType(candidate.Parameters[1].Type);
    }

    private static IEnumerable<Func<IMethodSymbol, bool>> BuildSourceOnlyResolutionTiers(
        Compilation compilation,
        ITypeSymbol sourceClassType,
        bool nullableEnabled,
        bool rootProvidesMappaContext)
    {
        bool IsExactSourceType(ITypeSymbol parameterType)
            => parameterType.IsEqualTo(sourceClassType, nullableEnabled);

        bool IsImplicitSourceType(ITypeSymbol parameterType)
            => IsExactSourceType(parameterType) ||
               compilation.HasImplicitConversion(sourceClassType, parameterType);

        if (rootProvidesMappaContext)
        {
            yield return candidate => candidate.Parameters.Length == 2 &&
                                      IsExactSourceType(candidate.Parameters[0].Type) &&
                                      candidate.ParameterIsMappaContext(compilation, 1);
        }

        yield return candidate => candidate.Parameters.Length == 1 &&
                                  IsExactSourceType(candidate.Parameters[0].Type);

        if (rootProvidesMappaContext)
        {
            yield return candidate => candidate.Parameters.Length == 2 &&
                                      IsImplicitSourceType(candidate.Parameters[0].Type) &&
                                      candidate.ParameterIsMappaContext(compilation, 1);
        }

        yield return candidate => candidate.Parameters.Length == 1 &&
                                  compilation.HasImplicitConversion(sourceClassType, candidate.Parameters[0].Type);
    }

    private static IEnumerable<Func<IMethodSymbol, bool>> BuildPropertyOnlyAndEmptyResolutionTiers(
        Compilation compilation,
        ITypeSymbol? sourcePropertyType,
        bool nullableEnabled,
        bool rootProvidesMappaContext)
    {
        if (sourcePropertyType is not null)
        {
            bool IsExactSourcePropertyType(ITypeSymbol parameterType)
                => parameterType.IsEqualTo(sourcePropertyType, nullableEnabled);

            bool IsImplicitSourcePropertyType(ITypeSymbol parameterType)
                => IsExactSourcePropertyType(parameterType) ||
                   compilation.HasImplicitConversion(sourcePropertyType, parameterType);

            if (rootProvidesMappaContext)
            {
                yield return candidate => candidate.Parameters.Length == 2 &&
                                          IsExactSourcePropertyType(candidate.Parameters[0].Type) &&
                                          candidate.ParameterIsMappaContext(compilation, 1);
            }

            yield return candidate => candidate.Parameters.Length == 1 &&
                                      IsExactSourcePropertyType(candidate.Parameters[0].Type);

            if (rootProvidesMappaContext)
            {
                yield return candidate => candidate.Parameters.Length == 2 &&
                                          IsImplicitSourcePropertyType(candidate.Parameters[0].Type) &&
                                          candidate.ParameterIsMappaContext(compilation, 1);
            }

            yield return candidate => candidate.Parameters.Length == 1 &&
                                      compilation.HasImplicitConversion(sourcePropertyType, candidate.Parameters[0].Type);
        }

        if (rootProvidesMappaContext)
        {
            yield return candidate => candidate.Parameters.Length == 1 &&
                                      candidate.ParameterIsMappaContext(compilation, 0);
        }

        yield return candidate => candidate.Parameters.Length == 0;
    }

    private static IEnumerable<IMethodSymbol> FilterCandidatesByNameReturnTypeAndStatic(
        IMethodSymbol[] methods,
        Compilation compilation,
        ITypeSymbol mapClass,
        string methodName,
        ITypeSymbol targetType,
        bool nullableEnabled,
        InvokeMethodStaticRequirement staticRequirement)
    {
        return methods.Where(candidate =>
            candidate.Name.Equals(methodName, StringComparison.Ordinal) &&
            compilation.IsSymbolAccessibleWithin(candidate, mapClass) &&
            MatchesReturnType(compilation, candidate.ReturnType, targetType, nullableEnabled) &&
            MatchesStaticRequirement(candidate, staticRequirement));
    }

    private static bool MatchesReturnType(
        Compilation compilation,
        ITypeSymbol returnType,
        ITypeSymbol targetType,
        bool nullableEnabled)
        => returnType.IsEqualTo(targetType, nullableEnabled) ||
           compilation.HasImplicitConversion(returnType, targetType);

    private static bool MatchesStaticRequirement(IMethodSymbol candidate, InvokeMethodStaticRequirement staticRequirement)
        => staticRequirement switch
        {
            InvokeMethodStaticRequirement.StaticOrNotStatic => true,
            InvokeMethodStaticRequirement.Static => candidate.IsStatic,
            InvokeMethodStaticRequirement.NotStatic => !candidate.IsStatic,
            _ => false,
        };

    private static ITypeSymbol GetNullSubstituteExpectedReturnType(ITypeSymbol sourceMemberType)
    {
        if (sourceMemberType.IsValueTypeNullable())
        {
            return sourceMemberType.GetTypeInsideNullable();
        }

        if (sourceMemberType is { IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated })
        {
            return sourceMemberType.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        }

        return sourceMemberType;
    }

    private static bool IsNullSubstituteInvokeMethodSignature(
        IMethodSymbol candidate,
        Compilation compilation,
        ITypeSymbol sourceMemberType,
        ITypeSymbol expectedReturnType,
        bool nullableEnabled,
        bool rootProvidesMappaContext)
    {
        if (candidate.MethodKind is not MethodKind.Ordinary
            || candidate.IsAsync
            || candidate.ReturnsAnyTaskType(compilation)
            || candidate.IsVoid()
            || candidate.ReturnsByRef
            || candidate.ReturnsByRefReadonly)
        {
            return false;
        }

        if (candidate.Parameters.Length is not (1 or 2))
        {
            return false;
        }

        if (candidate.Parameters[0].RefKind is not (RefKind.None or RefKind.In))
        {
            return false;
        }

        if (!candidate.ReturnType.IsEqualTo(expectedReturnType, nullableEnabled))
        {
            return false;
        }

        if (!IsNullSubstituteValueParameterType(candidate.Parameters[0].Type, sourceMemberType, expectedReturnType, nullableEnabled))
        {
            return false;
        }

        if (candidate.Parameters.Length == 1)
        {
            return true;
        }

        return rootProvidesMappaContext
               && candidate.ParameterIsMappaContext(compilation, 1)
               && candidate.Parameters[1].RefKind is (RefKind.None or RefKind.In);
    }

    private static bool IsNullSubstituteValueParameterType(
        ITypeSymbol parameterType,
        ITypeSymbol sourceMemberType,
        ITypeSymbol expectedReturnType,
        bool nullableEnabled)
    {
        if (parameterType.IsEqualTo(sourceMemberType, nullableEnabled))
        {
            return true;
        }

        // Accept T when the source is T?, and T? when matching without requiring NRT annotations.
        if (parameterType.IsEqualTo(expectedReturnType, nullableEnabled))
        {
            return true;
        }

        if (!nullableEnabled)
        {
            return false;
        }

        var annotatedReturn = expectedReturnType.WithNullableAnnotation(NullableAnnotation.Annotated);
        return parameterType.IsEqualTo(annotatedReturn, isNullableEnabled: true);
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