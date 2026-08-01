// <copyright file="InvokeMappingConstructorMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy used when invoking the mapping constructor of a class.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="constructor">The constructor.</param>
/// <param name="argumentStrategy">The argument strategy.</param>
/// <param name="requiresUnsafeAccessorOnConstructor"><c>true</c> when the constructor must be invoked via an unsafe accessor.</param>
internal sealed class InvokeMappingConstructorMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    IMethodSymbol constructor,
    MapStrategy argumentStrategy,
    bool requiresUnsafeAccessorOnConstructor = false)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the constructor to be used.
    /// </summary>
    public IMethodSymbol Constructor { get; } = constructor;

    /// <summary>
    /// Gets the strategy for the parameter.
    /// </summary>
    public MapStrategy ArgumentStrategy { get; } = argumentStrategy;

    /// <summary>
    /// Gets a value indicating whether the constructor must be invoked via an unsafe accessor.
    /// </summary>
    public bool RequiresUnsafeAccessorOnConstructor { get; } = requiresUnsafeAccessorOnConstructor;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new InvokeMappingConstructorMapStrategyBuilder(this);
}