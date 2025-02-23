// <copyright file="NullableToNullableMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a <see cref="Nullable{T}"/> to
/// another <see cref="Nullable{S}"/>.
/// </summary>
internal sealed class NullableToNullableMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullableToNullableMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="typeArgumentStrategy">The strategy that map types encapsulated by the nullable types.</param>
    public NullableToNullableMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        IMapStrategy typeArgumentStrategy)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.TypeArgumentStrategy = typeArgumentStrategy;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the strategy to map the types encapsulated by the nullable struct.
    /// </summary>
    public IMapStrategy TypeArgumentStrategy { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new NullableToNullableMapStrategyBuilder(this);
}