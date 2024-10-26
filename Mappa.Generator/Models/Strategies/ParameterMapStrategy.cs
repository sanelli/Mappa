// <copyright file="ParameterMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describe a strategy to map a property into a parameter.
/// </summary>
internal sealed class ParameterMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterMapStrategy"/> class.
    /// </summary>
    /// <param name="targetParameter">The target parameter.</param>
    /// <param name="sourceProperty">The source property.</param>
    /// <param name="parameterStrategy">The strategy to map from source to target.</param>
    public ParameterMapStrategy(IParameterSymbol targetParameter, IPropertySymbol? sourceProperty, IMapStrategy parameterStrategy)
    {
        this.TargetParameter = targetParameter;
        this.SourceProperty = sourceProperty;
        this.ParameterStrategy = parameterStrategy;
    }

    /// <summary>
    /// Gets the target parameter.
    /// </summary>
    public IParameterSymbol TargetParameter { get; }

    /// <summary>
    /// Gets the source property.
    /// </summary>
    public IPropertySymbol? SourceProperty { get; }

    /// <summary>
    /// Gets the mapping strategy.
    /// </summary>
    public IMapStrategy ParameterStrategy { get; }

    /// <inheritdoc/>
    public ITypeSymbol TargetType => this.TargetParameter.Type;

    /// <inheritdoc/>
    public ITypeSymbol SourceType => this.SourceProperty?.Type ?? throw new InvalidOperationException();

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.ArgumentStrategy;

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new ParameterMapStrategyBuilder(this);
}