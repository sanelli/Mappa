// <copyright file="MappaInvokeMethodAttributeStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Exceptions;
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
        var accessor = this.GetAccessor();
        var parameters = this.GetParameters(source);
        var code = $"{this.strategy.TargetType.ToDisplayString()} {targetTemporary} = {accessor}{this.strategy.Method.Name}({parameters});";
        return (targetTemporary, code);
    }

    private string GetAccessor()
    {
        var accessor = this.strategy.Method.IsStatic ? string.Empty : "this.";
        return accessor;
    }

    private string GetParameters(string source)
    {
        switch (this.strategy.Method.Parameters.Length)
        {
            case 0:
                return string.Empty;
            case 1:
                return source;
        }

        throw new MappaGeneratorException("Unexpected number of parameters.");
    }
}