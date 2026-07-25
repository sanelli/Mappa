// <copyright file="EnumToEnumMapStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Helpers;
using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="EnumToEnumMapStrategy"/> strategy.
/// </summary>
internal sealed class EnumToEnumMapStrategyBuilder
   : IMappaStrategyBuilder
{
    private readonly EnumToEnumMapStrategy strategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumToEnumMapStrategyBuilder"/> class.
    /// </summary>
    /// <param name="strategy">The strategy.</param>
    public EnumToEnumMapStrategyBuilder(EnumToEnumMapStrategy strategy)
    {
        this.strategy = strategy;
    }

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();

        var targetEnumFullType = this.strategy.TargetType.ToDisplayString();
        var temporary = context.NextTemporary();
        builder.AppendLine($"{targetEnumFullType} {temporary};");
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