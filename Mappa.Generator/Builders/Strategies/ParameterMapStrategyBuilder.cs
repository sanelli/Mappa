// <copyright file="ParameterMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for strategy <see cref="ParameterMapStrategy"/>.
/// </summary>
internal sealed class ParameterMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly ParameterMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public ParameterMapStrategyBuilder(ParameterMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();

        // For Mappa Invoke Attribute we let that specific builder
        // build the temporary if needed.
        if (this.strategy.ParameterStrategy is MappaInvokeMethodAttributeStrategy mappaInvokeMethodAttributeStrategy)
        {
            return mappaInvokeMethodAttributeStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
        }

        // It is not a specific builder: go on to create a source temporary and the code
        // for the property specific builder.
        var sourcePropertyTemporary = string.Empty;
        if (this.strategy.SourceProperty is not null)
        {
            sourcePropertyTemporary = context.NextTemporary();
            builder.AppendLine($"{this.strategy.ParameterStrategy.SourceType.ToDisplayString()} {sourcePropertyTemporary} = {source}.{this.strategy.SourceProperty.Name};");
        }

        (string targetTemporary, string code) = this.strategy.ParameterStrategy.GetBuilder().BuildSource(sourcePropertyTemporary, context, mappaGlobalOptions);
        if (!string.IsNullOrWhiteSpace(code))
        {
            builder.AppendLine(code);
            builder.AppendEmptyLine();
        }

        return (targetTemporary, builder.ToString());
    }
}