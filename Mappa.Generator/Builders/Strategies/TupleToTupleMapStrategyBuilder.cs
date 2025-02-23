// <copyright file="TupleToTupleMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="TupleToTupleMapStrategy"/>.
/// </summary>
internal sealed class TupleToTupleMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly TupleToTupleMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="TupleToTupleMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public TupleToTupleMapStrategyBuilder(TupleToTupleMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var elementTemporaries = new List<string>();
        var builder = new PrettyCode.StringBuilder();
        for (int index = 0; index < this.strategy.ElementStrategies.Length; ++index)
        {
            var elementStrategy = this.strategy.ElementStrategies[index];
            var sourceTemporary = context.NextTemporary();
            builder.AppendLine($"{elementStrategy.SourceType.ToDisplayString()} {sourceTemporary} = {source}.Item{index + 1};");

            var (targetTemporary, targetCode) = elementStrategy.GetBuilder().BuildSource(sourceTemporary, context, mappaGlobalOptions);
            if (!string.IsNullOrEmpty(targetCode))
            {
                builder.AppendLine(targetCode);
                builder.AppendEmptyLine();
            }

            elementTemporaries.Add(targetTemporary);
        }

        var tupleTemporary = context.NextTemporary();
        var buildingExpression = $"({string.Join(", ", elementTemporaries)})";
        if (!this.strategy.TargetType.IsTupleType)
        {
            buildingExpression = $"new {this.strategy.TargetType.ToDisplayString()}{buildingExpression}";
        }

        builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {tupleTemporary} = {buildingExpression};");

        return (tupleTemporary, builder.ToString());
    }
}