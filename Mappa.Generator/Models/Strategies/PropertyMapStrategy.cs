// <copyright file="PropertyMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// The strategy that can be applied between a target property and a source property or field.
/// </summary>
/// <param name="targetProperty">The target property.</param>
/// <param name="sourceProperty">The source property, if any.</param>
/// <param name="propertyStrategy">The strategy between these members.</param>
/// <param name="postConstructorInitializer"><c>true</c> if this property initializer must happen after the constructor invocation.</param>
/// <param name="chainedSourcePropertyPath">The chained source property path to read before mapping, if any.</param>
/// <param name="requiresUnsafeAccessorOnSource"><c>true</c> when the source member must be read via an unsafe accessor.</param>
/// <param name="requiresUnsafeAccessorOnTarget"><c>true</c> when the target property must be accessed via an unsafe accessor.</param>
/// <param name="requiresInterfaceCastOnSource"><c>true</c> when the source property must be read via an interface cast.</param>
/// <param name="requiresInterfaceCastOnTarget"><c>true</c> when the target property must be written via an interface cast.</param>
/// <param name="sourceField">The source field, if any (mutually exclusive with <paramref name="sourceProperty"/>).</param>
internal sealed class PropertyMapStrategy(
    IPropertySymbol targetProperty,
    IPropertySymbol? sourceProperty,
    MapStrategy propertyStrategy,
    bool postConstructorInitializer,
    ChainedSourcePropertyPathInfo? chainedSourcePropertyPath = null,
    bool requiresUnsafeAccessorOnSource = false,
    bool requiresUnsafeAccessorOnTarget = false,
    bool requiresInterfaceCastOnSource = false,
    bool requiresInterfaceCastOnTarget = false,
    IFieldSymbol? sourceField = null)
    : MapStrategy(targetProperty.Type, sourceProperty?.Type ?? sourceField?.Type ?? chainedSourcePropertyPath?.StartingSourceType ?? null!)
{
    /// <summary>
    /// Gets the target property.
    /// </summary>
    public IPropertySymbol TargetProperty { get; } = targetProperty;

    /// <summary>
    /// Gets the source property.
    /// </summary>
    public IPropertySymbol? SourceProperty { get; } = sourceProperty;

    /// <summary>
    /// Gets the source field.
    /// </summary>
    public IFieldSymbol? SourceField { get; } = sourceField;

    /// <summary>
    /// Gets the strategy to be applied between the source and target members.
    /// </summary>
    public MapStrategy PropertyStrategy { get; } = propertyStrategy;

    /// <summary>
    /// Gets a value indicating whether the property should be initialised after the constructor.
    /// </summary>
    public bool PostConstructorInitializer { get; } = postConstructorInitializer;

    /// <summary>
    /// Gets the chained source property path to read before mapping, if any.
    /// </summary>
    public ChainedSourcePropertyPathInfo? ChainedSourcePropertyPath { get; } = chainedSourcePropertyPath;

    /// <summary>
    /// Gets a value indicating whether the source member must be read via an unsafe accessor.
    /// </summary>
    public bool RequiresUnsafeAccessorOnSource { get; } = requiresUnsafeAccessorOnSource;

    /// <summary>
    /// Gets a value indicating whether the target property must be accessed via an unsafe accessor.
    /// </summary>
    public bool RequiresUnsafeAccessorOnTarget { get; } = requiresUnsafeAccessorOnTarget;

    /// <summary>
    /// Gets a value indicating whether the source property must be read via an interface cast.
    /// </summary>
    public bool RequiresInterfaceCastOnSource { get; } = requiresInterfaceCastOnSource;

    /// <summary>
    /// Gets a value indicating whether the target property must be written via an interface cast.
    /// </summary>
    public bool RequiresInterfaceCastOnTarget { get; } = requiresInterfaceCastOnTarget;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new PropertyMapStrategyBuilder(this);
}