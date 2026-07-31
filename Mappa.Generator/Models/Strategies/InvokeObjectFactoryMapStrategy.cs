// <copyright file="InvokeObjectFactoryMapStrategy.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Builders.Strategies;

using Microsoft.CodeAnalysis;

namespace Mappa.Generator.Models.Strategies;

/// <summary>
/// Strategy used to invoke an object factory.
/// </summary>
/// <param name="targetType">The target type.</param>
/// <param name="sourceType">The source type.</param>
/// <param name="objectFactory">The resolved object factory.</param>
/// <param name="parametersMapStrategies">The strategies applied via factory parameters.</param>
/// <param name="initializerStrategies">The strategies applied via initializers.</param>
/// <param name="assignToContextEntries">The context entries to assign after target construction.</param>
/// <param name="contextParameterName">The name of the context parameter, if any.</param>
internal sealed class InvokeObjectFactoryMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    ObjectFactory objectFactory,
    ParameterMapStrategy[] parametersMapStrategies,
    PropertyMapStrategy[] initializerStrategies,
    MappaAssignToContextEntry[] assignToContextEntries,
    string? contextParameterName)
        : MapStrategy(targetType, sourceType)
{
    /// <summary>
    /// Gets the resolved object factory.
    /// </summary>
    public ObjectFactory ObjectFactory { get; } = objectFactory;

    /// <summary>
    /// Gets the strategies that can be applied via factory parameters.
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

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new InvokeObjectFactoryMapStrategyBuilder(this);
}