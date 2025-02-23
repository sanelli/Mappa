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
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="format">The (optional) format to apply.</param>
/// <param name="cultureInfoSetting">The culture settings.</param>
/// <param name="cultureName">The name of the culture for user defined <paramref name="cultureInfoSetting"/>.</param>
internal sealed class InvokeToStringMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    string? format,
    CultureInfoSetting cultureInfoSetting,
    string? cultureName)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the (optional) format.
    /// </summary>
    internal string? Format { get; } = format;

    /// <summary>
    /// gets the (optional) culture info settings.
    /// </summary>
    internal CultureInfoSetting CultureInfoSetting { get; } = cultureInfoSetting;

    /// <summary>
    /// gets the (optional) culture name.
    /// </summary>
    internal string? CultureName { get; } = cultureName;

    /// <inheritdoc />
    internal override IMappaStrategyBuilder GetBuilder() => new InvokeToStringMapStrategyBuilder(this);
}