// <copyright file="LongToDateTimeMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a <see cref="DateTime"/> to
/// a <see cref="long"/> value.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="LongToDateTimeMapStrategy"/> class.
/// </remarks>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
internal sealed class LongToDateTimeMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType)
        : MapStrategy(targetType, sourceType)
{
    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new LongToDateTimeMapStrategyBuilder();
}