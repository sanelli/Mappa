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
/// <param name="caseInsensitiveStringToEnumMap">The case-insensitive string-to-enum matching setting.</param>
internal sealed class StringToEnumMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    BooleanSetting caseInsensitiveStringToEnumMap)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the case-insensitive string-to-enum matching setting.
    /// </summary>
    public BooleanSetting CaseInsensitiveStringToEnumMap { get; } = caseInsensitiveStringToEnumMap;

    /// <inheritdoc />
    internal override IMappaStrategyBuilder GetBuilder() => new StringToEnumMapStrategyBuilder(this);
}