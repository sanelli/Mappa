// <copyright file="DateTimeToDateOnlyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a <see cref="DateTime"/> to
/// a DateOnly value.
/// </summary>
internal sealed class DateTimeToDateOnlyMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeToDateOnlyMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    public DateTimeToDateOnlyMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new DateTimeToDateOnlyMapStrategyBuilder();
}