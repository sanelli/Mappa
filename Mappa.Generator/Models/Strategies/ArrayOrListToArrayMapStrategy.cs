// <copyright file="ArrayOrListToArrayMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map an array to another array.
/// </summary>
internal sealed class ArrayOrListToArrayMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayOrListToArrayMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="elementStrategy">The strategy that map the array element.</param>
    public ArrayOrListToArrayMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        IMapStrategy elementStrategy)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.ElementStrategy = elementStrategy;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the strategy to map the types encapsulated by the nullable struct.
    /// </summary>
    public IMapStrategy ElementStrategy { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new ArrayOrListToArrayMapStrategyBuilder(this);
}