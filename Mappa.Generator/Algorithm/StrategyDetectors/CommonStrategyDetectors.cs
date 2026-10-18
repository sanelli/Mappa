// <copyright file="CommonStrategyDetectors.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Algorithm.StrategyDetectors;

/// <summary>
/// Common strategy algorithms.
/// </summary>
internal static class CommonStrategyDetectors
{
    /// <summary>
    /// Attempt to identify a strategy for the element type of containers.
    /// </summary>
    /// <param name="context">The context of the mapping.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="compilation">The compilation unit.</param>
    /// <param name="elementStrategy">The strategy between the source and target types defined in the context.</param>
    /// <param name="cancellationToken">The cancellation token of the operation.</param>
    /// <returns><c>true</c> if the mapping exists, <c>false</c> otherwise.</returns>
    internal static bool TryGetElementStrategy(
        this MappaMapAlgorithmContext context,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        Compilation compilation,
        out MapStrategy elementStrategy,
        CancellationToken cancellationToken)
        => context.TryGetElementStrategy(
            targetType,
            sourceType,
            compilation,
            out elementStrategy,
            out _,
            out _,
            cancellationToken);

    /// <summary>
    /// Attempt to identify a strategy for the element type of containers.
    /// </summary>
    /// <param name="context">The context of the mapping.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="compilation">The compilation unit.</param>
    /// <param name="elementStrategy">The strategy between the source and target types defined in the context.</param>
    /// <param name="preferredSourceInterface">The resolved preferred source interface, if any.</param>
    /// <param name="preferredTargetInterface">The resolved preferred target interface, if any.</param>
    /// <param name="cancellationToken">The cancellation token of the operation.</param>
    /// <returns><c>true</c> if the mapping exists, <c>false</c> otherwise.</returns>
    internal static bool TryGetElementStrategy(
        this MappaMapAlgorithmContext context,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        Compilation compilation,
        out MapStrategy elementStrategy,
        out INamedTypeSymbol? preferredSourceInterface,
        out INamedTypeSymbol? preferredTargetInterface,
        CancellationToken cancellationToken)
    {
        elementStrategy = new NoMapStrategy(targetType, sourceType);
        preferredSourceInterface = null;
        preferredTargetInterface = null;

        if (sourceType is INamedTypeSymbol sourceNamed
            && targetType is INamedTypeSymbol targetNamed
            && !TryResolvePreferredElementInterfaces(
                context,
                compilation,
                sourceNamed,
                targetNamed,
                PreferredInterfaceFamily.IEnumerable,
                cancellationToken,
                out preferredSourceInterface,
                out preferredTargetInterface))
        {
            return false;
        }

        var sourceElementType = preferredSourceInterface?.TypeArguments[0]
            ?? sourceType.GetElementType(context, compilation, PreferredInterfaceSide.Source);
        var targetElementType = preferredTargetInterface?.TypeArguments[0]
            ?? targetType.GetElementType(context, compilation, PreferredInterfaceSide.Target);
        var derivedContext = new DerivedMappaMapAlgorithmContext(
            context,
            targetElementType,
            sourceElementType);
        var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(derivedContext, compilation, cancellationToken);
        elementStrategy = algorithm.GetStrategy();
        return elementStrategy is not NoMapStrategy;
    }

    /// <summary>
    /// Attempt to identify a strategy for the element type of containers.
    /// </summary>
    /// <param name="context">The context of the mapping.</param>
    /// <param name="compilation">The compilation unit.</param>
    /// <param name="elementStrategy">The strategy between the source and target types defined in the context.</param>
    /// <param name="cancellationToken">The cancellation token of the operation.</param>
    /// <returns><c>true</c> if the mapping exists, <c>false</c> otherwise.</returns>
    internal static bool TryGetElementStrategy(
        this MappaMapAlgorithmContext context,
        Compilation compilation,
        out MapStrategy elementStrategy,
        CancellationToken cancellationToken)
        => context.TryGetElementStrategy(
            context.TargetType,
            context.SourceType,
            compilation,
            out elementStrategy,
            cancellationToken);

