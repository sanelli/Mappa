// <copyright file="StringToEnumMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map <see cref="string"/> to <see cref="Enum"/>.
/// </summary>
internal sealed class StringToEnumMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringToEnumMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    public StringToEnumMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
    }

    /// <inheritdoc />
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc />
    public ITypeSymbol SourceType { get; }

    /// <inheritdoc />
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.StringToEnum;

    /// <inheritdoc />
    public IMappaStrategyBuilder GetBuilder() => new StringToEnumMapStrategyBuilder(this);
}