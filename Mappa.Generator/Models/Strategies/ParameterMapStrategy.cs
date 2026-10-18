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
/// <param name="requiresUnsafeAccessorOnSource"><c>true</c> when the source property must be read via an unsafe accessor.</param>
/// <param name="requiresInterfaceCastOnSource"><c>true</c> when the source property must be read via an interface cast.</param>
internal sealed class ParameterMapStrategy(
    IParameterSymbol targetParameter,
    IPropertySymbol? sourceProperty,
    MapStrategy parameterStrategy,
    bool requiresUnsafeAccessorOnSource = false,
    bool requiresInterfaceCastOnSource = false)
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

    /// <summary>
    /// Gets a value indicating whether the source property must be read via an unsafe accessor.
    /// </summary>
    internal bool RequiresUnsafeAccessorOnSource { get; } = requiresUnsafeAccessorOnSource;

    /// <summary>
    /// Gets a value indicating whether the source property must be read via an interface cast.
    /// </summary>
    internal bool RequiresInterfaceCastOnSource { get; } = requiresInterfaceCastOnSource;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new ParameterMapStrategyBuilder(this);
}