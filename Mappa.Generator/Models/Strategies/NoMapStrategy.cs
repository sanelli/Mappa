// <copyright file="NoMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to be used when no strategy can be detected.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
internal sealed class NoMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
        : MapStrategy(targetType, sourceType)
{
    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new NoMapStrategyBuilder();
}