    /// <summary>
    /// Gets (if possible) the mapping for the Key type and the Value type
    /// of the source and target dictionaries.
    /// </summary>
    /// <param name="context">The mapping context.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="keyStrategy">The strategy for the key type.</param>
    /// <param name="valueStrategy">The strategy for the value type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the mappings exist, <c>false</c> otherwise.</returns>
    internal static bool TryGetKeyAndValueStrategy(
            this MappaMapAlgorithmContext context,
            ITypeSymbol targetType,
            ITypeSymbol sourceType,
            Compilation compilation,
            out MapStrategy keyStrategy,
            out MapStrategy valueStrategy,
            CancellationToken cancellationToken)
        => context.TryGetKeyAndValueStrategy(
            targetType,
            sourceType,
            compilation,
            out keyStrategy,
            out valueStrategy,
            out _,
            out _,
            cancellationToken);

    /// <summary>
    /// Gets (if possible) the mapping for the Key type and the Value type
    /// of the source and target dictionaries.
    /// </summary>
    /// <param name="context">The mapping context.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="keyStrategy">The strategy for the key type.</param>
    /// <param name="valueStrategy">The strategy for the value type.</param>
    /// <param name="preferredSourceInterface">The resolved preferred source interface, if any.</param>
    /// <param name="preferredTargetInterface">The resolved preferred target interface, if any.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the mappings exist, <c>false</c> otherwise.</returns>
    internal static bool TryGetKeyAndValueStrategy(
            this MappaMapAlgorithmContext context,
            ITypeSymbol targetType,
            ITypeSymbol sourceType,
            Compilation compilation,
            out MapStrategy keyStrategy,
            out MapStrategy valueStrategy,
            out INamedTypeSymbol? preferredSourceInterface,
            out INamedTypeSymbol? preferredTargetInterface,
            CancellationToken cancellationToken)
    {
        keyStrategy = new NoMapStrategy(targetType, sourceType);
        valueStrategy = new NoMapStrategy(targetType, sourceType);
        preferredSourceInterface = null;
        preferredTargetInterface = null;

        var dictionaryFamily = GetDictionaryPreferredInterfaceFamily(sourceType, targetType, compilation);

        if (sourceType is INamedTypeSymbol sourceNamed
            && targetType is INamedTypeSymbol targetNamed
            && !TryResolvePreferredDictionaryInterfaces(
                context,
                compilation,
                sourceNamed,
                targetNamed,
                dictionaryFamily,
                cancellationToken,
                out preferredSourceInterface,
                out preferredTargetInterface))
        {
            return false;
        }

        var (sourceKeyType, sourceKeyValueType) = preferredSourceInterface is { TypeArguments.Length: 2 } sourceDictionary
            ? (sourceDictionary.TypeArguments[0], sourceDictionary.TypeArguments[1])
            : sourceType.GetKeyAndValueTypes(context, compilation, PreferredInterfaceSide.Source);
        var (targetKeyType, targetValueType) = preferredTargetInterface is { TypeArguments.Length: 2 } targetDictionary
            ? (targetDictionary.TypeArguments[0], targetDictionary.TypeArguments[1])
            : targetType.GetKeyAndValueTypes(context, compilation, PreferredInterfaceSide.Target);

        // Get strategy for key
        var keyContext = new DerivedMappaMapAlgorithmContext(
            context,
            targetKeyType,
            sourceKeyType);
        var keyAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(keyContext, compilation, cancellationToken);
        keyStrategy = keyAlgorithm.GetStrategy();

        // Get strategy for value
        var valueContext = new DerivedMappaMapAlgorithmContext(
            context,
            targetValueType,
            sourceKeyValueType);
        var valueAlgorithm = new TypeMapIdentifierWithMapMethodAlgorithm(valueContext, compilation, cancellationToken);
        valueStrategy = valueAlgorithm.GetStrategy();

        return keyStrategy is not NoMapStrategy && valueStrategy is not NoMapStrategy;
    }

    /// <summary>
    /// Gets (if possible) the mapping for the Key type and the Value type
    /// of the source and target dictionaries.
    /// </summary>
    /// <param name="context">The mapping context.</param>
    /// <param name="compilation">The compilation.</param>
    /// <param name="keyStrategy">The strategy for the key type.</param>
    /// <param name="valueStrategy">The strategy for the value type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns><c>true</c> if the mappings exist, <c>false</c> otherwise.</returns>
    internal static bool TryGetKeyAndValueStrategy(
            this MappaMapAlgorithmContext context,
            Compilation compilation,
            out MapStrategy keyStrategy,
            out MapStrategy valueStrategy,
            CancellationToken cancellationToken)
    => context.TryGetKeyAndValueStrategy(
        context.TargetType,
        context.SourceType,
        compilation,
        out keyStrategy,
        out valueStrategy,
        cancellationToken);

