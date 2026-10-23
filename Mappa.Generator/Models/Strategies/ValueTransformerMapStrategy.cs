// <copyright file="ValueTransformerMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Decorates a strategy so that value transformers run on the source value before the inner strategy
/// and on the mapped value after it.
/// </summary>
/// <param name="innerStrategy">The decorated strategy.</param>
/// <param name="sourceTransformers">The transformers applied to the source value, in application order.</param>
/// <param name="targetTransformers">The transformers applied to the mapped value, in application order.</param>
internal sealed class ValueTransformerMapStrategy(
    MapStrategy innerStrategy,
    ValueTransformer[] sourceTransformers,
    ValueTransformer[] targetTransformers)
    : MapStrategy(innerStrategy.TargetType, innerStrategy.SourceType)
{
    /// <summary>
    /// Gets the decorated strategy.
    /// </summary>
    internal MapStrategy InnerStrategy { get; } = innerStrategy;

    /// <summary>
    /// Gets the transformers applied to the source value, in application order.
    /// </summary>
    internal IReadOnlyList<ValueTransformer> SourceTransformers { get; } = sourceTransformers;

    /// <summary>
    /// Gets the transformers applied to the mapped value, in application order.
    /// </summary>
    internal IReadOnlyList<ValueTransformer> TargetTransformers { get; } = targetTransformers;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new ValueTransformerMapStrategyBuilder(this);
}