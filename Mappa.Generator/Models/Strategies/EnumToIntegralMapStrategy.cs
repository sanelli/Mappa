// <copyright file="EnumToIntegralMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map <see cref="Enum"/> to <see cref="string"/>.
/// </summary>
internal sealed class EnumToIntegralMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToIntegralMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="source">The name of the source.</param>
    public EnumToIntegralMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType, string source)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.Source = source;
    }

    /// <inheritdoc />
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc />
    public ITypeSymbol SourceType { get; }

    /// <inheritdoc />
    public string Source { get; }

    /// <inheritdoc />
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.EnumToIntegral;

    /// <inheritdoc />
    public IMappaStrategyBuilder GetBuilder() => new EnumToIntegralMapStrategyBuilder(this);
}