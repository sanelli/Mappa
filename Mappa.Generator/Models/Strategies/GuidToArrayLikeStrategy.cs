// <copyright file="GuidToArrayLikeStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map <see cref="Guid"/> to <see cref="Array"/>,
/// <see cref="Span{T}"/>, <see cref="ReadOnlySpan{T}"/>,
/// <see cref="Memory{T}"/>, <see cref="ReadOnlyMemory{T}"/>
/// of <see cref="byte"/>s.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
internal sealed class GuidToArrayLikeStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType)
    : MapStrategy(targetType, sourceType)
{
    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new GuidToArrayLikeStrategyBuilder(this);
}