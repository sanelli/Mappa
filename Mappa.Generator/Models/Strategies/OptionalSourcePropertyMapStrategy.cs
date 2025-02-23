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
internal sealed class OptionalSourcePropertyMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalSourcePropertyMapStrategy"/> class.
    /// </summary>
    /// <param name="strategy">The strategy to apply.</param>
    /// <param name="sourceProperty">The source property on which optional is applied.</param>
    public OptionalSourcePropertyMapStrategy(IMapStrategy strategy, IPropertySymbol sourceProperty)
    {
        this.InnerStrategy = strategy;
        this.SourceProperty = sourceProperty;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType => this.InnerStrategy.TargetType;

    /// <inheritdoc/>
    public ITypeSymbol SourceType => this.InnerStrategy.SourceType;

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule => this.InnerStrategy.Rule;

    /// <summary>
    /// Gets the strategy encapsulated.
    /// </summary>
    internal IMapStrategy InnerStrategy { get; }

    /// <summary>
    /// Gets the optional property details.
    /// </summary>
    internal IPropertySymbol SourceProperty { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new OptionalSourcePropertyMapStrategyBuilder(this);
}