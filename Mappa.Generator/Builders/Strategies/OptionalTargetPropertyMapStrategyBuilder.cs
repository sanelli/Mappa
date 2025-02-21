// <copyright file="OptionalTargetPropertyMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

using PrettyCode;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for strategy <see cref="OptionalTargetPropertyMapStrategy"/>.
/// </summary>
internal sealed class OptionalTargetPropertyMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly OptionalTargetPropertyMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionalTargetPropertyMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public OptionalTargetPropertyMapStrategyBuilder(OptionalTargetPropertyMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new StringBuilder();

        var (innerStrategyVariableName, innerStrategyCode) = this.strategy.InnerStrategy.GetBuilder().BuildSource(source, context, mappaGlobalOptions);
        if (!string.IsNullOrWhiteSpace(innerStrategyCode))
        {
            builder.AppendLine(innerStrategyCode);
        }

        builder.AppendLine($"if ({innerStrategyVariableName} != default)");
        using (builder.CurlyBracesBlock())
        {
            builder.AppendLine($"{context.GetCompositeTypeTargetName()}.{this.strategy.TargetProperty.Name} = {innerStrategyVariableName};");
        }

        return (string.Empty, builder.ToString());
    }
}