// <copyright file="PropertyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// The strategy that can be applied between two properties.
/// </summary>
/// <param name="targetProperty">The target property.</param>
/// <param name="sourceProperty">The source property.</param>
/// <param name="propertyStrategy">The strategy between these properties.</param>
/// <param name="postConstructorInitializer"><c>true</c> if this property initializer must happen after the constructor invocation.</param>
/// <param name="chainedSourcePropertyPath">The chained source property path to read before mapping, if any.</param>
internal sealed class PropertyMapStrategy(
    IPropertySymbol targetProperty,
    IPropertySymbol? sourceProperty,
    MapStrategy propertyStrategy,
    bool postConstructorInitializer,
    ChainedSourcePropertyPathInfo? chainedSourcePropertyPath = null)
    : MapStrategy(targetProperty.Type, sourceProperty?.Type ?? chainedSourcePropertyPath?.StartingSourceType ?? null!)
{
    /// <summary>
    /// Gets the target property.
    /// </summary>
    public IPropertySymbol TargetProperty { get; } = targetProperty;

    /// <summary>
    /// Gets the source property.
    /// </summary>
    public IPropertySymbol? SourceProperty { get; } = sourceProperty;

    /// <summary>
    /// Gets the strategy to be applied between the two properties.
    /// </summary>
    public MapStrategy PropertyStrategy { get; } = propertyStrategy;

    /// <summary>
    /// Gets a value indicating whether the property should be initialised after the constructor.
    /// </summary>
    public bool PostConstructorInitializer { get; } = postConstructorInitializer;

    /// <summary>
    /// Gets the chained source property path to read before mapping, if any.
    /// </summary>
    public ChainedSourcePropertyPathInfo? ChainedSourcePropertyPath { get; } = chainedSourcePropertyPath;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new PropertyMapStrategyBuilder(this);
}