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
internal sealed class MappaAssignFromContextAttributeStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappaAssignFromContextAttributeStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The type of the target.</param>
    /// <param name="attribute">The attribute.</param>
    /// <param name="contextParameterName">The name of the context parameter.</param>
    public MappaAssignFromContextAttributeStrategy(
        ITypeSymbol targetType,
        MappaAssignFromContextAttribute attribute,
        string contextParameterName)
    {
        this.TargetType = targetType;
        this.Attribute = attribute;
        this.ContextParameterName = contextParameterName;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType => this.TargetType;

    /// <summary>
    /// Gets the attribute.
    /// </summary>
    internal MappaAssignFromContextAttribute Attribute { get; }

    /// <summary>
    /// Gets the context parameter name.
    /// </summary>
    internal string ContextParameterName { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new MappaAssignFromContextAttributeStrategyBuilder(this);
}