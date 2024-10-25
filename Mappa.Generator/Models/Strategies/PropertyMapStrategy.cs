// <copyright file="PropertyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// The strategy that can be applied between two properties.
/// </summary>
internal sealed class PropertyMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyMapStrategy"/> class.
    /// </summary>
    /// <param name="targetProperty">The target property.</param>
    /// <param name="sourceProperty">The source property.</param>
    /// <param name="propertyStrategy">The strategy between these properties.</param>
    public PropertyMapStrategy(IPropertySymbol targetProperty, IPropertySymbol? sourceProperty, IMapStrategy propertyStrategy)
    {
        this.TargetProperty = targetProperty;
        this.SourceProperty = sourceProperty;
        this.PropertyStrategy = propertyStrategy;
    }

    /// <summary>
    /// Gets the target property.
    /// </summary>
    public IPropertySymbol TargetProperty { get; }

    /// <summary>
    /// Gets the source property.
    /// </summary>
    public IPropertySymbol? SourceProperty { get; }

    /// <summary>
    /// Gets the strategy to be applied between the two properties.
    /// </summary>
    public IMapStrategy PropertyStrategy { get; }

    /// <inheritdoc/>
    public ITypeSymbol TargetType => this.TargetProperty.Type;

    /// <inheritdoc/>
    public ITypeSymbol SourceType => this.SourceProperty?.Type ?? throw new InvalidOperationException();

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.PropertyStrategy;

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new PropertyMapStrategyBuilder(this);
}