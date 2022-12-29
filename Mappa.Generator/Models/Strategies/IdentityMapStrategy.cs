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
    /// <param name="rule">The rule applied.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="source">The source of the mapping.</param>
    public IdentityMapStrategy(MappaAlgorithmRule rule, ITypeSymbol targetType, ITypeSymbol sourceType, string source)
    {
        this.Rule = rule;
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.Source = source;
    }

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule { get; }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <inheritdoc/>
    public string Source { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new IdentityMapStrategyBuilder(this);
}