// <copyright file="DictionaryToDictionaryMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy to map a dictionary to a dictionary.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="keyStrategy">The strategy for the keys.</param>
/// <param name="valueStrategy">The strategy for the values.</param>
/// <param name="dictionaryAssignment">The dictionary assignment setting.</param>
/// <param name="replaceNullWithEmptyContainer">Enable or disable replacing a null source dictionary with an empty target.</param>
/// <param name="preferredSourceInterface">The resolved preferred source interface, if any.</param>
/// <param name="preferredTargetInterface">The resolved preferred target interface, if any.</param>
internal sealed class DictionaryToDictionaryMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    MapStrategy keyStrategy,
    MapStrategy valueStrategy,
    DictionaryAssignmentSetting dictionaryAssignment,
    BooleanSetting replaceNullWithEmptyContainer = BooleanSetting.Undefined,
    INamedTypeSymbol? preferredSourceInterface = null,
    INamedTypeSymbol? preferredTargetInterface = null)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the strategy for the keys.
    /// </summary>
    public MapStrategy KeyStrategy { get; } = keyStrategy;

    /// <summary>
    /// Gets the strategy for the values.
    /// </summary>
    public MapStrategy ValueStrategy { get; } = valueStrategy;

    /// <summary>
    /// Gets the dictionary assignment setting.
    /// </summary>
    internal DictionaryAssignmentSetting DictionaryAssignment { get; } = dictionaryAssignment;

    /// <summary>
    /// Gets a value indicating whether a null source dictionary should be replaced with an empty target.
    /// </summary>
    internal BooleanSetting ReplaceNullWithEmptyContainer { get; } = replaceNullWithEmptyContainer;

    /// <summary>
    /// Gets the resolved preferred source interface, if any.
    /// </summary>
    internal INamedTypeSymbol? PreferredSourceInterface { get; } = preferredSourceInterface;

    /// <summary>
    /// Gets the resolved preferred target interface, if any.
    /// </summary>
    internal INamedTypeSymbol? PreferredTargetInterface { get; } = preferredTargetInterface;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new DictionaryToDictionaryMapStrategyBuilder(this);
}