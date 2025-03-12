// <copyright file="CommonStrategyDetectors.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Extensions;
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
    {
        var sourceElementType = sourceType.GetElementType();
        var targetElementType = targetType.GetElementType();
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
    {
        var (sourceKeyType, sourceKeyValueType) = sourceType.GetKeyAndValueTypes();
        var (targetKeyType, targetValueType) = targetType.GetKeyAndValueTypes();

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
}