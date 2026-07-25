// <copyright file="EnumToStringMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>
using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map <see cref="Enum"/> to <see cref="string"/>.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="enumStringMapSetting">The enum and string member pairing setting.</param>
/// <param name="enumMapConfiguration">The resolved enum mapping configuration.</param>
internal sealed class EnumToStringMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    EnumStringMapSetting enumStringMapSetting,
    EnumMapConfiguration enumMapConfiguration)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the enum and string member pairing setting.
    /// </summary>
    public EnumStringMapSetting EnumStringMapSetting { get; } = enumStringMapSetting;

    /// <summary>
    /// Gets the resolved enum mapping configuration.
    /// </summary>
    public EnumMapConfiguration EnumMapConfiguration { get; } = enumMapConfiguration;

    /// <inheritdoc />
    internal override IMappaStrategyBuilder GetBuilder() => new EnumToStringMapStrategyBuilder(this);
}