    private static bool TryResolvePreferredElementInterfaces(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        INamedTypeSymbol sourceType,
        INamedTypeSymbol targetType,
        PreferredInterfaceFamily family,
        CancellationToken cancellationToken,
        out INamedTypeSymbol? preferredSourceInterface,
        out INamedTypeSymbol? preferredTargetInterface)
    {
        preferredSourceInterface = null;
        preferredTargetInterface = null;

        var targetCandidates = PreferredInterfaceResolver
            .EnumerateClosedInterfaceCandidates(targetType, family, compilation)
            .ToArray();
        var sourceCandidates = PreferredInterfaceResolver
            .EnumerateClosedInterfaceCandidates(sourceType, family, compilation)
            .ToArray();

        INamedTypeSymbol? resolvedTargetInterface = null;
        if (!PreferredInterfaceResolver.TryResolve(
                targetType,
                family,
                PreferredInterfaceSide.Target,
                context,
                compilation,
                candidate => IsTargetElementInterfaceUsable(
                    context,
                    compilation,
                    sourceType,
                    candidate,
                    sourceCandidates,
                    cancellationToken),
                out resolvedTargetInterface,
                out var targetOutcome)
            && targetOutcome == PreferredInterfaceResolutionOutcome.AttributeCannotBeUsed)
        {
            return false;
        }

        preferredTargetInterface = resolvedTargetInterface;

        if (!PreferredInterfaceResolver.TryResolve(
                sourceType,
                family,
                PreferredInterfaceSide.Source,
                context,
                compilation,
                candidate => IsSourceElementInterfaceUsable(
                    context,
                    compilation,
                    targetType,
                    candidate,
                    resolvedTargetInterface,
                    targetCandidates,
                    cancellationToken),
                out preferredSourceInterface,
                out var sourceOutcome)
            && sourceOutcome == PreferredInterfaceResolutionOutcome.AttributeCannotBeUsed)
        {
            return false;
        }

        return true;
    }

    private static bool TryResolvePreferredDictionaryInterfaces(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        INamedTypeSymbol sourceType,
        INamedTypeSymbol targetType,
        PreferredInterfaceFamily family,
        CancellationToken cancellationToken,
        out INamedTypeSymbol? preferredSourceInterface,
        out INamedTypeSymbol? preferredTargetInterface)
    {
        preferredSourceInterface = null;
        preferredTargetInterface = null;

        var targetCandidates = PreferredInterfaceResolver
            .EnumerateClosedInterfaceCandidates(targetType, family, compilation)
            .ToArray();
        var sourceCandidates = PreferredInterfaceResolver
            .EnumerateClosedInterfaceCandidates(sourceType, family, compilation)
            .ToArray();

        INamedTypeSymbol? resolvedTargetInterface = null;
        if (!PreferredInterfaceResolver.TryResolve(
                targetType,
                family,
                PreferredInterfaceSide.Target,
                context,
                compilation,
                candidate => IsTargetDictionaryInterfaceUsable(
                    context,
                    compilation,
                    family,
                    sourceType,
                    candidate,
                    sourceCandidates,
                    cancellationToken),
                out resolvedTargetInterface,
                out var targetOutcome)
            && targetOutcome == PreferredInterfaceResolutionOutcome.AttributeCannotBeUsed)
        {
            return false;
        }

        preferredTargetInterface = resolvedTargetInterface;

        if (!PreferredInterfaceResolver.TryResolve(
                sourceType,
                family,
                PreferredInterfaceSide.Source,
                context,
                compilation,
                candidate => IsSourceDictionaryInterfaceUsable(
                    context,
                    compilation,
                    family,
                    targetType,
                    candidate,
                    resolvedTargetInterface,
                    targetCandidates,
                    cancellationToken),
                out preferredSourceInterface,
                out var sourceOutcome)
            && sourceOutcome == PreferredInterfaceResolutionOutcome.AttributeCannotBeUsed)
        {
            return false;
        }

        return true;
    }

    private static PreferredInterfaceFamily GetDictionaryPreferredInterfaceFamily(
        ITypeSymbol sourceType,
        ITypeSymbol targetType,
        Compilation compilation)
    {
        if (sourceType.IsOrImplementIDictionary(compilation)
            || targetType.IsOrImplementIDictionary(compilation))
        {
            return PreferredInterfaceFamily.IDictionary;
        }

        return PreferredInterfaceFamily.IEnumerableOfKeyValuePair;
    }

