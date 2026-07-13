// <copyright file="ReadonlyDictionaryPropertyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy used when mapping to a dictionary property that
/// either does not have a setter or the setter is not accessible.
/// </summary>
/// <param name="targetProperty">The target property (a dictionary type).</param>
/// <param name="sourceProperty">The source property (a dictionary type).</param>
/// <param name="keyStrategy">The strategy for the key mapping.</param>
/// <param name="valueStrategy">The strategy for the value mappings.</param>
/// <param name="dictionaryAssignment">The dictionary assignment setting.</param>
internal sealed class ReadonlyDictionaryPropertyMapStrategy(
    IPropertySymbol targetProperty,
    IPropertySymbol sourceProperty,
    MapStrategy keyStrategy,
    MapStrategy valueStrategy,
    DictionaryAssignmentSetting dictionaryAssignment)
    : MapStrategy(targetProperty.Type, sourceProperty.Type)
{
    /// <summary>
    /// Gets the target property.
    /// </summary>
    internal IPropertySymbol TargetProperty { get; } = targetProperty;

    /// <summary>
    /// Gets the strategy for the keys.
    /// </summary>
    internal MapStrategy KeyStrategy { get; } = keyStrategy;

    /// <summary>
    /// Gets the strategy for the values.
    /// </summary>
    internal MapStrategy ValueStrategy { get; } = valueStrategy;

    /// <summary>
    /// Gets the dictionary assignment setting.
    /// </summary>
    internal DictionaryAssignmentSetting DictionaryAssignment { get; } = dictionaryAssignment;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder()
        => new ReadonlyDictionaryPropertyMapStrategyBuilder(this);
}