// <copyright file="EnumToEnumMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map an enum to another enum.
/// </summary>
internal sealed class EnumToEnumMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToEnumMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="source">The name of the source.</param>
    public EnumToEnumMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType, string source)
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

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.EnumToEnum;

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new EnumToEnumMapStrategyBuilder(this);
}