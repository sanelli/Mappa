// <copyright file="StringToGuidMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a <see cref="string"/> to
/// a numeric value.
/// </summary>
internal sealed class StringToGuidMapStrategy
    : IMapStrategy
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StringToGuidMapStrategy"/> class.
    /// </summary>
    /// <param name="targetType">The target type.</param>
    /// <param name="sourceType">The source type.</param>
    /// <param name="userSettings">The user settings.</param>
    public StringToGuidMapStrategy(ITypeSymbol targetType, ITypeSymbol sourceType, IMappaUserSettings userSettings)
    {
        this.TargetType = targetType;
        this.SourceType = sourceType;
        this.UserSettings = userSettings;
    }

    /// <inheritdoc/>
    public ITypeSymbol TargetType { get; }

    /// <inheritdoc/>
    public ITypeSymbol SourceType { get; }

    /// <summary>
    /// Gets the user settings.
    /// </summary>
    public IMappaUserSettings UserSettings { get; }

    /// <inheritdoc/>
    public MappaAlgorithmRule Rule => MappaAlgorithmRule.StringToGuid;

    /// <inheritdoc/>
    public IMappaStrategyBuilder GetBuilder() => new StringToGuidMapStrategyBuilder(this);
}