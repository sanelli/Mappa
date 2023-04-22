// <copyright file="InvokeToStringMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map anything to <see cref="string"/>
/// using the <see cref="object.ToString()"/>.
/// </summary>
internal sealed class InvokeToStringMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeToStringMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    public InvokeToStringMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
    }

    /// <inheritdoc />
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc />
    public ITypeSymbol SourceType { get; }

    /// <inheritdoc />
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.InvokeToString;

    /// <inheritdoc />
    public IMappaStrategyBuilder GetBuilder() => new InvokeToStringMapStrategyBuilder(this);
}