    private static bool IsSourceElementInterfaceUsable(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        INamedTypeSymbol targetType,
        INamedTypeSymbol sourceCandidate,
        INamedTypeSymbol? preferredTargetInterface,
        INamedTypeSymbol[] targetCandidates,
        CancellationToken cancellationToken)
    {
        if (sourceCandidate.TypeArguments.Length != 1)
        {
            return false;
        }

        var sourceElementType = sourceCandidate.TypeArguments[0];
        return GetTargetElementTypes(
                targetType,
                preferredTargetInterface,
                targetCandidates,
                compilation)
            .Any(targetElementType => TryGetNestedStrategy(
                context,
                targetElementType,
                sourceElementType,
                compilation,
                cancellationToken));
    }

    private static bool IsTargetElementInterfaceUsable(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        INamedTypeSymbol sourceType,
        INamedTypeSymbol targetCandidate,
        INamedTypeSymbol[] sourceCandidates,
        CancellationToken cancellationToken)
    {
        if (targetCandidate.TypeArguments.Length != 1)
        {
            return false;
        }

        var targetElementType = targetCandidate.TypeArguments[0];
        return GetSourceElementTypes(sourceType, sourceCandidates, compilation)
            .Any(sourceElementType => TryGetNestedStrategy(
                context,
                targetElementType,
                sourceElementType,
                compilation,
                cancellationToken));
    }

    private static bool IsSourceDictionaryInterfaceUsable(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        PreferredInterfaceFamily family,
        INamedTypeSymbol targetType,
        INamedTypeSymbol sourceCandidate,
        INamedTypeSymbol? preferredTargetInterface,
        INamedTypeSymbol[] targetCandidates,
        CancellationToken cancellationToken)
    {
        if (!TryGetDictionaryKeyValueTypes(sourceCandidate, family, compilation, out var sourceKeyType, out var sourceValueType))
        {
            return false;
        }

        return GetTargetDictionaryKeyValueTypes(
                targetType,
                family,
                preferredTargetInterface,
                targetCandidates,
                compilation)
            .Any(pair => TryGetNestedStrategy(context, pair.KeyType, sourceKeyType, compilation, cancellationToken)
                         && TryGetNestedStrategy(context, pair.ValueType, sourceValueType, compilation, cancellationToken));
    }

    private static bool IsTargetDictionaryInterfaceUsable(
        MappaMapAlgorithmContext context,
        Compilation compilation,
        PreferredInterfaceFamily family,
        INamedTypeSymbol sourceType,
        INamedTypeSymbol targetCandidate,
        INamedTypeSymbol[] sourceCandidates,
        CancellationToken cancellationToken)
    {
        if (!TryGetDictionaryKeyValueTypes(targetCandidate, family, compilation, out var targetKeyType, out var targetValueType))
        {
            return false;
        }

        return GetSourceDictionaryKeyValueTypes(sourceType, family, sourceCandidates, compilation)
            .Any(pair => TryGetNestedStrategy(context, targetKeyType, pair.KeyType, compilation, cancellationToken)
                         && TryGetNestedStrategy(context, targetValueType, pair.ValueType, compilation, cancellationToken));
    }

    private static IEnumerable<ITypeSymbol> GetTargetElementTypes(
        INamedTypeSymbol targetType,
        INamedTypeSymbol? preferredTargetInterface,
        INamedTypeSymbol[] targetCandidates,
        Compilation compilation)
    {
        if (preferredTargetInterface is { TypeArguments.Length: 1 })
        {
            return [preferredTargetInterface.TypeArguments[0]];
        }

        if (targetCandidates.Length > 0)
        {
            return DistinctTypeSymbols(
                targetCandidates
                    .Where(candidate => candidate.TypeArguments.Length == 1)
                    .Select(candidate => candidate.TypeArguments[0]));
        }

        return [targetType.GetElementType(null, compilation, PreferredInterfaceSide.Target)];
    }

    private static IEnumerable<ITypeSymbol> GetSourceElementTypes(
        INamedTypeSymbol sourceType,
        INamedTypeSymbol[] sourceCandidates,
        Compilation compilation)
    {
        if (sourceCandidates.Length > 0)
        {
            return DistinctTypeSymbols(
                sourceCandidates
                    .Where(candidate => candidate.TypeArguments.Length == 1)
                    .Select(candidate => candidate.TypeArguments[0]));
        }

        return [sourceType.GetElementType(null, compilation, PreferredInterfaceSide.Source)];
    }

    private static IEnumerable<ITypeSymbol> DistinctTypeSymbols(IEnumerable<ITypeSymbol> typeSymbols)
        => typeSymbols
            .GroupBy(typeSymbol => typeSymbol, SymbolEqualityComparer.Default)
            .Select(group => group.First());

