// <copyright file="ReadonlyStackPropertyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy used when mapping to a stack property that
/// either does not have a setter or the setter is not accessible.
/// </summary>
/// <param name="targetProperty">The target property (a stack type).</param>
/// <param name="sourceProperty">The source property (a collection type).</param>
/// <param name="elementStrategy">The strategy of the element mapping.</param>
internal sealed class ReadonlyStackPropertyMapStrategy(IPropertySymbol targetProperty, IPropertySymbol sourceProperty, MapStrategy elementStrategy)
    : MapStrategy(targetProperty.Type, sourceProperty.Type)
{
    /// <summary>
    /// Gets the target property.
    /// </summary>
    internal IPropertySymbol TargetProperty { get; } = targetProperty;

    /// <summary>
    /// Gets the strategy for the elements.
    /// </summary>
    internal MapStrategy ElementStrategy { get; } = elementStrategy;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder()
        => new ReadonlyStackPropertyMapStrategyBuilder(this);
}