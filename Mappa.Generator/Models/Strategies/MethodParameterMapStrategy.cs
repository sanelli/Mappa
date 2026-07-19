// <copyright file="MethodParameterMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map the source parameter of a method using a specific strategy.
/// </summary>
/// <param name="strategy">The strategy to be used for mapping the method parameter.</param>
/// <param name="beforeMapHooks">The hooks to invoke before mapping.</param>
/// <param name="afterMapHooks">The hooks to invoke after mapping.</param>
internal sealed class MethodParameterMapStrategy(
    MapStrategy strategy,
    MapHook[]? beforeMapHooks = null,
    MapHook[]? afterMapHooks = null)
        : MapStrategy(strategy.TargetType, strategy.SourceType)
{
    /// <summary>
    /// Gets the strategy to be used to map the method.
    /// </summary>
    internal MapStrategy Strategy { get; } = strategy;

    /// <summary>
    /// Gets the hooks to invoke before mapping.
    /// </summary>
    internal IReadOnlyList<MapHook> BeforeMapHooks { get; } = beforeMapHooks ?? [];

    /// <summary>
    /// Gets the hooks to invoke after mapping.
    /// </summary>
    internal IReadOnlyList<MapHook> AfterMapHooks { get; } = afterMapHooks ?? [];

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new MethodParameterMapStrategyBuilder(this);
}