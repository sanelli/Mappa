// <copyright file="OptionalSourcePropertyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for strategy <see cref="OptionalSourcePropertyMapStrategy"/>.
/// </summary>
internal sealed class OptionalSourcePropertyMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly OptionalSourcePropertyMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalSourcePropertyMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public OptionalSourcePropertyMapStrategyBuilder(OptionalSourcePropertyMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        var targetVariable = context.NextTemporary();

        builder.AppendLine($"{this.strategy.TargetType} {targetVariable};");
        builder.AppendLine($"if ({context.GetCompositeTypeSourceName()}.Has{this.strategy.SourceProperty.Name})");
        using (builder.CurlyBracesBlock())
        {
            var (innerVariable, innerCode) = this.strategy.InnerStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
            builder.AppendLine($"{innerCode}");
            builder.AppendLine($"{targetVariable} = {innerVariable};");
        }

        builder.AppendLine("else");
        using (builder.CurlyBracesBlock())
        {
            builder.AppendLine($"{targetVariable} = default;");
        }

        return (targetVariable, builder.ToString());
    }
}