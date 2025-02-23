// <copyright file="IdentityMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describe an identity type map strategy.
/// </summary>
internal sealed class IdentityMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    public IdentityMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new IdentityMapStrategyBuilder();
}