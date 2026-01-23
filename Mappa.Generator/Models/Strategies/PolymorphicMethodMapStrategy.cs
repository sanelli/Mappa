// <copyright file="PolymorphicMethodMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describe a strategy for mapping between two types that uses a map method polymorphic capabilities.
/// </summary>
/// <param name="targetType">The target type (which will not match the <paramref name="mapMethod"/> target type).</param>
/// <param name="sourceType">The source type of the mapping (which will not match the <paramref name="mapMethod"/> source type).</param>
/// <param name="mapMethod">The method to be used for the mapping.</param>
/// <param name="contextParameterName">The name of the context parameter.</param>
internal sealed class PolymorphicMethodMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType, MapMethod mapMethod, string? contextParameterName)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the method used for the mapping.
    /// </summary>
    public MapMethod MapMethod { get; } = mapMethod;

    /// <summary>
    /// Gets the name of the context parameter.
    /// </summary>
    internal string? ContextParameterName { get; } = contextParameterName;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new PolymorphicMethodMapStrategyBuilder(this);
}