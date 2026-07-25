// <copyright file="IntegralToEnumMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map <see cref="string"/> to <see cref="Enum"/>.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="enumMapConfiguration">The resolved enum mapping configuration.</param>
internal sealed class IntegralToEnumMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    EnumMapConfiguration enumMapConfiguration)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the resolved enum mapping configuration.
    /// </summary>
    public EnumMapConfiguration EnumMapConfiguration { get; } = enumMapConfiguration;

    /// <inheritdoc />
    internal override IMappaStrategyBuilder GetBuilder() => new IntegralToEnumMapStrategyBuilder(this);
}