    private static IEnumerable<(ITypeSymbol KeyType, ITypeSymbol ValueType)> GetTargetDictionaryKeyValueTypes(
        INamedTypeSymbol targetType,
        PreferredInterfaceFamily family,
        INamedTypeSymbol? preferredTargetInterface,
        INamedTypeSymbol[] targetCandidates,
        Compilation compilation)
    {
        if (preferredTargetInterface is not null
            && TryGetDictionaryKeyValueTypes(preferredTargetInterface, family, compilation, out var keyType, out var valueType))
        {
            return [(keyType, valueType)];
        }

        if (targetCandidates.Length > 0)
        {
            return targetCandidates
                .Select(candidate => TryGetDictionaryKeyValueTypes(candidate, family, compilation, out var key, out var value)
                    ? (KeyType: key, ValueType: value)
                    : default)
                .Where(pair => pair.KeyType is not null && pair.ValueType is not null)
                .Select(pair => (pair.KeyType!, pair.ValueType!))
                .Distinct(DictionaryKeyValuePairComparer.Instance);
        }

        var fallback = targetType.GetKeyAndValueTypes(null, compilation, PreferredInterfaceSide.Target);
        return [fallback];
    }

    private static IEnumerable<(ITypeSymbol KeyType, ITypeSymbol ValueType)> GetSourceDictionaryKeyValueTypes(
        INamedTypeSymbol sourceType,
        PreferredInterfaceFamily family,
        INamedTypeSymbol[] sourceCandidates,
        Compilation compilation)
    {
        if (sourceCandidates.Length > 0)
        {
            return sourceCandidates
                .Select(candidate => TryGetDictionaryKeyValueTypes(candidate, family, compilation, out var key, out var value)
                    ? (KeyType: key, ValueType: value)
                    : default)
                .Where(pair => pair.KeyType is not null && pair.ValueType is not null)
                .Select(pair => (pair.KeyType!, pair.ValueType!))
                .Distinct(DictionaryKeyValuePairComparer.Instance);
        }

        var fallback = sourceType.GetKeyAndValueTypes(null, compilation, PreferredInterfaceSide.Source);
        return [fallback];
    }

    private static bool TryGetDictionaryKeyValueTypes(
        INamedTypeSymbol candidate,
        PreferredInterfaceFamily family,
        Compilation compilation,
        out ITypeSymbol keyType,
        out ITypeSymbol valueType)
    {
        if (family == PreferredInterfaceFamily.IEnumerableOfKeyValuePair
            && candidate.TypeArguments.Length == 1
            && candidate.TypeArguments[0].IsKeyValuePair(compilation))
        {
            (keyType, valueType) = candidate.TypeArguments[0].GetKeyAndValueTypes(compilation);
            return true;
        }

        if (candidate.TypeArguments.Length == 2)
        {
            keyType = candidate.TypeArguments[0];
            valueType = candidate.TypeArguments[1];
            return true;
        }

        keyType = null!;
        valueType = null!;
        return false;
    }

    private static bool TryGetNestedStrategy(
        MappaMapAlgorithmContext context,
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        using (context.EnterUsabilityProbe())
        {
            var derivedContext = new DerivedMappaMapAlgorithmContext(context, targetType, sourceType);
            var algorithm = new TypeMapIdentifierWithMapMethodAlgorithm(derivedContext, compilation, cancellationToken);
            return algorithm.GetStrategy() is not NoMapStrategy;
        }
    }

    private sealed class DictionaryKeyValuePairComparer : IEqualityComparer<(ITypeSymbol KeyType, ITypeSymbol ValueType)>
    {
        internal static DictionaryKeyValuePairComparer Instance { get; } = new();

        public bool Equals((ITypeSymbol KeyType, ITypeSymbol ValueType) x, (ITypeSymbol KeyType, ITypeSymbol ValueType) y)
            => SymbolEqualityComparer.Default.Equals(x.KeyType, y.KeyType)
               && SymbolEqualityComparer.Default.Equals(x.ValueType, y.ValueType);

        public int GetHashCode((ITypeSymbol KeyType, ITypeSymbol ValueType) obj)
        {
            unchecked
            {
                var hash = SymbolEqualityComparer.Default.GetHashCode(obj.KeyType);
                hash = (hash * 397) ^ SymbolEqualityComparer.Default.GetHashCode(obj.ValueType);
                return hash;
            }
        }
    }
}