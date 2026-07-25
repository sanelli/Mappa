// <copyright file="EnumToIntegralMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="EnumToIntegralMapStrategy"/> strategy.
/// </summary>
internal sealed class EnumToIntegralMapStrategyBuilder
    : IMappaStrategyBuilder
{
    private readonly EnumToIntegralMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToIntegralMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public EnumToIntegralMapStrategyBuilder(EnumToIntegralMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();

        var temporary = context.NextTemporary();
        builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {temporary};");
        builder.AppendLine($"switch ({source})");
        using (builder.CurlyBracesBlock())
        {
            EnumMapSwitchCodeHelper.AppendSwitchArms(
                builder,
                this.strategy.EnumMapConfiguration,
                temporary,
                source);
        }

        return (temporary, builder.ToString());
    }
}