// <copyright file="EnumToStringMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map <see cref="Enum"/> to <see cref="string"/>.
/// </summary>
internal sealed class EnumToStringMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToStringMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    public EnumToStringMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
    }

    /// <inheritdoc />
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc />
    public ITypeSymbol SourceType { get; }

    /// <inheritdoc />
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.EnumToString;

    /// <inheritdoc />
    public IMappaStrategyBuilder GetBuilder() => new EnumToStringMapStrategyBuilder(this);
}