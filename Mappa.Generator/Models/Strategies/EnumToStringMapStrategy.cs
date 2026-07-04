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
internal sealed class EnumToStringMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    EnumStringMapSetting enumStringMapSetting)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the enum and string member pairing setting.
    /// </summary>
    public EnumStringMapSetting EnumStringMapSetting { get; } = enumStringMapSetting;

    /// <inheritdoc />
    internal override IMappaStrategyBuilder GetBuilder() => new EnumToStringMapStrategyBuilder(this);
}