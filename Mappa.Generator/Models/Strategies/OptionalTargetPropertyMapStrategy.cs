// <copyright file="OptionalTargetPropertyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to encapsulate a strategy when the target property
/// is optional (i.e. a property named "Has&lt;TargetProperty&gt;" exists.
/// </summary>
/// <param name="strategy">The strategy to apply.</param>
/// <param name="targetProperty">The target property on which optional is applied.</param>
internal sealed class OptionalTargetPropertyMapStrategy(MapStrategy strategy, IPropertySymbol targetProperty)
        : MapStrategy(strategy.TargetType, strategy.SourceType)
{
    /// <summary>
    /// Gets the strategy encapsulated.
    /// </summary>
    internal MapStrategy InnerStrategy { get; } = strategy;

    /// <summary>
    /// Gets the optional property details.
    /// </summary>
    internal IPropertySymbol TargetProperty { get; } = targetProperty;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new OptionalTargetPropertyMapStrategyBuilder(this);
}