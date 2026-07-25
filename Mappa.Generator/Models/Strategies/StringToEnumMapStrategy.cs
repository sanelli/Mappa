// <copyright file="StringToEnumMapStrategy.cs" company="Stefano Anelli">
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
/// <param name="caseInsensitiveEnumMap">The case-insensitive enum matching setting.</param>
/// <param name="enumStringMapSetting">The enum and string member pairing setting.</param>
/// <param name="enumMapConfiguration">The resolved enum mapping configuration.</param>
internal sealed class StringToEnumMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    BooleanSetting caseInsensitiveEnumMap,
    EnumStringMapSetting enumStringMapSetting,
    EnumMapConfiguration enumMapConfiguration)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the case-insensitive enum matching setting.
    /// </summary>
    public BooleanSetting CaseInsensitiveEnumMap { get; } = caseInsensitiveEnumMap;

    /// <summary>
    /// Gets the enum and string member pairing setting.
    /// </summary>
    public EnumStringMapSetting EnumStringMapSetting { get; } = enumStringMapSetting;

    /// <summary>
    /// Gets the resolved enum mapping configuration.
    /// </summary>
    public EnumMapConfiguration EnumMapConfiguration { get; } = enumMapConfiguration;

    /// <inheritdoc />
    internal override IMappaStrategyBuilder GetBuilder() => new StringToEnumMapStrategyBuilder(this);
}