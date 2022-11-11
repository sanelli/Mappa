// <copyright file="IdentityTypeMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.TypeMapStrategy;

/// <summary>
/// Describe an identity type map strategy.
/// </summary>
internal sealed class IdentityTypeMapStrategy
    : ITypeMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityTypeMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    public IdentityTypeMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }
}