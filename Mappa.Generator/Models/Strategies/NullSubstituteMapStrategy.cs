// <copyright file="NullSubstituteMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Substitutes a null source value with a constant before applying the element mapping strategy.
/// </summary>
/// <param name="targetType">The target type of the mapping.</param>
/// <param name="sourceType">The source member type.</param>
/// <param name="assignValue">The constant substitute value (typically a boxed <see cref="TypedConstant"/>).</param>
/// <param name="elementStrategy">The strategy applied after substitution.</param>
internal sealed class NullSubstituteMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    object? assignValue,
    MapStrategy elementStrategy)
    : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the constant substitute value.
    /// </summary>
    internal object? AssignValue { get; } = assignValue;

    /// <summary>
    /// Gets the strategy applied after null substitution.
    /// </summary>
    internal MapStrategy ElementStrategy { get; } = elementStrategy;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new NullSubstituteMapStrategyBuilder(this);
}