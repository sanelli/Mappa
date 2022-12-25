// <copyright file="IdentityMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="IdentityMapStrategy"/> strategy.
/// </summary>
internal sealed class IdentityMapStrategyBuilder
    : IMappaStrategyBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="identityMapStrategy">The strategy.</param>
    public IdentityMapStrategyBuilder(IdentityMapStrategy identityMapStrategy)
    {
        this.IdentityMapStrategy = identityMapStrategy;
    }

    /// <summary>
    /// Gets the strategy.
    /// </summary>
    private IdentityMapStrategy IdentityMapStrategy { get; }

    /// <inheritdoc/>
    public (string StrategySource, string Header) BuildSource()
    {
        return (this.IdentityMapStrategy.Source, string.Empty);
    }
}