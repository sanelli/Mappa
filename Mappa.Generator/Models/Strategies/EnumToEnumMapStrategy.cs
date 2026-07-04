// <copyright file="EnumToEnumMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map an enum to another enum.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="enumToEnumMapSetting">The enum-to-enum member pairing setting.</param>
internal sealed class EnumToEnumMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    EnumToEnumMapSetting enumToEnumMapSetting)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the enum-to-enum member pairing setting.
    /// </summary>
    public EnumToEnumMapSetting EnumToEnumMapSetting { get; } = enumToEnumMapSetting;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new EnumToEnumMapStrategyBuilder(this);
}