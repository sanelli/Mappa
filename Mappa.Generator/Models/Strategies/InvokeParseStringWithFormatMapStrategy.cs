// <copyright file="InvokeParseStringWithFormatMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a <see cref="string"/> to a specific type
/// the <c>Parse</c> or <c>ParseExact</c>.
/// </summary>
/// <remarks>
/// This currently supports the following types:
/// <list type="bullet">
/// <item><term><see cref="DateTime"/>;</term></item>
/// <item><term><see cref="DateTimeOffset"/>;</term></item>
/// <item><term><c>DateOnly</c>;</term></item>
/// <item><term><c>TimeOnly</c>;</term></item>
/// <item><term><see cref="TimeSpan"/>;</term></item>
/// </list>
/// </remarks>
internal sealed class InvokeParseStringWithFormatMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvokeParseStringWithFormatMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="rule">The rule to be applied.</param>
    /// <param name="format">The format to apply.</param>
    /// <param name="cultureInfoSetting">The culture info settings.</param>
    /// <param name="cultureName">The culture name when the culture info settings are user defined.</param>
    public InvokeParseStringWithFormatMapStrategy(
        ITypeSymbol targetType,
        ITypeSymbol sourceType,
        MappaAlgorithmRule rule,
        string? format,
        CultureInfoSetting cultureInfoSetting,
        string? cultureName)
    {
        this.Rule = rule;
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
    public MappaAlgorithmRule Rule { get; }

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new InvokeParseStringWithFormatMapStrategyBuilder(this);
}