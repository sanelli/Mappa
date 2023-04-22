// <copyright file="ArrayToArrayMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map an array to another array.
/// </summary>
internal sealed class ArrayToArrayMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArrayToArrayMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="childStrategy">The strategy that map types encapsulated by the nullable types.</param>
    public ArrayToArrayMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        IMapStrategy childStrategy)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.ChildStrategy = childStrategy;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the strategy to map the types encapsulated by the nullable struct.
    /// </summary>
    public IMapStrategy ChildStrategy { get; }

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.ArrayToArray;

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new ArrayToArrayMapStrategyBuilder(this);
}