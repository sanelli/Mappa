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
internal sealed class OptionalTargetPropertyMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalTargetPropertyMapStrategy"/> class.
    /// </summary>
    /// <param name="strategy">The strategy to apply.</param>
    /// <param name="targetProperty">The target property on which optional is applied.</param>
    public OptionalTargetPropertyMapStrategy(IMapStrategy strategy, IPropertySymbol targetProperty)
    {
        this.InnerStrategy = strategy;
        this.TargetProperty = targetProperty;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType => this.InnerStrategy.TargetType;

    /// <inheritdoc/>
    public ITypeSymbol SourceType => this.InnerStrategy.SourceType;

    /// <summary>
    /// Gets the strategy encapsulated.
    /// </summary>
    internal IMapStrategy InnerStrategy { get; }

    /// <summary>
    /// Gets the optional property details.
    /// </summary>
    internal IPropertySymbol TargetProperty { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new OptionalTargetPropertyMapStrategyBuilder(this);
}