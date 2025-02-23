// <copyright file="OptionalSourcePropertyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to encapsulate a strategy when the source property
/// is optional (i.e. a property named "Has&lt;SourceProperty&gt;" exists.
/// </summary>
/// <param name="strategy">The strategy to apply.</param>
/// <param name="sourceProperty">The source property on which optional is applied.</param>
internal sealed class OptionalSourcePropertyMapStrategy(MapStrategy strategy, IPropertySymbol sourceProperty)
        : MapStrategy(strategy.TargetType, strategy.SourceType)
{
    /// <summary>
    /// Gets the strategy encapsulated.
    /// </summary>
    internal MapStrategy InnerStrategy { get; } = strategy;

    /// <summary>
    /// Gets the optional property details.
    /// </summary>
    internal IPropertySymbol SourceProperty { get; } = sourceProperty;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new OptionalSourcePropertyMapStrategyBuilder(this);
}