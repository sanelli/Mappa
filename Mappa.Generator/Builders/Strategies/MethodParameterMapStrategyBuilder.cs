// <copyright file="MethodParameterMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="MethodParameterMapStrategy"/> strategy.
/// </summary>
internal sealed class MethodParameterMapStrategyBuilder
    : IMappaStrategyBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodParameterMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="methodParameterMapStrategy">The strategy.</param>
    public MethodParameterMapStrategyBuilder(MethodParameterMapStrategy methodParameterMapStrategy)
    {
        this.MethodParameterMapStrategy = methodParameterMapStrategy;
    }

    /// <summary>
    /// Gets the strategy.
    /// </summary>
    private MethodParameterMapStrategy MethodParameterMapStrategy { get; }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var (strategySource, header) = this.MethodParameterMapStrategy.Strategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
        return ($"return {strategySource};", header);
    }
}