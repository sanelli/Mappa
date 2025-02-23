// <copyright file="ParameterMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describe a strategy to map a property into a parameter.
/// </summary>
/// <param name="targetParameter">The target parameter.</param>
/// <param name="sourceProperty">The source property.</param>
/// <param name="parameterStrategy">The strategy to map from source to target.</param>
internal sealed class ParameterMapStrategy(IParameterSymbol targetParameter, IPropertySymbol? sourceProperty, MapStrategy parameterStrategy)
    : MapStrategy(targetParameter.Type, sourceProperty?.Type ?? null!)
{
    /// <summary>
    /// Gets the target parameter.
    /// </summary>
    internal IParameterSymbol TargetParameter { get; } = targetParameter;

    /// <summary>
    /// Gets the source property.
    /// </summary>
    internal IPropertySymbol? SourceProperty { get; } = sourceProperty;

    /// <summary>
    /// Gets the mapping strategy.
    /// </summary>
    internal MapStrategy ParameterStrategy { get; } = parameterStrategy;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new ParameterMapStrategyBuilder(this);
}