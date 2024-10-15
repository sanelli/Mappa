// <copyright file="MappaInvokeMethodAttributeStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="MappaInvokeMethodAttributeStrategy"/>.
/// </summary>
internal sealed class MappaInvokeMethodAttributeStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly MappaInvokeMethodAttributeStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappaInvokeMethodAttributeStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy to build.</param>
    public MappaInvokeMethodAttributeStrategyBuilder(MappaInvokeMethodAttributeStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var targetTemporary = context.NextTemporary();
        var code = $"{this.strategy.TargetType.ToDisplayString()} {targetTemporary} = {this.strategy.Method.Name}({source});";
        return (targetTemporary, code);
    }
}