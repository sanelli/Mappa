// <copyright file="InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a <see cref="string"/> to a <c>DateOnly</c> or <c>TimeOnly</c>
/// the <c>Parse</c> or <c>ParseExact</c>.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="format">The format to apply.</param>
/// <param name="cultureInfoSetting">The culture info settings.</param>
/// <param name="cultureName">The culture name when the culture info settings are user defined.</param>
internal sealed class InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    string? format,
    CultureInfoSetting cultureInfoSetting,
    string? cultureName)
    : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the format specified by the user.
    /// </summary>
    public string? Format { get; } = format;

    /// <summary>
    /// Gets the culture info settings.
    /// </summary>
    public CultureInfoSetting CultureInfoSetting { get; } = cultureInfoSetting;

    /// <summary>
    /// Gets the culture name.
    /// </summary>
    public string? CultureName { get; } = cultureName;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategyBuilder(this);
}