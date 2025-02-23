// <copyright file="DictionaryToDictionaryMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a dictionary to a dictionary.
/// </summary>
internal sealed class DictionaryToDictionaryMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryToDictionaryMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="keyStrategy">The strategy for the keys.</param>
    /// <param name="valueStrategy">The strategy for the values.</param>
    public DictionaryToDictionaryMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        IMapStrategy keyStrategy,
        IMapStrategy valueStrategy)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.KeyStrategy = keyStrategy;
        this.ValueStrategy = valueStrategy;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the strategy for the keys.
    /// </summary>
    public IMapStrategy KeyStrategy { get; }

    /// <summary>
    /// Gets the strategy for the values.
    /// </summary>
    public IMapStrategy ValueStrategy { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new DictionaryToDictionaryMapStrategyBuilder(this);
}