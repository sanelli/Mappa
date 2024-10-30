// <copyright file="InvokeToStringMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Attributes;
using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map anything to <see cref="string"/>
/// using the <see cref="object.ToString()"/>.
/// </summary>
internal sealed class InvokeToStringMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeToStringMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="format">The (optional) format to apply.</param>
    /// <param name="cultureInfoSetting">The culture settings.</param>
    /// <param name="cultureName">The name of the culture for user defined <paramref name="cultureInfoSetting"/>.</param>
    public InvokeToStringMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        string? format,
        MappaSettingsAttribute.CultureInfoSettings cultureInfoSetting,
        string? cultureName)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.Format = format;
        this.CultureInfoSetting = cultureInfoSetting;
        this.CultureName = cultureName;
    }

    /// <inheritdoc />
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc />
    public ITypeSymbol SourceType { get; }

    /// <inheritdoc />
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.InvokeToString;

    /// <summary>
    /// Gets the (optional) format.
    /// </summary>
    internal string? Format { get; }

    /// <summary>
    /// gets the (optional) culture info settings.
    /// </summary>
    internal MappaSettingsAttribute.CultureInfoSettings CultureInfoSetting { get; }

    /// <summary>
    /// gets the (optional) culture name.
    /// </summary>
    internal string? CultureName { get; }

    /// <inheritdoc />
    public IMappaStrategyBuilder GetBuilder() => new InvokeToStringMapStrategyBuilder(this);
}