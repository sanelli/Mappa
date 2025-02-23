// <copyright file="FromReferenceNullableMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a <see cref="Nullable{T}"/> to
/// another <see cref="Nullable{S}"/>.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="innerStrategy">The strategy that map types encapsulated by the nullable types.</param>
internal sealed class FromReferenceNullableMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    MapStrategy innerStrategy)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the strategy to map the types encapsulated by the nullable struct.
    /// </summary>
    public MapStrategy InnerStrategy { get; } = innerStrategy;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new FromReferenceNullableMapStrategyBuilder(this);
}