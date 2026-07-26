// <copyright file="QueryableProjectionMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to project an <see cref="System.Linq.IQueryable{T}"/> to another <see cref="System.Linq.IQueryable{T}"/>.
/// </summary>
/// <param name="targetType">The target queryable type.</param>
/// <param name="sourceType">The source queryable type.</param>
/// <param name="elementStrategy">The strategy for the queryable element types.</param>
/// <param name="sourceElementType">The source queryable element type.</param>
/// <param name="targetElementType">The target queryable element type.</param>
/// <param name="methodSymbol">The root map method symbol.</param>
internal sealed class QueryableProjectionMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    MapStrategy elementStrategy,
    ITypeSymbol sourceElementType,
    ITypeSymbol targetElementType,
    IMethodSymbol methodSymbol)
    : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the strategy for the queryable element types.
    /// </summary>
    internal MapStrategy ElementStrategy { get; } = elementStrategy;

    /// <summary>
    /// Gets the source queryable element type.
    /// </summary>
    internal ITypeSymbol SourceElementType { get; } = sourceElementType;

    /// <summary>
    /// Gets the target queryable element type.
    /// </summary>
    internal ITypeSymbol TargetElementType { get; } = targetElementType;

    /// <summary>
    /// Gets the root map method symbol.
    /// </summary>
    internal IMethodSymbol MethodSymbol { get; } = methodSymbol;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new QueryableProjectionMapStrategyBuilder(this);
}