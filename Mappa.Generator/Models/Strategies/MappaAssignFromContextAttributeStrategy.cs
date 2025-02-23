// <copyright file="MappaAssignFromContextAttributeStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map using the context parameter.
/// </summary>
/// <param name="targetType">The type of the target.</param>
/// <param name="attribute">The attribute.</param>
/// <param name="contextParameterName">The name of the context parameter.</param>
internal sealed class MappaAssignFromContextAttributeStrategy(
    ITypeSymbol targetType,
    MappaAssignFromContextAttribute attribute,
    string contextParameterName)
        : MapStrategy(targetType, targetType)
{
    /// <summary>
    /// Gets the attribute.
    /// </summary>
    internal MappaAssignFromContextAttribute Attribute { get; } = attribute;

    /// <summary>
    /// Gets the context parameter name.
    /// </summary>
    internal string ContextParameterName { get; } = contextParameterName;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new MappaAssignFromContextAttributeStrategyBuilder(this);
}