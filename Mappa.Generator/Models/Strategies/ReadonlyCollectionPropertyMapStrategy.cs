// <copyright file="ReadonlyCollectionPropertyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy used when mapping to a collection property that
/// either does not have a setter or the setter is not accessible.
/// </summary>
/// <param name="targetProperty">The target property (a collection type).</param>
/// <param name="sourceProperty">The source property (a collection type).</param>
/// <param name="elementStrategy">The strategy of the element mapping.</param>
/// <param name="preferredSourceInterface">The resolved preferred source interface, if any.</param>
/// <param name="preferredTargetInterface">The resolved preferred target interface, if any.</param>
internal sealed class ReadonlyCollectionPropertyMapStrategy(
    IPropertySymbol targetProperty,
    IPropertySymbol sourceProperty,
    MapStrategy elementStrategy,
    INamedTypeSymbol? preferredSourceInterface = null,
    INamedTypeSymbol? preferredTargetInterface = null)
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

    /// <summary>
    /// Gets the resolved preferred source interface, if any.
    /// </summary>
    internal INamedTypeSymbol? PreferredSourceInterface { get; } = preferredSourceInterface;

    /// <summary>
    /// Gets the resolved preferred target interface, if any.
    /// </summary>
    internal INamedTypeSymbol? PreferredTargetInterface { get; } = preferredTargetInterface;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder()
        => new ReadonlyCollectionPropertyMapStrategyBuilder(this);
}