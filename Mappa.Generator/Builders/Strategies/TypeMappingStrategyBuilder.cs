// <copyright file="TypeMappingStrategyBuilder.cs" company="Stefano Anelli">
// Copyright (c) Stefano Anelli. All rights reserved.
// </copyright>

using Mappa.Generator.Models;
using Mappa.Generator.Models.Strategies;

namespace Mappa.Generator.Builders.Strategies;

/// <summary>
/// Builder for <see cref="TypeMappingStrategy"/> strategy.
/// </summary>
/// <param name="strategy">The strategy.</param>
internal sealed class TypeMappingStrategyBuilder(TypeMappingStrategy strategy)
    : IMappaStrategyBuilder
{
    private readonly TypeMappingStrategy strategy = strategy;

    /// <inheritdoc/>
    public (string VariableName, string Code) BuildSource(string source, MappaBuilderContext context, MappaGlobalOptions mappaGlobalOptions)
    {
        var builder = new PrettyCode.StringBuilder();
        var targetTemporary = context.NextTemporary();

        builder.AppendLine($"{this.strategy.TargetType.ToDisplayString()} {targetTemporary};");
        builder.AppendLine($"switch ({source})");
        using (builder.CurlyBracesBlock())
        {
            foreach (var subtypeStrategy in this.strategy.SubtypesMappingsStrategies)
            {
                var subtypeStrategyTemporary = context.NextTemporary();
                builder.AppendLine($"case {subtypeStrategy.SourceType.ToDisplayString()} {subtypeStrategyTemporary}:");
                using (builder.Indent())
                {
                    var subtypeStrategyBuilder = subtypeStrategy.GetBuilder();
                    var (subtypeStrategyTargetVariable, subtypeStrategyCode) = subtypeStrategyBuilder.BuildSource(subtypeStrategyTemporary, context, mappaGlobalOptions);
                    builder.AppendLine(subtypeStrategyCode);
                    builder.AppendLine($"{targetTemporary} = {subtypeStrategyTargetVariable};");
                    builder.AppendLine("break;");
                }

                builder.AppendEmptyLine();
            }

            builder.AppendLine("default:");
            using (builder.Indent())
            {
                // TODO [#49] Implement the default section.
                builder.AppendLine("break;");
            }
        }

        return (targetTemporary, builder.ToString());
    }
}