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
internal sealed class InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="format">The format to apply.</param>
    /// <param name="cultureInfoSetting">The culture info settings.</param>
    /// <param name="cultureName">The culture name when the culture info settings are user defined.</param>
    internal InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        string? format,
        CultureInfoSetting cultureInfoSetting,
        string? cultureName)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.Format = format;
        this.CultureInfoSetting = cultureInfoSetting;
        this.CultureName = cultureName;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the format specified by the user.
    /// </summary>
    public string? Format { get; }

    /// <summary>
    /// Gets the culture info settings.
    /// </summary>
    public CultureInfoSetting CultureInfoSetting { get; }

    /// <summary>
    /// Gets the culture name.
    /// </summary>
    public string? CultureName { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new InvokeParseStringWithFormatForDateOnlyAndTimeOnlyMapStrategyBuilder(this);
}