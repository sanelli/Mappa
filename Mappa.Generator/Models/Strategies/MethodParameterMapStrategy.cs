// <copyright file="MethodParameterMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using System;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map the source parameter of a method using a specific strategy.
/// </summary>
internal sealed class MethodParameterMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodParameterMapStrategy"/> class.
    /// </summary>
    /// <param name="strategy">The strategy to be used for mapping the method parameter.</param>
    public MethodParameterMapStrategy(IMapStrategy strategy)
    {
        this.Strategy = strategy;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType => this.Strategy.TargetType;

    /// <inheritdoc/>
    public ITypeSymbol SourceType => this.Strategy.SourceType;

    /// <inheritdoc/>
    public string Source => this.Strategy.Source;

    /// <summary>
    /// Gets the strategy to be used to map the method.
    /// </summary>
    internal IMapStrategy Strategy { get; }
}