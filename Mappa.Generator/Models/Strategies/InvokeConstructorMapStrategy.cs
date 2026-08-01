// <copyright file="InvokeConstructorMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy used to invoke the constructor.
/// </summary>
/// <param name="targetType">The source type.</param>
/// <param name="sourceType">The target type.</param>
/// <param name="constructor">Gets the constructor.</param>
/// <param name="parametersMapStrategies">The strategies to be applied via constructor parameters.</param>
/// <param name="initializerStrategies">The strategies to be applied via initializers.</param>
/// <param name="assignToContextEntries">The context entries to assign after target construction.</param>
/// <param name="contextParameterName">The name of the context parameter, if any.</param>
/// <param name="requiresUnsafeAccessorOnConstructor"><c>true</c> when the constructor must be invoked via an unsafe accessor.</param>
internal sealed class InvokeConstructorMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    IMethodSymbol constructor,
    ParameterMapStrategy[] parametersMapStrategies,
    PropertyMapStrategy[] initializerStrategies,
    MappaAssignToContextEntry[] assignToContextEntries,
    string? contextParameterName,
    bool requiresUnsafeAccessorOnConstructor = false)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the constructor used.
    /// </summary>
    public IMethodSymbol Constructor { get; private set; } = constructor;

    /// <summary>
    /// Gets the strategies that can be applied via constructor parameters.
    /// </summary>
    public ParameterMapStrategy[] ParametersMapStrategies { get; } = parametersMapStrategies;

    /// <summary>
    /// Gets the strategies that can be applied via initializers.
    /// </summary>
    public PropertyMapStrategy[] InitializerStrategies { get; } = initializerStrategies;

    /// <summary>
    /// Gets the context entries to assign after target construction.
    /// </summary>
    public MappaAssignToContextEntry[] AssignToContextEntries { get; } = assignToContextEntries;

    /// <summary>
    /// Gets the name of the context parameter, if any.
    /// </summary>
    public string? ContextParameterName { get; } = contextParameterName;

    /// <summary>
    /// Gets a value indicating whether the constructor must be invoked via an unsafe accessor.
    /// </summary>
    public bool RequiresUnsafeAccessorOnConstructor { get; } = requiresUnsafeAccessorOnConstructor;

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new InvokeConstructorMapStrategyBuilder(this);
}