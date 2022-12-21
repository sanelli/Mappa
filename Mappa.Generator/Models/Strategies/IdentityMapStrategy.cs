// <copyright file="IdentityMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Describe an identity type map strategy.
/// </summary>
#pragma warning disable CA1812
internal sealed class IdentityMapStrategy
#pragma warning restore CA1812
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="source">The source of the mapping.</param>
    public IdentityMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType, string source)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.Source = source;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <inheritdoc/>
    public string Source { get; }
}