// <copyright file="MappaAssignFromConstantAttributeStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy used by the <see cref="MappaAssignFromConstantAttribute"/>.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="attribute">The attribute containing the constant value.</param>
internal sealed class MappaAssignFromConstantAttributeStrategy(ITypeSymbol targetType, MappaAssignFromConstantAttribute attribute)
    : MapStrategy(targetType, null!)
{
    /// <summary>
    /// Gets the attribute.
    /// </summary>
    internal MappaAssignFromConstantAttribute Attribute { get; } = attribute;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new MappaAssignFromConstantAttributeStrategyBuilder(this);
}