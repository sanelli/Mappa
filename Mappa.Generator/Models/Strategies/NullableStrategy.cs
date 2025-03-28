// <copyright file="NullableStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy used to define a mapping when nullability is involved.
/// </summary>
/// <param name="sourceType">The source type.</param>
/// <param name="targetType">The target type.</param>
/// <param name="elementStrategy">The strategy between the types when stripped of nullable.</param>
internal sealed class NullableStrategy(ITypeSymbol targetType, ITypeSymbol sourceType, MapStrategy elementStrategy)
    : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the strategy of the inner element of the nullable.
    /// </summary>
    internal MapStrategy ElementStrategy { get; } = elementStrategy;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new NullableStrategyBuilder(this);
}