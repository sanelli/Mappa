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
internal sealed class InvokeConstructorMapStrategy(
    ITypeSymbol targetType,
    ITypeSymbol sourceType,
    IMethodSymbol constructor,
    ParameterMapStrategy[] parametersMapStrategies,
    PropertyMapStrategy[] initializerStrategies)
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

    /// <inheritdoc/>
    internal override IMappaStrategyBuilder GetBuilder() => new InvokeConstructorMapStrategyBuilder(this);
}